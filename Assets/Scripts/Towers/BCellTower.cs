using MicroDefense.Data;
using MicroDefense.Enemies;
using UnityEngine;

namespace MicroDefense.Towers
{
    public class BCellTower : TowerBase
    {
        protected override bool TryPerformAttack()
        {
            EnemyBase target = FindNearestEnemy(currentTierStats.range);
            if (target == null) return false;

            SpawnProjectile(target);
            if (currentTierStats.specialAbility == SpecialAbilityType.AgglutinationSlow)
            {
                target.ApplySlow(0.30f, 2.0f);
            }
            return true;
        }

        private void SpawnProjectile(EnemyBase target)
        {
            GameObject proj = new GameObject("Projectile");
            proj.transform.position = transform.position;
            
            SpriteRenderer sr = proj.AddComponent<SpriteRenderer>();
            sr.color = Color.cyan;
            sr.sortingOrder = 10;
            
            Texture2D tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            Color[] pix = new Color[1024];
            Vector2 center = Vector2.one * 16;
            for (int i = 0; i < 1024; i++)
            {
                int y = i / 32;
                int x = i % 32;
                pix[i] = Vector2.Distance(new Vector2(x, y), center) <= 14 ? Color.cyan : new Color(0,0,0,0);
            }
            tex.SetPixels(pix);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), Vector2.one * 0.5f, 100);
            
            CircleCollider2D col = proj.AddComponent<CircleCollider2D>();
            col.radius = 0.2f;
            col.isTrigger = true;
            
            Projectile p = proj.AddComponent<Projectile>();
            p.SetTarget(target, currentTierStats.damage, Data.ProjectileConfig);
        }
    }
}
