using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
                winPanel.SetActive(true);
                
                if (winTitleText != null)
                    winTitleText.text = "VICTORY!";
                
                if (winMessageText != null)
                    winMessageText.text = message;

                Time.timeScale = 0f; // Pause game
            }
        }

        /// <summary>
        /// Tampilkan Lose Panel
        /// </summary>
        public void ShowLosePanel(string message = "Defeated!")
        {
            if (losePanel != null)
            {
                losePanel.SetActive(true);
                
                if (loseTitleText != null)
                    loseTitleText.text = "DEFEATED!";
                
                if (loseMessageText != null)
                    loseMessageText.text = message;

                Time.timeScale = 0f; // Pause game
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
            
            // 2. Destroy GameOverUI (this) juga
            if (Instance != null && Instance == this)
            {
                Debug.Log("[GameOverUI] Destroying GameOverUI");
                Instance = null;
                Destroy(gameObject);
            }
            
            // 3. Force destroy semua object di DontDestroyOnLoad scene
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
            
            // 4. Reset static instances (kalau ada yang hardcode)
            // Ini akan di-reinitialize saat scene load
            ResetStaticInstances();
        }
        
        /// <summary>
        /// Reset semua static instance reference (optional safety)
        /// </summary>
        private void ResetStaticInstances()
        {
            // Note: Kebanyakan manager akan auto-reset karena script di-reload
            // Tapi kalau ada yang stuck, bisa tambahkan di sini
            
            Debug.Log("[GameOverUI] Static instances will be reinitialized on scene load.");
        }

        /// <summary>
        /// Next Level - load next scene in build index
        /// </summary>
        private void OnNextLevelClicked()
        {
            Time.timeScale = 1f; // Resume time
            
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            
            // Check if next scene exists
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
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
