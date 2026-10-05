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

        public void SetTarget(EnemyBase t, float dmg, ProjectileConfig config)
        {
            target = t;
            damage = dmg;
            speed = config.speed;
            lifetime = config.lifetime;
            
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
                Destroy(gameObject);
                return;
            }

            elapsed += Time.deltaTime;
            if (elapsed > lifetime)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target.transform.position) < 0.3f)
            {
                target.TakeDamage(damage, false);
                Destroy(gameObject);
            }
        }
    }
}
