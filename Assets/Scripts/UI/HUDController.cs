using MicroDefense.Core;
using UnityEngine;
using UnityEngine.UI;

namespace MicroDefense.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("UI Text References")]
        [SerializeField] private Text plasmaText;
        [SerializeField] private Text waveText;
        [SerializeField] private Text leaksText;

        private void OnEnable()
        {
            if (PlasmaManager.Instance != null)
                PlasmaManager.Instance.OnPlasmaChanged += UpdatePlasmaUI;

            if (WaveManager.Instance != null)
                WaveManager.Instance.OnWaveStatusChanged += UpdateWaveUI;

            if (LeakManager.Instance != null)
                LeakManager.Instance.OnLeakChanged += UpdateLeaksUI;
        }

        private void OnDisable()
        {
            if (PlasmaManager.Instance != null)
                PlasmaManager.Instance.OnPlasmaChanged -= UpdatePlasmaUI;

            if (WaveManager.Instance != null)
                WaveManager.Instance.OnWaveStatusChanged -= UpdateWaveUI;

            if (LeakManager.Instance != null)
                LeakManager.Instance.OnLeakChanged -= UpdateLeaksUI;
        }

        private void UpdatePlasmaUI(int plasmaAmount)
        {
            if (plasmaText != null)
                plasmaText.text = plasmaAmount.ToString("N0");
        }

        private void UpdateWaveUI(int currentWave, int totalWaves)
        {
            if (waveText != null)
                waveText.text = $"WAVE {currentWave} / {totalWaves}";
        }

        private void UpdateLeaksUI(int currentLeaks, int maxLeaks)
        {
            if (leaksText != null)
                leaksText.text = $"{currentLeaks} / {maxLeaks}";
        }
    }
}
