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

            bool isTier3Apoptosis = currentTierStats.specialAbility == SpecialAbilityType.ApoptosisBlast;
            float projectileDamage = isTier3Apoptosis ? currentTierStats.damage * 0.50f : currentTierStats.damage;
            SpawnProjectile(target, isTier3Apoptosis, projectileDamage);

            if (currentTierStats.specialAbility == SpecialAbilityType.ArmorShred)
                target.ApplyArmorShred(0.25f, 3.0f);

            return true;
        }

    }
}
