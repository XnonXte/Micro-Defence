using System;
using System.Collections;
using System.Collections.Generic;
using MicroDefense.Data;
using MicroDefense.Enemies;
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
            // Auto-populate waves from Resources if empty
            if (waves.Count == 0)
            {
                LoadWavesFromResources();
            }
            PublishWaveStatus();
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
            if (isWaveSpawning || currentWaveIndex >= waves.Count) return;
            completionTriggered = false;
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

            if (CompleteUIManager.Instance != null)
                CompleteUIManager.Instance.ShowWaveAnnouncement($"WAVE {currentWaveIndex + 1}");

            PublishWaveStatus();
            yield return new WaitForSeconds(waveAnnouncementDuration);
            yield return StartCoroutine(SpawnWave(waveData));

            isWaveSpawning = false;
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
            foreach (var group in waveData.pathSpawnGroups)
            {
                if (group.initialDelay > 0f)
                    yield return new WaitForSeconds(group.initialDelay);

                for (int i = 0; i < group.count; i++)
                {
                    SpawnEnemyOnPath(group.enemyData, group.pathIndex);
                    if (group.spawnInterval > 0f)
                        yield return new WaitForSeconds(group.spawnInterval);
                }
            }
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
            Debug.Log($"[WaveManager] Waiting {delayBetweenWaves} seconds before starting next wave...");
            yield return new WaitForSeconds(delayBetweenWaves);
            Debug.Log($"[WaveManager] Delay finished, calling StartNextWave()");
            StartNextWave();
        }

        private void PublishWaveStatus()
        {
            OnWaveStatusChanged?.Invoke(currentWaveIndex + 1, Mathf.Max(1, waves.Count));
        }
    }
}
