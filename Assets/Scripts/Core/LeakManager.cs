using System;
using UnityEngine;

namespace MicroDefense.Core
{
    /// <summary>
    /// LeakManager - Menghitung jumlah enemy yang lolos ke target
    /// Start dari 0 leak, naik setiap enemy lolos, kalah saat leak = maxCapacity
    /// </summary>
    public class LeakManager : MonoBehaviour
    {
        public static LeakManager Instance { get; private set; }

        [Header("Leak Capacity Settings")]
        [SerializeField] private int maxLeakCapacity = 20;
        [SerializeField] private int currentLeakCount;
        
        public int CurrentLeakCount => currentLeakCount;
        public int MaxLeakCapacity => maxLeakCapacity;

        public event Action<int, int> OnLeakChanged; // (currentLeak, maxCapacity)
        public event Action<int, int> OnHealthChanged; // Backward compatibility - maps to LeakChanged
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
            currentLeakCount = 0; // Start dengan 0 leak
            defeatTriggered = false;
            initialized = false;
        }

        private void Start()
        {
            currentLeakCount = 0;
            defeatTriggered = false;
            initialized = true;
            OnLeakChanged?.Invoke(currentLeakCount, maxLeakCapacity);
            OnHealthChanged?.Invoke(currentLeakCount, maxLeakCapacity); // Backward compatibility
        }

        /// <summary>
        /// Dipanggil saat enemy lolos ke target. Leak count bertambah 1.
        /// </summary>
        public void RegisterLeak(int damage = 1)
        {
            if (!initialized || defeatTriggered) return;
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.WaveInProgress)
                return;
            
            currentLeakCount += damage;
            if (currentLeakCount > maxLeakCapacity) currentLeakCount = maxLeakCapacity;

            OnLeakChanged?.Invoke(currentLeakCount, maxLeakCapacity);
            OnHealthChanged?.Invoke(currentLeakCount, maxLeakCapacity); // Backward compatibility

            AudioManager.Instance?.PlayEnemyLolos();

            Debug.Log($"[LeakManager] Enemy reached target! Leaks: {currentLeakCount}/{maxLeakCapacity}");

            // Kalah saat leak mencapai kapasitas maksimal
            if (currentLeakCount >= maxLeakCapacity)
            {
                TriggerDefeat();
            }
        }

        private void TriggerDefeat()
        {
            if (defeatTriggered) return;
            defeatTriggered = true;
            Debug.Log("[LeakManager] Leak capacity reached! Player defeated!");
            
            AudioManager.Instance?.PlayGameOver();

            OnDefeat?.Invoke();
            
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
        /// Reduce leak count (opsional untuk power-ups)
        /// </summary>
        public void ReduceLeak(int amount)
        {
            currentLeakCount -= amount;
            if (currentLeakCount < 0) currentLeakCount = 0;

            OnLeakChanged?.Invoke(currentLeakCount, maxLeakCapacity);
            OnHealthChanged?.Invoke(currentLeakCount, maxLeakCapacity);
        }

        /// <summary>
        /// Reset leak count (untuk restart game)
        /// </summary>
        public void ResetLeaks()
        {
            currentLeakCount = 0;
            defeatTriggered = false;
            OnLeakChanged?.Invoke(currentLeakCount, maxLeakCapacity);
            OnHealthChanged?.Invoke(currentLeakCount, maxLeakCapacity);
        }
    }
}
