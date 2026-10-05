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
            GameObject towerObj = new GameObject(name);
            towerObj.transform.position = position;

            // Add components based on tower type
            if (name.Contains("B-Cell"))
            {
                AddSpriteRenderer(towerObj, Color.cyan);
                AddCircleCollider(towerObj, 3f);
                var bCell = towerObj.AddComponent<BCellTower>();
                bCell.SetTowerData(towerData);
            }
            else if (name.Contains("T-Cell"))
            {
                AddSpriteRenderer(towerObj, Color.yellow);
                AddCircleCollider(towerObj, 2f);
                var tCell = towerObj.AddComponent<TCellTower>();
                tCell.SetTowerData(towerData);
            }
            else if (name.Contains("Macrophage"))
            {
                AddSpriteRenderer(towerObj, Color.green);
                AddCircleCollider(towerObj, 2.5f);
                var macro = towerObj.AddComponent<MacrophageTower>();
                macro.SetTowerData(towerData);
            }

            // (no log)
        }

        private void AddSpriteRenderer(GameObject obj, Color color)
        {
            var sprite = obj.AddComponent<SpriteRenderer>();
            sprite.color = color;
            // Create simple circle sprite programmatically
            Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[64 * 64];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            sprite.sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }

        private void AddCircleCollider(GameObject obj, float radius)
        {
            var collider = obj.AddComponent<CircleCollider2D>();
            collider.radius = radius;
            collider.isTrigger = true;
        }
    }
}
