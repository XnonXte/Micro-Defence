using System.Collections.Generic;
using MicroDefense.Core;
using MicroDefense.Data;
using MicroDefense.Towers;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

namespace MicroDefense.Grid
{
    public enum TileType
    {
        OffTrackSide, // Side of path ("NoPath"): B-Cell and T-Cell ONLY
        OnTrackPath,  // Directly on path ("Path"): Macrophage ONLY
        Any           // Universal tile
    }

    public class PlacementGridManager : MonoBehaviour
    {
        public static PlacementGridManager Instance { get; private set; }

        [Header("Tilemap System (Tile Palette Support)")]
        [SerializeField] private Tilemap pathTilemap;   // Track Tilemap (Jalur Virus / Macrophage Only)
        [SerializeField] private Tilemap noPathTilemap; // Non-Track Tilemap (Pinggir Jalur / B-Cell & T-Cell Only)

        [Header("GameObject Grid System (Fallback)")]
        [SerializeField] private LayerMask placementTileLayer;

        [Header("Ghost Preview Settings")]
        [SerializeField] private float ghostPreviewScale = 1f;
        [SerializeField] private float ghostPreviewAlpha = 0.7f;
        [SerializeField] private Sprite bCellGhostSprite;
        [SerializeField] private Sprite tCellGhostSprite;
        [SerializeField] private Sprite macrophageGhostSprite;

        [Header("Runtime Drag State")]
        private TowerDataSO currentDraggingTower;
        private GameObject ghostPreviewObj;
        private SpriteRenderer ghostSpriteRenderer;

        // For GameObject GridTile mode
        private GridTile currentTargetTile;

        // For Tilemap Grid mode
        private Vector3Int currentTilemapCellPos;
        private Vector3 currentTilemapWorldCenter;
        private bool isUsingTilemapPlacement;

        private HashSet<Vector3Int> occupiedTilemapCells = new HashSet<Vector3Int>();
        private bool isCurrentPlacementValid;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Reset grid state on scene load - fresh start
            ResetGridState();
        }

        private void Update()
        {
            // Handle Esc key to cancel active drag (Compatible with both New Input System & Legacy Input)
            if (currentDraggingTower != null && IsEscapeKeyPressed())
            {
                CancelDragPlacement();
            }
        }

        private bool IsEscapeKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                return true;
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            try
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    return true;
                }
            }
            catch { }
