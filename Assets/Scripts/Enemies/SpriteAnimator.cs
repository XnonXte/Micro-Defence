using UnityEngine;

namespace MicroDefense.Enemies
{
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] animationFrames;
        [SerializeField] private float frameDuration = 0.1f;
        
        private SpriteRenderer spriteRenderer;
        private int currentFrameIndex;
        private float frameDurationTimer;
        private bool isAnimating;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("[SpriteAnimator] SpriteRenderer not found on " + gameObject.name);
                enabled = false;
                return;
            }

            if (animationFrames == null || animationFrames.Length == 0)
            {
                Debug.LogError("[SpriteAnimator] No animation frames assigned to " + gameObject.name);
                enabled = false;
                return;
            }

            isAnimating = true;
            currentFrameIndex = 0;
            frameDurationTimer = 0f;
            UpdateFrame();
        }

        private void Update()
        {
            if (!isAnimating || animationFrames.Length == 0) return;

            frameDurationTimer += Time.deltaTime;

            if (frameDurationTimer >= frameDuration)
            {
                frameDurationTimer -= frameDuration;
                currentFrameIndex = (currentFrameIndex + 1) % animationFrames.Length;
                UpdateFrame();
            }
        }

        private void UpdateFrame()
        {
            if (spriteRenderer != null && animationFrames.Length > 0)
            {
                spriteRenderer.sprite = animationFrames[currentFrameIndex];
            }
        }

        public void SetAnimationSpeed(float speed)
        {
            frameDuration = speed > 0 ? 1f / speed : 0.1f;
        }

        public void Pause() => isAnimating = false;
        public void Resume() => isAnimating = true;
    }
}
