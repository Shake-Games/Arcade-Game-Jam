using System.Collections.Generic;
using UnityEngine;

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
        public float spacing = 1f; // keep this 1f to keep pellets 1 unit apart

        [Header("Hierarchy Management")]
        public Transform pelletParent;

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
                    if (pelletPool.Count == 0)
                    {
                        Debug.LogWarning("Pellet pool ran out of pellets!");
                        return;
                    }

                    GameObject pellet = pelletPool.Dequeue();

                    int posX = x - offsetX;
                    int posY = y - offsetY;

                    pellet.transform.position = new Vector3(posX * spacing, posY * spacing, 0);
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