using System.Collections.Generic;
using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace MicroDefense.Towers
{
    public class MacrophageTower : TowerBase
    {
        [Header("Aiming Rotation")]
        [SerializeField] private bool rotateTowardTarget = true;
        [SerializeField] private float aimRotationOffset = 90f;

        [Header("Macrophage Status")]
        public float currentHealth;
        public float maxHealth;

        private List<EnemyBase> currentlyBlockedEnemies = new List<EnemyBase>();

        protected override void Awake()
        {
            base.Awake();
            maxHealth = currentTierStats.maxHealth;
            currentHealth = maxHealth;
            PlayIdleAnimation();
        }

        /// <summary>Plays the Idle animation state (Blink only for Macrophage).</summary>
        private void PlayIdleAnimation()
        {
            if (animator != null)
                animator.SetTrigger(Animator.StringToHash("Idle"));
        }

        public override void ApplyTierStats(int tierLevel)
        {
            base.ApplyTierStats(tierLevel);
            if (currentTierStats.range < 5f)
                currentTierStats.range = 5f;
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
            EnemyBase nearestEnemy = FindNearestEnemy(currentTierStats.range);

            if (rotateTowardTarget && nearestEnemy != null)
            {
                Vector3 direction = nearestEnemy.transform.position - transform.position;
                direction.z = 0f;
                if (direction.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle + aimRotationOffset);
                }
            }

            // Unblock enemies that left range or died
            for (int i = currentlyBlockedEnemies.Count - 1; i >= 0; i--)
            {
                EnemyBase enemy = currentlyBlockedEnemies[i];
                if (enemy == null || enemy.currentHealth <= 0 || !enemiesInRange.Contains(enemy))
                {
                    if (enemy != null)
                    {
                        enemy.SetBlocked(false);
                        var navFollower = enemy.GetComponent<NavMeshPathFollower>();
                        if (navFollower != null) navFollower.SetBlockedBy(false);
                    }
                    currentlyBlockedEnemies.RemoveAt(i);
                }
            }

            float incomingDamage = 0f;
            foreach (var enemy in enemiesInRange)
            {
                if (enemy == null || enemy.currentHealth <= 0) continue;

                if (!currentlyBlockedEnemies.Contains(enemy))
                {
                    enemy.SetBlocked(true);
                    var navFollower = enemy.GetComponent<NavMeshPathFollower>();
                    if (navFollower != null) navFollower.SetBlockedBy(true, transform);
                    currentlyBlockedEnemies.Add(enemy);

                    if (currentTierStats.specialAbility == SpecialAbilityType.ContinuousPhagocytosis)
                    {
                        enemy.OnEnemyDied -= HandleEnemyDiedWhileBlocked;
                        enemy.OnEnemyDied += HandleEnemyDiedWhileBlocked;
                    }
                }

                if (enemy.Data != null)
                {
                    incomingDamage += enemy.Data.damageToCell * Time.deltaTime;
                }
            }

            if (incomingDamage > 0f)
            {
                TakeDamage(incomingDamage);
            }
        }

        protected override bool TryPerformAttack()
        {
            if (currentlyBlockedEnemies.Count == 0) return false;

            // Macrophage DOES NOT have an Attack animation - only plays Idle (Blink)
            // No animator.SetTrigger(AttackHash) call here

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
            float reductionFactor = 1.0f;
            if (currentTierStats.specialAbility == SpecialAbilityType.DamageReduction)
            {
                reductionFactor = 0.85f; // 15% damage reduction
            }

            float finalDamage = rawDamage * reductionFactor;
            currentHealth -= finalDamage;
            if (currentHealth < 0) currentHealth = 0;

            // Macrophage menerima damage untuk menahan virus, tetapi virus TIDAK ikut hancur saat Macrophage mati
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void HandleEnemyDiedWhileBlocked()
        {
            // Tier 3 Special: Heal Macrophage 5% of Max HP on enemy death while blocked
            float healAmount = maxHealth * 0.05f;
            currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        }

        private void Die()
        {
            // Unblock all enemies before destroying
            foreach (var enemy in currentlyBlockedEnemies)
            {
                if (enemy != null)
                {
                    enemy.SetBlocked(false);
                    var navFollower = enemy.GetComponent<NavMeshPathFollower>();
                    if (navFollower != null) navFollower.SetBlockedBy(false);
                }
            }
            currentlyBlockedEnemies.Clear();
            Destroy(gameObject);
        }
    }
}
