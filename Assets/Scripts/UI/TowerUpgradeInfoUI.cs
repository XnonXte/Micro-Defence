using MicroDefense.Core;
using MicroDefense.Towers;
using TMPro;
using UnityEngine;

namespace MicroDefense.UI
{
    /// <summary>
    /// Menampilkan info tier dan upgrade cost di atas/bawah tower saat upgrade tool sedang di-drag
    /// </summary>
    public class TowerUpgradeInfoUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TowerBase tower;
        
        [Header("UI Elements")]
        private GameObject infoContainer;
        private TextMeshPro tierText;
        private TextMeshPro costText;
        
        private bool isShowing;

        private void Awake()
        {
            if (tower == null)
                tower = GetComponent<TowerBase>();
        }

        private void Start()
        {
            CreateInfoUI();
            HideInfo();
        }

        private void CreateInfoUI()
        {
            // Container untuk semua UI
            infoContainer = new GameObject("UpgradeInfoContainer");
            infoContainer.transform.SetParent(transform);
            infoContainer.transform.localPosition = Vector3.zero;
            infoContainer.transform.localScale = Vector3.one;

            // Tier Text (di atas tower)
            GameObject tierObj = new GameObject("TierText");
            tierObj.transform.SetParent(infoContainer.transform);
            tierObj.transform.localPosition = new Vector3(0f, 1.5f, 0f); // Di atas tower
            tierObj.transform.localScale = Vector3.one;
            
            tierText = tierObj.AddComponent<TextMeshPro>();
            tierText.alignment = TextAlignmentOptions.Center;
            tierText.fontSize = 6f; // Diperbesar lagi dari 4.5f ke 6f
            tierText.fontStyle = FontStyles.Bold;
            tierText.color = Color.white;
            tierText.sortingOrder = 100;

            // Cost Text (di bawah tower)
            GameObject costObj = new GameObject("CostText");
            costObj.transform.SetParent(infoContainer.transform);
            costObj.transform.localPosition = new Vector3(0f, -1.5f, 0f); // Di bawah tower
            costObj.transform.localScale = Vector3.one;
            
            costText = costObj.AddComponent<TextMeshPro>();
            costText.alignment = TextAlignmentOptions.Center;
            costText.fontSize = 5.5f; // Diperbesar lagi dari 4f ke 5.5f
            costText.fontStyle = FontStyles.Bold;
            costText.color = new Color(1f, 0.85f, 0f); // Warna gold untuk cost
            costText.sortingOrder = 100;
        }

        public void ShowInfo()
        {
            if (tower == null || infoContainer == null) return;

            isShowing = true;
            infoContainer.SetActive(true);
            UpdateInfo();
        }

        public void HideInfo()
        {
            if (infoContainer == null) return;

            isShowing = false;
            infoContainer.SetActive(false);
        }

        private void UpdateInfo()
        {
            if (tower == null) return;

            // Update Tier Text
            int currentTier = tower.currentTierLevel;
            tierText.text = $"TIER {currentTier}";

            // Update Cost Text
            if (tower.CanUpgrade(out int cost))
            {
                // Cek apakah player punya cukup plasma
                bool canAfford = PlasmaManager.Instance != null && PlasmaManager.Instance.CanAfford(cost);
                
                costText.text = $"${cost}";
                costText.color = canAfford ? new Color(0f, 1f, 0f) : new Color(1f, 0.2f, 0.2f); // Hijau jika cukup, merah jika tidak
            }
            else
            {
                // Max tier
                costText.text = "MAX";
                costText.color = Color.cyan;
            }
        }

        private void Update()
        {
            // Update info setiap frame saat showing (untuk real-time plasma check)
            if (isShowing)
            {
                UpdateInfo();
            }
        }

        private void OnDestroy()
        {
            if (infoContainer != null)
                Destroy(infoContainer);
        }
    }
}
