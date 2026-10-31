using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button level4Button;
    [SerializeField] private Button level5Button;
    [SerializeField] private MainMenuController mainMenuController;

    private void Start()
    {
        // Setup back button
        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);
        else
            Debug.LogWarning("[LevelSelectController] Back button not assigned!");

        // Setup level buttons
        if (level1Button != null)
            level1Button.onClick.AddListener(() => LoadLevel("1"));
        
        if (level2Button != null)
            level2Button.onClick.AddListener(() => LoadLevel("2"));
        
        if (level3Button != null)
            level3Button.onClick.AddListener(() => LoadLevel("3"));
        
        if (level4Button != null)
            level4Button.onClick.AddListener(() => LoadLevel("4"));
        
        if (level5Button != null)
            level5Button.onClick.AddListener(() => LoadLevel("5"));

        // Hide panel at start
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
        else
            Debug.LogError("[LevelSelectController] Level Select Panel not assigned!");
        
        // Find MainMenuController if not assigned
        if (mainMenuController == null)
            mainMenuController = FindAnyObjectByType<MainMenuController>();
    }

    public void ShowLevelSelect()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(true);
    }

    public void HideLevelSelect()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
    }

    private void LoadLevel(string sceneName)
    {
        Debug.Log($"[LevelSelectController] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    private void BackToMainMenu()
    {
        HideLevelSelect();
        if (mainMenuController != null)
            mainMenuController.ShowMainMenu();
    }
}
