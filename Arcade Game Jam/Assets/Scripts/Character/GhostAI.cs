using System.Collections;
using UnityEngine;

namespace SG
{
    public class GhostAI : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float checkDistance = 0.6f;
        [SerializeField] private float directionChangeCooldown = 1f;

        [Header("Animation Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] upSprites;
        [SerializeField] private Sprite[] downSprites;
        [SerializeField] private Sprite[] leftSprites;
        [SerializeField] private Sprite[] rightSprites;
        [SerializeField] private float animationSpeed = 0.2f;

        private Vector2 moveDirection;
        private float animTimer;
        private int currentFrame;

        private float directionChangeTimer = 0f;
        private bool animationPaused = false;

        private void Start()
        {
            PickNewDirection();
            ResetDirectionChangeTimer();
        }

        private void Update()
        {
            directionChangeTimer -= Time.deltaTime;

            if (IsWallAhead(moveDirection))
            {
                PickNewDirection();
                ResetDirectionChangeTimer();
                animationPaused = true;
            }
            else
            {
                animationPaused = false;
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
                Animate();
            }

            if (directionChangeTimer <= 0f)
            {
                PickNewDirection();
                ResetDirectionChangeTimer();
            }
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
            if (spriteRenderer == null) return;

            animTimer += Time.deltaTime;
            if (animTimer >= animationSpeed)
            {
                animTimer = 0f;
                currentFrame = (currentFrame + 1) % 2;

                if (moveDirection == Vector2.up)
                    spriteRenderer.sprite = upSprites[currentFrame];
                else if (moveDirection == Vector2.down)
                    spriteRenderer.sprite = downSprites[currentFrame];
                else if (moveDirection == Vector2.left)
                    spriteRenderer.sprite = leftSprites[currentFrame];
                else if (moveDirection == Vector2.right)
                    spriteRenderer.sprite = rightSprites[currentFrame];
            }
        }
    }
}
