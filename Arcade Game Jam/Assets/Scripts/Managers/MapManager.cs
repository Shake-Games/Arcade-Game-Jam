using UnityEngine;

namespace SG
{
    public class MapManager : MonoBehaviour
    {
        public GameObject[] mapTiles;  // Assign your map tiles or objects in inspector
        public int gridWidth = 10;
        public int gridHeight = 10;
        public float spacing = 1f;

        private float offsetX;
        private float offsetY;

        void Start()
        {
            offsetX = (gridWidth - 1) * spacing * 0.5f;
            offsetY = (gridHeight - 1) * spacing * 0.5f;

            PlaceMapOnGrid();
        }

        void PlaceMapOnGrid()
        {
            if (mapTiles == null || mapTiles.Length == 0) return;

            int tileIndex = 0;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (tileIndex >= mapTiles.Length)
                        return; // No more tiles to place

                    GameObject tile = mapTiles[tileIndex];
                    tile.transform.position = new Vector3(x * spacing - offsetX, y * spacing - offsetY, 0);
                    tileIndex++;
                }
            }
        }
    }
}
