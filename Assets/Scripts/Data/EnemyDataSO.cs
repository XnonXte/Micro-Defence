using UnityEngine;

namespace MicroDefense.Data
{
    public enum EnemyType
    {
        CommonVirus,
        MutantVirus,
        ArmoredVirus,
        SwarmVirus,
        MiniVirus,
        BossOmega
    }

    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Micro-Defense/Enemy Data")]
    public class EnemyDataSO : ScriptableObject
    {
        public EnemyType enemyType;
        public string enemyName;
        public GameObject enemyPrefab;

        [Header("Base Stats (GDD v0.1.1)")]
        public float baseHealth = 100f;
        public float moveSpeed = 2.0f;       // In Grid units / sec
        public float flatArmor = 0f;         // Flat damage reduction per hit
        public float damageToCell = 15f;     // DPS dealt when damaging player cell/leak
        public int plasmaReward = 10;        // Plasma currency awarded on death

        [Header("Swarm Virus Life Phases")]
        public int swarmLives = 3;
        public float swarmHealthPerLife = 100f;
        public Sprite swarmSpriteLife1;
        public Sprite swarmSpriteLife2;
        public Sprite swarmSpriteLife3;

        [Header("Special Behaviors")]
        public bool isSplitter;              // Swarm Virus feature
        public EnemyDataSO splitChildData;   // Mini Virus data
        public int splitCount = 3;

        public bool isBoss;                  // Boss Omega feature
        public float replicationInterval = 8.0f; // Spawns minions every 8-10 seconds
        public EnemyDataSO[] replicationMinionPool;
    }
}
