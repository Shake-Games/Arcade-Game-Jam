using UnityEngine;
using TMPro;

namespace SG
{
    public class GameManager : MonoBehaviour
    {
        private int score = 0;
        public TextMeshProUGUI scoreText;

        void Start()
        {
            UpdateScoreUI();
        }

        public void AddScore(int amount)
        {
            score += amount;
            UpdateScoreUI();
        }

        void UpdateScoreUI()
        {
            if (scoreText != null)
                scoreText.text = "Score: " + score;
        }
    }
}
