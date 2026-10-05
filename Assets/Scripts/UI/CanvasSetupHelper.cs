using UnityEngine;
using UnityEngine.UI;
using MicroDefense.UI;
using MicroDefense.Data;

public class CanvasSetupHelper : MonoBehaviour
{
    [SerializeField] private TowerDataSO bCellData;
    [SerializeField] private TowerDataSO tCellData;
    [SerializeField] private TowerDataSO macrophageData;

    public void CreateUICanvas()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        // Add TowerShopUI script to Canvas
        TowerShopUI shopUI = canvasObj.AddComponent<TowerShopUI>();

        // Create Top HUD Panel
        GameObject hudPanel = new GameObject("HUD Panel");
        hudPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform hudRect = hudPanel.AddComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0, 1);
        hudRect.anchorMax = new Vector2(1, 1);
        hudRect.offsetMin = new Vector2(0, -60);
        hudRect.offsetMax = new Vector2(0, 0);
        hudPanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);

        // HUD Texts
        CreateTextInPanel(hudPanel, "Plasma Text", "Plasma: 300", Vector2.zero);
        CreateTextInPanel(hudPanel, "Wave Text", "Wave: 1/1", Vector2.zero);
        CreateTextInPanel(hudPanel, "Leaks Text", "Leaks: 0/5", Vector2.zero);

        // Create Bottom Shop Panel
        GameObject shopPanel = new GameObject("Shop Panel");
        shopPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform shopRect = shopPanel.AddComponent<RectTransform>();
        shopRect.anchorMin = new Vector2(0, 0);
        shopRect.anchorMax = new Vector2(1, 0);
        shopRect.offsetMin = new Vector2(0, 0);
        shopRect.offsetMax = new Vector2(0, 60);
        shopPanel.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        // Shop Buttons
        Button bCellBtn = CreateButtonInPanel(shopPanel, "B-Cell Button", "B-Cell");
        Button tCellBtn = CreateButtonInPanel(shopPanel, "T-Cell Button", "T-Cell");
        Button macroBtn = CreateButtonInPanel(shopPanel, "Macrophage Button", "Macrophage");
        Button startWaveBtn = CreateButtonInPanel(shopPanel, "Start Wave Button", "Start Wave");

        // Assign buttons to TowerShopUI
        var bCellButtonField = typeof(TowerShopUI).GetField("bCellButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tCellButtonField = typeof(TowerShopUI).GetField("tCellButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var macrophageButtonField = typeof(TowerShopUI).GetField("macrophageButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var startWaveButtonField = typeof(TowerShopUI).GetField("startWaveButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        bCellButtonField?.SetValue(shopUI, bCellBtn);
        tCellButtonField?.SetValue(shopUI, tCellBtn);
        macrophageButtonField?.SetValue(shopUI, macroBtn);
        startWaveButtonField?.SetValue(shopUI, startWaveBtn);

        // Assign data to TowerShopUI
        var bCellDataField = typeof(TowerShopUI).GetField("bCellData", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tCellDataField = typeof(TowerShopUI).GetField("tCellData", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var macrophageDataField = typeof(TowerShopUI).GetField("macrophageData", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        bCellDataField?.SetValue(shopUI, bCellData);
        tCellDataField?.SetValue(shopUI, tCellData);
        macrophageDataField?.SetValue(shopUI, macrophageData);
    }

    private void CreateTextInPanel(GameObject parent, string name, string content, Vector2 position)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(300, 50);
    }

    private Button CreateButtonInPanel(GameObject parent, string name, string buttonText)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.6f, 0.8f, 1);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.2f, 0.6f, 0.8f, 1);
        colors.highlightedColor = new Color(0.3f, 0.7f, 0.9f, 1);
        colors.pressedColor = new Color(0.1f, 0.5f, 0.7f, 1);
        btn.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        Text text = textObj.AddComponent<Text>();
        text.text = buttonText;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 18;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(150, 50);

        return btn;
    }
}
