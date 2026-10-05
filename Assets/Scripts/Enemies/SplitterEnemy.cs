using UnityEngine;

namespace MicroDefense.Enemies
{
    public class SplitterEnemy : EnemyBase
    {
        [Header("Splitter Settings")]
        [SerializeField] private GameObject miniVirusPrefab;

        protected override void Die()
        {
            if (enemyData != null && enemyData.isSplitter && enemyData.splitChildData != null)
            {
                SpawnMiniViruses();
            }

            base.Die();
        }

        private void SpawnMiniViruses()
        {
            int count = enemyData.splitCount;
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), 0f, Random.Range(-0.3f, 0.3f));
                GameObject prefabToSpawn = miniVirusPrefab != null ? miniVirusPrefab : enemyData.splitChildData.enemyPrefab;

                if (prefabToSpawn != null)
                {
                    GameObject spawned = Instantiate(prefabToSpawn, transform.position + offset, Quaternion.identity);
                    EnemyBase childEnemy = spawned.GetComponent<EnemyBase>();
                    if (childEnemy != null)
                    {
                        childEnemy.Initialize(enemyData.splitChildData);
                    }
                }
            }
        }
    }
}
