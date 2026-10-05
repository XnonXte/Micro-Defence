using MicroDefense.Data;
using MicroDefense.Grid;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MicroDefense.UI
{
    /// <summary>
    /// Attached to UI Tower Shop Buttons/Icons to enable 2D Drag & Drop tower placement.
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
            // Optional click feedback or sound
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (towerData == null || EventSystem.current == null) return;

            PlacementGridManager.Instance?.StartDragPlacement(towerData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (towerData == null || PlacementGridManager.Instance == null) return;
            PlacementGridManager.Instance.UpdateDragPlacement(GetMouseWorldPosition(eventData));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (towerData == null || PlacementGridManager.Instance == null) return;
            PlacementGridManager.Instance.EndDragPlacement(GetMouseWorldPosition(eventData));
        }

        private Vector3 GetMouseWorldPosition(PointerEventData eventData)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return Vector3.zero;

            Vector3 screenPos = eventData.position;
            screenPos.z = 0f;
            return mainCam.ScreenToWorldPoint(screenPos);
        }
    }
}
