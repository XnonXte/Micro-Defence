using MicroDefense.Core;
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

        [Header("Projectile")]
        [Tooltip("Muzzle/nozzle Transform. Should be a child of the rotating turret. " +
                 "If empty, projectiles spawn at the tower's root position.")]
        [SerializeField] private Transform projectileSpawnPoint;

        [Header("Animation")]
        [SerializeField] private int animationLayer = 0;

        private static readonly int IsAttackingHash =
            Animator.StringToHash("IsAttacking");

        private EnemyBase pendingTarget;

        private bool shotPending;
        private bool shootingStateCaptured;

        private int previousStateHash;
        private int shootingStateHash;

        protected override void Awake()
        {
            base.Awake();
            PlayIdleAnimation();
        }

        private void PlayIdleAnimation()
        {
            if (animator != null)
                animator.SetBool(Animator.StringToHash("Idle"), true);
        }

        protected override void Update()
        {
            base.Update();

            if (!shotPending || animator == null)
                return;

            AnimatorStateInfo currentState =
                animator.GetCurrentAnimatorStateInfo(animationLayer);

            // Detect the shooting animation dynamically.
            if (!shootingStateCaptured)
            {
                if (animator.IsInTransition(animationLayer))
                {
                    AnimatorStateInfo nextState =
                        animator.GetNextAnimatorStateInfo(animationLayer);

                    if (nextState.fullPathHash != previousStateHash)
                    {
                        shootingStateHash = nextState.fullPathHash;
                        shootingStateCaptured = true;
                    }
                }
                else if (currentState.fullPathHash != previousStateHash)
                {
                    shootingStateHash = currentState.fullPathHash;
                    shootingStateCaptured = true;
                }

                return;
            }

            // Wait until the shooting animation completes.
            if (currentState.fullPathHash == shootingStateHash &&
                currentState.normalizedTime >= 1f)
            {
                CompleteAttack();
            }
        }

        protected override bool TryPerformAttack()
        {
            // An attack is already in progress; never queue a second one.
            if (shotPending) return false;

            EnemyBase target = FindNearestEnemy(currentTierStats.range);
            if (target == null) return false;

            if (rotateTowardTarget)
            {
                Vector3 direction = target.transform.position - transform.position;
                direction.z = 0f;

                if (direction.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.Euler(
                        0f, 0f, angle + aimRotationOffset
                    );
                }
            }

            pendingTarget = target;
            shotPending = true;
            shootingStateCaptured = false;

            if (animator != null)
            {
                previousStateHash =
                    animator.GetCurrentAnimatorStateInfo(animationLayer).fullPathHash;

                animator.SetBool(IsAttackingHash, true);
            }
            else
            {
                CompleteAttack();
            }

            return true;
        }

        private void CompleteAttack()
        {
            if (animator != null)
                animator.SetBool(IsAttackingHash, false);

            EnemyBase target = pendingTarget;

            // Reset pending attack state.
            pendingTarget = null;
            shotPending = false;
            shootingStateCaptured = false;

            if (target == null)
                return;

            // Play shooting SFX.
            AudioManager.Instance?.PlayBCellShooting();

            // Spawn projectile from the nozzle after the shooting animation finishes.
            SpawnProjectile(target, projectileSpawnPoint);

            // Apply slow ability.
            if (currentTierStats.specialAbility ==
                SpecialAbilityType.AgglutinationSlow)
            {
                target.ApplySlow(
                    currentTierStats.specialEffectMagnitude,
                    currentTierStats.specialEffectDuration
                );
            }
        }
    }
}