using UnityEngine;
using System.Collections;

namespace MicroDefense.Enemies
{
    /// <summary>
    /// Menambahkan efek shake/getaran visual saat enemy menerima damage dari tower
    /// Universal script untuk semua jenis enemy (Common, Swarm, Armored, Mutant, Boss, dll)
    /// </summary>
    public class EnemyHitEffect : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private bool enableShake = true;
        [SerializeField] private float shakeDuration = 0.15f;
        [SerializeField] private float shakeIntensity = 0.08f;
        [SerializeField] private float shakeSpeed = 40f;
        
        [Header("Rotation Shake")]
        [SerializeField] private bool enableRotationShake = true;
        [SerializeField] private float rotationShakeAmount = 8f;
        
        [Header("Scale Pulse")]
        [SerializeField] private bool enableScalePulse = true;
        [SerializeField] private float scalePulseAmount = 0.08f;
        [SerializeField] private float scalePulseDuration = 0.12f;
        
        [Header("Color Flash")]
        [SerializeField] private bool enableColorFlash = true;
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashDuration = 0.1f;
        
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;
        private bool isShaking = false;
        
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        
        private EnemyBase enemyBase;
        private float lastHealth;
        
        private void Start()
        {
            // Store original transform values
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
            
            // Get sprite renderer untuk color flash
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            
            // Get EnemyBase component
            enemyBase = GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                lastHealth = enemyBase.currentHealth;
            }
        }
        
        private void Update()
        {
            // Check jika enemy menerima damage
            if (enemyBase != null)
            {
                if (enemyBase.currentHealth < lastHealth && enemyBase.currentHealth > 0)
                {
                    // Trigger shake effect
                    TriggerHitEffect();
                }
                lastHealth = enemyBase.currentHealth;
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
        /// Efek pulse scale (shrink saat kena hit)
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
        /// Efek flash color (white flash)
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
        /// Manual trigger dari luar (bisa dipanggil dari EnemyBase.PlayHitEffect())
        /// </summary>
        public void PlayHitEffect()
        {
            TriggerHitEffect();
        }
        
        /// <summary>
        /// Update original transform (untuk enemy yang bergerak)
        /// </summary>
        private void LateUpdate()
        {
            // Update original position setiap frame saat tidak sedang shake
            // Karena enemy bergerak, kita perlu update reference position
            if (!isShaking)
            {
                originalPosition = transform.localPosition;
                originalRotation = transform.localRotation;
            }
        }
        
        private void OnDestroy()
        {
            // Cleanup - reset transform dan stop semua coroutines
            StopAllCoroutines();
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }
}
