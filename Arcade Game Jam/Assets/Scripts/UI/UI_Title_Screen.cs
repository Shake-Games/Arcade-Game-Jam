using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace SG
{
    public class UI_Title_Screen : MonoBehaviour
    {
        [Header("Splash Text")]
        public TextMeshProUGUI splashText;
        public float minFontSize = 34f;
        public float maxFontSize = 38f;
        public float pulseSpeed = 2f;

        private void Update()
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            splashText.fontSize = Mathf.Lerp(minFontSize, maxFontSize, t);
        }


        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void QuitGame()
        {
            Debug.Log("Application quit.");
            Application.Quit();
        }
    }
}