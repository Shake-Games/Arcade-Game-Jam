using UnityEngine;
using UnityEngine.InputSystem;

namespace SG
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private LayerMask wallLayer;

        [Header("Animation Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float animationSpeed = 0.2f;

        [Header("Up Sprites")]
        [SerializeField] private Sprite[] upSprites;

        [Header("Down Sprites")]
        [SerializeField] private Sprite[] downSprites;

        [Header("Left Sprites")]
        [SerializeField] private Sprite[] leftSprites;

        [Header("Right Sprites")]
        [SerializeField] private Sprite[] rightSprites;

        private Vector2 moveDirection = Vector2.zero;
        private Vector2 desiredDirection = Vector2.zero;

        private PlayerControls controls;
        private Rigidbody2D rb;

        private float animationTimer = 0f;
        private int animationFrame = 0;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;

            controls = new PlayerControls();
            controls.Gameplay.Move.performed += ctx => SetDirection(ctx.ReadValue<Vector2>());
            controls.Gameplay.Move.canceled += ctx => SetDirection(Vector2.zero);
        }

        private void OnEnable()
        {
            controls.Gameplay.Enable();
        }

        private void OnDisable()
        {
            controls.Gameplay.Disable();
        }

        private void SetDirection(Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                Vector2 rounded = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));

                if (Mathf.Abs(rounded.x) > Mathf.Abs(rounded.y))
                    desiredDirection = new Vector2(rounded.x, 0);
                else
                    desiredDirection = new Vector2(0, rounded.y);

                if (moveDirection == Vector2.zero || CanMove(desiredDirection))
                    moveDirection = desiredDirection;
            }
        }

        private void FixedUpdate()
        {
            if (desiredDirection != moveDirection && CanMove(desiredDirection))
            {
                moveDirection = desiredDirection;
            }

            if (moveDirection != Vector2.zero)
            {
                Vector2 newPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPos);

                AnimateMovement();
            }
        }

        private void AnimateMovement()
        {
            animationTimer += Time.fixedDeltaTime;

            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;
                animationFrame = (animationFrame + 1) % 2;

                if (moveDirection == Vector2.up)
                    spriteRenderer.sprite = upSprites[animationFrame];
                else if (moveDirection == Vector2.down)
                    spriteRenderer.sprite = downSprites[animationFrame];
                else if (moveDirection == Vector2.left)
                    spriteRenderer.sprite = leftSprites[animationFrame];
                else if (moveDirection == Vector2.right)
                    spriteRenderer.sprite = rightSprites[animationFrame];
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                moveDirection = Vector2.zero;
            }
        }

        private bool CanMove(Vector2 direction)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, 0.6f, wallLayer);
            return hit.collider == null;
        }
    }
}
