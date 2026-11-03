using System.Collections;
using System.Collections.Generic;
using MicroDefense.Data;
using UnityEngine;
using UnityEngine.AI;

namespace MicroDefense.Enemies
{
    /// <summary>
    /// Omega Virus - Boss enemy yang bisa spawn minion virus di sekitarnya
    /// </summary>
    public class OmegaVirus : EnemyBase
    {
        [Header("Omega Boss Settings")]
        [SerializeField] private bool enableSpawning = true;
        
        [Header("Spawn Configuration")]
        [Tooltip("Jumlah minion yang akan di-spawn setiap interval")]
        [SerializeField] private int minionsPerSpawn = 3;
        
        [Tooltip("Interval waktu antara spawn (detik)")]
        [SerializeField] private float spawnInterval = 5f;
        
        [Tooltip("Radius spawn di sekitar Omega (meter)")]
        [SerializeField] private float spawnRadius = 3f;
        
        [Tooltip("Spawn saat HP turun ke threshold (0-1). Contoh: 0.75 = spawn saat HP 75%")]
        [SerializeField] private float[] healthThresholds = new float[] { 0.75f, 0.5f, 0.25f };
        
        [Header("Minion Types")]
        [Tooltip("List prefab virus yang bisa di-spawn (Common, Swarm, Armored, Mutant)")]
        [SerializeField] private List<EnemyDataSO> minionTypes = new List<EnemyDataSO>();
        
        [Header("Spawn Visual Effect")]
        [SerializeField] private GameObject spawnEffectPrefab;
        [SerializeField] private float spawnEffectDuration = 0.5f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;
        
        // Runtime
        private float nextSpawnTime;
        private int currentThresholdIndex = 0;
        private HashSet<int> triggeredThresholds = new HashSet<int>();
        private bool isSpawning = false;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (showDebugLogs)
            {
                Debug.Log($"[OmegaVirus.Awake] Initialized. enableSpawning: {enableSpawning}, minionTypes.Count: {minionTypes.Count}, spawnInterval: {spawnInterval}");
            }
            
            if (enableSpawning && spawnInterval > 0)
            {
                nextSpawnTime = Time.time + spawnInterval;
                if (showDebugLogs)
                    Debug.Log($"[OmegaVirus.Awake] Next spawn scheduled at Time: {nextSpawnTime}");
            }
            else
            {
                if (showDebugLogs)
                    Debug.LogWarning($"[OmegaVirus.Awake] Spawning disabled or invalid interval. enableSpawning: {enableSpawning}, spawnInterval: {spawnInterval}");
            }
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (!enableSpawning)
            {
                if (showDebugLogs && Time.frameCount % 300 == 0) // Log every 5 seconds at 60fps
                    Debug.LogWarning("[OmegaVirus.Update] Spawning is disabled!");
                return;
            }
            
            if (minionTypes.Count == 0)
            {
                if (showDebugLogs && Time.frameCount % 300 == 0)
                    Debug.LogWarning("[OmegaVirus.Update] Minion types list is empty!");
                return;
            }
            
            // Check health threshold spawning
            CheckHealthThresholdSpawning();
            
            // Check interval spawning
            if (spawnInterval > 0 && Time.time >= nextSpawnTime)
            {
                if (showDebugLogs)
                    Debug.Log($"[OmegaVirus.Update] Spawn time reached! Current time: {Time.time}, Next spawn was: {nextSpawnTime}");
                
                StartCoroutine(SpawnMinions(minionsPerSpawn));
                nextSpawnTime = Time.time + spawnInterval;
                
                if (showDebugLogs)
                    Debug.Log($"[OmegaVirus.Update] Next spawn scheduled at: {nextSpawnTime}");
            }
        }
        
        /// <summary>
        /// Cek apakah HP sudah mencapai threshold untuk spawn
        /// </summary>
        private void CheckHealthThresholdSpawning()
        {
            if (enemyData == null) return;
            
            float healthPercent = currentHealth / enemyData.baseHealth;
            
            for (int i = 0; i < healthThresholds.Length; i++)
            {
                // Cek apakah threshold belum triggered dan HP sudah dibawah threshold
                if (!triggeredThresholds.Contains(i) && healthPercent <= healthThresholds[i])
                {
                    triggeredThresholds.Add(i);
                    
                    if (showDebugLogs)
                        Debug.Log($"[OmegaVirus] Health threshold {healthThresholds[i] * 100}% triggered! Spawning extra minions.");
                    
                    // Spawn lebih banyak minion saat threshold
                    int bonusMinions = minionsPerSpawn + 2;
                    StartCoroutine(SpawnMinions(bonusMinions));
                    break;
                }
            }
        }
        
