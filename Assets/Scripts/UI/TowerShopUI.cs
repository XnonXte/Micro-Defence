    using MicroDefense.Core;
    using MicroDefense.Data;
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

            private void Start()
            {
                SetupButtonDragHandler(bCellButton, bCellData, "B-Cell");
                SetupButtonDragHandler(macrophageButton, macrophageData, "Macrophage");
                SetupButtonDragHandler(tCellButton, tCellData, "T-Cell");

                if (startWaveButton != null)
                    startWaveButton.onClick.AddListener(OnStartWaveClicked);
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
