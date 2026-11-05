using UnityEngine;
using System.Collections;

namespace MicroDefense.Towers
{
    /// <summary>
    /// Menambahkan efek shake/getaran visual saat Macrophage menerima damage
    /// </summary>
    public class MacrophageHitEffect : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private bool enableShake = true;
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private float shakeIntensity = 0.1f;
        [SerializeField] private float shakeSpeed = 30f;
        
        [Header("Rotation Shake (Optional)")]
        [SerializeField] private bool enableRotationShake = true;
        [SerializeField] private float rotationShakeAmount = 10f;
        
        [Header("Scale Pulse (Optional)")]
        [SerializeField] private bool enableScalePulse = true;
        [SerializeField] private float scalePulseAmount = 0.1f;
        [SerializeField] private float scalePulseDuration = 0.15f;
        
        [Header("Color Flash (Optional)")]
        [SerializeField] private bool enableColorFlash = true;
        [SerializeField] private Color flashColor = new Color(1f, 0.5f, 0.5f, 1f); // Light red
        [SerializeField] private float flashDuration = 0.1f;
        
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;
        private bool isShaking = false;
        
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        
        private MacrophageTower macrophageTower;
        private float lastHealth;
        
        private void Start()
        {
            // Store original transform values
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
            
            // Get sprite renderer untuk color flash
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            
            // Get MacrophageTower component
            macrophageTower = GetComponent<MacrophageTower>();
            if (macrophageTower != null)
            {
                lastHealth = macrophageTower.currentHealth;
            }
        }
        
        private void Update()
        {
            // Check jika Macrophage menerima damage
            if (macrophageTower != null)
            {
                if (macrophageTower.currentHealth < lastHealth)
                {
                    // Trigger shake effect
                    TriggerHitEffect();
                }
                lastHealth = macrophageTower.currentHealth;
            }
        }
        
        /// <summary>
        /// Trigger semua efek saat hit
        /// </summary>
        public void TriggerHitEffect()
        {
            if (enableShake && !isShaking)
            {
                StartCoroutine(ShakeCoroutine());
            }
            
            if (enableScalePulse)
            {
                StartCoroutine(ScalePulseCoroutine());
            }
            
            if (enableColorFlash && spriteRenderer != null)
            {
                StartCoroutine(ColorFlashCoroutine());
            }
        }
        
        /// <summary>
        /// Efek shake position + rotation
        /// </summary>
        private IEnumerator ShakeCoroutine()
        {
            isShaking = true;
            float elapsed = 0f;
            
            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / shakeDuration;
                
                // Intensitas berkurang seiring waktu
                float currentIntensity = shakeIntensity * (1f - progress);
                
                // Random offset untuk shake
                float offsetX = Mathf.Sin(Time.time * shakeSpeed) * currentIntensity;
                float offsetY = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * currentIntensity;
                
                transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);
                
                // Rotation shake
                if (enableRotationShake)
                {
                    float rotationOffset = Mathf.Sin(Time.time * shakeSpeed * 2f) * rotationShakeAmount * (1f - progress);
                    transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, rotationOffset);
                }
                
                yield return null;
            }
            
            // Reset ke posisi original
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            isShaking = false;
        }
        
        /// <summary>
        /// Efek pulse scale
        /// </summary>
        private IEnumerator ScalePulseCoroutine()
        {
            float elapsed = 0f;
            
            while (elapsed < scalePulseDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / scalePulseDuration;
                
                // Ease out quad
                float easeProgress = 1f - (1f - progress) * (1f - progress);
                
                // Scale down lalu kembali normal
                float scale = 1f - (scalePulseAmount * (1f - easeProgress));
                transform.localScale = originalScale * scale;
                
                yield return null;
            }
            
            // Reset ke scale original
            transform.localScale = originalScale;
        }
        
        /// <summary>
        /// Efek flash color
        /// </summary>
        private IEnumerator ColorFlashCoroutine()
        {
            if (spriteRenderer == null) yield break;
            
            spriteRenderer.color = flashColor;
            
            float elapsed = 0f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / flashDuration;
                
                spriteRenderer.color = Color.Lerp(flashColor, originalColor, progress);
                
                yield return null;
            }
            
            spriteRenderer.color = originalColor;
        }
        
        /// <summary>
        /// Manual trigger dari luar (bisa dipanggil dari script lain)
        /// </summary>
        public void PlayHitEffect()
        {
            TriggerHitEffect();
        }
        
        /// <summary>
        /// Update original position (jika Macrophage bergerak/berrotasi)
        /// </summary>
        public void UpdateOriginalTransform()
        {
            if (!isShaking)
            {
                originalPosition = transform.localPosition;
                originalRotation = transform.localRotation;
                originalScale = transform.localScale;
            }
        }
        
        private void OnDestroy()
        {
            // Cleanup - reset transform
            StopAllCoroutines();
        }
    }
}
