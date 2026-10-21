using UnityEngine;
using MicroDefense.Enemies;
using MicroDefense.Data;

namespace MicroDefense.Towers
{
    public class Projectile : MonoBehaviour
    {
        private EnemyBase target;
        private float damage;
        private float speed;
        private float lifetime;
        private float elapsed = 0f;
        private bool hasHit = false;
        private bool isTrueDamage;

        public void SetTarget(EnemyBase t, float dmg, ProjectileConfig config, bool trueDamage = false)
        {
            target = t;
            damage = dmg;
            isTrueDamage = trueDamage;
            speed = config.speed;
            lifetime = config.lifetime;
            
            Debug.Log($"[Projectile] Initialized - Target: {(t != null ? t.name : "null")}, Damage: {damage}, Speed: {speed}");

            // Apply visual config
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = config.color;
            }
            
            transform.localScale = Vector3.one * config.size;
        }

        private void Update()
        {
            if (target == null || target.currentHealth <= 0)
            {
                Debug.Log("[Projectile] Target is null or dead - destroying");
                Destroy(gameObject);
                return;
            }

            elapsed += Time.deltaTime;
            if (elapsed > lifetime)
            {
                Debug.LogWarning("[Projectile] Lifetime exceeded - destroying");
                Destroy(gameObject);
                return;
            }

            // Move towards target
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Check hit distance - MAKE IT BIGGER
            float hitDistance = 0.5f;
            if (Vector3.Distance(transform.position, target.transform.position) < hitDistance)
            {
                if (!hasHit)
                {
                    hasHit = true;
                    Debug.Log($"[Projectile] HIT! Dealing {damage} damage to {target.name}. Current health: {target.currentHealth}");
                    target.TakeDamage(damage, isTrueDamage);
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasHit) return;

            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy == target)
            {
                hasHit = true;
                Debug.Log($"[Projectile] COLLISION HIT! Dealing {damage} damage to {target.name}. Current health: {target.currentHealth}");
                target.TakeDamage(damage, false);
                Destroy(gameObject);
            }
        }
    }
}
