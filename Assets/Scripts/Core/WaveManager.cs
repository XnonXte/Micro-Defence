using System;
using System.Collections;
using System.Collections.Generic;
using MicroDefense.Data;
using MicroDefense.Enemies;
using MicroDefense.Core;
using UnityEngine;

namespace MicroDefense.Core
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        [Header("Wave Configuration")]
        [SerializeField] private List<WaveDataSO> waves = new List<WaveDataSO>();

        [Header("Wave Timing")]
        [SerializeField] private float waveAnnouncementDuration = 2.5f;
        [SerializeField] private float delayBetweenWaves = 3f;
        [SerializeField] private bool autoStartFirstWave = true;
        [SerializeField] private float autoStartDelay = 1f;

        [Header("Pathing System")]
        [SerializeField] private bool useNavMesh;
        [SerializeField] private Transform targetCoreBase;
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField] private Transform spawnPoint;

        [Header("Multi-Path System")]
        [SerializeField] private List<SpawnPath> spawnPaths = new List<SpawnPath>();

        [Header("Runtime Status")]
        public int currentWaveIndex;
        public int activeEnemyCount;
        public bool isWaveSpawning;
        public int TotalWaves => waves != null ? waves.Count : 0;

        public event Action<int, int> OnWaveStatusChanged;
        public event Action OnWaveCompleted;
        public event Action OnAllWavesCompleted;

        private bool completionTriggered;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Jangan jalankan WaveManager di Main Menu
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentSceneName.ToLower().Contains("menu"))
            {
                Debug.Log("[WaveManager] Detected Main Menu scene. WaveManager disabled.");
                enabled = false;
                return;
            }
            
            // Auto-populate waves from Resources if empty
            if (waves.Count == 0)
            {
                LoadWavesFromResources();
            }
            PublishWaveStatus();
            
            // Auto-start first wave jika enabled
            if (autoStartFirstWave && waves.Count > 0)
            {
                StartCoroutine(AutoStartFirstWave());
            }
        }
        
        private IEnumerator AutoStartFirstWave()
        {
            Debug.Log($"[WaveManager] Auto-starting first wave in {autoStartDelay} seconds...");
            yield return new WaitForSecondsRealtime(autoStartDelay);
            
            if (currentWaveIndex == 0 && !isWaveSpawning)
            {
                Debug.Log("[WaveManager] Auto-starting wave 1");
                StartNextWave();
            }
            else
            {
                Debug.LogWarning($"[WaveManager] Cannot auto-start. currentWaveIndex: {currentWaveIndex}, isWaveSpawning: {isWaveSpawning}");
            }
        }

        private void LoadWavesFromResources()
        {
            // Load all WaveDataSO assets from Resources/Data/WaveData
            WaveDataSO[] loadedWaves = Resources.LoadAll<WaveDataSO>("Data/WaveData");
            
            if (loadedWaves.Length == 0)
            {
                Debug.LogWarning("[WaveManager] No waves found in Resources/Data/WaveData");
                return;
            }

            // Sort by name to ensure consistent order
            System.Array.Sort(loadedWaves, (a, b) => a.name.CompareTo(b.name));
            
            waves.Clear();
            foreach (var wave in loadedWaves)
            {
                if (wave != null)
                {
                    waves.Add(wave);
                    Debug.Log($"[WaveManager] Loaded wave: {wave.name}");
                }
            }

            Debug.Log($"[WaveManager] Auto-loaded {waves.Count} waves from Resources");
        }

        public void StartNextWave()
        {
            Debug.Log($"[WaveManager.StartNextWave] Called. currentWaveIndex: {currentWaveIndex}, waves.Count: {waves.Count}, isWaveSpawning: {isWaveSpawning}");
            
            if (isWaveSpawning)
            {
                Debug.LogWarning("[WaveManager.StartNextWave] Wave is already spawning. Ignoring.");
                return;
            }
            
            if (currentWaveIndex >= waves.Count)
            {
                Debug.LogWarning($"[WaveManager.StartNextWave] No more waves. currentWaveIndex: {currentWaveIndex}, waves.Count: {waves.Count}");
                return;
            }
            
            completionTriggered = false;
            Debug.Log($"[WaveManager.StartNextWave] Starting wave {currentWaveIndex + 1}");
            StartCoroutine(RunWave(waves[currentWaveIndex]));
        }

        public void ResetRun()
        {
            StopAllCoroutines();
            currentWaveIndex = 0;
            activeEnemyCount = 0;
            isWaveSpawning = false;
            completionTriggered = false;
            
            // Clear wave data completely to force reload on next Start()
            waves.Clear();
            
            PublishWaveStatus();
            Debug.Log("[WaveManager.ResetRun] Wave state fully reset for retry");
        }

        private IEnumerator RunWave(WaveDataSO waveData)
        {
            isWaveSpawning = true;
            completionTriggered = false;
            activeEnemyCount = 0;

            if (GameManager.Instance != null)
                GameManager.Instance.SetGameState(GameState.WaveInProgress);

            AudioManager.Instance?.PlayIncomingWave();

            if (CompleteUIManager.Instance != null)
                CompleteUIManager.Instance.ShowWaveAnnouncement($"WAVE {currentWaveIndex + 1}");

            PublishWaveStatus();
            yield return new WaitForSecondsRealtime(waveAnnouncementDuration);
            
            Debug.Log($"[WaveManager.RunWave] Starting spawn for wave {currentWaveIndex + 1}");
            yield return StartCoroutine(SpawnWave(waveData));
            
            Debug.Log($"[WaveManager.RunWave] Spawn finished. Setting isWaveSpawning = false. activeEnemyCount: {activeEnemyCount}");
            isWaveSpawning = false;
            
            Debug.Log($"[WaveManager.RunWave] Calling TryCompleteWave()");
            TryCompleteWave();
        }

        private IEnumerator SpawnWave(WaveDataSO waveData)
        {
            if (waveData == null) yield break;

            if (waveData.pathSpawnGroups != null && waveData.pathSpawnGroups.Count > 0)
            {
                yield return StartCoroutine(SpawnWaveMultiPath(waveData));
            }
        }

        private IEnumerator SpawnWaveMultiPath(WaveDataSO waveData)
        {
            Debug.Log($"[WaveManager.SpawnWaveMultiPath] Starting spawn for {waveData.pathSpawnGroups.Count} groups");
            
            // Track completed spawn groups manually
            int totalGroups = waveData.pathSpawnGroups.Count;
            int completedGroups = 0;
            
            foreach (var group in waveData.pathSpawnGroups)
            {
                // Start each group spawn as separate coroutine (parallel spawning)
                StartCoroutine(SpawnGroupOnPathWithCallback(group, () => {
                    completedGroups++;
                    Debug.Log($"[WaveManager.SpawnWaveMultiPath] Group completed. {completedGroups}/{totalGroups}");
                }));
            }
            
            // Wait for all spawn groups to finish
            Debug.Log($"[WaveManager.SpawnWaveMultiPath] Waiting for all {totalGroups} groups to complete...");
            yield return new WaitUntil(() => completedGroups >= totalGroups);
            Debug.Log($"[WaveManager.SpawnWaveMultiPath] All spawn groups completed!");
        }
        
        private IEnumerator SpawnGroupOnPathWithCallback(PathSpawnGroup group, System.Action onComplete)
        {
            yield return StartCoroutine(SpawnGroupOnPath(group));
            onComplete?.Invoke();
        }
        
        private IEnumerator SpawnGroupOnPath(PathSpawnGroup group)
        {
            Debug.Log($"[WaveManager.SpawnGroupOnPath] Starting group: {group.count} enemies on path {group.pathIndex}, initialDelay: {group.initialDelay}s, spawnInterval: {group.spawnInterval}s");
            
            // Wait for initial delay
            if (group.initialDelay > 0f)
            {
                Debug.Log($"[WaveManager.SpawnGroupOnPath] Waiting initial delay: {group.initialDelay}s");
                yield return new WaitForSecondsRealtime(group.initialDelay);
            }

            // Spawn enemies in this group
            for (int i = 0; i < group.count; i++)
            {
                Debug.Log($"[WaveManager.SpawnGroupOnPath] Spawning enemy {i + 1}/{group.count}");
                SpawnEnemyOnPath(group.enemyData, group.pathIndex);
                
                if (i < group.count - 1 && group.spawnInterval > 0f)
                {
                    yield return new WaitForSecondsRealtime(group.spawnInterval);
                }
            }
            
            Debug.Log($"[WaveManager.SpawnGroupOnPath] Group finished spawning {group.count} enemies");
        }

        private void SpawnEnemyOnPath(EnemyDataSO enemyData, int pathIndex)
        {
            if (enemyData == null || enemyData.enemyPrefab == null) return;

            // Validate path index
            if (pathIndex < 0 || pathIndex >= spawnPaths.Count)
            {
                Debug.LogWarning($"[WaveManager] Invalid path index {pathIndex}. Total paths: {spawnPaths.Count}");
                return;
            }

            SpawnPath path = spawnPaths[pathIndex];
            Vector3 position = path.spawnPoint != null ? path.spawnPoint.position : Vector3.zero;

            GameObject enemyObject = Instantiate(enemyData.enemyPrefab, position, Quaternion.identity);
            EnemyBase enemy = enemyObject.GetComponent<EnemyBase>();
            if (enemy == null)
            {
                Destroy(enemyObject);
                return;
            }

            enemy.Initialize(enemyData);
            enemy.OnEnemyDied += HandleEnemyRemoved;
            activeEnemyCount++;

            // Setup pathing with NavMesh
            if (path.useNavMesh && path.targetPoint != null)
            {
                NavMeshPathFollower follower = enemyObject.GetComponent<NavMeshPathFollower>();
                if (follower != null)
                {
                    if (path.waypoints.Count > 0)
                    {
                        // Use waypoints with NavMesh
                        follower.SetWaypoints(path.waypoints);
                    }
                    else
                    {
                        // Direct NavMesh pathing to target
                        follower.InitializeNavMesh(path.targetPoint);
                    }
                }
            }
        }

        private void SpawnEnemy(EnemyDataSO enemyData)
        {
            // This method is kept for emergency fallback only
            Debug.LogError("[WaveManager] SpawnEnemy() called but multi-path system should be used. Ignoring.");
        }

        private void HandleEnemyRemoved()
        {
            activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
            Debug.Log($"[WaveManager] Enemy removed. activeEnemyCount: {activeEnemyCount}, isWaveSpawning: {isWaveSpawning}, completionTriggered: {completionTriggered}");
            TryCompleteWave();
        }

        private void TryCompleteWave()
        {
            Debug.Log($"[WaveManager.TryCompleteWave] Check - completionTriggered: {completionTriggered}, isWaveSpawning: {isWaveSpawning}, activeEnemyCount: {activeEnemyCount}");
            if (completionTriggered || isWaveSpawning || activeEnemyCount > 0) return;
            Debug.Log($"[WaveManager.TryCompleteWave] All conditions met, calling CompleteWave()");
            CompleteWave();
        }

        private void CompleteWave()
        {
            if (completionTriggered) return;
            completionTriggered = true;
            
            Debug.Log($"[WaveManager] Wave {currentWaveIndex + 1} completed!");

            if (currentWaveIndex < waves.Count && PlasmaManager.Instance != null)
                PlasmaManager.Instance.AddPlasma(waves[currentWaveIndex].completionBonusPlasma);

            currentWaveIndex++;
            OnWaveCompleted?.Invoke();
            
            Debug.Log($"[WaveManager] currentWaveIndex incremented to {currentWaveIndex}, TotalWaves: {TotalWaves}");

            if (currentWaveIndex >= waves.Count)
            {
                Debug.Log($"[WaveManager] All waves completed!");
                OnAllWavesCompleted?.Invoke();
                
                // Play victory SFX
                AudioManager.Instance?.PlayGameOver();
                
                if (CompleteUIManager.Instance != null)
                    CompleteUIManager.Instance.ShowVictory();
                if (GameManager.Instance != null)
                    GameManager.Instance.SetGameState(GameState.Victory);
                return;
            }

            PublishWaveStatus();
            if (GameManager.Instance != null)
                GameManager.Instance.SetGameState(GameState.Preparation);
            
            Debug.Log($"[WaveManager] Starting delay before next wave (delayBetweenWaves: {delayBetweenWaves}s)");
            StartCoroutine(StartFollowingWave());
        }

        private IEnumerator StartFollowingWave()
        {
            Debug.Log($"[WaveManager.StartFollowingWave] Starting coroutine. Will wait {delayBetweenWaves} seconds before next wave...");
            Debug.Log($"[WaveManager.StartFollowingWave] Time.timeScale: {Time.timeScale}");
            
            float elapsedTime = 0f;
            while (elapsedTime < delayBetweenWaves)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            
            Debug.Log($"[WaveManager.StartFollowingWave] Delay finished after {elapsedTime} seconds. Calling StartNextWave()");
            Debug.Log($"[WaveManager.StartFollowingWave] currentWaveIndex: {currentWaveIndex}, waves.Count: {waves.Count}, isWaveSpawning: {isWaveSpawning}");
            StartNextWave();
        }

        private void PublishWaveStatus()
        {
            OnWaveStatusChanged?.Invoke(currentWaveIndex + 1, Mathf.Max(1, waves.Count));
        }
    }
}
