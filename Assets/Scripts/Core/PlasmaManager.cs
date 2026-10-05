using System;
using UnityEngine;

namespace MicroDefense.Core
{
    public class PlasmaManager : MonoBehaviour
    {
        public static PlasmaManager Instance { get; private set; }

        [Header("Plasma Economy")]
        [SerializeField] private int startingPlasma = 300;
        public int CurrentPlasma { get; private set; }

        public event Action<int> OnPlasmaChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            CurrentPlasma = startingPlasma;
        }

        private void Start()
        {
            OnPlasmaChanged?.Invoke(CurrentPlasma);
        }

        public void AddPlasma(int amount)
        {
            if (amount <= 0) return;
            CurrentPlasma += amount;
            OnPlasmaChanged?.Invoke(CurrentPlasma);
        }

        public bool TrySpendPlasma(int amount)
        {
            if (amount <= 0) return true;
            if (CurrentPlasma >= amount)
            {
                CurrentPlasma -= amount;
                OnPlasmaChanged?.Invoke(CurrentPlasma);
                return true;
            }
            return false;
        }

        public bool CanAfford(int amount)
        {
            return CurrentPlasma >= amount;
        }
    }
}
