using System;
using System.Collections.Generic;
using MicroDefense.Core;
using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Towers
{
    public abstract class TowerBase : MonoBehaviour
    {
        [Header("Data Config")]
        [SerializeField] protected TowerDataSO towerData;
        public TowerDataSO Data => towerData;

        [Header("Runtime Tier & Stats")]
        public int currentTierLevel = 1;
        public TowerTierData currentTierStats;

        protected float attackTimer;
        public event Action<int> OnTierUpgraded;

        protected virtual void Awake()
        {
            if (towerData != null)
            {
                ApplyTierStats(1);
            }
        }

        public void SetTowerData(TowerDataSO data)
        {
            towerData = data;
            if (towerData != null)
            {
                ApplyTierStats(1);
            }
        }

        public virtual void ApplyTierStats(int tierLevel)
        {
            currentTierLevel = tierLevel;
            currentTierStats = towerData.GetTier(tierLevel);
            
            // Safety check: if range is 0, set default
            if (currentTierStats.range <= 0)
                currentTierStats.range = 5f;
            
            OnTierUpgraded?.Invoke(currentTierLevel);
        }

        public virtual bool CanUpgrade(out int cost)
        {
            cost = 0;
            if (towerData == null || towerData.tiers == null || currentTierLevel >= towerData.tiers.Count)
                return false;

            TowerTierData nextTier = towerData.GetTier(currentTierLevel + 1);
            cost = nextTier.upgradeCost;
            return PlasmaManager.Instance != null && PlasmaManager.Instance.CanAfford(cost);
        }

        public virtual bool TryUpgrade()
        {
            if (CanUpgrade(out int cost))
            {
                if (PlasmaManager.Instance.TrySpendPlasma(cost))
                {
                    ApplyTierStats(currentTierLevel + 1);
                    return true;
                }
            }
            return false;
        }

        protected virtual void Update()
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                if (TryPerformAttack())
                {
                    attackTimer = 1.0f / Mathf.Max(0.1f, currentTierStats.attackSpeed);
                }
            }
        }

        protected abstract bool TryPerformAttack();

        protected EnemyBase FindNearestEnemy(float range)
        {
            EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
            EnemyBase nearest = null;
            float minDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                if (enemy.currentHealth <= 0) continue;

                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist <= range && dist < minDistance)
                {
                    minDistance = dist;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        protected List<EnemyBase> FindEnemiesInRange(float range)
        {
            EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
            List<EnemyBase> result = new List<EnemyBase>();

            foreach (var enemy in enemies)
            {
                if (enemy.currentHealth <= 0) continue;

                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist <= range)
                {
                    result.Add(enemy);
                }
            }

            return result;
        }
    }
}
