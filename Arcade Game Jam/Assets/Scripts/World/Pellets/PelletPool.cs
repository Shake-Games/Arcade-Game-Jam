using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SG
{
    public class PelletPool : MonoBehaviour
    {
        [Header("Pellet Settings")]
        public GameObject pelletPrefab;
        public int poolSize = 100;

        [Header("Grid Settings")]
        public int gridWidth = 14;
        public int gridHeight = 10;
        public float spacing = 0.64f;

        [Header("Hierarchy Management")]
        public Transform pelletParent;
        public Tilemap wallTilemap;

        private Queue<GameObject> pelletPool = new Queue<GameObject>();

        void Start()
        {
            InitializePool();
            SpawnPellets();
        }

        void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject pellet = Instantiate(pelletPrefab);
                pellet.SetActive(false);
                if (pelletParent != null)
                    pellet.transform.SetParent(pelletParent);
                pelletPool.Enqueue(pellet);
            }
        }

        void SpawnPellets()
        {
            int offsetX = (gridWidth - 1) / 2;
            int offsetY = (gridHeight - 1) / 2;

            int pelletsSpawned = 0;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    float posX = Mathf.Round((x - offsetX) * spacing / spacing) * spacing;
                    float posY = Mathf.Round((y - offsetY) * spacing / spacing) * spacing;
                    Vector3 worldPos = new Vector3(posX, posY, 0);
                    Vector3Int cellPos = wallTilemap.WorldToCell(worldPos);

                    if (wallTilemap != null && wallTilemap.HasTile(cellPos))
                        continue; // Skip if wall tile is here

                    if (pelletPool.Count == 0)
                    {
                        Debug.LogWarning("Pellet pool ran out of pellets!");
                        return;
                    }

                    GameObject pellet = pelletPool.Dequeue();
                    pellet.transform.position = worldPos;
                    pellet.SetActive(true);

                    if (pelletParent != null)
                        pellet.transform.SetParent(pelletParent);

                    pelletsSpawned++;
                }
            }

            Debug.Log($"Total pellets spawned: {pelletsSpawned}");
        }

        public void ReturnPelletToPool(GameObject pellet)
        {
            pellet.SetActive(false);
            if (pelletParent != null)
                pellet.transform.SetParent(pelletParent);
            pelletPool.Enqueue(pellet);
        }
    }
}
