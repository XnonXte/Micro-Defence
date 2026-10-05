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

        [Header("Pathing System (NavMesh vs Waypoints)")]
        [SerializeField] private bool useNavMesh = false;
        [SerializeField] private Transform targetCoreBase; // Required for Full NavMesh mode
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField] private Transform spawnPoint;

        [Header("Runtime Status")]
        public int currentWaveIndex = 0;
        public int activeEnemyCount = 0;
        public bool isWaveSpawning = false;

        public event Action<int, int> OnWaveStatusChanged; // (currentWave, totalWaves)
        public event Action OnWaveCompleted;

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
            OnWaveStatusChanged?.Invoke(currentWaveIndex + 1, waves.Count);
        }

        public void StartNextWave()
        {
            if (isWaveSpawning || currentWaveIndex >= waves.Count) return;

            StartCoroutine(SpawnWaveRoutine(waves[currentWaveIndex]));
        }

        private IEnumerator SpawnWaveRoutine(WaveDataSO waveData)
        {
            isWaveSpawning = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameState.WaveInProgress);
            }

            OnWaveStatusChanged?.Invoke(currentWaveIndex + 1, waves.Count);

            foreach (var group in waveData.spawnGroups)
            {
                if (group.initialDelay > 0f)
                {
                    yield return new WaitForSeconds(group.initialDelay);
                }

                for (int i = 0; i < group.count; i++)
                {
                    SpawnEnemy(group.enemyData);
                    yield return new WaitForSeconds(group.spawnInterval);
                }
            }

            isWaveSpawning = false;
        }

        private void SpawnEnemy(EnemyDataSO enemyData)
        {
            if (enemyData == null || enemyData.enemyPrefab == null) return;

            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : (waypoints.Count > 0 ? waypoints[0].position : Vector3.zero);
            GameObject enemyObj = Instantiate(enemyData.enemyPrefab, spawnPos, Quaternion.identity);

            EnemyBase enemyComp = enemyObj.GetComponent<EnemyBase>();
            if (enemyComp != null)
            {
                enemyComp.Initialize(enemyData);
                enemyComp.OnEnemyDied += HandleEnemyDied;
                activeEnemyCount++;
            }

            if (useNavMesh)
            {
                NavMeshPathFollower navFollower = enemyObj.GetComponent<NavMeshPathFollower>();
                if (navFollower != null && targetCoreBase != null)
                    navFollower.InitializeNavMesh(targetCoreBase);
            }
        }

        private void HandleEnemyDied()
        {
            activeEnemyCount--;
            if (activeEnemyCount <= 0 && !isWaveSpawning)
            {
                CompleteWave();
            }
        }

        private void CompleteWave()
        {
            if (currentWaveIndex < waves.Count)
            {
                int bonus = waves[currentWaveIndex].completionBonusPlasma;
                if (PlasmaManager.Instance != null && bonus > 0)
                {
                    PlasmaManager.Instance.AddPlasma(bonus);
                }
            }

            currentWaveIndex++;
            OnWaveCompleted?.Invoke();

            if (currentWaveIndex >= waves.Count)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Victory);
                }
            }
            else
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Preparation);
                }
                OnWaveStatusChanged?.Invoke(currentWaveIndex + 1, waves.Count);
            }
        }
    }
}
