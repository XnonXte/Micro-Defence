using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MicroDefense.Core;

namespace MicroDefense.UI
{
    /// <summary>
    /// Health Bar Vertikal dengan cairan yang berkurang dari atas ke bawah
    /// Menggunakan 2 gambar: wadahBening.png (container) dan cairanPlasma.png (isi)
    /// </summary>
    public class VerticalHealthBar : MonoBehaviour
    {
        public static VerticalHealthBar Instance { get; private set; }

        [Header("Referensi Gambar")]
        [Tooltip("Gambar wadah/container (lapisan depan)")]
        [SerializeField] private Image wadahBening;
        
        [Tooltip("Gambar cairan/isi (lapisan belakang, akan di-mask)")]
        [SerializeField] private Image cairanPlasma;

        [Header("Pengaturan Health (Read-Only - Sinkron dengan LeakManager)")]
        [Tooltip("Max HP diambil otomatis dari LeakManager. Tidak bisa diubah manual.")]
        [SerializeField] public int maxHP = 200; // Default value, akan di-override oleh LeakManager
        
        [Tooltip("Current HP saat ini")]
        [SerializeField] private int currentHP;
        
        [Header("Mode")]
        [Tooltip("PENTING: Jika true, abaikan LeakManager dan pakai nilai maxHP dari Inspector")]
        [SerializeField] private bool useCustomHealth = false;
        
        [Header("Animasi")]
        [Tooltip("Durasi animasi penurunan cairan (detik)")]
        [SerializeField] private float drainDuration = 0.3f;
        
        [Tooltip("Aktifkan efek shake saat terkena damage")]
        [SerializeField] private bool useShakeEffect = true;
        
        [Tooltip("Intensitas shake")]
        [SerializeField] private float shakeIntensity = 5f;
        
        [Tooltip("Durasi shake (detik)")]
        [SerializeField] private float shakeDuration = 0.2f;

        [Header("Visual Feedback")]
        [Tooltip("Warna cairan saat HP penuh")]
        [SerializeField] private Color fullHealthColor = Color.cyan;
        
        [Tooltip("Warna cairan saat HP menengah (50%)")]
        [SerializeField] private Color midHealthColor = Color.yellow;
        
        [Tooltip("Warna cairan saat HP rendah")]
        [SerializeField] private Color lowHealthColor = Color.red;

        // Internal state
        private float targetFillAmount = 1f;
        private float currentFillAmount = 1f;
        private Coroutine drainCoroutine;
        private Coroutine shakeCoroutine;
        private Vector3 originalWadahPosition;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Simpan posisi asli wadah untuk shake effect
            if (wadahBening != null)
            {
                originalWadahPosition = wadahBening.rectTransform.localPosition;
            }
        }

        private void Start()
        {
            // PILIH MODE: Custom atau sinkron dengan LeakManager
            if (useCustomHealth)
            {
                // MODE CUSTOM: Pakai nilai dari Inspector, abaikan LeakManager
                Debug.Log($"[VerticalHealthBar] Using CUSTOM health: maxHP={maxHP}");
                currentHP = maxHP; // Force set ke maxHP dari Inspector
            }
            else
            {
                // MODE SINKRON: Ambil dari LeakManager
                if (LeakManager.Instance != null)
                {
                    maxHP = LeakManager.Instance.MaxHealth;
                    currentHP = LeakManager.Instance.CurrentHealth;
                    Debug.Log($"[VerticalHealthBar] Synced with LeakManager: maxHP={maxHP}, currentHP={currentHP}");
                    
                    // Subscribe untuk update
                    LeakManager.Instance.OnHealthChanged += OnHealthChanged;
                }
                else
                {
                    Debug.LogWarning("[VerticalHealthBar] LeakManager not found! Using default values.");
                    currentHP = maxHP;
                }
            }
            
            // Set bar ke kondisi awal
            UpdateHealthBar(currentHP, maxHP, false);
        }

        private void OnDestroy()
        {
            // Unsubscribe saat di-destroy
            if (LeakManager.Instance != null)
            {
                LeakManager.Instance.OnHealthChanged -= OnHealthChanged;
            }
            
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Callback dari LeakManager saat health berubah
        /// </summary>
        private void OnHealthChanged(int newHealth, int maxHealth)
        {
            TakeDamage(currentHP - newHealth);
        }

        /// <summary>
        /// Kurangi HP dan update tampilan bar
        /// </summary>
        /// <param name="damage">Jumlah damage yang diterima</param>
        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            // Kurangi HP
            currentHP -= damage;
            if (currentHP < 0) currentHP = 0;

            // Update tampilan bar dengan animasi
            UpdateHealthBar(currentHP, maxHP, true);

            // Shake effect
            if (useShakeEffect && wadahBening != null)
            {
                if (shakeCoroutine != null)
                    StopCoroutine(shakeCoroutine);
                shakeCoroutine = StartCoroutine(ShakeWadah());
            }

            // Check game over
            if (currentHP <= 0)
            {
                OnHealthDepleted();
            }
        }

