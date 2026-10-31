using MicroDefense.Core;
using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Towers
{
    public class TCellTower : TowerBase
    {
        [Header("Aiming")]
        [SerializeField] private bool rotateTowardTarget = true;
        [SerializeField] private float aimRotationOffset = 180f;

        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private float attackAnimDuration = 0.5f;
        private float attackAnimTimer = 0f;

        protected override void Awake()
        {
            base.Awake();
            PlayIdleAnimation();
        }

        /// <summary>Plays the Idle animation state (Blink for T-Cell).</summary>
        private void PlayIdleAnimation()
        {
            if (animator != null)
                animator.SetBool(Animator.StringToHash("Idle"), true);
        }

        protected override void Update()
        {
            base.Update();
            
            // Handle attack animation timer - auto-stop attack animation after duration
            if (attackAnimTimer > 0f)
            {
                attackAnimTimer -= Time.deltaTime;
                if (attackAnimTimer <= 0f && animator != null)
                {
                    animator.SetBool(IsAttackingHash, false);
                }
            }
        }

        protected override bool TryPerformAttack()
        {
            EnemyBase target = FindNearestEnemy(currentTierStats.range);
            if (target == null) return false;

            if (rotateTowardTarget)
            {
                Vector3 direction = target.transform.position - transform.position;
                direction.z = 0f;
                if (direction.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle + aimRotationOffset);
                }
            }

            // Trigger attack animation - T-Cell plays Shooting animation
            if (animator != null)
            {
                animator.SetBool(IsAttackingHash, true);
                attackAnimTimer = attackAnimDuration;
            }

            // Play T-Cell shooting SFX
            AudioManager.Instance?.PlayTCellShooting();

            bool isTier3Apoptosis = currentTierStats.specialAbility == SpecialAbilityType.ApoptosisBlast;
            float projectileDamage = isTier3Apoptosis ? currentTierStats.damage * 0.50f : currentTierStats.damage;
            SpawnProjectile(target, isTier3Apoptosis, projectileDamage);

            if (currentTierStats.specialAbility == SpecialAbilityType.ArmorShred)
                target.ApplyArmorShred(0.25f, 3.0f);

            return true;
        }

    }
}
