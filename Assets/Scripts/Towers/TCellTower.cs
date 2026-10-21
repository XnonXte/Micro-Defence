using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Towers
{
    public class TCellTower : TowerBase
    {

        protected override bool TryPerformAttack()
        {
            EnemyBase target = FindNearestEnemy(currentTierStats.range);
            if (target == null) return false;
            bool isTier3Apoptosis = currentTierStats.specialAbility == SpecialAbilityType.ApoptosisBlast;
            float projectileDamage = isTier3Apoptosis ? currentTierStats.damage * 0.50f : currentTierStats.damage;
            SpawnProjectile(target, isTier3Apoptosis, projectileDamage);

            if (currentTierStats.specialAbility == SpecialAbilityType.ArmorShred)
                target.ApplyArmorShred(0.25f, 3.0f);

            return true;
        }

    }
}
