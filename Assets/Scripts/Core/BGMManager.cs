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
                Debug.LogError("[BGMManager] AudioManager.Instance is NULL! Make sure AudioManager exists in scene.");
                return;
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"[BGMManager] Current scene: '{currentSceneName}'");
            
            AudioClip bgmToPlay = null;

            // Determine which BGM to play based on scene name
            switch (currentSceneName.ToLower())
            {
                case "mainmenu":
                case "main menu":
                case "menu":
                    bgmToPlay = AudioManager.Instance.mainMenuBGM;
                    Debug.Log($"[BGMManager] Playing Main Menu BGM. Clip assigned: {bgmToPlay != null}");
                    break;

                case "1":
                case "2":
                case "3":
                case "level1":
                case "level2":
                case "level3":
                    bgmToPlay = AudioManager.Instance.level1To5BGM;
                    Debug.Log($"[BGMManager] Playing Level 1-5 BGM for scene: {currentSceneName}");
                    break;

                case "4":
                case "5":
                case "level4":
                case "level5":
                    bgmToPlay = AudioManager.Instance.level6To10BGM;
                    Debug.Log($"[BGMManager] Playing Level 6-10 BGM for scene: {currentSceneName}");
                    break;

                default:
                    Debug.LogWarning($"[BGMManager] No BGM mapping for scene: '{currentSceneName}'");
                    break;
            }

            // Play the BGM
            if (bgmToPlay != null)
            {
                Debug.Log($"[BGMManager] Playing BGM clip: {bgmToPlay.name}");
                AudioManager.Instance.PlayBGM(bgmToPlay);
            }
            else
            {
                Debug.LogError($"[BGMManager] BGM clip is NULL for scene '{currentSceneName}'! Assign audio clip to AudioManager.");
            }
        }
    }
}
