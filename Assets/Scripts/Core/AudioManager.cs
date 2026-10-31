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

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (bgmAudioSource == null)
            {
                GameObject bgmObj = new GameObject("BGMSource");
                bgmObj.transform.SetParent(transform);
                bgmAudioSource = bgmObj.AddComponent<AudioSource>();
                bgmAudioSource.loop = true;
                bgmAudioSource.playOnAwake = false;
            }

            if (sfxAudioSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxAudioSource = sfxObj.AddComponent<AudioSource>();
                sfxAudioSource.playOnAwake = false;
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
            if (clip == null) return;
            if (bgmAudioSource.clip == clip && bgmAudioSource.isPlaying) return;

            bgmAudioSource.clip = clip;
            bgmAudioSource.Play();
        }

        public void StopBGM()
        {
            bgmAudioSource.Stop();
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;
            sfxAudioSource.PlayOneShot(clip, volumeScale);
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
        public void PlayCardHover() => PlaySFX(cardHover);
        public void PlayCardClick() => PlaySFX(cardClick);
        public void PlayButtonHover() => PlaySFX(buttonHover);
        public void PlayButtonClick() => PlaySFX(buttonClick);
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
            sfxAudioSource.volume = volume;
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
            float bgmVol = PlayerPrefs.GetFloat(BGM_VOL_KEY, 1f);
            float sfxVol = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);

            bgmAudioSource.volume = bgmVol;
            sfxAudioSource.volume = sfxVol;

            if (audioMixer != null)
            {
                audioMixer.SetFloat("BGMVolume", bgmVol > 0 ? Mathf.Log10(bgmVol) * 20 : -80f);
                audioMixer.SetFloat("SFXVolume", sfxVol > 0 ? Mathf.Log10(sfxVol) * 20 : -80f);
            }
        }
    }
}
