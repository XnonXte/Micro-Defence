using MicroDefense.Data;
using UnityEngine;

namespace MicroDefense.Enemies
{
    public class BossEnemy : EnemyBase
    {
        private float replicationTimer;

        protected override void Awake()
        {
            base.Awake();
            replicationTimer = Random.Range(8.0f, 10.0f);
        }

        protected override void Update()
        {
            base.Update();

            if (currentHealth > 0 && enemyData != null && enemyData.isBoss)
            {
                replicationTimer -= Time.deltaTime;
                if (replicationTimer <= 0f)
                {
                    ExecuteReplicationCall();
                    replicationTimer = Random.Range(8.0f, 10.0f);
                }
            }
        }

        private void ExecuteReplicationCall()
        {
            if (enemyData.replicationMinionPool == null || enemyData.replicationMinionPool.Length == 0)
                return;

            // GDD Spec: Spawn 2-3 Common OR 1 Mutant
            // Strategy: Decide type first, then spawn correct count

            // Find Common and Mutant in pool
            EnemyDataSO commonVirus = null;
            EnemyDataSO mutantVirus = null;

            foreach (var minion in enemyData.replicationMinionPool)
            {
                if (minion.enemyType == EnemyType.CommonVirus)
                    commonVirus = minion;
                else if (minion.enemyType == EnemyType.MutantVirus)
                    mutantVirus = minion;
            }

            // Randomly decide: 50% chance Common, 50% chance Mutant
            bool spawnCommon = Random.value > 0.5f;
            EnemyDataSO selectedMinionData = null;
            int spawnCount = 0;

            if (spawnCommon && commonVirus != null)
            {
                selectedMinionData = commonVirus;
                spawnCount = Random.Range(2, 4);
            }
            else if (mutantVirus != null)
            {
                selectedMinionData = mutantVirus;
                spawnCount = 1;
            }
            else if (commonVirus != null)
            {
                selectedMinionData = commonVirus;
                spawnCount = Random.Range(2, 4);
            }
            else
            {
                return;
            }

            if (selectedMinionData == null || selectedMinionData.enemyPrefab == null)
                return;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
                GameObject minionObj = Instantiate(selectedMinionData.enemyPrefab, transform.position + offset, Quaternion.identity);

                EnemyBase minionEnemy = minionObj.GetComponent<EnemyBase>();
                if (minionEnemy != null)
                    minionEnemy.Initialize(selectedMinionData);
            }
        }
    }
}
