using UnityEngine;
using MicroDefense.Data;
using MicroDefense.Towers;

namespace MicroDefense.Core
{
    /// <summary>
    /// Helper script untuk auto-spawn towers di scene untuk testing.
    /// Attach ke _Managers GameObject dan configure di Inspector.
    /// </summary>
    public class TowerSpawner : MonoBehaviour
    {
        [Header("Tower Data")]
        [SerializeField] private TowerDataSO bCellData;
        [SerializeField] private TowerDataSO tCellData;
        [SerializeField] private TowerDataSO macrophageData;

        [Header("Tower Positions")]
        [SerializeField] private Vector3 bCellPosition = new Vector3(-2, 0, 0);
        [SerializeField] private Vector3 tCellPosition = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 macrophagePosition = new Vector3(2, 0, 0);

        [Header("Auto Spawn on Start")]
        [SerializeField] private bool autoSpawnOnStart = false;

        private void Start()
        {
            if (autoSpawnOnStart)
                SpawnTestTowers();
        }

        public void SpawnTestTowers()
        {
            if (bCellData != null && bCellData.tiers.Count > 0)
                SpawnTower(bCellData, bCellPosition, "B-Cell Tower");

            if (tCellData != null && tCellData.tiers.Count > 0)
                SpawnTower(tCellData, tCellPosition, "T-Cell Tower");

            if (macrophageData != null && macrophageData.tiers.Count > 0)
                SpawnTower(macrophageData, macrophagePosition, "Macrophage Tower");
        }

        private void SpawnTower(TowerDataSO towerData, Vector3 position, string name)
        {
            if (towerData == null) return;
            if (PlasmaManager.Instance != null && !PlasmaManager.Instance.TrySpendPlasma(Mathf.Max(0, towerData.baseCost)))
                return;

            GameObject towerObj = new GameObject(name);
            towerObj.transform.position = position;

            // Add components based on tower type
            if (name.Contains("B-Cell"))
            {
                AddSpriteRenderer(towerObj, towerData, Color.cyan);
                AddCircleCollider(towerObj, 3f);
                var bCell = towerObj.AddComponent<BCellTower>();
                bCell.SetTowerData(towerData);
            }
            else if (name.Contains("T-Cell"))
            {
                AddSpriteRenderer(towerObj, towerData, Color.yellow);
                AddCircleCollider(towerObj, 2f);
                var tCell = towerObj.AddComponent<TCellTower>();
                tCell.SetTowerData(towerData);
            }
            else if (name.Contains("Macrophage"))
            {
                AddSpriteRenderer(towerObj, towerData, Color.green);
                AddCircleCollider(towerObj, 2.5f);
                var macro = towerObj.AddComponent<MacrophageTower>();
                macro.SetTowerData(towerData);
            }

            // (no log)
        }

        private void AddSpriteRenderer(GameObject obj, TowerDataSO towerData, Color fallbackColor)
        {
            var spriteRenderer = obj.AddComponent<SpriteRenderer>();
            Sprite sourceSprite = towerData != null && towerData.towerPrefab != null
                ? towerData.towerPrefab.GetComponent<SpriteRenderer>()?.sprite
                : null;

            spriteRenderer.sprite = sourceSprite;
            spriteRenderer.color = sourceSprite != null ? Color.white : fallbackColor;
        }

        private void AddCircleCollider(GameObject obj, float radius)
        {
            var collider = obj.AddComponent<CircleCollider2D>();
            collider.radius = radius;
            collider.isTrigger = true;
        }
    }
}
