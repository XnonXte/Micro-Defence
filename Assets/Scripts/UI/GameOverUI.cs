using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MicroDefense.Core;
using MicroDefense.Towers;
using MicroDefense.Enemies;

namespace MicroDefense.UI
{
    /// <summary>
    /// Menampilkan Win Panel dan Lose Panel dengan tombol navigasi
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        public static GameOverUI Instance { get; private set; }

        [Header("Panel References")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        [Header("Win Panel Buttons")]
        [SerializeField] private Button winRetryButton;
        [SerializeField] private Button winMenuButton;
        [SerializeField] private Button winNextButton;

        [Header("Lose Panel Buttons")]
        [SerializeField] private Button loseRetryButton;
        [SerializeField] private Button loseMenuButton;

        [Header("Optional Text")]
        [SerializeField] private TMP_Text winTitleText;
        [SerializeField] private TMP_Text winMessageText;
        [SerializeField] private TMP_Text loseTitleText;
        [SerializeField] private TMP_Text loseMessageText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Hide panels on start
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
        }

        private void Start()
        {
            // Setup button listeners
            SetupButtons();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void SetupButtons()
        {
            // Win Panel Buttons
            if (winRetryButton != null)
                winRetryButton.onClick.AddListener(OnRetryClicked);
            
            if (winMenuButton != null)
                winMenuButton.onClick.AddListener(OnMenuClicked);
            
            if (winNextButton != null)
                winNextButton.onClick.AddListener(OnNextLevelClicked);

            // Lose Panel Buttons
            if (loseRetryButton != null)
                loseRetryButton.onClick.AddListener(OnRetryClicked);
            
            if (loseMenuButton != null)
                loseMenuButton.onClick.AddListener(OnMenuClicked);
        }

        /// <summary>
        /// Tampilkan Win Panel
        /// </summary>
        public void ShowWinPanel(string message = "Victory!")
        {
            if (winPanel != null)
            {
                // Hide semua UI gameplay
                HideAllGameplayUI();
                
                winPanel.SetActive(true);
                
                if (winTitleText != null)
                    winTitleText.text = "VICTORY!";
                
                if (winMessageText != null)
                    winMessageText.text = message;

                // Gunakan GameManager untuk pause (sama seperti pause button)
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Victory);
                }
                else
                {
                    Time.timeScale = 0f; // Fallback jika GameManager tidak ada
                }
            }
        }

        /// <summary>
        /// Tampilkan Lose Panel
        /// </summary>
        public void ShowLosePanel(string message = "Defeated!")
        {
            if (losePanel != null)
            {
                // Hide semua UI gameplay
                HideAllGameplayUI();
                
                losePanel.SetActive(true);
                
                if (loseTitleText != null)
                    loseTitleText.text = "DEFEATED!";
                
                if (loseMessageText != null)
                    loseMessageText.text = message;

                // Gunakan GameManager untuk pause (sama seperti pause button)
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Defeat);
                }
                else
                {
                    Time.timeScale = 0f; // Fallback jika GameManager tidak ada
                }
            }
        }
        
        /// <summary>
        /// Hide semua UI gameplay (HUD, Tower Shop, dll.)
        /// JANGAN nonaktifkan Canvas - hanya nonaktifkan komponen UI dan interaksi
        /// JANGAN touch button GameOver/Win panel!
        /// </summary>
        private void HideAllGameplayUI()
        {
            // Hide TowerShopUI - disable komponen DAN semua buttons/interaksi
            TowerShopUI shopUI = FindAnyObjectByType<TowerShopUI>();
            if (shopUI != null && shopUI.gameObject != gameObject)
            {
                // Nonaktifkan komponen TowerShopUI
                shopUI.enabled = false;
                
                // Disable buttons di dalam TowerShopUI TAPI skip button milik GameOverUI
                Button[] allButtons = shopUI.GetComponentsInChildren<Button>(true);
                foreach (Button btn in allButtons)
                {
                    // Skip button yang ada di winPanel atau losePanel
                    if (IsButtonInGameOverPanels(btn.transform))
                        continue;
                    
                    btn.interactable = false;
                    btn.gameObject.SetActive(false);
                }
                
                // Disable semua TowerDragHandler (untuk tower cards)
                TowerDragHandler[] dragHandlers = shopUI.GetComponentsInChildren<TowerDragHandler>(true);
                foreach (TowerDragHandler handler in dragHandlers)
                {
                    handler.enabled = false;
                    handler.gameObject.SetActive(false);
                }
                
                // Disable TowerUpgradeDragHandler (upgrade tool / pipet)
                TowerUpgradeDragHandler[] upgradeHandlers = shopUI.GetComponentsInChildren<TowerUpgradeDragHandler>(true);
                foreach (TowerUpgradeDragHandler handler in upgradeHandlers)
                {
                    handler.enabled = false;
                    handler.gameObject.SetActive(false);
                }
                
                // Hide semua TMP_Text gameplay (plasma, wave, health, dll)
                TMP_Text[] allTexts = shopUI.GetComponentsInChildren<TMP_Text>(true);
                foreach (TMP_Text txt in allTexts)
                {
                    // Skip text yang ada di GameOver panels
                    if (IsButtonInGameOverPanels(txt.transform))
                        continue;
                    
                    txt.gameObject.SetActive(false);
                }
                
                // Hide semua Image components (wadah plasma, icons, dll)
                UnityEngine.UI.Image[] allImages = shopUI.GetComponentsInChildren<UnityEngine.UI.Image>(true);
                foreach (UnityEngine.UI.Image img in allImages)
                {
                    // Skip images yang ada di GameOver panels
                    if (IsButtonInGameOverPanels(img.transform))
                        continue;
                    
                    img.gameObject.SetActive(false);
                }
                
                // Hide panel-panel spesifik
                HideChildPanel(shopUI.transform, "ShopPanel");
                HideChildPanel(shopUI.transform, "HUD");
                HideChildPanel(shopUI.transform, "InfoPanel");
                HideChildPanel(shopUI.transform, "TowerCards");
                HideChildPanel(shopUI.transform, "WaveButton");
                HideChildPanel(shopUI.transform, "PlasmaPanel");
                HideChildPanel(shopUI.transform, "Plasma");
                HideChildPanel(shopUI.transform, "LeaksPanel");
                HideChildPanel(shopUI.transform, "Leaks");
                HideChildPanel(shopUI.transform, "WaveStatusPanel");
                HideChildPanel(shopUI.transform, "WaveStatus");
                HideChildPanel(shopUI.transform, "UpgradeTool");
                HideChildPanel(shopUI.transform, "Pipet");
            }
            
            // Hide SimpleUI - disable komponennya
            if (SimpleUI.Instance != null && SimpleUI.Instance.gameObject != gameObject)
            {
                SimpleUI.Instance.enabled = false;
            }
            
            // Hide CompleteUIManager - disable komponennya
            if (CompleteUIManager.Instance != null && CompleteUIManager.Instance.gameObject != gameObject)
            {
                CompleteUIManager.Instance.enabled = false;
            }
            
            // Hide PauseController (pause button)
            PauseController pauseController = FindAnyObjectByType<PauseController>();
            if (pauseController != null && pauseController.gameObject != gameObject)
            {
                pauseController.gameObject.SetActive(false);
            }
            
            // Hide PauseMenuUI panel
            PauseMenuUI pauseMenuUI = FindAnyObjectByType<PauseMenuUI>();
            if (pauseMenuUI != null && pauseMenuUI.gameObject != gameObject)
            {
                pauseMenuUI.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Check apakah button ada di dalam winPanel atau losePanel
        /// </summary>
        private bool IsButtonInGameOverPanels(Transform buttonTransform)
        {
            Transform current = buttonTransform;
            while (current != null)
            {
                // Cek apakah parent adalah winPanel atau losePanel
                if (winPanel != null && current.gameObject == winPanel)
                    return true;
                if (losePanel != null && current.gameObject == losePanel)
                    return true;
                
                current = current.parent;
            }
            return false;
        }

        /// <summary>
        /// Helper untuk hide child panel dengan nama tertentu
        /// </summary>
        private void HideChildPanel(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null && child.gameObject != winPanel && child.gameObject != losePanel)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Retry - reload current scene dengan cleanup proper
        /// </summary>
        private void OnRetryClicked()
        {
            Debug.Log("[GameOverUI] Retry button clicked - Reloading scene...");
            
            // PENTING: Destroy semua GameObject yang bisa bikin bug
            DestroyAllPersistentObjects();
            
            // Resume time sebelum load scene
            Time.timeScale = 1f;
            
            // Reload scene saat ini
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        /// <summary>
        /// Menu - load MainMenu scene
        /// </summary>
        private void OnMenuClicked()
        {
            Debug.Log("[GameOverUI] Menu button clicked - Loading MainMenu...");
            
            // Destroy semua GameObject persistent
            DestroyAllPersistentObjects();
            
            // Resume time sebelum load scene
            Time.timeScale = 1f;
            
            // Coba load scene "MainMenu" (case-sensitive)
            string targetScene = "MainMenu";
            
            // Cek apakah scene MainMenu ada di Build Settings
            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                
                if (sceneName == targetScene)
                {
                    sceneExists = true;
                    break;
                }
            }
            
            if (sceneExists)
            {
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                Debug.LogWarning($"[GameOverUI] Scene '{targetScene}' not found in Build Settings! Loading first scene (index 0).");
                SceneManager.LoadScene(0);
            }
        }

        /// <summary>
        /// Destroy SEMUA GameObject yang persistent (DontDestroyOnLoad dan Singleton)
        /// Ini penting untuk Retry biar fresh start
        /// </summary>
        private void DestroyAllPersistentObjects()
        {
            Debug.Log("[GameOverUI] Destroying all persistent objects for clean restart...");
            
            // 1. Destroy CompleteUIManager (pakai DontDestroyOnLoad)
            if (CompleteUIManager.Instance != null)
            {
                Debug.Log("[GameOverUI] Destroying CompleteUIManager");
                Destroy(CompleteUIManager.Instance.gameObject);
            }
            
            // 2. Destroy SimpleUI (juga pakai DontDestroyOnLoad)
            if (SimpleUI.Instance != null)
            {
                Debug.Log("[GameOverUI] Destroying SimpleUI");
                Destroy(SimpleUI.Instance.gameObject);
            }
            
            // 3. Destroy semua tower yang ada di scene
            DestroyAllTowers();
            
            // 4. Destroy semua enemy yang masih ada
            DestroyAllEnemies();
            
            // 5. Destroy GameOverUI (this) juga
            if (Instance != null && Instance == this)
            {
                Debug.Log("[GameOverUI] Destroying GameOverUI");
                Instance = null;
                Destroy(gameObject);
            }
            
            // 6. Force destroy semua object di DontDestroyOnLoad scene
            GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                // Skip object yang sedang kita pakai (GameOverUI)
                if (obj != gameObject)
                {
                    Debug.Log($"[GameOverUI] Destroying persistent object: {obj.name}");
                    Destroy(obj);
                }
            }
        }
        
        /// <summary>
        /// Destroy semua tower yang ada di scene
        /// </summary>
        private void DestroyAllTowers()
        {
            // Cari semua object dengan komponen TowerBase
            MicroDefense.Towers.TowerBase[] towers = FindObjectsByType<MicroDefense.Towers.TowerBase>();
            foreach (var tower in towers)
            {
                Debug.Log($"[GameOverUI] Destroying tower: {tower.name}");
                Destroy(tower.gameObject);
            }
        }
        
        /// <summary>
        /// Destroy semua enemy yang masih ada di scene
        /// </summary>
        private void DestroyAllEnemies()
        {
            MicroDefense.Enemies.EnemyBase[] enemies = FindObjectsByType<MicroDefense.Enemies.EnemyBase>();
            foreach (var enemy in enemies)
            {
                Debug.Log($"[GameOverUI] Destroying enemy: {enemy.name}");
                Destroy(enemy.gameObject);
            }
        }

        /// <summary>
        /// Next Level - load next scene in build index
        /// </summary>
        private void OnNextLevelClicked()
        {
            Debug.Log("[GameOverUI] Next Level button clicked");
            
            // PENTING: Destroy semua persistent objects untuk clean state
            DestroyAllPersistentObjects();
            
            // Resume time sebelum load scene
            Time.timeScale = 1f;
            
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            
            // Check if next scene exists
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log($"[GameOverUI] Loading next level (scene index {nextSceneIndex})");
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                // No next level, go back to menu or reload current
                Debug.Log("[GameOverUI] No next level, going to menu.");
                OnMenuClicked();
            }
        }
    }
}
