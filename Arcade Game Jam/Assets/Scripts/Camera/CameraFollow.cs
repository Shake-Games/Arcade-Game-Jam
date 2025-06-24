using UnityEngine;

namespace SG
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform activeGhost;
        [SerializeField] private Transform pacman;

        [Header("Follow Settings")]
        [SerializeField] private float followSpeed = 5f;

        [Header("Zoom Settings")]
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 10f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float zoomDistanceFactor = 0.5f;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
                Debug.LogError("CameraFollow script must be attached to a Camera.");
        }

        private void LateUpdate()
        {
            if (activeGhost == null || pacman == null) return;

            Vector3 targetPos = new Vector3(activeGhost.position.x, activeGhost.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

            float distance = Vector2.Distance(pacman.position, activeGhost.position);
            float targetZoom = Mathf.Clamp(minZoom + distance * zoomDistanceFactor, minZoom, maxZoom);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
        }

        public Transform target
        {
            get => activeGhost;
            set => activeGhost = value;
        }

        public void SetPacman(Transform newPacman)
        {
            pacman = newPacman;
        }
    }
}
