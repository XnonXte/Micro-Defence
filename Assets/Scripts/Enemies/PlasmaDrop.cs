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

        private SpriteRenderer spriteRenderer;
        private Collider2D dropCollider;
        private Vector3 startPosition;
        private Vector3 baseScale;
        private bool collected;

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

        private void OnMouseDown()
        {
            if (collected) return;
            collected = true;

            if (dropCollider != null)
                dropCollider.enabled = false;

            if (PlasmaManager.Instance != null)
                PlasmaManager.Instance.AddPlasma(value);

            StartCoroutine(FadeOutAndDestroy());
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
