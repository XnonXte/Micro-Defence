using System.Collections.Generic;
using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace MicroDefense.Towers
{
    public class MacrophageTower : TowerBase
    {
        [Header("Macrophage Status")]
        public float currentHealth;
        public float maxHealth;

        private List<EnemyBase> currentlyBlockedEnemies = new List<EnemyBase>();

        protected override void Awake()
        {
            base.Awake();
            maxHealth = currentTierStats.maxHealth;
            currentHealth = maxHealth;
        }

        public override void ApplyTierStats(int tierLevel)
        {
            base.ApplyTierStats(tierLevel);
            maxHealth = currentTierStats.maxHealth;
            currentHealth = maxHealth;
        }

        protected override void Update()
        {
            base.Update();

            if (currentHealth > 0)
            {
                MaintainBlockingAndAbsorbDamage();
            }
        }

        private void MaintainBlockingAndAbsorbDamage()
        {
            List<EnemyBase> enemiesInRange = FindEnemiesInRange(currentTierStats.range);

            // Unblock enemies that left range or died
            for (int i = currentlyBlockedEnemies.Count - 1; i >= 0; i--)
            {
                EnemyBase enemy = currentlyBlockedEnemies[i];
                if (enemy == null || enemy.currentHealth <= 0 || !enemiesInRange.Contains(enemy))
                {
                    if (enemy != null)
                    {
                        enemy.SetBlocked(false);
                        // Reset NavMeshPathFollower blocker info
                        var navFollower = enemy.GetComponent<NavMeshPathFollower>();
                        if (navFollower != null) navFollower.SetBlockedBy(false);
                    }
                    currentlyBlockedEnemies.RemoveAt(i);
                }
            }

            // Block new enemies in range & calculate incoming damage
            float incomingDamage = 0f;
            foreach (var enemy in enemiesInRange)
            {
                if (enemy == null || enemy.currentHealth <= 0) continue;

                if (!currentlyBlockedEnemies.Contains(enemy))
                {
                    enemy.SetBlocked(true);
                    // Beri tahu NavMeshPathFollower posisi tower ini agar enemy tidak berhenti jauh
                    var navFollower = enemy.GetComponent<NavMeshPathFollower>();
                    if (navFollower != null) navFollower.SetBlockedBy(true, transform);
                    currentlyBlockedEnemies.Add(enemy);

                    // Hook Tier 3 Phagocytosis
                    if (currentTierStats.specialAbility == SpecialAbilityType.ContinuousPhagocytosis)
                    {
                        enemy.OnEnemyDied -= HandleEnemyDiedWhileBlocked;
                        enemy.OnEnemyDied += HandleEnemyDiedWhileBlocked;
                    }
                }

                // Collect enemy DPS
                if (enemy.Data != null)
                {
                    incomingDamage += enemy.Data.damageToCell * Time.deltaTime;
                }
            }

            // Absorb incoming DPS from blocked enemies
            if (incomingDamage > 0f)
            {
                TakeDamage(incomingDamage);
            }
        }

        protected override bool TryPerformAttack()
        {
            if (currentlyBlockedEnemies.Count == 0) return false;

            // Melee Cleave attack to all currently blocked enemies
            for (int i = currentlyBlockedEnemies.Count - 1; i >= 0; i--)
            {
                EnemyBase enemy = currentlyBlockedEnemies[i];
                if (enemy != null && enemy.currentHealth > 0)
                {
                    enemy.TakeDamage(currentTierStats.damage, isTrueDamage: false);
                }
            }

            return true;
        }

        public void TakeDamage(float rawDamage)
        {
            // Tier 2: 15% Damage Reduction
            float reductionFactor = 1.0f;
            if (currentTierStats.specialAbility == SpecialAbilityType.DamageReduction)
            {
                reductionFactor = 1.0f - Mathf.Clamp01(currentTierStats.damageReductionPercent / 100f);
            }
            
            float finalDamage = rawDamage * reductionFactor;

            currentHealth -= finalDamage;
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void HandleEnemyDiedWhileBlocked()
        {
            if (currentTierStats.specialAbility == SpecialAbilityType.ContinuousPhagocytosis && currentHealth > 0)
            {
                float healAmount = maxHealth * 0.05f; // 5% Max HP
                currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
            }
        }

        private void UnblockAllEnemies()
        {
            foreach (var enemy in currentlyBlockedEnemies)
            {
                if (enemy != null)
                {
                    enemy.SetBlocked(false);
                    var navFollower = enemy.GetComponent<NavMeshPathFollower>();
                    if (navFollower != null) navFollower.SetBlockedBy(false);
                    enemy.OnEnemyDied -= HandleEnemyDiedWhileBlocked;
                }
            }
            currentlyBlockedEnemies.Clear();
        }

        private void Die()
        {
            UnblockAllEnemies();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            UnblockAllEnemies();
        }
    }
}
