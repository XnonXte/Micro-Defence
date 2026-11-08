using System.Collections;
using MicroDefense.Core;
using UnityEngine;

namespace MicroDefense.Enemies
{
    public sealed class PlasmaDrop : MonoBehaviour
    {
        [SerializeField] private int value = 10;
        [SerializeField] private float dropDuration = 0.35f;
        [SerializeField] private float bounceHeight = 0.35f;
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private float lifetime = 10f;
        [SerializeField] private float warningThreshold = 0.3f;
        [SerializeField] private float blinkMinInterval = 0.5f;
        [SerializeField] private float blinkMaxInterval = 0.5f;

        private SpriteRenderer spriteRenderer;
        private Collider2D dropCollider;
        private Vector3 startPosition;
        private Vector3 baseScale;
        private Coroutine expirationCoroutine;
        private Coroutine fadeOutCoroutine;
        private bool collected;
        private bool destructionStarted;

        public void Initialize(int plasmaValue, float dropLifetime)
        {
            value = Mathf.Max(1, plasmaValue);
            lifetime = Mathf.Max(0.25f, dropLifetime);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            dropCollider = GetComponent<Collider2D>();
            
            // Set layer ke TransparentFX (layer 1) agar tidak terdeteksi oleh enemy spacing check
            // Layer ini tidak termasuk dalam enemyLayer mask di NavMeshPathFollower
            gameObject.layer = 1; // Layer 1 = "TransparentFX"
            
            // Collider trigger diperlukan untuk OnMouseDown() bekerja
            if (dropCollider != null)
            {
                dropCollider.isTrigger = true;
            }
            
            startPosition = transform.position;
            baseScale = transform.localScale;
        }

        private void Start()
        {
            StartCoroutine(DropAnimation());

            if (expirationCoroutine == null)
            {
                expirationCoroutine = StartCoroutine(ExpirationRoutine());
            }
        }

        private IEnumerator DropAnimation()
        {
            float elapsed = 0f;
            Vector3 endPosition = startPosition;
            Vector3 startAnimationPosition = startPosition + Vector3.up * bounceHeight;
            transform.position = startAnimationPosition;
            transform.localScale = baseScale * 0.65f;

            while (elapsed < dropDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / dropDuration);
                float eased = Mathf.SmoothStep(0f, 1f, progress);
                transform.position = Vector3.Lerp(startAnimationPosition, endPosition, eased);
                transform.localScale = Vector3.Lerp(baseScale * 0.65f, baseScale, eased);
                yield return null;
            }

            transform.position = endPosition;
            transform.localScale = baseScale;
        }

        private IEnumerator ExpirationRoutine()
        {
            float elapsed = 0f;
            float warningWindow = Mathf.Clamp(lifetime * warningThreshold, 0.25f, lifetime);

            while (elapsed < lifetime && !collected && !destructionStarted)
            {
                elapsed += Time.deltaTime;
                float remaining = Mathf.Max(0f, lifetime - elapsed);
                float warningProgress = Mathf.Clamp01(1f - (remaining / Mathf.Max(0.001f, warningWindow)));

                if (spriteRenderer != null)
                {
                    Color color = spriteRenderer.color;
                    color.a = Mathf.Lerp(1f, 0.35f, Mathf.Clamp01(elapsed / lifetime));

                    if (remaining <= warningWindow)
                    {
                        float blinkRate = Mathf.Lerp(blinkMaxInterval, blinkMinInterval, warningProgress);
                        float pulse = Mathf.PingPong(Time.time * (1f / Mathf.Max(blinkRate, 0.05f)), 1f);
                        color.a = Mathf.Lerp(0.35f, 1f, pulse);
                    }

                    spriteRenderer.color = color;
                }

                yield return null;
            }

            if (collected || destructionStarted)
            {
                yield break;
            }

            BeginDestruction();
        }

        private void BeginDestruction()
        {
            if (destructionStarted)
            {
                return;
            }

            destructionStarted = true;

            if (expirationCoroutine != null)
            {
                StopCoroutine(expirationCoroutine);
                expirationCoroutine = null;
            }

            if (dropCollider != null)
            {
                dropCollider.enabled = false;
            }

            if (fadeOutCoroutine == null)
            {
                fadeOutCoroutine = StartCoroutine(FadeOutAndDestroy());
            }
        }

        private void OnMouseDown()
        {
            if (collected || destructionStarted) return;
            collected = true;

            if (dropCollider != null)
                dropCollider.enabled = false;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlasmaPickup();
            }

            if (PlasmaManager.Instance != null)
                PlasmaManager.Instance.AddPlasma(value);

            BeginDestruction();
        }

        private IEnumerator FadeOutAndDestroy()
        {
            float elapsed = 0f;
            Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
            Vector3 startScale = transform.localScale;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / fadeDuration);

                if (spriteRenderer != null)
                {
                    Color color = startColor;
                    color.a = Mathf.Lerp(startColor.a, 0f, progress);
                    spriteRenderer.color = color;
                }

                transform.localScale = Vector3.Lerp(startScale, startScale * 1.25f, progress);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
