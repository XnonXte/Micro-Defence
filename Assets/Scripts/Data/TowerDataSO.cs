using System;
using System.Collections.Generic;
using UnityEngine;

namespace MicroDefense.Data
{
    public enum TowerType
    {
        BCell,
        Macrophage,
        TCell
    }

    public enum SpecialAbilityType
    {
        None,
        AgglutinationSlow,          // B-Cell Tier 3: 30% slow for 2s
        ContinuousPhagocytosis,      // Macrophage Tier 3: Heal 5% Max HP on enemy death
        DamageReduction,            // Macrophage Tier 2: 15% damage reduction
        ArmorShred,                 // T-Cell Tier 2: -25% armor for 3s
        ApoptosisBlast              // T-Cell Tier 3: 50% True Damage AOE
    }

    [Serializable]
    public struct ProjectileConfig
    {
        public float speed;           // Projectile movement speed
        public float size;            // Projectile scale
        public Color color;           // Projectile color
        public float lifetime;        // How long before destroy
    }

    [Serializable]
    public struct TowerTierData
    {
        public int tierLevel;
        public string tierName;
        public int upgradeCost;          // Cost to unlock/upgrade to this tier
        public float damage;
        public float attackSpeed;        // Attacks per second
        public float range;              // In Grid units
        public float maxHealth;
        public float damageReductionPercent; // e.g. 15% for Macrophage Tier 2
        public SpecialAbilityType specialAbility;
        public float specialEffectMagnitude; // e.g. 0.30f for 30% slow or 0.25f for armor shred
        public float specialEffectDuration;  // Duration of slow or armor shred in seconds
    }

    [CreateAssetMenu(fileName = "NewTowerData", menuName = "Micro-Defense/Tower Data")]
    public class TowerDataSO : ScriptableObject
    {
        public TowerType towerType;
        public string towerName;
        [TextArea] public string description;
        public int baseCost;             // Cost to place Tier 1 tower
        public GameObject towerPrefab;
        public Sprite icon;

        [Header("Projectile Config")]
        [SerializeField] private ProjectileConfig projectileConfig = new ProjectileConfig
        {
            speed = 8f,
            size = 0.3f,
            color = Color.cyan,
            lifetime = 10f
        };

        public ProjectileConfig ProjectileConfig => projectileConfig;

        [Header("Upgrade Tiers (1 to 3)")]
        public List<TowerTierData> tiers = new List<TowerTierData>();

        public TowerTierData GetTier(int tierLevel)
        {
            if (tiers == null || tiers.Count == 0)
            {
                Debug.LogWarning($"[TowerDataSO] Upgrade Tiers list is empty in asset '{name}'! Please add at least 1 Tier element in the Inspector.");
                return default;
            }

            int index = Mathf.Clamp(tierLevel - 1, 0, tiers.Count - 1);
            return tiers[index];
        }
    }
}
