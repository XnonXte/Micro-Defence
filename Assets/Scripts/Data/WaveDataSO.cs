using System;
using System.Collections.Generic;
using UnityEngine;

namespace MicroDefense.Data
{
    [Serializable]
    public struct EnemySpawnGroup
    {
        public EnemyDataSO enemyData;
        public int count;
        public float spawnInterval;   // Time between spawns in seconds
        public float initialDelay;    // Delay before starting this group spawn
    }

    [CreateAssetMenu(fileName = "NewWaveData", menuName = "Micro-Defense/Wave Data")]
    public class WaveDataSO : ScriptableObject
    {
        public int waveNumber;
        public string waveName;
        public List<EnemySpawnGroup> spawnGroups = new List<EnemySpawnGroup>();
        public int completionBonusPlasma = 50;
    }
}
