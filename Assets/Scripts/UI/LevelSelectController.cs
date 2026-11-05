using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MicroDefense.Core;

public class LevelSelectController : MonoBehaviour
{
    [System.Serializable]
    public class LevelButtonEntry
    {
        public int levelNumber;
        public Button button;
        [Tooltip("Optional: overlay/lock icon to show when locked")]
        public GameObject lockOverlay;
    }

    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private Button backButton;

    [Header("Level Buttons (New Setup - Recommended)")]
    [SerializeField] private LevelButtonEntry[] levelButtons = new LevelButtonEntry[5];

    [Header("Legacy Buttons (Fallback)")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button level4Button;
    [SerializeField] private Button level5Button;

    [SerializeField] private MainMenuController mainMenuController;

    [Header("Lock Visual")]
    [SerializeField] private Color lockedTint = new Color(0.45f, 0.45f, 0.45f, 1f);
    [SerializeField] private Color unlockedTint = Color.white;
    [SerializeField] private bool dimButtonTransitionColors = true;

    private void Start()
    {
        // Setup back button
        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);
        else
            Debug.LogWarning("[LevelSelectController] Back button not assigned!");

        SetupLevelButtons();

        // Hide panel at start
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
        else
            Debug.LogError("[LevelSelectController] Level Select Panel not assigned!");

        // Find MainMenuController if not assigned
        if (mainMenuController == null)
            mainMenuController = FindAnyObjectByType<MainMenuController>();
    }

    private void OnEnable()
    {
        // Refresh lock state whenever panel becomes active (e.g., after returning from a level)
        RefreshLevelLocks();
    }

    private void SetupLevelButtons()
    {
        bool hasNewSetup = false;
        if (levelButtons != null)
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                var entry = levelButtons[i];
                if (entry == null || entry.button == null) continue;
                hasNewSetup = true;

                int level = entry.levelNumber > 0 ? entry.levelNumber : (i + 1);
                entry.levelNumber = level;

                entry.button.onClick.RemoveAllListeners();
                entry.button.onClick.AddListener(() => TryLoadLevel(level));
            }
        }

        // Legacy fallback (existing Inspector wiring)
        if (!hasNewSetup)
        {
            if (level1Button != null) { level1Button.onClick.RemoveAllListeners(); level1Button.onClick.AddListener(() => TryLoadLevel(1)); }
            if (level2Button != null) { level2Button.onClick.RemoveAllListeners(); level2Button.onClick.AddListener(() => TryLoadLevel(2)); }
            if (level3Button != null) { level3Button.onClick.RemoveAllListeners(); level3Button.onClick.AddListener(() => TryLoadLevel(3)); }
            if (level4Button != null) { level4Button.onClick.RemoveAllListeners(); level4Button.onClick.AddListener(() => TryLoadLevel(4)); }
            if (level5Button != null) { level5Button.onClick.RemoveAllListeners(); level5Button.onClick.AddListener(() => TryLoadLevel(5)); }
        }

        RefreshLevelLocks();
    }

    public void RefreshLevelLocks()
    {
        int unlocked = LevelProgress.GetUnlockedLevel();
        Debug.Log($"[LevelSelectController] Unlocked level: {unlocked}");

        bool usedNewSetup = false;
        if (levelButtons != null)
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                var entry = levelButtons[i];
                if (entry == null || entry.button == null) continue;
                usedNewSetup = true;

                int level = entry.levelNumber > 0 ? entry.levelNumber : (i + 1);
                ApplyLockState(entry.button, level, unlocked, entry.lockOverlay);
            }
        }

        if (!usedNewSetup)
        {
            ApplyLockState(level1Button, 1, unlocked, null);
            ApplyLockState(level2Button, 2, unlocked, null);
            ApplyLockState(level3Button, 3, unlocked, null);
            ApplyLockState(level4Button, 4, unlocked, null);
            ApplyLockState(level5Button, 5, unlocked, null);
        }
    }

    private void ApplyLockState(Button button, int level, int unlockedLevel, GameObject lockOverlay)
    {
        if (button == null) return;

        bool unlocked = level <= 1 || level <= unlockedLevel;
        button.interactable = unlocked;

        // Tint all graphics on the button (Image/Text children) so it looks grey when locked
        var graphics = button.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] == null) continue;
            // Don't override fully transparent elements unintentionally
            if (graphics[i].color.a <= 0.001f) continue;
            graphics[i].color = unlocked ? unlockedTint : lockedTint;
        }
        
        // Optional: also dim the button's transition colors so hover/pressed doesn't look bright when locked
        if (dimButtonTransitionColors)
        {
            var colors = button.colors;
            colors.disabledColor = lockedTint;
            if (!unlocked)
            {
                colors.normalColor = lockedTint;
                colors.highlightedColor = lockedTint;
                colors.pressedColor = lockedTint;
                colors.selectedColor = lockedTint;
            }
            else
            {
                // Keep unlocked readable; Unity defaults are fine, but ensure not stuck grey
                colors.normalColor = Color.white;
            }
            button.colors = colors;
        }

        if (lockOverlay != null)
            lockOverlay.SetActive(!unlocked);
    }

    public void ShowLevelSelect()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(true);
        RefreshLevelLocks();
    }

    public void HideLevelSelect()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
    }

    private void TryLoadLevel(int level)
    {
        if (!LevelProgress.IsLevelUnlocked(level))
        {
            Debug.Log($"[LevelSelectController] Level {level} is locked.");
            return;
        }
        LoadLevel(level.ToString());
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

