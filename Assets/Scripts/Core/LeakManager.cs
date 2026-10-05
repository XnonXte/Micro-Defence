using System;
using UnityEngine;

namespace MicroDefense.Core
{
    public class LeakManager : MonoBehaviour
    {
        public static LeakManager Instance { get; private set; }

        [Header("Leak Rules (GDD v0.1.1 Wireframe: 5 max leaks)")]
        [SerializeField] private int maxLeaks = 5;
        public int CurrentLeaks { get; private set; }
        public int MaxLeaks => maxLeaks;

        public event Action<int, int> OnLeakChanged; // (current, max)
        public event Action OnDefeat;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            CurrentLeaks = 0;
        }

        private void Start()
        {
            OnLeakChanged?.Invoke(CurrentLeaks, maxLeaks);
        }

        public void RegisterLeak(int leakAmount = 1)
        {
            CurrentLeaks += leakAmount;
            OnLeakChanged?.Invoke(CurrentLeaks, maxLeaks);

            if (CurrentLeaks >= maxLeaks)
            {
                OnDefeat?.Invoke();
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Defeat);
                }
            }
        }
    }
}
