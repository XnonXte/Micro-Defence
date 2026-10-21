using System;
using UnityEngine;

namespace MicroDefense.Core
{
    /// <summary>
    /// PlayerHealth - Mengatur health player yang berkurang saat enemy sampai target
    /// Health berkurang berdasarkan damage dari setiap enemy
    /// </summary>
    public class LeakManager : MonoBehaviour
    {
        public static LeakManager Instance { get; private set; }

        [Header("Player Health Settings")]
        [SerializeField] private int maxHealth = 20;
        [SerializeField] private int currentHealth;
        
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        public event Action<int, int> OnLeakChanged; // (current, max) - untuk backward compatibility
        public event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
        public event Action OnDefeat;

        private bool defeatTriggered;
        private bool initialized;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            currentHealth = maxHealth;
            defeatTriggered = false;
            initialized = false;
        }

        private void Start()
        {
            currentHealth = maxHealth;
            defeatTriggered = false;
            initialized = true;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnLeakChanged?.Invoke(currentHealth, maxHealth); // backward compatibility
        }

        /// <summary>
        /// Dipanggil saat enemy sampai ke target. Damage dikurangi dari health.
        /// </summary>
        public void RegisterLeak(int damage = 1)
        {
            if (!initialized || defeatTriggered) return;
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.WaveInProgress)
                return;
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnLeakChanged?.Invoke(currentHealth, maxHealth); // backward compatibility

            Debug.Log($"[PlayerHealth] Enemy reached target! Damage: {damage}, Remaining Health: {currentHealth}/{maxHealth}");

            if (currentHealth <= 0)
            {
                TriggerDefeat();
            }
        }

        private void TriggerDefeat()
        {
            if (defeatTriggered) return;
            defeatTriggered = true;
            Debug.Log("[PlayerHealth] Player defeated! Health reached 0.");
            OnDefeat?.Invoke();
            
            // Show defeat screen
            if (CompleteUIManager.Instance != null)
            {
                CompleteUIManager.Instance.ShowDefeat();
            }
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameState.Defeat);
            }
        }

        /// <summary>
        /// Heal player (opsional untuk power-ups nanti)
        /// </summary>
        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnLeakChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Reset health ke max (untuk restart game)
        /// </summary>
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnLeakChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}
