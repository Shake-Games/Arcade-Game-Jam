using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SG
{
    public class PACManAI : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float directionChangeCooldown = 1f;

        [Header("Animation Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] animationSprites;
        [SerializeField] private float animationSpeed = 0.2f;

        [Header("Grid Settings")]
        [SerializeField] private Tilemap wallTilemap;

        private Vector2 moveDirection;
        private float animTimer;
        private int currentFrame;

        private float directionChangeTimer = 0f;
        private bool animationPaused = false;

        private Vector3 targetPosition;
        private bool isMoving = false;

        private float gridSpacing = 0.64f;

        private void Start()
        {
            transform.position = new Vector3(
                Mathf.Round(transform.position.x / gridSpacing) * gridSpacing,
                Mathf.Round(transform.position.y / gridSpacing) * gridSpacing,
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

            Debug.Log($"PAC-MAN POS: {transform.position}");
        }

        private bool IsWallAhead(Vector2 dir)
        {
            if (wallTilemap == null) return false;

            Vector3 nextPosition = transform.position + (Vector3)(dir * gridSpacing);
            Vector3Int cellPosition = wallTilemap.WorldToCell(nextPosition);
            return wallTilemap.HasTile(cellPosition);
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
            if (animationPaused || spriteRenderer == null) return;
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
