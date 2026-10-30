using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MicroDefense.UI
{
    public class WaveProgressBar : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image frontBar;
        [SerializeField] private RectTransform backBar;
        [SerializeField] private Image virus;

        [Header("Sizing")]
        [SerializeField] private float barScale = 1f;
        [SerializeField] private float virusScale = 1f;
        [SerializeField] private float positionOffsetX = 0f; // Offset untuk adjust virus position

        [Header("Behavior")]
        [SerializeField] private float smoothDuration = 0.5f;

        [Header("Animation")]
        [SerializeField] private bool useIdleBob = true;
        [SerializeField] private float bobAmount = 0.1f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private bool usePopEffect = true;
        [SerializeField] private float popDuration = 0.3f;
        [SerializeField] private float popScale = 1.2f;

        [Header("Debug")]
        [SerializeField] private int debugCurrentWave = 1;
        [SerializeField] private int debugTotalWave = 4;

        private int currentWave = 1;
        private int totalWave = 4;
        private float displayedProgress = 0f;
        private float targetProgress = 0f;
        private Coroutine smoothCoroutine;
        private Coroutine bobCoroutine;
        private Vector3 virusBasePosition; // Track base position during bob animation
        private Vector3 virusOriginalScale;
        private bool hasReachedFinal = false;

        public event System.Action OnReachedFinalWave;

        private void Awake()
        {
            ValidateReferences();
            ApplySizesFromSprites();
            SetWaveInstant(currentWave, totalWave);
            virusOriginalScale = virus.rectTransform.localScale;
        }

        private void OnEnable()
        {
            if (useIdleBob && virus != null)
            {
                if (bobCoroutine != null) StopCoroutine(bobCoroutine);
                bobCoroutine = StartCoroutine(BobVirus());
            }
        }

        private void OnDisable()
        {
            if (bobCoroutine != null)
            {
                StopCoroutine(bobCoroutine);
                bobCoroutine = null;
            }
        }

        private void OnValidate()
        {
            if (backBar == null || frontBar == null || virus == null) return;
            
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ApplySizesFromSprites();
                
                // Debug preview
                if (debugCurrentWave > 0 && debugTotalWave > 0)
                {
                    int clampedCurrent = Mathf.Clamp(debugCurrentWave, 1, debugTotalWave);
                    float debugProgress = debugTotalWave <= 1 ? 1f : (clampedCurrent - 1f) / (debugTotalWave - 1f);
                    UpdateProgressVisuals(debugProgress, 0f); // Instant update
                }
            }
            #endif
        }

        public void ApplySizesFromSprites()
        {
            if (backBar == null || frontBar == null || virus == null) return;

            Sprite backSprite = backBar.GetComponent<Image>().sprite;
            Sprite frontSprite = frontBar.GetComponent<Image>().sprite;
            Sprite virusSprite = virus.sprite;

            if (backSprite == null || frontSprite == null || virusSprite == null)
            {
                Debug.LogWarning("[WaveProgressBar] Missing sprites. Cannot apply sizes.");
                return;
            }

            // Get sprite rect sizes
            float backWidth = backSprite.rect.width;
            float backHeight = backSprite.rect.height;
            float virusWidth = virusSprite.rect.width;
            float virusHeight = virusSprite.rect.height;

            // Apply bar sizes
            backBar.sizeDelta = new Vector2(backWidth * barScale, backHeight * barScale);
            frontBar.rectTransform.sizeDelta = new Vector2(backWidth * barScale, backHeight * barScale);

            // Apply virus size (maintain ratio)
            float scale = (backHeight * barScale) / backHeight * virusScale;
            virus.rectTransform.sizeDelta = new Vector2(virusWidth * scale, virusHeight * scale);

            // Set virus pivot to center
            virus.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public void SetWave(int currentWaveIndex, int totalWaveCount)
        {
            currentWave = Mathf.Clamp(currentWaveIndex, 1, Mathf.Max(1, totalWaveCount));
            totalWave = Mathf.Max(1, totalWaveCount);

            float newProgress = totalWave <= 1 ? 1f : (currentWave - 1f) / (totalWave - 1f);
            targetProgress = Mathf.Clamp01(newProgress);

            // Smooth animate to new progress
            if (smoothCoroutine != null) StopCoroutine(smoothCoroutine);
            smoothCoroutine = StartCoroutine(SmoothUpdateProgress(targetProgress, smoothDuration));

            // Check if reached final wave
            if (currentWave >= totalWave && !hasReachedFinal)
            {
                hasReachedFinal = true;
                OnReachedFinalWave?.Invoke();
            }
        }

        public void SetWaveInstant(int currentWaveIndex, int totalWaveCount)
        {
            currentWave = Mathf.Clamp(currentWaveIndex, 1, Mathf.Max(1, totalWaveCount));
            totalWave = Mathf.Max(1, totalWaveCount);

            float newProgress = totalWave <= 1 ? 1f : (currentWave - 1f) / (totalWave - 1f);
            displayedProgress = Mathf.Clamp01(newProgress);
            targetProgress = displayedProgress;

            UpdateProgressVisuals(displayedProgress, 0f);

            // Check if reached final wave
            if (currentWave >= totalWave && !hasReachedFinal)
            {
                hasReachedFinal = true;
                OnReachedFinalWave?.Invoke();
            }
        }

        private IEnumerator SmoothUpdateProgress(float targetProg, float duration)
        {
            float elapsed = 0f;
            float startProgress = displayedProgress;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                displayedProgress = Mathf.Lerp(startProgress, targetProg, t);
                UpdateProgressVisuals(displayedProgress, duration);

                yield return null;
            }

            displayedProgress = targetProg;
            UpdateProgressVisuals(displayedProgress, 0f);

            // Pop effect on wave change
            if (usePopEffect)
            {
                yield return StartCoroutine(PopVirus());
            }
        }

        private void UpdateProgressVisuals(float progress, float animDuration)
        {
            if (frontBar == null || virus == null || backBar == null) return;

            // Update fill amount
            frontBar.fillAmount = progress;

            // Calculate virus position - SIMPLE: just follow the fill bar end
            float barWidth = backBar.rect.width;
            
            // Calculate position from -width/2 to +width/2
            float minX = -barWidth / 2f;
            float maxX = barWidth / 2f;
            
            // Lerp virus position to follow fill bar
            float newX = Mathf.Lerp(minX, maxX, progress) + positionOffsetX;

            // Update base position (for bob animation to use)
            virusBasePosition = new Vector2(newX, 0f);
            
            // If not bobbing, directly set position
            if (bobCoroutine == null)
            {
                virus.rectTransform.anchoredPosition = virusBasePosition;
            }
            
            Debug.Log($"[WaveProgressBar] Progress: {progress}, NewX: {newX}, Offset: {positionOffsetX}");
        }

        private IEnumerator BobVirus()
        {
            float time = 0f;

            while (true)
            {
                time += Time.deltaTime;
                float bobY = Mathf.Sin(time * bobSpeed * Mathf.PI) * bobAmount;
                // Always use virusBasePosition as the center point for bobbing
                virus.rectTransform.anchoredPosition = virusBasePosition + new Vector3(0, bobY, 0);
                yield return null;
            }
        }

        private IEnumerator PopVirus()
        {
            float elapsed = 0f;
            Vector3 originalScale = virusOriginalScale;
            Vector3 startScale = originalScale * popScale;

            while (elapsed < popDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / popDuration);
                float scale = Mathf.Lerp(popScale, 1f, t);
                virus.rectTransform.localScale = originalScale * scale;
                yield return null;
            }

            virus.rectTransform.localScale = originalScale;
        }

        private void ValidateReferences()
        {
            if (frontBar == null) Debug.LogError("[WaveProgressBar] frontBar is not assigned!");
            if (backBar == null) Debug.LogError("[WaveProgressBar] backBar is not assigned!");
            if (virus == null) Debug.LogError("[WaveProgressBar] virus is not assigned!");
        }

        // Context menu for manual size application
        [ContextMenu("Apply Sizes From Sprites")]
        private void ContextMenuApplySizes()
        {
            ApplySizesFromSprites();
        }
    }
}
