using UnityEngine;
using UnityEngine.SceneManagement;

namespace MicroDefense.Core
{
    /// <summary>
    /// Automatically plays appropriate BGM based on the current scene
    /// Attach this to a GameObject in each scene that needs BGM
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        private void Start()
        {
            // Small delay to ensure AudioManager is ready
            Invoke(nameof(PlayBGMForCurrentScene), 0.1f);
        }
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Play BGM when scene is loaded
            PlayBGMForCurrentScene();
        }

        private void PlayBGMForCurrentScene()
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogError("[BGMManager] AudioManager.Instance is NULL! Make sure AudioManager exists in Main Menu and has DontDestroyOnLoad.");
                return;
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"[BGMManager] Current scene: '{currentSceneName}'");
            
            AudioClip bgmToPlay = null;

            // Determine which BGM to play based on scene name
            string sceneLower = currentSceneName.ToLower();
            
            if (sceneLower.Contains("menu") || sceneLower == "mainmenu")
            {
                bgmToPlay = AudioManager.Instance.mainMenuBGM;
                Debug.Log($"[BGMManager] Detected Main Menu scene. BGM clip assigned: {bgmToPlay != null}");
            }
            else if (sceneLower == "1" || sceneLower == "2" || sceneLower == "3" || 
                     sceneLower == "level1" || sceneLower == "level2" || sceneLower == "level3")
            {
                bgmToPlay = AudioManager.Instance.level1To5BGM;
                Debug.Log($"[BGMManager] Detected Level 1-3 scene. BGM clip assigned: {bgmToPlay != null}");
            }
            else if (sceneLower == "4" || sceneLower == "5" || 
                     sceneLower == "level4" || sceneLower == "level5")
            {
                bgmToPlay = AudioManager.Instance.level6To10BGM;
                Debug.Log($"[BGMManager] Detected Level 4-5 scene. BGM clip assigned: {bgmToPlay != null}");
            }
            else
            {
                Debug.LogWarning($"[BGMManager] No BGM mapping for scene: '{currentSceneName}'. Scene not recognized.");
                return;
            }

            // Play the BGM
            if (bgmToPlay != null)
            {
                Debug.Log($"[BGMManager] ✓ Playing BGM clip: {bgmToPlay.name}");
                AudioManager.Instance.PlayBGM(bgmToPlay);
            }
            else
            {
                Debug.LogError($"[BGMManager] ✗ BGM clip is NULL for scene '{currentSceneName}'! Go to Main Menu scene → Select AudioManager GameObject → Assign audio clips in Inspector.");
            }
        }
    }
}
