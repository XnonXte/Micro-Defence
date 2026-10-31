using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Towers
{
    public class BCellTower : TowerBase
    {
        [Header("Aiming")]
        [SerializeField] private bool rotateTowardTarget = true;
        [SerializeField] private float aimRotationOffset = 90f;

        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private float attackAnimDuration = 0.5f;
        private float attackAnimTimer = 0f;

        protected override void Awake()
        {
            base.Awake();
            PlayIdleAnimation();
        }

        /// <summary>Plays the Idle animation state (Blink for B-Cell).</summary>
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

            // Trigger attack animation - B-Cell plays Shooting animation
            if (animator != null)
            {
                animator.SetBool(IsAttackingHash, true);
                attackAnimTimer = attackAnimDuration;
            }

            // Use base class SpawnProjectile (ADJUSTABLE damage)
            SpawnProjectile(target);

            // Apply slow ability if available
            if (currentTierStats.specialAbility == SpecialAbilityType.AgglutinationSlow)
            {
                target.ApplySlow(currentTierStats.specialEffectMagnitude, currentTierStats.specialEffectDuration);
            }

            return true;
        }
    }
}
