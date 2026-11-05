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
    public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
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
        [SerializeField] private float hoverCooldown = 0.1f;
        
        [Tooltip("Minimum delay between click sounds (seconds)")]
        [SerializeField] private float clickCooldown = 0.05f;
        
        private Button button;
        private float lastHoverTime = -999f;
        private float lastClickTime = -999f;
        
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
            
            // Cooldown check - prevent spam
            if (Time.time - lastHoverTime < hoverCooldown)
                return;
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonHover();
                lastHoverTime = Time.time;
            }
        }
        
        /// <summary>
        /// Dipanggil saat button di-click
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // Hanya play sound jika button interactable
            if (!playClickSound || button == null || !button.interactable)
                return;
            
            // Cooldown check - prevent spam
            if (Time.time - lastClickTime < clickCooldown)
                return;
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
                lastClickTime = Time.time;
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
                AudioManager.Instance.PlayButtonClick();
            }
        }
        
        /// <summary>
        /// Method public untuk manually trigger hover sound
        /// </summary>
        public void PlayHoverSound()
        {
            if (playHoverSound && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonHover();
            }
        }
    }
}
