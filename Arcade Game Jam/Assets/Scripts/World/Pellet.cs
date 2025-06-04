using UnityEngine;

namespace SG
{
    public class Pellet : MonoBehaviour
    {
        [SerializeField] private int scoreValue = 10;

        private PelletPool pelletPool;

        private void Awake()
        {
            pelletPool = FindObjectOfType<PelletPool>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Pacman"))
            {
                FindObjectOfType<GameManager>()?.AddScore(scoreValue);
                pelletPool?.ReturnPelletToPool(gameObject);
            }
        }
    }
}
