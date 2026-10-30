using UnityEngine;
using MicroDefense.UI;

public class WaveProgressBarTest : MonoBehaviour
{
    [SerializeField] private WaveProgressBar waveProgressBar;
    [SerializeField] private float updateInterval = 2f;

    private float timer = 0f;
    private int currentWave = 1;
    private int totalWaves = 4;

    private void Start()
    {
        if (waveProgressBar == null)
        {
            Debug.LogError("[WaveProgressBarTest] WaveProgressBar not assigned!");
            return;
        }

        // Initialize at wave 1
        waveProgressBar.SetWaveInstant(currentWave, totalWaves);
        Debug.Log($"[WaveProgressBarTest] Started at Wave {currentWave}/{totalWaves}");
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;
            currentWave++;

            if (currentWave > totalWaves)
            {
                currentWave = 1;
                Debug.Log("[WaveProgressBarTest] Reset to Wave 1");
            }

            waveProgressBar.SetWave(currentWave, totalWaves);
            Debug.Log($"[WaveProgressBarTest] Updated to Wave {currentWave}/{totalWaves}");
        }
    }
}
