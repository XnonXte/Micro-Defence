using System;
using System.Collections.Generic;
using MicroDefense.Core;
using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Towers
{
    public abstract class TowerBase : MonoBehaviour
    {
        [Header("Data Config")]
        [SerializeField] protected TowerDataSO towerData;
        public TowerDataSO Data => towerData;

        [Header("Runtime Tier & Stats")]
        public int currentTierLevel = 1;
        public TowerTierData currentTierStats;

        [Header("Animation")]
        [SerializeField] protected Animator animator;
        protected static readonly int AttackHash = Animator.StringToHash("Attack");

        protected float attackTimer;
        public event Action<int> OnTierUpgraded;

        protected virtual void Awake()
        {
            if (towerData != null)
            {
                ApplyTierStats(1);
            }

            // Auto-find animator if not assigned
            if (animator == null)
                animator = GetComponent<Animator>();
            
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        public void SetTowerData(TowerDataSO data)
        {
            towerData = data;
            if (towerData != null)
            {
                ApplyTierStats(1);
            }
        }

        public virtual void ApplyTierStats(int tierLevel)
        {
            currentTierLevel = tierLevel;
            currentTierStats = towerData.GetTier(tierLevel);

            if (currentTierStats.range <= 0)
                currentTierStats.range = 5f;

            OnTierUpgraded?.Invoke(currentTierLevel);
        }

        public virtual bool CanUpgrade(out int cost)
        {
            cost = 0;
            if (towerData == null || towerData.tiers == null || currentTierLevel >= towerData.tiers.Count)
                return false;

            TowerTierData nextTier = towerData.GetTier(currentTierLevel + 1);
            cost = nextTier.upgradeCost;
            return PlasmaManager.Instance != null && PlasmaManager.Instance.CanAfford(cost);
        }

        public virtual bool TryUpgrade()
        {
            if (CanUpgrade(out int cost))
            {
                if (PlasmaManager.Instance.TrySpendPlasma(cost))
                {
                    ApplyTierStats(currentTierLevel + 1);
                    MicroDefense.UI.TowerShopUI shop = FindAnyObjectByType<MicroDefense.UI.TowerShopUI>();
                    if (shop != null) shop.RefreshUI();
                    return true;
                }
            }
            return false;
        }

        protected virtual void Update()
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                if (TryPerformAttack())
                {
                    attackTimer = 1.0f / Mathf.Max(0.1f, currentTierStats.attackSpeed);
                }
            }
        }

        protected abstract bool TryPerformAttack();

        protected EnemyBase FindNearestEnemy(float range)
        {
            if (range <= 0f) range = 5f;
            EnemyBase[] enemies = FindObjectsByType<EnemyBase>();
            EnemyBase nearest = null;
            float minDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                if (enemy.currentHealth <= 0) continue;

                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist <= range && dist < minDistance)
                {
                    minDistance = dist;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        protected List<EnemyBase> FindEnemiesInRange(float range)
        {
            if (range <= 0f) range = 5f;
            EnemyBase[] enemies = FindObjectsByType<EnemyBase>();
            List<EnemyBase> result = new List<EnemyBase>();

            foreach (var enemy in enemies)
            {
                if (enemy.currentHealth <= 0) continue;

                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist <= range)
                {
                    result.Add(enemy);
                }
            }

            return result;
        }

        protected void SpawnProjectile(EnemyBase target)
        {
            SpawnProjectile(target, false, currentTierStats.damage);
        }

        protected void SpawnProjectile(EnemyBase target, bool isTrueDamage, float damage)
        {
            if (target == null || towerData == null) return;

            GameObject projectileObj = new GameObject("Projectile");
            projectileObj.transform.position = transform.position;
            projectileObj.layer = gameObject.layer;

            SpriteRenderer sr = projectileObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 5;

            Sprite projectileSprite = towerData.projectileSprite;
            if (projectileSprite != null)
            {
                sr.sprite = projectileSprite;
                sr.color = Color.white;
            }
            else
            {
                Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                Color[] pixels = new Color[256];
                Vector2 center = Vector2.one * 8;

                for (int i = 0; i < 256; i++)
                {
                    int y = i / 16;
                    int x = i % 16;
                    pixels[i] = Vector2.Distance(new Vector2(x, y), center) <= 7 ? Color.cyan : new Color(0, 0, 0, 0);
                }

                tex.SetPixels(pixels);
                tex.Apply();

                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), Vector2.one * 0.5f, 100);
                sr.sprite = sprite;
            }

            CircleCollider2D collider = projectileObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.25f;
            collider.isTrigger = true;

            Projectile projectile = projectileObj.AddComponent<Projectile>();

            if (damage <= 0)
            {
                damage = 10f;
                Debug.LogWarning($"[TowerBase] Damage is 0 or less in {towerData.towerName} Tier {currentTierLevel}! Using fallback: {damage}");
            }

            projectile.SetTarget(target, damage, towerData.ProjectileConfig, isTrueDamage);
        }
    }
}
