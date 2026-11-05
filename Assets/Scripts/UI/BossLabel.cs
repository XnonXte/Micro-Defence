using UnityEngine;
using TMPro;

namespace MicroDefense.UI
{
    /// <summary>
    /// Menampilkan label "BOSS" merah di atas boss enemy
    /// </summary>
    public class BossLabel : MonoBehaviour
    {
        [Header("Label Settings")]
        [SerializeField] private string labelText = "BOSS";
        [SerializeField] private Color labelColor = Color.red;
        [SerializeField] private float fontSize = 24f;
        [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0); // Offset di atas boss
        
        [Header("Animation (Optional)")]
        [SerializeField] private bool enablePulse = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseScale = 0.1f;
        
        [Header("Billboard (Always Face Camera)")]
        [SerializeField] private bool enableBillboard = true;
        
        private GameObject labelObject;
        private TextMeshPro textMesh;
        private Transform mainCamera;
        private Vector3 initialScale;
        
        private void Start()
        {
            CreateBossLabel();
            mainCamera = Camera.main?.transform;
        }
        
        /// <summary>
        /// Buat label text di atas boss
        /// </summary>
        private void CreateBossLabel()
        {
            // Create GameObject untuk label
            labelObject = new GameObject("BossLabel");
            labelObject.transform.SetParent(transform);
            labelObject.transform.localPosition = offset;
            labelObject.transform.localRotation = Quaternion.identity;
            
            // Add TextMeshPro component
            textMesh = labelObject.AddComponent<TextMeshPro>();
            textMesh.text = labelText;
            textMesh.fontSize = fontSize;
            textMesh.color = labelColor;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontStyle = FontStyles.Bold;
            
            // Sorting layer untuk render di atas sprites
            textMesh.sortingOrder = 100;
            
            // Set initial scale untuk pulse animation
            initialScale = labelObject.transform.localScale;
            
            Debug.Log($"[BossLabel] Label '{labelText}' created at offset {offset}");
        }
        
        private void Update()
        {
            if (labelObject == null || textMesh == null) return;
            
            // Billboard effect - label selalu menghadap camera
            if (enableBillboard && mainCamera != null)
            {
                labelObject.transform.rotation = mainCamera.rotation;
            }
            
            // Pulse animation
            if (enablePulse)
            {
                float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
                labelObject.transform.localScale = initialScale * scale;
            }
        }
        
        /// <summary>
        /// Update label text secara runtime
        /// </summary>
        public void SetLabelText(string newText)
        {
            if (textMesh != null)
            {
                textMesh.text = newText;
            }
        }
        
        /// <summary>
        /// Update label color secara runtime
        /// </summary>
        public void SetLabelColor(Color newColor)
        {
            if (textMesh != null)
            {
                textMesh.color = newColor;
            }
        }
        
        /// <summary>
        /// Hide label
        /// </summary>
        public void HideLabel()
        {
            if (labelObject != null)
            {
                labelObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Show label
        /// </summary>
        public void ShowLabel()
        {
            if (labelObject != null)
            {
                labelObject.SetActive(true);
            }
        }
        
        private void OnDestroy()
        {
            // Cleanup
            if (labelObject != null)
            {
                Destroy(labelObject);
            }
        }
    }
}
