using UnityEngine;
using UnityEngine.SceneManagement;

namespace MicroDefense.Core
{
    /// <summary>
    /// Automatically plays appropriate BGM based on the current scene
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        private void Start()
        {
            PlayBGMForCurrentScene();
        }

        private void PlayBGMForCurrentScene()
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("[BGMManager] AudioManager not found!");
                return;
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            AudioClip bgmToPlay = null;

            // Determine which BGM to play based on scene name
            switch (currentSceneName.ToLower())
            {
                case "MainMenu":
                case "main menu":
                case "menu":
                    bgmToPlay = AudioManager.Instance.mainMenuBGM;
                    Debug.Log("[BGMManager] Playing Main Menu BGM");
                    break;

                case "1":
                case "2":
                case "3":
                    bgmToPlay = AudioManager.Instance.level1To5BGM;
                    Debug.Log($"[BGMManager] Playing Level 1-5 BGM for scene: {currentSceneName}");
                    break;

                case "4":
                case "5":
                    bgmToPlay = AudioManager.Instance.level6To10BGM;
                    Debug.Log($"[BGMManager] Playing Level 6-10 BGM for scene: {currentSceneName}");
                    break;

                default:
                    Debug.LogWarning($"[BGMManager] No BGM mapping for scene: {currentSceneName}");
                    break;
            }

            // Play the BGM
            if (bgmToPlay != null)
            {
                AudioManager.Instance.PlayBGM(bgmToPlay);
            }
            else
            {
                Debug.LogWarning("[BGMManager] BGM clip is null, cannot play");
            }
        }
    }
}
