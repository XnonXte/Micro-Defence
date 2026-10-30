using MicroDefense.Core;
using MicroDefense.Data;
using MicroDefense.Grid;
using MicroDefense.UI;
using MicroDefense.Towers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public sealed class SceneAutoSetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSceneComponents()
    {
        // Ensure PlacementGridManager
        PlacementGridManager pgm = FindAnyObjectByType<PlacementGridManager>();
        if (pgm == null)
        {
            var pgmObj = new GameObject("PlacementGridManager");
            pgm = pgmObj.GetComponent<PlacementGridManager>() ?? pgmObj.AddComponent<PlacementGridManager>();
            SetupGrid(pgm);
        }

        // Ensure TowerShopUI
        TowerShopUI shop = FindAnyObjectByType<TowerShopUI>();
        if (shop == null)
        {
            var shopObj = new GameObject("TowerShopUI");
            shop = shopObj.GetComponent<TowerShopUI>() ?? shopObj.AddComponent<TowerShopUI>();
            SetupShop(shop);
        }

        // Ensure UpgradeTool
        TowerUpgradeDragHandler upgrade = FindAnyObjectByType<TowerUpgradeDragHandler>();
        if (upgrade == null)
        {
            var upgradeObj = new GameObject("UpgradeTool");
            upgrade = upgradeObj.addComponent<TowerUpgradeDragHandler>();
            SetupUpgradeTool(upgrade);
        }
    }

    private static void SetupGrid(PlacementGridManager pgm)
    {
        Tilemap path = FindAnyObjectByType<Tilemap>();
        Tilemap noPath = null;
        foreach (var t in FindObjectsByType<Tilemap>())
        {
            if (t.name.ToLower().Contains("path")) path = t;
            else if (t.name.ToLower().Contains("nopath") || t.name.ToLower().Contains("offtrack")) noPath = t;
        }
        if (noPath == null) noPath = path;

        var pathField = typeof(PlacementGridManager).GetField("pathTilemap",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var noPathField = typeof(PlacementGridManager).GetField("noPathTilemap",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pathField?.SetValue(pgm, path);
        noPathField?.SetValue(pgm, noPath);

        // Ensure GridTile on all tile objects
        foreach (var obj in FindObjectsByType<GameObject>())
        {
            if (obj.GetComponent<GridTile>() == null)
                obj.AddComponent<GridTile>();
        }
    }

    private static void SetupShop(TowerShopUI shop)
    {
        var bCell = Resources.Load<TowerDataSO>("Data/TowerData/B Cell");
        var macrophage = Resources.Load<TowerDataSO>("Data/TowerData/Macrophage cell");
        var tCell = Resources.Load<TowerDataSO>("Data/TowerData/T Cell");

        var bField = typeof(TowerShopUI).GetField("bCellData",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var mField = typeof(TowerShopUI).GetField("macrophageData",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tField = typeof(TowerShopUI).GetField("tCellData",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bField?.SetValue(shop, bCell);
        mField?.SetValue(shop, macrophage);
        tField?.SetValue(shop, tCell);
    }

    private static void SetupUpgradeTool(TowerUpgradeDragHandler upgrade)
    {
        upgrade.transform.SetAsLastSibling();
    }
}