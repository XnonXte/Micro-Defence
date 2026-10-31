using MicroDefense.Core;
using MicroDefense.Towers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MicroDefense.UI
{
    /// <summary>
    /// Upgrade tool drag handler - follows cursor exactly during drag
    /// </summary>
    public class TowerUpgradeDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float targetRadius = 2.5f;
        [SerializeField] private float indicatorSize = 3f;
        [SerializeField] private Sprite idleBackground;
        [SerializeField] private Sprite draggingBackground;
        [SerializeField] private Image backgroundImage;

        private bool isDragging;
        private GameObject gridIndicator;
        private SpriteRenderer gridIndicatorRenderer;
        private RectTransform rectTransform;
        
        // Store original UI state untuk restore
        private Vector2 defaultAnchoredPosition;
        private Vector2 defaultSizeDelta;
        private Vector2 defaultAnchorMin;
        private Vector2 defaultAnchorMax;
        private Vector2 defaultPivot;
        private Vector3 defaultLocalScale;
        private Quaternion defaultLocalRotation;
        private Transform defaultParent;
        private Canvas defaultCanvas;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (backgroundImage == null)
                backgroundImage = transform.Find("Background")?.GetComponent<Image>();
            
            if (rectTransform != null)
            {
                defaultAnchoredPosition = rectTransform.anchoredPosition;
                defaultSizeDelta = rectTransform.sizeDelta;
                defaultAnchorMin = rectTransform.anchorMin;
                defaultAnchorMax = rectTransform.anchorMax;
                defaultPivot = rectTransform.pivot;
                defaultLocalScale = rectTransform.localScale;
                defaultLocalRotation = rectTransform.localRotation;
                defaultParent = rectTransform.parent;
                defaultCanvas = rectTransform.GetComponentInParent<Canvas>();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            
            // Play pipet picked SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPipetPicked();
            }
            
            if (backgroundImage != null && draggingBackground != null)
                backgroundImage.sprite = draggingBackground;
            
            CreateIndicator();
            UpdateIndicator(eventData);
            ShowAllTowerUpgradeInfo();
            
            // Pindahkan ke overlay canvas agar bisa follow cursor di world space
            if (rectTransform != null)
            {
                Canvas overlayCanvas = GetOrCreateOverlayCanvas();
                rectTransform.SetParent(overlayCanvas.transform, false);
                rectTransform.SetAsLastSibling();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || rectTransform == null)
                return;

            UpdateIndicator(eventData);
            
            // Update posisi tool untuk follow cursor di screen space
            Vector3 screenPos = eventData.position;
            
            // Naikin Y offset dari cursor user
            screenPos.y += 170f;
            
            // Konversi screen position ke UI canvas position
            if (rectTransform.parent != null && rectTransform.parent.GetComponent<Canvas>() != null)
            {
                Canvas canvas = rectTransform.parent.GetComponent<Canvas>();
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    // Direct screen space positioning
                    rectTransform.position = screenPos;
                }
                else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    rectTransform.position = screenPos;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            isDragging = false;
            Vector3 worldPosition = GetMouseWorldPosition(eventData);
            TowerBase target = FindTowerAt(worldPosition);
            
            if (target != null)
                target.TryUpgrade();

            // Play pipet unpicked SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPipetUnpicked();
            }

            HideAllTowerUpgradeInfo();
            DestroyIndicator();
            ResetPosition();
            
            if (backgroundImage != null && idleBackground != null)
                backgroundImage.sprite = idleBackground;
        }

        private Canvas GetOrCreateOverlayCanvas()
        {
            // Cari atau buat overlay canvas
            Canvas[] canvases = FindObjectsByType<Canvas>();
            foreach (Canvas c in canvases)
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                    return c;
            }
            
            // Jika tidak ada, gunakan parent canvas
            return defaultCanvas;
        }

        private void CreateIndicator()
        {
            if (gridIndicator != null)
                return;

            gridIndicator = new GameObject("UpgradeGridIndicator");
            gridIndicatorRenderer = gridIndicator.AddComponent<SpriteRenderer>();
            gridIndicatorRenderer.sprite = CreateIndicatorSprite();
            gridIndicatorRenderer.sortingOrder = 90;
            
            // Use indicatorSize from inspector - adjust if needed
            gridIndicator.transform.localScale = Vector3.one * indicatorSize;
        }

        private void UpdateIndicator(PointerEventData eventData)
        {
            if (gridIndicator == null)
                return;

            Vector3 worldPosition = GetMouseWorldPosition(eventData);
            worldPosition.x = Mathf.Round(worldPosition.x);
            worldPosition.y = Mathf.Round(worldPosition.y);
            worldPosition.z = 0f;
            
            gridIndicator.transform.position = worldPosition;

            TowerBase target = FindTowerAt(worldPosition);
            bool hasEnoughPlasma = target != null && target.CanUpgrade(out int cost) &&
                PlasmaManager.Instance != null && PlasmaManager.Instance.CanAfford(cost);
            
            gridIndicatorRenderer.color = hasEnoughPlasma
                ? new Color(0.2f, 1f, 0.2f, 0.65f)
                : new Color(1f, 0.15f, 0.15f, 0.65f);
        }

        private void DestroyIndicator()
        {
            if (gridIndicator != null)
                Destroy(gridIndicator);
            gridIndicator = null;
            gridIndicatorRenderer = null;
        }

        private Sprite CreateIndicatorSprite()
        {
            Texture2D texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
        }

        private void ResetPosition()
        {
            if (rectTransform == null)
                return;

            rectTransform.SetParent(defaultParent, false);
            rectTransform.anchorMin = defaultAnchorMin;
            rectTransform.anchorMax = defaultAnchorMax;
            rectTransform.pivot = defaultPivot;
            rectTransform.sizeDelta = defaultSizeDelta;
            rectTransform.localScale = defaultLocalScale;
            rectTransform.localRotation = defaultLocalRotation;
            rectTransform.anchoredPosition = defaultAnchoredPosition;
        }

        private TowerBase FindTowerAt(Vector3 worldPosition)
        {
            TowerBase[] towers = FindObjectsByType<TowerBase>();
            TowerBase closest = null;
            float closestDistance = targetRadius;

            foreach (TowerBase tower in towers)
            {
                if (tower == null || tower.Data == null)
                    continue;

                float distance = Vector2.Distance(worldPosition, tower.transform.position);
                if (distance <= closestDistance)
                {
                    closest = tower;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private void ShowAllTowerUpgradeInfo()
        {
            TowerBase[] towers = FindObjectsByType<TowerBase>(FindObjectsSortMode.None);
            foreach (var tower in towers)
            {
                if (tower == null) continue;
                
                // Tambahkan TowerUpgradeInfoUI component jika belum ada
                TowerUpgradeInfoUI infoUI = tower.GetComponent<TowerUpgradeInfoUI>();
                if (infoUI == null)
                {
                    infoUI = tower.gameObject.AddComponent<TowerUpgradeInfoUI>();
                }
                
                infoUI.ShowInfo();
            }
        }

        private void HideAllTowerUpgradeInfo()
        {
            TowerBase[] towers = FindObjectsByType<TowerBase>(FindObjectsSortMode.None);
            foreach (var tower in towers)
            {
                if (tower == null) continue;
                
                TowerUpgradeInfoUI infoUI = tower.GetComponent<TowerUpgradeInfoUI>();
                if (infoUI != null)
                {
                    infoUI.HideInfo();
                }
            }
        }

        private Vector3 GetMouseWorldPosition(PointerEventData eventData)
        {
            Camera camera = Camera.main;
            if (camera == null)
                return Vector3.zero;

            Vector3 screenPosition = eventData.position;
            Vector3 worldPosition = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));
            worldPosition.z = 0f;
            return worldPosition;
        }
    }
}
