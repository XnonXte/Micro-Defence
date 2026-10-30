using MicroDefense.Data;
using MicroDefense.Grid;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MicroDefense.UI
{
    /// <summary>
    /// Tower placement drag handler - creates ghost preview that follows cursor exactly
    /// </summary>
    public class TowerDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [Header("Tower Data")]
        [SerializeField] private TowerDataSO towerData;

        public void SetTowerData(TowerDataSO data)
        {
            towerData = data;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"[TowerDragHandler] OnPointerDown on {gameObject.name}");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log($"[TowerDragHandler] OnBeginDrag on {gameObject.name}");
            
            if (towerData == null)
            {
                Debug.LogError($"[TowerDragHandler] towerData is null!");
                return;
            }

            if (EventSystem.current == null)
            {
                Debug.LogError($"[TowerDragHandler] EventSystem is null!");
                return;
            }

            if (PlacementGridManager.Instance == null)
            {
                Debug.LogError($"[TowerDragHandler] PlacementGridManager.Instance is null!");
                return;
            }

            PlacementGridManager.Instance.StartDragPlacement(towerData);
            Debug.Log($"[TowerDragHandler] Started drag placement for {towerData.towerName}");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (towerData == null || PlacementGridManager.Instance == null)
                return;

            Vector3 worldPos = GetMouseWorldPosition(eventData);
            PlacementGridManager.Instance.UpdateDragPlacement(worldPos);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (towerData == null || PlacementGridManager.Instance == null)
                return;

            Vector3 worldPos = GetMouseWorldPosition(eventData);
            PlacementGridManager.Instance.EndDragPlacement(worldPos);
            Debug.Log($"[TowerDragHandler] Ended drag placement");
        }

        /// <summary>
        /// Konversi pointer event position ke world position
        /// </summary>
        private Vector3 GetMouseWorldPosition(PointerEventData eventData)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                Debug.LogError("[TowerDragHandler] Main camera not found!");
                return Vector3.zero;
            }

            // Ambil screen position dari pointer
            Vector3 screenPos = eventData.position;
            
            // Konversi screen ke world space
            // Z = 0 untuk 2D orthographic view
            Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            worldPos.z = 0f;

            return worldPos;
        }
    }
}
