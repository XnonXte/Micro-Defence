using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Towers
{
    public class BCellTower : TowerBase
    {
        [Header("Animation")]
        [SerializeField] private Animator animator;

        [Header("Aiming")]
        [SerializeField] private bool rotateTowardTarget = true;
        [SerializeField] private float aimRotationOffset = 90f;

        // Animator parameter names — must match what you set in the Animator Controller
        private static readonly int IdleHash   = Animator.StringToHash("Idle");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        protected override void Awake()
        {
            base.Awake();

            // Auto-find Animator if not assigned in Inspector
            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            // Start playing Idle immediately
            PlayIdle();
        }

        /// <summary>Plays the Idle animation state.</summary>
        private void PlayIdle()
        {
            if (animator != null)
                animator.SetTrigger(IdleHash);
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

            // Trigger attack animation
            if (animator != null)
                animator.SetTrigger(AttackHash);

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
