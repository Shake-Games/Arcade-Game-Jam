using UnityEngine;
using UnityEngine.InputSystem;

namespace SG
{
    public class GhostManager : MonoBehaviour
    {
        [Header("Ghost Settings")]
        [SerializeField] private GameObject[] ghosts;
        [SerializeField] private CameraFollow cameraFollow;

        private PlayerControls controls;
        private int currentIndex = 0;

        private void Awake()
        {
            controls = new PlayerControls();

            controls.Gameplay.SwapGhost.performed += ctx => SwapToNextGhost();
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        private void Start()
        {
            SetActiveGhost(0);
        }

        private void SwapToNextGhost()
        {
            currentIndex = (currentIndex + 1) % ghosts.Length;
            SetActiveGhost(currentIndex);
        }

        private void SetActiveGhost(int index)
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                bool isActive = (i == index);
                ghosts[i].GetComponent<PlayerInputManager>().enabled = isActive;
            }

            cameraFollow.target = ghosts[index].transform;
        }
    }
}
