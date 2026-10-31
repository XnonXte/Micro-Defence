using MicroDefense.Core;
using UnityEngine;

namespace MicroDefense.UI
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        public void TogglePause()
        {
            // Play button click SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentState == GameState.Paused)
                {
                    Time.timeScale = 1f;
                    GameManager.Instance.ResumeGame();
                    if (pausePanel != null) pausePanel.SetActive(false);
                }
                else
                {
                    GameManager.Instance.PauseGame();
                    if (pausePanel != null) pausePanel.SetActive(true);
                    Time.timeScale = 0f;
                }
            }
        }

        public void Resume()
        {
            // Play button click SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            if (GameManager.Instance != null)
            {
                Time.timeScale = 1f;
                GameManager.Instance.ResumeGame();
                if (pausePanel != null) pausePanel.SetActive(false);
            }
        }
    }
}
