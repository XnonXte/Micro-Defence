using UnityEngine;

namespace MicroDefense.Grid
{
    /// <summary>
    /// Component attached to 2D Grid Tile GameObjects in the scene to define placement rules.
    /// </summary>
    public class GridTile : MonoBehaviour
    {
        [Header("Tile Placement Rule")]
        public TileType tileType = TileType.OffTrackSide;
        public bool IsOccupied = false;
        public GameObject PlacedTower = null;
    }
}
