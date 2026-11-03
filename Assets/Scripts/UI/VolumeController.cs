using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MicroDefense.Core;

namespace MicroDefense.UI
{
    /// <summary>
    /// Controller untuk volume sliders (BGM & SFX)
    /// Attach ke GameObject dengan 2 sliders untuk BGM dan SFX
    /// </summary>
    public class VolumeController : MonoBehaviour
    {
        [Header("Volume Sliders")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        
        [Header("Volume Text Labels (Optional)")]
        [SerializeField] private TMP_Text bgmValueText;
        [SerializeField] private TMP_Text sfxValueText;
        
        [Header("Settings")]
        [SerializeField] private bool showPercentage = true;
        [SerializeField] private string percentFormat = "0"; // "0" for 75, "0.0" for 75.5
        
        private void Start()
        {
            InitializeSliders();
            LoadVolumeSettings();
        }
        
        private void OnEnable()
        {
            // Subscribe to slider events
            if (bgmSlider != null)
                bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            
            if (sfxSlider != null)
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        private void OnDisable()
        {
            // Unsubscribe from slider events
            if (bgmSlider != null)
                bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
            
            if (sfxSlider != null)
                sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
        
        /// <summary>
        /// Initialize slider settings
        /// </summary>
        private void InitializeSliders()
        {
            // Set slider range 0-1
            if (bgmSlider != null)
            {
                bgmSlider.minValue = 0f;
                bgmSlider.maxValue = 1f;
                bgmSlider.wholeNumbers = false;
            }
            
            if (sfxSlider != null)
            {
                sfxSlider.minValue = 0f;
                sfxSlider.maxValue = 1f;
                sfxSlider.wholeNumbers = false;
            }
        }
        
        /// <summary>
        /// Load saved volume settings dan update UI
        /// </summary>
        private void LoadVolumeSettings()
        {
            // Load from PlayerPrefs (same keys as AudioManager)
            float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            
            // Update sliders tanpa trigger event
            if (bgmSlider != null)
            {
                bgmSlider.SetValueWithoutNotify(bgmVolume);
                UpdateBGMText(bgmVolume);
            }
            
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(sfxVolume);
                UpdateSFXText(sfxVolume);
            }
        }
        
        /// <summary>
        /// Called when BGM slider value changes
        /// </summary>
        private void OnBGMVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(value);
            }
            
            UpdateBGMText(value);
            
            // Optional: Play preview sound saat adjust volume
            // (Tidak perlu karena BGM sudah playing)
        }
        
        /// <summary>
        /// Called when SFX slider value changes
        /// </summary>
        private void OnSFXVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
            }
            
            UpdateSFXText(value);
            
            // Play preview sound saat adjust SFX volume
            PlaySFXPreview();
        }
        
        /// <summary>
        /// Update BGM volume text label
        /// </summary>
        private void UpdateBGMText(float value)
        {
            if (bgmValueText != null)
            {
                if (showPercentage)
                {
                    bgmValueText.text = $"{(value * 100).ToString(percentFormat)}%";
                }
                else
                {
                    bgmValueText.text = value.ToString("F2");
                }
            }
        }
        
        /// <summary>
        /// Update SFX volume text label
        /// </summary>
        private void UpdateSFXText(float value)
        {
            if (sfxValueText != null)
            {
                if (showPercentage)
                {
                    sfxValueText.text = $"{(value * 100).ToString(percentFormat)}%";
                }
                else
                {
                    sfxValueText.text = value.ToString("F2");
                }
            }
        }
        
        /// <summary>
        /// Play preview sound saat adjust SFX volume
        /// </summary>
        private void PlaySFXPreview()
        {
            if (AudioManager.Instance != null)
            {
                // Play button click sebagai preview
                AudioManager.Instance.PlayButtonClick();
            }
        }
        
        /// <summary>
        /// Public method untuk set BGM volume dari luar
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            if (bgmSlider != null)
            {
                bgmSlider.value = Mathf.Clamp01(volume);
            }
        }
        
        /// <summary>
        /// Public method untuk set SFX volume dari luar
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            if (sfxSlider != null)
            {
                sfxSlider.value = Mathf.Clamp01(volume);
            }
        }
        
        /// <summary>
        /// Reset volume ke default (100%)
        /// </summary>
        public void ResetToDefault()
        {
            SetBGMVolume(1f);
            SetSFXVolume(1f);
        }
        
        /// <summary>
        /// Mute BGM
        /// </summary>
        public void MuteBGM()
        {
            SetBGMVolume(0f);
        }
        
        /// <summary>
        /// Mute SFX
        /// </summary>
        public void MuteSFX()
        {
            SetSFXVolume(0f);
        }
        
        /// <summary>
        /// Unmute BGM (set ke volume terakhir atau default)
        /// </summary>
        public void UnmuteBGM()
        {
            float lastVolume = PlayerPrefs.GetFloat("LastBGMVolume", 1f);
            SetBGMVolume(lastVolume);
        }
        
        /// <summary>
        /// Unmute SFX (set ke volume terakhir atau default)
        /// </summary>
        public void UnmuteSFX()
        {
            float lastVolume = PlayerPrefs.GetFloat("LastSFXVolume", 1f);
            SetSFXVolume(lastVolume);
        }
    }
}
