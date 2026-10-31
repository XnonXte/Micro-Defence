using MicroDefense.Data;
using UnityEngine;

namespace MicroDefense.Enemies
{
    public sealed class SwarmVirus : EnemyBase
    {
        private int remainingLives;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Initialize(EnemyDataSO data)
        {
            base.Initialize(data);
            remainingLives = Mathf.Max(1, data.swarmLives);
            currentHealth = Mathf.Max(1f, data.swarmHealthPerLife);
            ApplyLifeSprite();
        }

        public override void TakeDamage(float rawDamage, bool isTrueDamage = false)
        {
            if (currentHealth <= 0f || enemyData == null)
                return;

            float effectiveArmor = currentArmor * (1f - GetArmorShredFactor());
            float finalDamage = isTrueDamage ? rawDamage : Mathf.Max(1f, rawDamage - effectiveArmor);
            currentHealth -= finalDamage;
            PublishHealthChanged(enemyData.swarmHealthPerLife);

            if (currentHealth > 0f)
                return;

            remainingLives--;
            if (remainingLives <= 0)
            {
                Die();
                return;
            }

            currentHealth = Mathf.Max(1f, enemyData.swarmHealthPerLife);
            ApplyLifeSprite();
            PublishHealthChanged(enemyData.swarmHealthPerLife);
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

        private float GetArmorShredFactor()
        {
            return 0f;
        }
    }
}