#endif
            return false;
        }

        #region Grid State Management

        public void ResetGridState()
        {
            // Clear occupied tiles
            occupiedTilemapCells.Clear();
            
            // Cancel any active drag
            CancelDragPlacement();
            
            // Reset drag state
            currentDraggingTower = null;
            currentTargetTile = null;
            isCurrentPlacementValid = false;
            
            Debug.Log("[PlacementGridManager.ResetGridState] Grid state reset for new round");
        }

        #endregion

        #region Drag & Drop Placement Methods

        public void StartDragPlacement(TowerDataSO towerData)
        {
            if (towerData == null) return;

            currentDraggingTower = towerData;
            CreateGhostPreview(towerData);
            AudioManager.Instance?.PlayPipetPicked();
            Debug.Log($"[PlacementGridManager.StartDragPlacement] Created ghost preview for {towerData.towerName}");
        }

        public void UpdateDragPlacement(Vector3 mouseWorldPos)
        {
            if (currentDraggingTower == null || ghostPreviewObj == null) return;

            if (EventSystem.current == null) return;

            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // Hitung offset dari sprite bounds agar perfectly centered di cursor
            Vector3 spriteOffset = Vector3.zero;
            if (ghostSpriteRenderer != null && ghostSpriteRenderer.sprite != null)
            {
                Sprite sprite = ghostSpriteRenderer.sprite;
                Debug.Log($"[UpdateDragPlacement] Mouse: ({mouseWorldPos.x:F2}, {mouseWorldPos.y:F2}), Sprite: {sprite.name}");
            }

            // -------------------------------------------------------------
            // OPTION A: Tilemap System (Unity Tile Palette)
            // -------------------------------------------------------------
            Tilemap activeReferenceTilemap = pathTilemap != null ? pathTilemap : noPathTilemap;
            if (activeReferenceTilemap != null)
            {
                isUsingTilemapPlacement = true;
                Vector3Int cellPos = activeReferenceTilemap.WorldToCell(mouseWorldPos);
                currentTilemapCellPos = cellPos;
                currentTilemapWorldCenter = activeReferenceTilemap.GetCellCenterWorld(cellPos);

                bool hasPathTile = pathTilemap != null && pathTilemap.HasTile(cellPos);
                bool hasNoPathTile = noPathTilemap != null && noPathTilemap.HasTile(cellPos);

                // Overlap rule: if both path and noPath exist on same cell → categorize as noPath
                bool isPureTrack = hasPathTile && !hasNoPathTile;
                bool isNoPath = hasNoPathTile; // includes pure noPath AND overlap

                bool isCompatible = false;
                if (currentDraggingTower.towerType == TowerType.Macrophage)
                {
                    // Macrophage: ONLY pure track (no noPath tile allowed, even on overlap)
                    isCompatible = isPureTrack;
                }
                else
                {
                    // B-Cell & T-Cell: any cell that is not pure track (off-track, overlap, or any tile)
                    isCompatible = !isPureTrack;
                }

                bool isFree = !occupiedTilemapCells.Contains(cellPos);
                bool canAfford = PlasmaManager.Instance != null && PlasmaManager.Instance.CanAfford(currentDraggingTower.baseCost);
                bool tileExists = hasPathTile || hasNoPathTile;

                isCurrentPlacementValid = isCompatible && isFree && canAfford && tileExists;

                ghostPreviewObj.transform.position = currentTilemapWorldCenter;
                SetGhostColor(isCurrentPlacementValid ? new Color(0.2f, 1f, 0.2f, 0.7f) : new Color(1f, 0.2f, 0.2f, 0.7f));
                return;
            }

            // -------------------------------------------------------------
            // OPTION B: GameObject GridTile System (Fallback)
            // -------------------------------------------------------------
            isUsingTilemapPlacement = false;
            Collider2D col = (placementTileLayer.value != 0) 
                ? Physics2D.OverlapPoint(mousePos2D, placementTileLayer) 
                : Physics2D.OverlapPoint(mousePos2D);

            if (col != null)
            {
                GridTile tile = col.GetComponent<GridTile>();
                if (tile != null)
                {
                    currentTargetTile = tile;
                    bool isCompatible = IsTileCompatibleWithTower(tile.tileType, currentDraggingTower.towerType);
                    bool isFree = !tile.IsOccupied;
                    bool canAfford = PlasmaManager.Instance != null && PlasmaManager.Instance.CanAfford(currentDraggingTower.baseCost);

                    isCurrentPlacementValid = isCompatible && isFree && canAfford;

                    ghostPreviewObj.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0f);
                    SetGhostColor(isCurrentPlacementValid ? new Color(0.2f, 1f, 0.2f, 0.7f) : new Color(1f, 0.2f, 0.2f, 0.7f));
                    return;
                }
            }

            // Default Invalid Hover State - tampilkan ghost di posisi cursor dengan warna merah
            currentTargetTile = null;
            isCurrentPlacementValid = false;
            ghostPreviewObj.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f);
            SetGhostColor(new Color(1f, 0.2f, 0.2f, 0.7f));
        }

        public void EndDragPlacement(Vector3 mouseWorldPos)
        {
            if (currentDraggingTower == null)
            {
                DestroyGhostPreview();
                return;
            }

            UpdateDragPlacement(mouseWorldPos);

            if (isCurrentPlacementValid)
            {
                if (isUsingTilemapPlacement)
                {
                    PlaceTowerOnTilemapCell(currentTilemapCellPos, currentTilemapWorldCenter, currentDraggingTower);
                }
                else if (currentTargetTile != null)
                {
                    PlaceTowerOnGridTile(currentTargetTile, currentDraggingTower);
                }
            }
            else
            {
                // placement invalid — ghost shows red, no action needed
            }

            DestroyGhostPreview();
            currentDraggingTower = null;
            currentTargetTile = null;
        }

        public void CancelDragPlacement()
        {
            DestroyGhostPreview();
            if (currentDraggingTower != null)
            {
                AudioManager.Instance?.PlayPipetUnpicked();
            }
            currentDraggingTower = null;
            currentTargetTile = null;
        }

        #endregion

        #region Tower Placement Core

        public void RemoveOccupiedCell(Vector3Int cellPos)
        {
            if (occupiedTilemapCells.Contains(cellPos))
            {
                occupiedTilemapCells.Remove(cellPos);
                Debug.Log($"[PlacementGridManager] Removed occupied cell: {cellPos}");
            }
        }

        private void PlaceTowerOnTilemapCell(Vector3Int cellPos, Vector3 spawnPos, TowerDataSO towerData)
        {
            if (!TryPayTowerCost(towerData))
                return;

            GameObject spawned = SpawnTowerInstance(towerData, spawnPos);
            occupiedTilemapCells.Add(cellPos);
            AudioManager.Instance?.PlayTurretPlaced();
        }

        private void PlaceTowerOnGridTile(GridTile tile, TowerDataSO towerData)
        {
            if (!TryPayTowerCost(towerData))
                return;

            Vector3 spawnPos = new Vector3(tile.transform.position.x, tile.transform.position.y, 0f);
            GameObject spawned = SpawnTowerInstance(towerData, spawnPos);

            tile.IsOccupied = true;
            tile.PlacedTower = spawned;
            AudioManager.Instance?.PlayTurretPlaced();
        }

        private bool TryPayTowerCost(TowerDataSO towerData)
        {
            if (towerData == null) return false;
            if (PlasmaManager.Instance == null) return true;

            int cost = Mathf.Max(0, towerData.baseCost);
            if (PlasmaManager.Instance.TrySpendPlasma(cost))
            {
                return true;
            }
            else
            {
                AudioManager.Instance?.PlayNotEnoughBalance();
                return false;
            }
        }

        private GameObject SpawnTowerInstance(TowerDataSO towerData, Vector3 position)
        {
            GameObject spawned = null;
            if (towerData.towerPrefab != null)
            {
                spawned = Instantiate(towerData.towerPrefab, position, Quaternion.identity);
            }
            else
            {
                spawned = CreateProceduralTowerObject(towerData, position);
            }

            TowerBase towerComp = spawned.GetComponent<TowerBase>();
            if (towerComp != null)
            {
                towerComp.SetTowerData(towerData);
                towerComp.ApplyTierStats(1);
                MicroDefense.UI.TowerShopUI shop = FindAnyObjectByType<MicroDefense.UI.TowerShopUI>();
                if (shop != null) shop.RegisterPlacedTower(towerComp);
            }

            return spawned;
        }

        private bool IsTileCompatibleWithTower(TileType tileType, TowerType towerType)
        {
            if (tileType == TileType.Any) return true;

            if (towerType == TowerType.Macrophage)
            {
                return tileType == TileType.OnTrackPath;
            }

            // B-Cell & T-Cell: boleh di OffTrackSide maupun Any (tidak di jalur path murni)
            return tileType == TileType.OffTrackSide;
        }

        #endregion

        #region Ghost Preview Visual Helper

        private void CreateGhostPreview(TowerDataSO towerData)
        {
            DestroyGhostPreview();

            ghostPreviewObj = new GameObject($"GhostPreview_{towerData.towerName}");
            ghostPreviewObj.layer = LayerMask.NameToLayer("Default");
            
            ghostSpriteRenderer = ghostPreviewObj.AddComponent<SpriteRenderer>();
            ghostSpriteRenderer.sortingOrder = 100;
            ghostSpriteRenderer.sortingLayerName = "Default";

            // Get sprite dari inspector dulu
            Sprite towerSprite = null;
            
            if (towerData.towerType == TowerType.BCell && bCellGhostSprite != null)
            {
                towerSprite = bCellGhostSprite;
            }
            else if (towerData.towerType == TowerType.TCell && tCellGhostSprite != null)
            {
                towerSprite = tCellGhostSprite;
            }
            else if (towerData.towerType == TowerType.Macrophage && macrophageGhostSprite != null)
            {
                towerSprite = macrophageGhostSprite;
            }

            // FALLBACK: Get sprite dari prefab
            if (towerSprite == null && towerData.towerPrefab != null)
            {
                SpriteRenderer prefabSr = towerData.towerPrefab.GetComponent<SpriteRenderer>();
                if (prefabSr != null && prefabSr.sprite != null)
                {
                    towerSprite = prefabSr.sprite;
                }
            }

            // FALLBACK: Coba icon dari TowerDataSO
            if (towerSprite == null && towerData.icon != null)
            {
                towerSprite = towerData.icon;
            }

            // FINAL FALLBACK: Generate kotak
            if (towerSprite != null)
            {
                ghostSpriteRenderer.sprite = towerSprite;
            }
            else
            {
                Color color = towerData.towerType switch
                {
                    TowerType.BCell => Color.cyan,
                    TowerType.TCell => Color.yellow,
                    TowerType.Macrophage => Color.green,
                    _ => Color.white
                };
                ghostSpriteRenderer.sprite = CreateSimpleCircleSprite(color);
            }

            // Apply ghost preview scale dari inspector
            ghostPreviewObj.transform.localScale = Vector3.one * ghostPreviewScale;

            // Position di world space dengan Z = 0
            ghostPreviewObj.transform.position = Vector3.zero;

            SetGhostColor(new Color(0.2f, 1f, 0.2f, ghostPreviewAlpha));
        }

        private void SetGhostColor(Color color)
        {
            if (ghostSpriteRenderer != null)
            {
                ghostSpriteRenderer.color = color;
            }
        }

        private void DestroyGhostPreview()
        {
            if (ghostPreviewObj != null)
            {
                Destroy(ghostPreviewObj);
                ghostPreviewObj = null;
                ghostSpriteRenderer = null;
            }
        }

        private GameObject CreateProceduralTowerObject(TowerDataSO towerData, Vector3 position)
        {
            GameObject towerObj = new GameObject(towerData.towerName);
            towerObj.transform.position = position;

            Color color = towerData.towerType switch
            {
                TowerType.BCell => Color.cyan,
                TowerType.TCell => Color.yellow,
                TowerType.Macrophage => Color.green,
                _ => Color.white
            };

            var sr = towerObj.AddComponent<SpriteRenderer>();
            sr.color = color;
            sr.sprite = CreateSimpleCircleSprite(color);

            var col = towerObj.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;
            col.isTrigger = true;

            switch (towerData.towerType)
            {
                case TowerType.BCell:
                    towerObj.AddComponent<BCellTower>();
                    break;
                case TowerType.TCell:
                    towerObj.AddComponent<TCellTower>();
                    break;
                case TowerType.Macrophage:
                    towerObj.AddComponent<MacrophageTower>();
                    break;
            }

            return towerObj;
        }

        private Sprite CreateSimpleCircleSprite(Color color)
        {
            Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[64 * 64];
            Vector2 center = new Vector2(32, 32);

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * 64 + x] = dist <= 30f ? color : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }

        #endregion
    }
}
