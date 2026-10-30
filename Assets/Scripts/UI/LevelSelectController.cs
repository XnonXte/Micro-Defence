using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct LevelData
{
    public string levelName;
    public string sceneName;
    public string description;
    public int difficulty; // 1-5 stars
}

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private MainMenuController mainMenuController;

    [SerializeField] private LevelData[] levels = new LevelData[]
    {
        new LevelData 
        { 
            levelName = "Level 1", 
            sceneName = "Tilemap", 
            description = "Tutorial - Learn the basics", 
            difficulty = 1 
        },
        new LevelData 
        { 
            levelName = "Level 2", 
            sceneName = "level1", 
            description = "Easy - Warm up", 
            difficulty = 2 
        },
        new LevelData 
        { 
            levelName = "Level 3", 
            sceneName = "level2", 
            description = "Medium - Getting challenging", 
            difficulty = 3 
        },
    };

    private void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);

        BuildLevelButtons();
        levelSelectPanel.SetActive(false);
    }

    private void BuildLevelButtons()
    {
        if (levelButtonContainer == null) return;

        // Clear existing buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create button for each level
        for (int i = 0; i < levels.Length; i++)
        {
            int index = i;
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            Button button = buttonObj.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(() => SelectLevel(index));
            }

            // Setup button text and info
            TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 1)
                texts[0].text = levels[index].levelName;
            if (texts.Length >= 2)
                texts[1].text = levels[index].description;
            if (texts.Length >= 3)
                texts[2].text = GetDifficultyStars(levels[index].difficulty);
        }
    }

    private string GetDifficultyStars(int difficulty)
    {
        string stars = "";
        for (int i = 0; i < difficulty; i++)
            stars += "★";
        for (int i = difficulty; i < 5; i++)
            stars += "☆";
        return stars;
    }

    public void ShowLevelSelect()
    {
        levelSelectPanel.SetActive(true);
    }

    public void HideLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    private void SelectLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levels[levelIndex].sceneName);
        }
    }

    private void BackToMainMenu()
    {
        HideLevelSelect();
        mainMenuController.ShowMainMenu();
    }
}
