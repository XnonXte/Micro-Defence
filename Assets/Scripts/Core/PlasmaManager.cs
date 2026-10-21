using System;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Core
{
    public class PlasmaManager : MonoBehaviour
    {
        public static PlasmaManager Instance { get; private set; }

        [Header("Plasma Economy")]
        [SerializeField] private int startingPlasma = 300;
        [SerializeField] private float dropScale = 0.45f;
        public int CurrentPlasma { get; private set; }
        public float DropScale => dropScale;

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

        public void SpawnDrop(Vector3 position, int value)
        {
            GameObject drop = new GameObject("PlasmaDrop");
            drop.transform.position = position;
            drop.transform.localScale = Vector3.one * Mathf.Max(0.01f, dropScale);

            SpriteRenderer renderer = drop.AddComponent<SpriteRenderer>();
            renderer.sprite = Resources.Load<Sprite>("plasma");
            renderer.sortingOrder = 20;

            if (renderer.sprite == null)
            {
                Destroy(drop);
                Debug.LogWarning("PlasmaManager: Resources/plasma.png was not found.");
                return;
            }

            CircleCollider2D collider = drop.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = false;

            var pickup = drop.AddComponent<PlasmaDrop>();
            var valueField = typeof(PlasmaDrop).GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            valueField?.SetValue(pickup, Mathf.Max(1, value));
        }
    }
}
