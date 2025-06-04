using System.Collections;
using UnityEngine;

namespace SG
{
    public class PACManAI : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float checkDistance = 0.6f;
        [SerializeField] private float directionChangeCooldown = 1f;

        [Header("Animation Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] animationSprites;
        [SerializeField] private float animationSpeed = 0.2f;

        private Vector2 moveDirection;
        private float animTimer;
        private int currentFrame;

        private float directionChangeTimer = 0f;
        private bool animationPaused = false;

        private Vector3 targetPosition;
        private bool isMoving = false;

        private float gridSpacing = 1f;

        private void Start()
        {
            var pelletPool = FindObjectOfType<PelletPool>();
            if (pelletPool != null)
                gridSpacing = pelletPool.spacing;

            // Snap Pacman position to grid on start:
            transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Round(transform.position.y),
                transform.position.z
            );

            targetPosition = transform.position;
            PickNewDirection();
            ResetDirectionChangeTimer();
        }

        private void Update()
        {
            directionChangeTimer -= Time.deltaTime;

            if (!isMoving)
            {
                if (IsWallAhead(moveDirection))
                {
                    PickNewDirection();
                    ResetDirectionChangeTimer();
                    animationPaused = true;
                }
                else
                {
                    animationPaused = false;
                    StartCoroutine(MoveToNextGridPosition());
                }
            }

            if (directionChangeTimer <= 0f)
            {
                PickNewDirection();
                ResetDirectionChangeTimer();
            }

            Animate();
            RotateToDirection(moveDirection);
        }

        private IEnumerator MoveToNextGridPosition()
        {
            isMoving = true;
            Vector3 startPos = transform.position;
            targetPosition = startPos + (Vector3)(moveDirection * gridSpacing);

            float elapsed = 0f;
            float duration = gridSpacing / moveSpeed;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, targetPosition, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            isMoving = false;
        }

        private bool IsWallAhead(Vector2 dir)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, checkDistance, LayerMask.GetMask("Walls"));
            return hit.collider != null;
        }

        private void PickNewDirection()
        {
            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            Vector2 newDir;

            for (int i = 0; i < 10; i++)
            {
                newDir = directions[Random.Range(0, directions.Length)];
                if (!IsWallAhead(newDir) && newDir != -moveDirection)
                {
                    moveDirection = newDir;
                    return;
                }
            }

            moveDirection = Vector2.zero;
        }

        private void ResetDirectionChangeTimer()
        {
            directionChangeTimer = Random.Range(directionChangeCooldown, directionChangeCooldown + 1f);
        }

        private void Animate()
        {
            if (animationPaused) return;
            if (animationSprites == null || animationSprites.Length == 0) return;

            animTimer += Time.deltaTime;
            if (animTimer >= animationSpeed)
            {
                animTimer = 0f;
                currentFrame = (currentFrame + 1) % animationSprites.Length;
                spriteRenderer.sprite = animationSprites[currentFrame];
            }
        }

        private void RotateToDirection(Vector2 dir)
        {
            if (dir == Vector2.right)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (dir == Vector2.up)
                transform.rotation = Quaternion.Euler(0, 0, 90);
            else if (dir == Vector2.left)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (dir == Vector2.down)
                transform.rotation = Quaternion.Euler(0, 180, 270);
        }
    }
}