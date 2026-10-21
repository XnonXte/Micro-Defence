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
            PublishWaveStatus();
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
            PublishWaveStatus();
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
            if (waveData == null || waveData.spawnGroups == null) yield break;

            foreach (var group in waveData.spawnGroups)
            {
                if (group.initialDelay > 0f)
                    yield return new WaitForSeconds(group.initialDelay);

                for (int i = 0; i < group.count; i++)
                {
                    SpawnEnemy(group.enemyData);
                    if (group.spawnInterval > 0f)
                        yield return new WaitForSeconds(group.spawnInterval);
                }
            }
        }

        private void SpawnEnemy(EnemyDataSO enemyData)
        {
            if (enemyData == null || enemyData.enemyPrefab == null) return;

            Vector3 position = spawnPoint != null
                ? spawnPoint.position
                : waypoints.Count > 0 ? waypoints[0].position : Vector3.zero;

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

            if (useNavMesh && targetCoreBase != null)
            {
                NavMeshPathFollower follower = enemyObject.GetComponent<NavMeshPathFollower>();
                if (follower != null)
                    follower.InitializeNavMesh(targetCoreBase);
            }
        }

        private void HandleEnemyRemoved()
        {
            activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
            TryCompleteWave();
        }

        private void TryCompleteWave()
        {
            if (completionTriggered || isWaveSpawning || activeEnemyCount > 0) return;
            CompleteWave();
        }

        private void CompleteWave()
        {
            if (completionTriggered) return;
            completionTriggered = true;

            if (currentWaveIndex < waves.Count && PlasmaManager.Instance != null)
                PlasmaManager.Instance.AddPlasma(waves[currentWaveIndex].completionBonusPlasma);

            currentWaveIndex++;
            OnWaveCompleted?.Invoke();

            if (currentWaveIndex >= waves.Count)
            {
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
            StartCoroutine(StartFollowingWave());
        }

        private IEnumerator StartFollowingWave()
        {
            yield return new WaitForSeconds(delayBetweenWaves);
            StartNextWave();
        }

        private void PublishWaveStatus()
        {
            OnWaveStatusChanged?.Invoke(currentWaveIndex + 1, Mathf.Max(1, waves.Count));
        }
    }
}
