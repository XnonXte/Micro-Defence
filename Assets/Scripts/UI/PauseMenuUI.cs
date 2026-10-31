using MicroDefense.Core;
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
            // Unpause sebelum reload
            Time.timeScale = 1f;
            
            // Reload scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnExitClicked()
        {
            // Unpause sebelum exit
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

