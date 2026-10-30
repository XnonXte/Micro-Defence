using System;
using System.Collections.Generic;
using UnityEngine;

namespace MicroDefense.Data
{
    [Serializable]
    public struct PathSpawnGroup
    {
        public int pathIndex;           // Which path to spawn on (0 or 1)
        public EnemyDataSO enemyData;
        public int count;
        public float spawnInterval;
        public float initialDelay;
    }

    [Serializable]
    public struct SpawnPath
    {
        public string pathName;
        public Transform spawnPoint;
        public Transform targetPoint;
        public List<Transform> waypoints;
        public bool useNavMesh;
    }

    [CreateAssetMenu(fileName = "NewWaveData", menuName = "Micro-Defense/Wave Data")]
    public class WaveDataSO : ScriptableObject
    {
        [Header("Wave Info")]
        public int waveNumber;
        public string waveName;
        
        [Header("Spawn Configuration")]
        public List<PathSpawnGroup> pathSpawnGroups = new List<PathSpawnGroup>();
        
        [Header("Rewards")]
        public int completionBonusPlasma = 50;
    }
}
