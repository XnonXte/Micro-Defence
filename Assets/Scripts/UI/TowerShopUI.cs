    using MicroDefense.Core;
    using MicroDefense.Data;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    namespace MicroDefense.UI
    {
        public class TowerShopUI : MonoBehaviour
        {
            [Header("Tower ScriptableObjects")]
            [SerializeField] private TowerDataSO bCellData;
            [SerializeField] private TowerDataSO macrophageData;
            [SerializeField] private TowerDataSO tCellData;

            [Header("UI Buttons")]
            [SerializeField] private Button bCellButton;
            [SerializeField] private Button macrophageButton;
            [SerializeField] private Button tCellButton;
            [SerializeField] private Button startWaveButton;

            [Header("HUD TextMeshPro References")]
            [SerializeField] private TMP_Text plasmaText;
            [SerializeField] private TMP_Text waveText;
            [SerializeField] private TMP_Text healthText;
            [SerializeField] private TMP_Text waveStatusText;

            private void OnEnable()
            {
                SubscribeToManagers();
            }

            private void Start()
            {
                SetupButtonDragHandler(bCellButton, bCellData, "B-Cell");
                SetupButtonDragHandler(macrophageButton, macrophageData, "Macrophage");
                SetupButtonDragHandler(tCellButton, tCellData, "T-Cell");

                if (startWaveButton != null)
                    startWaveButton.onClick.AddListener(OnStartWaveClicked);

                SubscribeToManagers();
            }

            private void OnDisable()
            {
                if (WaveManager.Instance != null) WaveManager.Instance.OnWaveStatusChanged -= UpdateWave;
                if (LeakManager.Instance != null) LeakManager.Instance.OnHealthChanged -= UpdateHealth;
                if (PlasmaManager.Instance != null) PlasmaManager.Instance.OnPlasmaChanged -= UpdatePlasma;
            }

            private void SubscribeToManagers()
            {
                if (WaveManager.Instance != null)
                {
                    WaveManager.Instance.OnWaveStatusChanged -= UpdateWave;
                    WaveManager.Instance.OnWaveStatusChanged += UpdateWave;
                }

                if (LeakManager.Instance != null)
                {
                    LeakManager.Instance.OnHealthChanged -= UpdateHealth;
                    LeakManager.Instance.OnHealthChanged += UpdateHealth;
                }

                if (PlasmaManager.Instance != null)
                {
                    PlasmaManager.Instance.OnPlasmaChanged -= UpdatePlasma;
                    PlasmaManager.Instance.OnPlasmaChanged += UpdatePlasma;
                }

                RefreshHud();
            }

            private void RefreshHud()
            {
                if (WaveManager.Instance != null)
                    UpdateWave(WaveManager.Instance.currentWaveIndex + 1, Mathf.Max(1, WaveManager.Instance.TotalWaves));
                if (LeakManager.Instance != null)
                    UpdateHealth(LeakManager.Instance.CurrentHealth, LeakManager.Instance.MaxHealth);
                if (PlasmaManager.Instance != null)
                    UpdatePlasma(PlasmaManager.Instance.CurrentPlasma);
            }

            private void UpdateWave(int current, int total)
            {
                if (waveText != null) waveText.text = $"WAVE {Mathf.Clamp(current, 1, Mathf.Max(1, total))} / {Mathf.Max(1, total)}";
                if (waveStatusText != null)
                    waveStatusText.text = WaveManager.Instance != null && WaveManager.Instance.isWaveSpawning ? "INCOMING ENEMIES" : "READY TO DEPLOY";
            }

            private void UpdateHealth(int current, int max)
            {
                if (healthText != null) healthText.text = $"HEALTH {current}/{max}";
            }

            private void UpdatePlasma(int amount)
            {
                if (plasmaText != null) plasmaText.text = $"PLASMA {amount}";
            }

            private void SetupButtonDragHandler(Button btn, TowerDataSO data, string towerName)
            {
                if (btn == null) return;

                TowerDragHandler dragHandler = btn.GetComponent<TowerDragHandler>();
                if (dragHandler == null)
                    dragHandler = btn.gameObject.AddComponent<TowerDragHandler>();

                dragHandler.SetTowerData(data);
            }

            private void OnStartWaveClicked()
            {
                WaveManager.Instance?.StartNextWave();
            }
        }
    }
