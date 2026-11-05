using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private ScrollRect creditsScrollRect;
    [SerializeField] private RectTransform creditsContent;
    [SerializeField] private Button backButton;
    [SerializeField] private float autoScrollSpeed = 20f;

    private MainMenuController mainMenuController;
    private bool isAutoScrolling;

    [System.Serializable]
    public struct CreditEntry
    {
        public string title;
        [TextArea(1, 5)] // Allow multiple lines in Inspector
        public string names;
    }

    [Header("Credit Entries")]
    [SerializeField] private List<CreditEntry> credits = new List<CreditEntry>
    {
        new CreditEntry { title = "DEVELOPMENT", names = "Game Developer" },
        new CreditEntry { title = "DESIGN", names = "Game Designer" },
        new CreditEntry { title = "PROGRAMMING", names = "Programmer" },
        new CreditEntry { title = "ART", names = "Artist" },
        new CreditEntry { title = "MUSIC & SOUND", names = "Audio Designer" },
        new CreditEntry { title = "SPECIAL THANKS", names = "Everyone who supported this project" },
    };

    [Header("Text Styling")]
    [SerializeField] private Color titleColor = Color.black;
    [SerializeField] private Color namesColor = Color.black;
    [SerializeField] private int titleFontSize = 28;
    [SerializeField] private int namesFontSize = 20;
    [SerializeField] private bool titleBold = true;
    [SerializeField] private bool namesBold = true;
    
    [Header("Main Title")]
    [SerializeField] private string mainTitle = "CREDITS";
    [SerializeField] private Color mainTitleColor = Color.yellow;
    [SerializeField] private int mainTitleFontSize = 40;
    
    [Header("End Text")]
    [SerializeField] private string endText = "Thank you for playing!";
    [SerializeField] private Color endTextColor = Color.cyan;

    private void Start()
    {
        mainMenuController = GetComponentInParent<MainMenuController>();
        if (mainMenuController == null)
            mainMenuController = FindAnyObjectByType<MainMenuController>();
        
        if (backButton != null)
        {
            if (mainMenuController != null)
                backButton.onClick.AddListener(() => mainMenuController.ShowMainMenu());
            else
                Debug.LogError("[CreditsController] MainMenuController not found!");
        }

        if (creditsScrollRect != null)
            creditsScrollRect.onValueChanged.AddListener(OnScroll);

        BuildCreditsUI();
        ResetScroll();
        StartAutoScroll();
    }

    private void BuildCreditsUI()
    {
        if (creditsContent == null) return;

        // Clear existing content
        foreach (Transform child in creditsContent)
        {
            Destroy(child.gameObject);
        }

        // Add Vertical Layout Group if not exists
        VerticalLayoutGroup layoutGroup = creditsContent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = creditsContent.gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true; // Force expand width untuk center alignment
            layoutGroup.childAlignment = TextAnchor.UpperCenter; // Center horizontally
            layoutGroup.spacing = 10;
            layoutGroup.padding = new RectOffset(20, 20, 20, 20);
        }
        else
        {
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
        }

        // Add LayoutElement to Content
        LayoutElement contentLayout = creditsContent.GetComponent<LayoutElement>();
        if (contentLayout == null)
        {
            contentLayout = creditsContent.gameObject.AddComponent<LayoutElement>();
        }
        contentLayout.preferredWidth = 600;
        contentLayout.preferredHeight = 0; // Will auto-expand

        // Add title
        CreateText(mainTitle, mainTitleFontSize, FontStyle.Bold, mainTitleColor, 60);

        // Add spacing
        CreateSpacing(20);

        // Add credit entries
        foreach (var entry in credits)
        {
            FontStyle titleStyle = titleBold ? FontStyle.Bold : FontStyle.Normal;
            FontStyle namesStyle = namesBold ? FontStyle.Bold : FontStyle.Normal;
            
            CreateText(entry.title, titleFontSize, titleStyle, titleColor, 50);
            CreateText(entry.names, namesFontSize, namesStyle, namesColor, 40);
            CreateSpacing(30);
        }

        // Add end text
        CreateSpacing(40);
        CreateText(endText, 24, FontStyle.Bold, endTextColor, 50);
        CreateSpacing(60);
    }

    private void CreateText(string text, int fontSize, FontStyle fontStyle, Color color, float minHeight)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(creditsContent, false);

        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(600, minHeight);

        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        
        // Convert FontStyle to FontStyles
        TMPro.FontStyles tmpFontStyle = TMPro.FontStyles.Normal;
        if (fontStyle == FontStyle.Bold)
            tmpFontStyle = TMPro.FontStyles.Bold;
        else if (fontStyle == FontStyle.Italic)
            tmpFontStyle = TMPro.FontStyles.Italic;
        else if (fontStyle == FontStyle.BoldAndItalic)
            tmpFontStyle = TMPro.FontStyles.Bold | TMPro.FontStyles.Italic;
        
        tmpText.fontStyle = tmpFontStyle;
        tmpText.color = color;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.richText = true;
        tmpText.enableWordWrapping = true; // Enable word wrap
        tmpText.overflowMode = TextOverflowModes.Overflow; // Allow overflow untuk multi-line

        LayoutElement layout = textObj.AddComponent<LayoutElement>();
        layout.minHeight = minHeight;
        layout.preferredWidth = 600;
        layout.flexibleHeight = 1; // Allow flexible height untuk multi-line
    }

    private void CreateSpacing(float height)
    {
        GameObject spacer = new GameObject("Spacer");
        spacer.transform.SetParent(creditsContent, false);

        RectTransform rectTransform = spacer.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(600, height);

        LayoutElement layout = spacer.AddComponent<LayoutElement>();
        layout.preferredHeight = height;
    }

    private void StartAutoScroll()
    {
        isAutoScrolling = true;
    }

    private void StopAutoScroll()
    {
        isAutoScrolling = false;
    }

    private void OnScroll(Vector2 position)
    {
        // Stop auto-scroll if user manually scrolls
        if (position.y > 0.01f && position.y < 0.99f)
            StopAutoScroll();
    }

    private void Update()
    {
        if (!isAutoScrolling || creditsScrollRect == null) return;

        // Auto-scroll down
        Vector2 newPosition = creditsScrollRect.normalizedPosition;
        newPosition.y -= Time.deltaTime * autoScrollSpeed / 100f;

        if (newPosition.y <= 0)
        {
            newPosition.y = 0;
            isAutoScrolling = false;
        }

        creditsScrollRect.normalizedPosition = newPosition;
    }

    private void ResetScroll()
    {
        if (creditsScrollRect != null)
            creditsScrollRect.normalizedPosition = new Vector2(0, 1); // Top
    }
}
