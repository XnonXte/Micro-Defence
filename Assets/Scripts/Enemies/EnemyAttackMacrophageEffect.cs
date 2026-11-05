using UnityEngine;
using System.Collections;
using MicroDefense.Core;

namespace MicroDefense.Enemies
{
    /// <summary>
    /// Efek continuous shake/rotation pada enemy saat sedang menyerang Macrophage
    /// Enemy akan bergetar/berputar selama mereka di-block oleh Macrophage
    /// </summary>
    public class EnemyAttackMacrophageEffect : MonoBehaviour
    {
        [Header("Attack Animation Settings")]
        [SerializeField] private bool enableAttackShake = true;
        [SerializeField] private float shakeIntensity = 0.15f;
        [SerializeField] private float shakeSpeed = 2f; // Pelan untuk smooth bobbing
        
        [Header("Attack Rotation (Chomping Effect)")]
        [SerializeField] private bool enableAttackRotation = false; // DISABLED by default
        [SerializeField] private float rotationSpeed = 180f; // Degrees per second
        [SerializeField] private float rotationAmount = 15f; // Max rotation angle
        
        [Header("Scale Pulsing (Breathing Effect)")]
        [SerializeField] private bool enableScalePulse = false; // Disabled untuk lebih clean
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private float pulseAmount = 0.08f;
        
        [Header("Attack Sound")]
        [SerializeField] private bool enableAttackSound = true;
        [SerializeField] private float soundInterval = 0.5f; // Play sound every 0.5s
        
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;
        
        private EnemyBase enemyBase;
        private bool isAttacking = false;
        private float attackTime = 0f;
        private float lastSoundTime = 0f;
        
        private void Start()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
            
            enemyBase = GetComponent<EnemyBase>();
        }
        
        private void Update()
        {
            if (enemyBase == null) return;
            
            // Check jika enemy sedang di-block (attacking Macrophage)
            bool wasAttacking = isAttacking;
            isAttacking = enemyBase.IsBlocked;
            
            if (isAttacking)
            {
                // Play attack sound saat mulai attack
                if (!wasAttacking && enableAttackSound)
                {
                    PlayAttackSound();
                    lastSoundTime = Time.time;
                }
                
                // Play sound periodically during attack
                if (enableAttackSound && Time.time - lastSoundTime >= soundInterval)
                {
                    PlayAttackSound();
                    lastSoundTime = Time.time;
                }
                
                attackTime += Time.deltaTime;
                ApplyAttackEffects();
            }
            else
            {
                // Reset ke posisi/rotasi normal saat tidak attack
                if (wasAttacking)
                {
                    ResetTransform();
                }
            }
        }
        
        /// <summary>
        /// Play enemy hit sound (attacking Macrophage)
        /// </summary>
        private void PlayAttackSound()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEnemyHit();
            }
        }
        
        /// <summary>
        /// Apply continuous attack effects (smooth bobbing motion)
        /// </summary>
        private void ApplyAttackEffects()
        {
            Vector3 targetPosition = originalPosition;
            Quaternion targetRotation = originalRotation;
            Vector3 targetScale = originalScale;
            
            // Smooth vertical bobbing (naik-turun pelan)
            if (enableAttackShake)
            {
                // Hanya gerakan vertikal (Y axis) yang smooth
                float bobbingY = Mathf.Sin(attackTime * shakeSpeed) * shakeIntensity;
                targetPosition += new Vector3(0f, bobbingY, 0f);
            }
            
            // Rotation (chomping/biting motion) - DISABLED by default
            if (enableAttackRotation)
            {
                // Oscillate rotation back and forth (like chomping)
                float rotationAngle = Mathf.Sin(attackTime * rotationSpeed * Mathf.Deg2Rad) * rotationAmount;
                targetRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
            }
            
            // Scale pulse (breathing/expanding while attacking) - DISABLED by default
            if (enableScalePulse)
            {
                float scaleMultiplier = 1f + Mathf.Sin(attackTime * pulseSpeed) * pulseAmount;
                targetScale = originalScale * scaleMultiplier;
            }
            
            // Apply all transforms
            transform.localPosition = targetPosition;
            transform.localRotation = targetRotation;
            transform.localScale = targetScale;
        }
        
        /// <summary>
        /// Reset transform to original (smooth)
        /// </summary>
        private void ResetTransform()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 10f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * 10f);
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 10f);
            attackTime = 0f;
        }
        
        /// <summary>
        /// Update original transform (untuk enemy yang bergerak)
        /// </summary>
        private void LateUpdate()
        {
            // Update reference position jika tidak sedang attacking
            if (!isAttacking)
            {
                originalPosition = transform.localPosition;
                originalRotation = transform.localRotation;
            }
        }
        
        private void OnDestroy()
        {
            // Cleanup
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            transform.localScale = originalScale;
        }
    }
}
