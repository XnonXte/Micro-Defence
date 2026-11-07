using MicroDefense.Data;
using UnityEngine;

namespace MicroDefense.Enemies
{
    public sealed class SwarmVirus : EnemyBase
    {
        private int remainingLives;

        public override void Initialize(EnemyDataSO data)
        {
            base.Initialize(data);
            remainingLives = Mathf.Max(1, data.swarmLives);
            currentHealth = Mathf.Max(1f, data.swarmHealthPerLife);
            ApplyLifeSprite();
        }

        public override void TakeDamage(float rawDamage, bool isTrueDamage = false)
        {
            // Don't process damage if already dead or no data
            if (enemyData == null)
            {
                Debug.LogWarning($"[SwarmVirus.TakeDamage] enemyData is null on {gameObject.name}");
                return;
            }
            
            // If already at 0 health with 0 lives, we're truly dead - ignore further damage
            if (currentHealth <= 0f && remainingLives <= 0)
            {
                Debug.Log($"[SwarmVirus.TakeDamage] {gameObject.name} is already dead, ignoring damage");
                return;
            }

            float effectiveArmor = currentArmor * (1f - GetArmorShredFactor());
            float finalDamage = isTrueDamage ? rawDamage : Mathf.Max(1f, rawDamage - effectiveArmor);
            currentHealth -= finalDamage;
            
            Debug.Log($"[SwarmVirus.TakeDamage] {gameObject.name} took {finalDamage} damage. Health: {currentHealth}/{enemyData.swarmHealthPerLife}, Lives: {remainingLives}");
            
            PublishHealthChanged(enemyData.swarmHealthPerLife);

            // Still has health in current life
            if (currentHealth > 0f)
                return;

            // Lost a life
            remainingLives--;
            Debug.Log($"[SwarmVirus] {gameObject.name} lost a life! Remaining lives: {remainingLives}");
            
            if (remainingLives <= 0)
            {
                Debug.Log($"[SwarmVirus] {gameObject.name} has no more lives, calling Die()");
                Die();
                return;
            }

            // Still has lives left - regenerate
            currentHealth = Mathf.Max(1f, enemyData.swarmHealthPerLife);
            ApplyLifeSprite();
            PublishHealthChanged(enemyData.swarmHealthPerLife);
            Debug.Log($"[SwarmVirus] {gameObject.name} regenerated with {currentHealth} HP");
        }

        private void ApplyLifeSprite()
        {
            if (spriteRenderer == null || enemyData == null)
                return;

            Sprite nextSprite = remainingLives >= 3
                ? enemyData.swarmSpriteLife1
                : remainingLives == 2
                    ? enemyData.swarmSpriteLife2
                    : enemyData.swarmSpriteLife3;

            if (nextSprite != null)
                spriteRenderer.sprite = nextSprite;
        }

        protected override void Die()
        {
            Debug.Log($"[SwarmVirus.Die] {gameObject.name} is dying. Invoking OnEnemyDied event.");
            base.Die();
        }

        private float GetArmorShredFactor()
        {
            return 0f;
        }
    }
}
