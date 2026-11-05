using UnityEngine;
using UnityEngine.Audio;

namespace MicroDefense.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource uiSfxAudioSource; // Separate source for UI hover/click to prevent spam breaking gameplay SFX

        [Header("Audio Mixer (Optional)")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("BGM Clips")]
        public AudioClip mainMenuBGM;
        public AudioClip level1To5BGM;
        public AudioClip level6To10BGM;

        [Header("SFX Clips")]
        public AudioClip turretPlaced;
        public AudioClip turretUpgraded;
        public AudioClip tCellShooting;
        public AudioClip bCellShooting;
        public AudioClip plasmaPickup;
        public AudioClip pipetPicked;
        public AudioClip pipetUnpicked;
        public AudioClip notEnoughBalance;
        public AudioClip macrophageHurt;
        public AudioClip incomingWave;
        public AudioClip gameOver;
        public AudioClip enemyLolos;
        public AudioClip enemyHit;
        public AudioClip cardHover;
        public AudioClip cardClick;
        public AudioClip buttonHover;
        public AudioClip buttonClick;
        public AudioClip bossGrunt;
        
        [Header("UI Extra SFX")]
        public AudioClip uiClickA;
        public AudioClip uiClickB;
        public AudioClip uiTapA;
        public AudioClip uiTapB;
        public AudioClip uiSwitchA;
        public AudioClip uiSwitchB;

        private const string BGM_VOL_KEY = "BGMVolume";
        private const string SFX_VOL_KEY = "SFXVolume";
        private const float HOVER_SFX_COOLDOWN = 0.25f;
        private const float UI_SFX_COOLDOWN = 0.05f;
        private const int HOVER_BURST_LIMIT = 4;
        private const float HOVER_BURST_WINDOW = 0.35f;
        private const float HOVER_BURST_SUPPRESS = 0.6f;
        
        private float lastHoverSfxTime = -999f;
        private float lastUiSfxTime = -999f;
        private AudioClip lastHoverClip;
        private int hoverBurstCount = 0;
        private float hoverBurstWindowStart = -999f;
        private float hoverSuppressUntil = -999f;

        private void Awake()
        {
            // Singleton pattern: hanya boleh ada satu instance AudioManager
            if (Instance != null && Instance != this)
            {
                Debug.Log("[AudioManager] Instance already exists. Destroying duplicate from scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            // DontDestroyOnLoad membuat AudioManager tetap hidup di semua scene
            DontDestroyOnLoad(gameObject);
            
            Debug.Log("[AudioManager] Initialized as singleton with DontDestroyOnLoad. Will persist across all scenes.");

            // Ensure we have audio sources
            if (bgmAudioSource == null)
            {
                GameObject bgmObj = new GameObject("BGMSource");
                bgmObj.transform.SetParent(transform);
                bgmAudioSource = bgmObj.AddComponent<AudioSource>();
                bgmAudioSource.loop = true;
                bgmAudioSource.playOnAwake = false;
                bgmAudioSource.spatialBlend = 0f;
                Debug.Log("[AudioManager] Created BGM AudioSource");
            }

            if (sfxAudioSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxAudioSource = sfxObj.AddComponent<AudioSource>();
                sfxAudioSource.playOnAwake = false;
                sfxAudioSource.spatialBlend = 0f;
                Debug.Log("[AudioManager] Created SFX AudioSource");
            }
            
            if (uiSfxAudioSource == null)
            {
                GameObject uiSfxObj = new GameObject("UISFXSource");
                uiSfxObj.transform.SetParent(transform);
                uiSfxAudioSource = uiSfxObj.AddComponent<AudioSource>();
                uiSfxAudioSource.playOnAwake = false;
                uiSfxAudioSource.spatialBlend = 0f;
                Debug.Log("[AudioManager] Created UI SFX AudioSource");
            }

            LoadVolumeSettings();
        }

        private void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.MainMenu)
            {
                PlayBGM(mainMenuBGM);
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager.PlayBGM] Clip is null");
                return;
            }
            
            if (bgmAudioSource == null)
            {
                Debug.LogError("[AudioManager.PlayBGM] bgmAudioSource is null or destroyed!");
                return;
            }
            
            if (bgmAudioSource.clip == clip && bgmAudioSource.isPlaying)
            {
                return; // Already playing this clip
            }

            bgmAudioSource.clip = clip;
            bgmAudioSource.Play();
        }

        public void StopBGM()
        {
            if (bgmAudioSource == null)
            {
                Debug.LogWarning("[AudioManager.StopBGM] bgmAudioSource is null");
                return;
            }
            
            bgmAudioSource.Stop();
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            PlaySFXInternal(sfxAudioSource, "SFXSource", clip, volumeScale, false, false);
        }
        
        private void PlayUISFX(AudioClip clip, float volumeScale = 1f)
        {
            // UI sounds (especially hover) use a dedicated source so spam can't break gameplay SFX
            PlaySFXInternal(uiSfxAudioSource, "UISFXSource", clip, volumeScale, true, false);
        }
        
        private void PlayHoverUISFX(AudioClip clip)
        {
            // Hover throttling is handled by caller; bypass internal hover/UI throttles to avoid double-consumption
            PlaySFXInternal(uiSfxAudioSource, "UISFXSource", clip, 1f, true, true);
        }
        
        private void PlaySFXInternal(AudioSource source, string sourceName, AudioClip clip, float volumeScale, bool isUI, bool bypassUiThrottle)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager.PlaySFX] Clip is null");
                return;
            }
            
            // Safety net: throttle hover clips even if called directly
            if (!bypassUiThrottle && (clip == buttonHover || clip == cardHover))
            {
                if (!CanPlayHoverSfx(clip)) return;
                isUI = true; // force hover through UI path behavior
                source = uiSfxAudioSource;
                sourceName = "UISFXSource";
                bypassUiThrottle = true;
            }
            else if (isUI && !bypassUiThrottle)
            {
                if (!CanPlayUiSfx()) return;
            }
            
            if (source == null)
            {
                // Recreate source if it was lost/destroyed
                GameObject obj = new GameObject(sourceName);
                obj.transform.SetParent(transform);
                source = obj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                source.volume = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);
                if (sourceName == "UISFXSource") uiSfxAudioSource = source;
                else sfxAudioSource = source;
                Debug.LogWarning($"[AudioManager.PlaySFX] {sourceName} missing; recreated.");
            }
            
            // Keep source in a valid state
            if (!source.enabled) source.enabled = true;
            if (source.mute) source.mute = false;
            if (!source.gameObject.activeInHierarchy) return;
            
            // Sync volume with saved setting (do not force non-zero; user may have set 0)
            float savedVol = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);
            if (!Mathf.Approximately(source.volume, savedVol)) source.volume = savedVol;
            
            float vol = Mathf.Clamp01(volumeScale);
            
            // For UI SFX, replace current sound to avoid spam buildup; for gameplay SFX use PlayOneShot overlap
            if (isUI)
            {
                if (source.isPlaying) source.Stop();
                source.clip = clip;
                source.time = 0f;
                source.Play();
                return;
            }
            
            source.PlayOneShot(clip, vol);
        }
        
        private bool CanPlayHoverSfx(AudioClip clip)
        {
            if (clip == null) return false;
            float now = Time.unscaledTime;
            
            // If we detected a hover spam burst, suppress hover SFX briefly
            if (now < hoverSuppressUntil) return false;
            
            // Burst detection across all buttons/cards
            if (now - hoverBurstWindowStart > HOVER_BURST_WINDOW)
            {
                hoverBurstWindowStart = now;
                hoverBurstCount = 0;
            }
            hoverBurstCount++;
            if (hoverBurstCount > HOVER_BURST_LIMIT)
            {
                hoverSuppressUntil = now + HOVER_BURST_SUPPRESS;
                hoverBurstCount = 0;
                return false;
            }
            
            // Global hover throttle (prevents spam across ALL buttons/cards)
            if (now - lastHoverSfxTime < HOVER_SFX_COOLDOWN)
                return false;
            
            lastHoverSfxTime = now;
            lastHoverClip = clip;
            return true;
        }
        
        private bool CanPlayUiSfx()
        {
            if (Time.unscaledTime - lastUiSfxTime < UI_SFX_COOLDOWN)
                return false;
            lastUiSfxTime = Time.unscaledTime;
            return true;
        }

        // Helper methods for playing specific named SFX
        public void PlayTurretPlaced() => PlaySFX(turretPlaced);
        public void PlayTurretUpgraded() => PlaySFX(turretUpgraded);
        public void PlayTCellShooting() => PlaySFX(tCellShooting);
        public void PlayBCellShooting() => PlaySFX(bCellShooting);
        public void PlayPlasmaPickup() => PlaySFX(plasmaPickup);
        public void PlayPipetPicked() => PlaySFX(pipetPicked);
        public void PlayPipetUnpicked() => PlaySFX(pipetUnpicked);
        public void PlayNotEnoughBalance() => PlaySFX(notEnoughBalance);
        public void PlayMacrophageHurt() => PlaySFX(macrophageHurt);
        public void PlayIncomingWave() => PlaySFX(incomingWave);
        public void PlayGameOver() => PlaySFX(gameOver);
        public void PlayEnemyLolos() => PlaySFX(enemyLolos);
        public void PlayEnemyHit() => PlaySFX(enemyHit);
        public void PlayCardHover()
        {
            if (!CanPlayHoverSfx(cardHover)) return;
            PlayHoverUISFX(cardHover);
        }
        
        public void PlayCardClick()
        {
            if (!CanPlayUiSfx()) return;
            PlayUISFX(cardClick);
        }
        
        public void PlayButtonHover()
        {
            if (!CanPlayHoverSfx(buttonHover)) return;
            PlayHoverUISFX(buttonHover);
        }
        
        public void PlayButtonClick()
        {
            if (!CanPlayUiSfx()) return;
            PlayUISFX(buttonClick);
        }
        public void PlayBossGrunt() => PlaySFX(bossGrunt);
        public void PlayUIClickA() => PlaySFX(uiClickA);
        public void PlayUIClickB() => PlaySFX(uiClickB);
        public void PlayUITapA() => PlaySFX(uiTapA);
        public void PlayUITapB() => PlaySFX(uiTapB);
        public void PlayUISwitchA() => PlaySFX(uiSwitchA);
        public void PlayUISwitchB() => PlaySFX(uiSwitchB);

        // Volume Controls
        public void SetBGMVolume(float volume)
        {
            if (bgmAudioSource == null)
            {
                Debug.LogError("[AudioManager.SetBGMVolume] bgmAudioSource is null!");
                return;
            }
            
            volume = Mathf.Clamp01(volume);
            bgmAudioSource.volume = volume;
            PlayerPrefs.SetFloat(BGM_VOL_KEY, volume);
            PlayerPrefs.Save();

            if (audioMixer != null)
            {
                float db = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
                audioMixer.SetFloat("BGMVolume", db);
            }
        }

        public void SetSFXVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            
            if (sfxAudioSource == null && uiSfxAudioSource == null)
            {
                Debug.LogError("[AudioManager.SetSFXVolume] sfxAudioSource is null!");
                // Still save preference
            }
            
            if (sfxAudioSource != null) sfxAudioSource.volume = volume;
            if (uiSfxAudioSource != null) uiSfxAudioSource.volume = volume;
            
            PlayerPrefs.SetFloat(SFX_VOL_KEY, volume);
            PlayerPrefs.Save();

            if (audioMixer != null)
            {
                float db = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
                audioMixer.SetFloat("SFXVolume", db);
            }
        }

        private void LoadVolumeSettings()
        {
            if (bgmAudioSource == null)
            {
                Debug.LogError("[AudioManager.LoadVolumeSettings] Audio sources are null!");
                return;
            }
            
            float bgmVol = PlayerPrefs.GetFloat(BGM_VOL_KEY, 1f);
            float sfxVol = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);

            bgmAudioSource.volume = bgmVol;
            if (sfxAudioSource != null) sfxAudioSource.volume = sfxVol;
            if (uiSfxAudioSource != null) uiSfxAudioSource.volume = sfxVol;

            if (audioMixer != null)
            {
                audioMixer.SetFloat("BGMVolume", bgmVol > 0 ? Mathf.Log10(bgmVol) * 20 : -80f);
                audioMixer.SetFloat("SFXVolume", sfxVol > 0 ? Mathf.Log10(sfxVol) * 20 : -80f);
            }
        }
    }
}
