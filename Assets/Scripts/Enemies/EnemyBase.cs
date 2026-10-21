using System;
using System.Collections;
using MicroDefense.Core;
using MicroDefense.Data;
using UnityEngine;

namespace MicroDefense.Enemies
{
    public class EnemyBase : MonoBehaviour
    {
        [Header("Enemy Configuration")]
        [SerializeField] protected EnemyDataSO enemyData;
        public EnemyDataSO Data => enemyData;

        [Header("Runtime Stats")]
        public float currentHealth;
        public float currentSpeed;
        public float currentArmor;
        public bool IsBlocked { get; private set; }

        // Status Effects
        private bool isSlowed;
        private float slowTimer;
        private float slowFactor = 1.0f; // e.g. 0.70 for 30% slow

        private bool hasArmorShred;
        private float armorShredTimer;
        private float armorShredFactor = 0f; // e.g. 0.25 for 25% shred

        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action OnEnemyDied;

        protected virtual void Awake()
        {
            if (enemyData != null)
            {
                Initialize(enemyData);
            }
        }

        public virtual void SetBlocked(bool blocked)
        {
            IsBlocked = blocked;
        }

        public virtual void Initialize(EnemyDataSO data)
        {
            enemyData = data;
            currentHealth = data.baseHealth;
            currentSpeed = data.moveSpeed;
            currentArmor = data.flatArmor;

            OnHealthChanged?.Invoke(currentHealth, enemyData.baseHealth);
        }

        protected virtual void Update()
        {
            HandleStatusEffects();
        }

        private void HandleStatusEffects()
        {
            // Slow Effect Update
            if (isSlowed)
            {
                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0f)
                {
                    isSlowed = false;
                    slowFactor = 1.0f;
                    currentSpeed = enemyData.moveSpeed;
                }
            }

            // Armor Shred Update
            if (hasArmorShred)
            {
                armorShredTimer -= Time.deltaTime;
                if (armorShredTimer <= 0f)
                {
                    hasArmorShred = false;
                    armorShredFactor = 0f;
                    currentArmor = enemyData.flatArmor;
                }
            }
        }

        public virtual void TakeDamage(float rawDamage, bool isTrueDamage = false)
        {
            if (currentHealth <= 0) return;

            float finalDamage;

            if (isTrueDamage)
            {
                finalDamage = rawDamage;
            }
            else
            {
                float effectiveArmor = currentArmor * (1.0f - armorShredFactor);
                finalDamage = Mathf.Max(1.0f, rawDamage - effectiveArmor);
            }

            currentHealth -= finalDamage;
            OnHealthChanged?.Invoke(currentHealth, enemyData.baseHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void ApplySlow(float slowPercent, float duration)
        {
            isSlowed = true;
            slowTimer = duration;
            slowFactor = Mathf.Clamp01(1.0f - slowPercent);
            currentSpeed = enemyData.moveSpeed * slowFactor;
        }

        public void ApplyArmorShred(float shredPercent, float duration)
        {
            hasArmorShred = true;
            armorShredTimer = duration;
            armorShredFactor = shredPercent;
            currentArmor = enemyData.flatArmor * (1.0f - armorShredFactor);
        }

        protected virtual void Die()
        {
            if (enemyData != null && PlasmaManager.Instance != null)
            {
                PlasmaManager.Instance.SpawnDrop(transform.position, enemyData.plasmaReward);
            }

            OnEnemyDied?.Invoke();
            Destroy(gameObject);
        }

        public virtual void OnReachedEndPath()
        {
            if (LeakManager.Instance != null && enemyData != null)
            {
                // Gunakan damage dari enemyData, default 1 jika tidak ada
                int damage = Mathf.RoundToInt(enemyData.damageToCell);
                if (damage <= 0) damage = 1;
                
                LeakManager.Instance.RegisterLeak(damage);
                Debug.Log($"[EnemyBase] {enemyData.enemyName} reached target! Dealt {damage} damage to player.");
            }
            
            OnEnemyDied?.Invoke(); // Trigger event untuk WaveManager
            Destroy(gameObject);
        }
    }
}
