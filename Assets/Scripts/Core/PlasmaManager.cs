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
        [SerializeField] private float dropLifetime = 8f;
        [SerializeField] private float maxDropSpawnRadius = 2.5f;
        public int CurrentPlasma { get; private set; }
        public float DropScale => dropScale;
        public float DropLifetime => dropLifetime;
        public float MaxDropSpawnRadius => maxDropSpawnRadius;

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
            SpawnDrop(position, value, dropLifetime, maxDropSpawnRadius);
        }

        public void SpawnDrop(Vector3 position, int value, float lifetime)
        {
            SpawnDrop(position, value, lifetime, maxDropSpawnRadius);
        }

        public void SpawnDrop(Vector3 position, int value, float lifetime, float spawnRadius)
        {
            float safeLifetime = Mathf.Max(0.25f, lifetime);
            float safeRadius = Mathf.Max(0f, spawnRadius);
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * safeRadius;
            Vector3 dropPosition = new Vector3(position.x + randomOffset.x, position.y + randomOffset.y, position.z);

            GameObject drop = new GameObject("PlasmaDrop");
            drop.transform.position = dropPosition;
            drop.transform.localScale = Vector3.one * Mathf.Max(0.01f, dropScale);
            
            // Set layer ke TransparentFX (layer 1) - layer ini tidak digunakan untuk enemy detection
            // Enemy spacing check hanya melihat layer "Enemy", jadi plasma drop tidak akan terdeteksi
            drop.layer = 1; // Layer 1 = "TransparentFX"

            SpriteRenderer renderer = drop.AddComponent<SpriteRenderer>();
            renderer.sprite = Resources.Load<Sprite>("plasma");
            renderer.sortingOrder = 150; // Set tinggi agar selalu di atas semua virus (virus biasanya 10-50)

            if (renderer.sprite == null)
            {
                Destroy(drop);
                Debug.LogWarning("PlasmaManager: Resources/plasma.png was not found.");
                return;
            }

            // Tambahkan collider sebagai trigger agar OnMouseDown() bisa bekerja
            // Layer TransparentFX membuat ini tidak terdeteksi oleh enemy spacing check
            CircleCollider2D collider = drop.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;

            var pickup = drop.AddComponent<PlasmaDrop>();
            pickup.Initialize(Mathf.Max(1, value), safeLifetime);
        }
    }
}
