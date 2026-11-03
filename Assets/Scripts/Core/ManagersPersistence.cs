using UnityEngine;

namespace MicroDefense.Core
{
    /// <summary>
    /// Attach ke parent GameObject yang berisi semua Managers
    /// Akan otomatis handle duplicate managers saat scene transition
    /// </summary>
    public class ManagersPersistence : MonoBehaviour
    {
        public static ManagersPersistence Instance { get; private set; }
        
        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;
        
        private void Awake()
        {
            // Check jika sudah ada instance lain
            if (Instance != null && Instance != this)
            {
                if (showDebugLogs)
                    Debug.Log($"[ManagersPersistence] Duplicate detected! Destroying {gameObject.name}");
                
                // Destroy GameObject ini (yang duplicate)
                Destroy(gameObject);
                return;
            }
            
            // Set sebagai instance dan persist across scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (showDebugLogs)
                Debug.Log($"[ManagersPersistence] Initialized. This Managers object will persist across scenes.");
        }
        
        private void OnDestroy()
        {
            // Clear instance jika ini yang di-destroy
            if (Instance == this)
            {
                Instance = null;
                
                if (showDebugLogs)
                    Debug.Log("[ManagersPersistence] Instance cleared.");
            }
        }
    }
}
