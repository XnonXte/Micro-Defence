using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button backButton;

    private MainMenuController mainMenuController;

    private void Start()
    {
        mainMenuController = GetComponentInParent<MainMenuController>();
        
        // Setup BGM volume slider
        if (bgmVolumeSlider != null)
        {
            float savedBgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
            bgmVolumeSlider.value = savedBgm;
            bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        // Setup SFX volume slider
        if (sfxVolumeSlider != null)
        {
            float savedSfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.value = savedSfx;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Setup quality dropdown
        if (qualityDropdown != null)
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.onValueChanged.AddListener(SetQuality);
            RefreshQualityOptions();
        }

        // Setup fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        // Setup back button
        if (backButton != null)
            backButton.onClick.AddListener(() => mainMenuController.ShowMainMenu());
    }

    private void SetBGMVolume(float volume)
    {
        if (MicroDefense.Core.AudioManager.Instance != null)
        {
            MicroDefense.Core.AudioManager.Instance.SetBGMVolume(volume);
        }
        else
        {
            AudioListener.volume = volume;
        }
    }

    private void SetSFXVolume(float volume)
    {
        if (MicroDefense.Core.AudioManager.Instance != null)
        {
            MicroDefense.Core.AudioManager.Instance.SetSFXVolume(volume);
        }
    }

    private void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void RefreshQualityOptions()
    {
        if (qualityDropdown == null) return;
        
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
    }
}
