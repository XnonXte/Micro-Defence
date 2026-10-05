using System.Collections.Generic;
using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Towers
{
    public class TCellTower : TowerBase
    {
        [Header("T-Cell AOE Radius (GDD: 1.5 Grid)")]
        [SerializeField] private float aoeRadius = 1.5f;

        protected override bool TryPerformAttack()
        {
            List<EnemyBase> enemies = FindEnemiesInRange(currentTierStats.range);
            if (enemies.Count == 0) return false;

            ExecuteCytotoxicSlash(enemies);
            return true;
        }

        private void ExecuteCytotoxicSlash(List<EnemyBase> enemies)
        {
            bool isTier3Apoptosis = (currentTierStats.specialAbility == SpecialAbilityType.ApoptosisBlast);
            bool isTier2Shred = (currentTierStats.specialAbility == SpecialAbilityType.ArmorShred);

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                if (isTier3Apoptosis)
                {
                    float trueDamage = currentTierStats.damage * 0.50f;
                    enemy.TakeDamage(trueDamage, isTrueDamage: true);
                }
                else
                {
                    enemy.TakeDamage(currentTierStats.damage, isTrueDamage: false);
                }

                if (isTier2Shred)
                {
                    enemy.ApplyArmorShred(0.25f, 3.0f);
                }
            }
        }
    }
}