        /// <summary>
        /// Spawn minion virus di sekitar Omega
        /// </summary>
        private IEnumerator SpawnMinions(int count)
        {
            if (isSpawning) yield break;
            isSpawning = true;
            
            // Play boss grunt SFX saat mau spawn pasukan
            if (MicroDefense.Core.AudioManager.Instance != null)
            {
                MicroDefense.Core.AudioManager.Instance.PlayBossGrunt();
            }
            
            if (showDebugLogs)
                Debug.Log($"[OmegaVirus] Spawning {count} minions around position {transform.position}");
            
            for (int i = 0; i < count; i++)
            {
                // Random minion type
                EnemyDataSO randomMinion = minionTypes[Random.Range(0, minionTypes.Count)];
                
                // Random position di sekitar Omega dalam radius
                Vector3 spawnPosition = GetRandomNavMeshPosition(transform.position, spawnRadius);
                
                if (spawnPosition != Vector3.zero)
                {
                    SpawnMinion(randomMinion, spawnPosition);
                    
                    // Delay kecil antara spawn
                    yield return new WaitForSeconds(0.2f);
                }
            }
            
            isSpawning = false;
        }
        
        /// <summary>
        /// Spawn satu minion di posisi tertentu
        /// </summary>
        private void SpawnMinion(EnemyDataSO minionData, Vector3 position)
        {
            if (minionData == null)
            {
                Debug.LogError("[OmegaVirus.SpawnMinion] Minion data is NULL!");
                return;
            }
            
            if (minionData.enemyPrefab == null)
            {
                Debug.LogError($"[OmegaVirus.SpawnMinion] Minion prefab is NULL for {minionData.name}!");
                return;
            }
            
            if (showDebugLogs)
                Debug.Log($"[OmegaVirus.SpawnMinion] Spawning {minionData.enemyName} at {position}");
            
            // Spawn visual effect
            if (spawnEffectPrefab != null)
            {
                GameObject effect = Instantiate(spawnEffectPrefab, position, Quaternion.identity);
                Destroy(effect, spawnEffectDuration);
            }
            
            // Spawn minion
            GameObject minionObject = Instantiate(minionData.enemyPrefab, position, Quaternion.identity);
            EnemyBase minion = minionObject.GetComponent<EnemyBase>();
            
            if (minion != null)
            {
                minion.Initialize(minionData);
                
                // Setup pathing - minion akan follow path yang sama dengan Omega
                NavMeshPathFollower follower = minionObject.GetComponent<NavMeshPathFollower>();
                NavMeshPathFollower omegaFollower = GetComponent<NavMeshPathFollower>();
                
                if (follower != null && omegaFollower != null)
                {
                    // Copy target dari Omega
                    follower.CopyTargetFrom(omegaFollower);
                }
                
                if (showDebugLogs)
                    Debug.Log($"[OmegaVirus] Spawned {minionData.enemyName} at {position}");
            }
        }
        
        /// <summary>
        /// Cari posisi random di NavMesh sekitar origin
        /// </summary>
        private Vector3 GetRandomNavMeshPosition(Vector3 origin, float radius)
        {
            if (showDebugLogs)
                Debug.Log($"[OmegaVirus.GetRandomNavMeshPosition] Trying to find position near {origin} with radius {radius}");
            
            // Coba beberapa kali untuk find valid position
            for (int i = 0; i < 10; i++)
            {
                // Random direction
                Vector2 randomCircle = Random.insideUnitCircle * radius;
                Vector3 randomPoint = origin + new Vector3(randomCircle.x, 0, randomCircle.y);
                
                // Check apakah ada NavMesh di posisi ini
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, radius, NavMesh.AllAreas))
                {
                    if (showDebugLogs)
                        Debug.Log($"[OmegaVirus.GetRandomNavMeshPosition] Found valid position at {hit.position} (attempt {i + 1})");
                    return hit.position;
                }
            }
            
            // Fallback: return origin jika tidak ada posisi valid
            if (showDebugLogs)
                Debug.LogWarning($"[OmegaVirus.GetRandomNavMeshPosition] Could not find valid NavMesh position near {origin}, using origin as fallback");
            
            return origin;
        }
        
        /// <summary>
        /// Override Die untuk spawn minion terakhir saat mati
        /// </summary>
        protected override void Die()
        {
            if (enableSpawning && minionTypes.Count > 0)
            {
                if (showDebugLogs)
                    Debug.Log("[OmegaVirus] Boss defeated! Spawning final wave of minions.");
                
                // Spawn banyak minion saat mati (death spawn)
                StartCoroutine(SpawnMinions(minionsPerSpawn * 2));
            }
            
            base.Die();
        }
        
        /// <summary>
        /// Spawn minion manual dari luar (bisa dipanggil dari event lain)
        /// </summary>
        public void TriggerManualSpawn(int count = -1)
        {
            if (count <= 0)
                count = minionsPerSpawn;
            
            StartCoroutine(SpawnMinions(count));
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw spawn radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
            
            // Draw health thresholds indicator
            if (enemyData != null)
            {
                for (int i = 0; i < healthThresholds.Length; i++)
                {
                    float healthPercent = currentHealth / enemyData.baseHealth;
                    
                    if (healthPercent <= healthThresholds[i])
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                    }
                    
                    Vector3 offset = Vector3.up * (i + 1) * 0.5f;
                    Gizmos.DrawSphere(transform.position + offset, 0.1f);
                }
            }
        }
        #endif
    }
}