        /// <summary>
        /// Reset health ke nilai maksimal
        /// </summary>
        public void ResetHealth()
        {
            currentHP = maxHP;
            UpdateHealthBar(currentHP, maxHP, false);
        }

        /// <summary>
        /// Set health ke nilai tertentu
        /// </summary>
        public void SetHealth(int health, bool animate = true)
        {
            currentHP = Mathf.Clamp(health, 0, maxHP);
            UpdateHealthBar(currentHP, maxHP, animate);
        }

        /// <summary>
        /// Update tampilan bar sesuai HP saat ini
        /// </summary>
        private void UpdateHealthBar(int currentHealth, int maxHealth, bool animate)
        {
            if (cairanPlasma == null) return;

            // Hitung fill amount (1.0 = penuh, 0.0 = kosong)
            float healthRatio = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
            targetFillAmount = Mathf.Clamp01(healthRatio);

            // Update warna berdasarkan HP
            UpdateHealthColor(healthRatio);

            // Animasi atau langsung update
            if (animate && drainDuration > 0)
            {
                // Stop animasi sebelumnya jika ada
                if (drainCoroutine != null)
                    StopCoroutine(drainCoroutine);
                
                drainCoroutine = StartCoroutine(AnimateDrain());
            }
            else
            {
                // Update langsung tanpa animasi
                currentFillAmount = targetFillAmount;
                cairanPlasma.fillAmount = currentFillAmount;
            }
        }

        /// <summary>
        /// Animasi penurunan cairan secara smooth
        /// </summary>
        private IEnumerator AnimateDrain()
        {
            float elapsed = 0f;
            float startFill = currentFillAmount;

            while (elapsed < drainDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / drainDuration);
                
                // Smooth easing
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                
                currentFillAmount = Mathf.Lerp(startFill, targetFillAmount, smoothT);
                cairanPlasma.fillAmount = currentFillAmount;

                yield return null;
            }

            // Pastikan nilai akhir tepat
            currentFillAmount = targetFillAmount;
            cairanPlasma.fillAmount = currentFillAmount;
        }

        /// <summary>
        /// Update warna cairan berdasarkan persentase HP
        /// </summary>
        private void UpdateHealthColor(float healthRatio)
        {
            if (cairanPlasma == null) return;

            Color newColor;

            if (healthRatio > 0.5f)
            {
                // HP > 50%: Interpolasi dari full ke mid
                float t = (healthRatio - 0.5f) * 2f; // 0.5-1.0 -> 0-1
                newColor = Color.Lerp(midHealthColor, fullHealthColor, t);
            }
            else
            {
                // HP <= 50%: Interpolasi dari low ke mid
                float t = healthRatio * 2f; // 0-0.5 -> 0-1
                newColor = Color.Lerp(lowHealthColor, midHealthColor, t);
            }

            cairanPlasma.color = newColor;
        }

        /// <summary>
        /// Efek shake/getar pada wadah saat terkena damage
        /// </summary>
        private IEnumerator ShakeWadah()
        {
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                
                // Shake intensity berkurang seiring waktu
                float t = 1f - (elapsed / shakeDuration);
                float currentIntensity = shakeIntensity * t;

                // Random offset untuk shake
                float offsetX = Random.Range(-currentIntensity, currentIntensity);
                float offsetY = Random.Range(-currentIntensity, currentIntensity);

                wadahBening.rectTransform.localPosition = originalWadahPosition + new Vector3(offsetX, offsetY, 0f);

                yield return null;
            }

            // Kembalikan ke posisi asli
            wadahBening.rectTransform.localPosition = originalWadahPosition;
        }

        /// <summary>
        /// Dipanggil saat HP habis (0)
        /// </summary>
        private void OnHealthDepleted()
        {
            Debug.Log("[VerticalHealthBar] Health depleted! Game Over!");
            
            // LeakManager sudah handle game over, jadi kita tidak perlu panggil lagi
            // Tapi bisa tambahkan efek visual tambahan di sini jika perlu
        }

        /// <summary>
        /// Validasi setup di Inspector
        /// </summary>
        private void OnValidate()
        {
            if (cairanPlasma != null)
            {
                // Pastikan Image menggunakan Filled type
                if (cairanPlasma.type != Image.Type.Filled)
                {
                    Debug.LogWarning("[VerticalHealthBar] cairanPlasma Image harus menggunakan Type: Filled!");
                }

                // Pastikan Fill Method adalah Vertical
                if (cairanPlasma.fillMethod != Image.FillMethod.Vertical)
                {
                    Debug.LogWarning("[VerticalHealthBar] cairanPlasma Fill Method harus Vertical!");
                }

                // Pastikan Fill Origin adalah Bottom (cairan berkurang dari atas)
                if (cairanPlasma.fillOrigin != (int)Image.OriginVertical.Bottom)
                {
                    Debug.LogWarning("[VerticalHealthBar] cairanPlasma Fill Origin harus Bottom!");
                }
            }
        }

        #region Public Getters
        public int CurrentHP => currentHP;
        public int MaxHP => maxHP;
        public float HealthRatio => maxHP > 0 ? (float)currentHP / maxHP : 0f;
        #endregion
    }
}
