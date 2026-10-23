    using MicroDefense.Core;
    using MicroDefense.Data;
    using MicroDefense.Towers;
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
            [SerializeField] private TowerUpgradeDragHandler upgradeTool;

            [Header("HUD TextMeshPro References")]
            [SerializeField] private TMP_Text plasmaText;
            [SerializeField] private TMP_Text waveText;
            [SerializeField] private TMP_Text healthText;
            [SerializeField] private TMP_Text waveStatusText;

            [Header("Layout Panels")]
            [SerializeField] private RectTransform plasmaPanel;
            [SerializeField] private RectTransform[] towerPanels = new RectTransform[3];
            [SerializeField] private RectTransform leaksPanel;
            [SerializeField] private RectTransform waveStatusPanel;

            [Header("Tower Card UI")]
            [SerializeField] private TMP_Text[] towerNameTexts = new TMP_Text[3];
            [SerializeField] private TMP_Text[] towerCostTexts = new TMP_Text[3];
            [SerializeField] private Button[] towerUpgradeButtons = new Button[3];
            [SerializeField] private TMP_Text leaksLevelText;

            private TowerBase[] placedTowers = new TowerBase[3];

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

                for (int i = 0; i < towerUpgradeButtons.Length; i++)
                {
                    int index = i;
                    if (towerUpgradeButtons[index] != null)
                        towerUpgradeButtons[index].onClick.AddListener(() => UpgradeTower(index));
                }

                RefreshTowerCards();
                SubscribeToManagers();
            }

            private void OnDisable()
            {
                if (WaveManager.Instance != null) WaveManager.Instance.OnWaveStatusChanged -= UpdateWave;
                if (LeakManager.Instance != null) LeakManager.Instance.OnHealthChanged -= UpdateHealth;
                if (PlasmaManager.Instance != null) PlasmaManager.Instance.OnPlasmaChanged -= UpdatePlasma;
                for (int i = 0; i < towerUpgradeButtons.Length; i++)
                {
                    int index = i;
                    if (towerUpgradeButtons[index] != null)
                        towerUpgradeButtons[index].onClick.RemoveAllListeners();
                }
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
                RefreshTowerCards();
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
                RefreshTowerCards();
            }

            private void UpdatePlasma(int amount)
            {
                if (plasmaText != null) plasmaText.text = $"PLASMA {amount}";
                RefreshTowerCards();
            }

            public void RefreshUI()
            {
                RefreshHud();
                RefreshTowerCards();
            }

            private void RefreshTowerCards()
            {
                TowerDataSO[] data = { bCellData, macrophageData, tCellData };
                for (int i = 0; i < data.Length; i++)
                {
                    TowerDataSO tower = data[i];
                    if (towerNameTexts != null && i < towerNameTexts.Length && towerNameTexts[i] != null)
                        towerNameTexts[i].text = tower != null ? tower.towerName : "TOWER";
                    if (towerCostTexts != null && i < towerCostTexts.Length && towerCostTexts[i] != null)
                        towerCostTexts[i].text = tower != null ? $"COST: {tower.baseCost} PLASMA" : "COST: -";

                    int upgradeCost = GetNextUpgradeCost(tower);
                    bool canUpgrade = placedTowers[i] != null && placedTowers[i].CanUpgrade(out upgradeCost);
                    if (towerUpgradeButtons != null && i < towerUpgradeButtons.Length && towerUpgradeButtons[i] != null)
                    {
                        towerUpgradeButtons[i].interactable = canUpgrade;
                        TMP_Text label = towerUpgradeButtons[i].GetComponentInChildren<TMP_Text>();
                        if (label != null) label.text = canUpgrade ? $"UPGRADE {upgradeCost}" : "UPGRADE";
                    }
                }

                if (leaksLevelText != null && LeakManager.Instance != null)
                    leaksLevelText.text = $"LEAKS LEVEL\n{LeakManager.Instance.MaxHealth - LeakManager.Instance.CurrentHealth}";
            }

            private int GetNextUpgradeCost(TowerDataSO tower)
            {
                if (tower == null || tower.tiers == null || tower.tiers.Count < 2) return 0;
                return tower.tiers[1].upgradeCost;
            }

            private void UpgradeTower(int index)
            {
                if (index < 0 || index >= placedTowers.Length || placedTowers[index] == null) return;
                placedTowers[index].TryUpgrade();
                RefreshTowerCards();
            }

            public void RegisterPlacedTower(TowerBase tower)
            {
                if (tower == null || tower.Data == null) return;
                int index = GetTowerIndex(tower.Data.towerType);
                if (index >= 0) placedTowers[index] = tower;
                RefreshTowerCards();
            }

            private int GetTowerIndex(TowerType type)
            {
                if (type == TowerType.BCell) return 0;
                if (type == TowerType.Macrophage) return 1;
                if (type == TowerType.TCell) return 2;
                return -1;
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
