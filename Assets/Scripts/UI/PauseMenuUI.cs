using MicroDefense.Core;
using MicroDefense.Grid;
using MicroDefense.Towers;
using MicroDefense.Enemies;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MicroDefense.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            // Setup button listeners
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            
            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClicked);

            // Hide pause panel at start
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        public void ShowPausePanel()
        {
            if (pausePanel != null)
                pausePanel.SetActive(true);
        }

        public void HidePausePanel()
        {
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        private void OnResumeClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
            
            HidePausePanel();
            Time.timeScale = 1f;
        }

        private void OnRestartClicked()
        {
            Debug.Log("[PauseMenuUI] Restart button clicked - Reloading scene...");
            
            // PENTING: Destroy semua GameObject yang bisa bikin bug
            DestroyAllPersistentObjects();
            
            // Resume time sebelum load scene
            Time.timeScale = 1f;
            
            // Reload scene saat ini
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
        
        /// <summary>
        /// Destroy SEMUA GameObject yang persistent (DontDestroyOnLoad dan Singleton)
        /// Ini penting untuk Restart biar fresh start
        /// </summary>
        private void DestroyAllPersistentObjects()
        {
            Debug.Log("[PauseMenuUI] Destroying all persistent objects for clean restart...");
            
            // 1. Destroy CompleteUIManager (pakai DontDestroyOnLoad)
            if (CompleteUIManager.Instance != null)
            {
                Debug.Log("[PauseMenuUI] Destroying CompleteUIManager");
                Destroy(CompleteUIManager.Instance.gameObject);
            }
            
            // 2. Destroy SimpleUI (juga pakai DontDestroyOnLoad)
            if (SimpleUI.Instance != null)
            {
                Debug.Log("[PauseMenuUI] Destroying SimpleUI");
                Destroy(SimpleUI.Instance.gameObject);
            }
            
            // 3. Destroy AudioManager (DontDestroyOnLoad)
            if (AudioManager.Instance != null)
            {
                Debug.Log("[PauseMenuUI] Destroying AudioManager");
                Destroy(AudioManager.Instance.gameObject);
            }
            
            // 4. Destroy GameManager (jika ada)
            if (GameManager.Instance != null)
            {
                Debug.Log("[PauseMenuUI] Destroying GameManager");
                Destroy(GameManager.Instance.gameObject);
            }
            
            // 5. Destroy WaveManager
            if (WaveManager.Instance != null)
            {
                Debug.Log("[PauseMenuUI] Destroying WaveManager");
                Destroy(WaveManager.Instance.gameObject);
            }
            
            // 6. Destroy PlasmaManager
            if (PlasmaManager.Instance != null)
            {
                Debug.Log("[PauseMenuUI] Destroying PlasmaManager");
                Destroy(PlasmaManager.Instance.gameObject);
            }
            
            // 7. Destroy LeakManager
            if (LeakManager.Instance != null)
            {
                Debug.Log("[PauseMenuUI] Destroying LeakManager");
                Destroy(LeakManager.Instance.gameObject);
            }
            
            // 8. Destroy PlacementGridManager
            if (PlacementGridManager.Instance != null)
            {
                Debug.Log("[PauseMenuUI] Destroying PlacementGridManager");
                Destroy(PlacementGridManager.Instance.gameObject);
            }
            
            // 9. Destroy semua tower yang ada di scene
            DestroyAllTowers();
            
            // 10. Destroy semua enemy yang masih ada
            DestroyAllEnemies();
        }
        
        /// <summary>
        /// Destroy semua tower yang ada di scene
        /// </summary>
        private void DestroyAllTowers()
        {
            MicroDefense.Towers.TowerBase[] towers = FindObjectsByType<MicroDefense.Towers.TowerBase>();
            foreach (var tower in towers)
            {
                Debug.Log($"[PauseMenuUI] Destroying tower: {tower.name}");
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
                Debug.Log($"[PauseMenuUI] Destroying enemy: {enemy.name}");
                Destroy(enemy.gameObject);
            }
        }

        private void OnExitClicked()
        {
            Debug.Log("[PauseMenuUI] Exit button clicked - Going to main menu...");
            
            // PENTING: Destroy semua GameObject persistent sebelum ke main menu
            DestroyAllPersistentObjects();
            
            // Resume time sebelum load scene
            Time.timeScale = 1f;
            
            // Load main menu
            SceneManager.LoadScene("MainMenu");
        }

        private void OnDestroy()
        {
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(OnResumeClicked);
            
            if (restartButton != null)
                restartButton.onClick.RemoveListener(OnRestartClicked);
            
            if (exitButton != null)
                exitButton.onClick.RemoveListener(OnExitClicked);
        }
    }
}

