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

        private bool pendingApoptosis;
        private float pendingDamage;

        protected override void Awake()
        {
            base.Awake();
            PlayIdleAnimation();
        }

        /// <summary>
        /// Plays the Idle animation state (Blink for T-Cell).
        /// </summary>
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

            // Dynamically detect the shooting animation.
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

            // Wait for the shooting animation to finish.
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

            // Store target and attack data.
            pendingTarget = target;

            pendingApoptosis =
                currentTierStats.specialAbility == SpecialAbilityType.ApoptosisBlast;

            pendingDamage = pendingApoptosis
                ? currentTierStats.damage * 0.50f
                : currentTierStats.damage;

            shotPending = true;
            shootingStateCaptured = false;

            // Start shooting animation.
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

        /// <summary>
        /// Executes the attack after the shooting animation finishes.
        /// </summary>
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

            // Play T-Cell shooting SFX.
            AudioManager.Instance?.PlayTCellShooting();

            // Spawn projectile from the nozzle with the appropriate damage.
            SpawnProjectile(target, pendingApoptosis, pendingDamage, projectileSpawnPoint);

            // Apply Armor Shred.
            if (currentTierStats.specialAbility == SpecialAbilityType.ArmorShred)
            {
                target.ApplyArmorShred(0.25f, 3.0f);
            }
        }
    }
}