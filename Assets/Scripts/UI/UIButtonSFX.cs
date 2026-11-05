using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MicroDefense.Core;

namespace MicroDefense.UI
{
    /// <summary>
    /// Tambahkan ke Button untuk auto-play SFX saat hover dan click
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // Global debounce across ALL UIButtonSFX instances (fixes spam hover causing SFX to break)
        private static float s_LastAnyHoverTime = -999f;
        private static int s_LastAnyHoverFrame = -999;
        private static float s_LastAnyClickTime = -999f;
        
        [Header("SFX Settings")]
        [Tooltip("Play hover sound saat mouse masuk ke button")]
        [SerializeField] private bool playHoverSound = true;
        
        [Tooltip("Play click sound saat button di-click")]
        [SerializeField] private bool playClickSound = true;
        
        [Header("Volume Control")]
        [Range(0f, 1f)]
        [SerializeField] private float hoverVolume = 1f;
        
        [Range(0f, 1f)]
        [SerializeField] private float clickVolume = 1f;
        
        [Header("Spam Prevention")]
        [Tooltip("Minimum delay between hover sounds (seconds)")]
        [SerializeField] private float hoverCooldown = 0.25f;
        
        [Tooltip("Minimum delay between click sounds (seconds)")]
        [SerializeField] private float clickCooldown = 0.08f;
        
        private Button button;
        private float lastHoverTime = -999f;
        private float lastClickTime = -999f;
        private bool isHovering = false;
        
        private void OnDisable()
        {
            isHovering = false;
        }
        
        private void Awake()
        {
            button = GetComponent<Button>();
        }
        
        /// <summary>
        /// Dipanggil saat mouse hover ke button
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Hanya play sound jika button interactable
            if (!playHoverSound || button == null || !button.interactable)
                return;
            
            // Prevent repeated enter events
            if (isHovering) return;
            isHovering = true;
            
            float now = Time.unscaledTime;
            
            // Global debounce across all buttons (most important for spam fix)
            if (Time.frameCount == s_LastAnyHoverFrame) return;
            if (now - s_LastAnyHoverTime < hoverCooldown) return;
            if (now - lastHoverTime < hoverCooldown) return;
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonHover();
                lastHoverTime = now;
                s_LastAnyHoverTime = now;
                s_LastAnyHoverFrame = Time.frameCount;
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
        }
        
        /// <summary>
        /// Dipanggil saat button di-click
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // Hanya play sound jika button interactable
            if (!playClickSound || button == null || !button.interactable)
                return;
            
            float now = Time.unscaledTime;
            if (now - s_LastAnyClickTime < clickCooldown) return;
            if (now - lastClickTime < clickCooldown) return;
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
                lastClickTime = now;
                s_LastAnyClickTime = now;
            }
        }
        
        /// <summary>
        /// Method public untuk manually trigger click sound
        /// Bisa dipanggil dari onClick event jika perlu
        /// </summary>
        public void PlayClickSound()
        {
            if (playClickSound && AudioManager.Instance != null)
            {
                if (Time.unscaledTime - lastClickTime < clickCooldown) return;
                AudioManager.Instance.PlayButtonClick();
                lastClickTime = Time.unscaledTime;
            }
        }
        
        /// <summary>
        /// Method public untuk manually trigger hover sound
        /// </summary>
        public void PlayHoverSound()
        {
            if (playHoverSound && AudioManager.Instance != null)
            {
                if (Time.unscaledTime - lastHoverTime < hoverCooldown) return;
                AudioManager.Instance.PlayButtonHover();
                lastHoverTime = Time.unscaledTime;
            }
        }
    }
}
