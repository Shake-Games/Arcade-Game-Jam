using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SG
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerInputManager : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public LayerMask wallLayer; // Layer mask to detect walls

        private Vector2 moveDirection = Vector2.zero;    // Current direction player is moving
        private Vector2 desiredDirection = Vector2.zero; // Direction player wants to move next

        private PlayerControls controls;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;

            controls = new PlayerControls();

            // Listen for movement input and update desired direction
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

                // Keep only the axis with the largest magnitude to avoid diagonal movement
                if (Mathf.Abs(rounded.x) > Mathf.Abs(rounded.y))
                    desiredDirection = new Vector2(rounded.x, 0);
                else
                    desiredDirection = new Vector2(0, rounded.y);

                // Start moving immediately if not moving or if path is clear in desired direction
                if (moveDirection == Vector2.zero || CanMove(desiredDirection))
                    moveDirection = desiredDirection;
            }
            else
            {
                // Optional: stop movement on input release
                // moveDirection = Vector2.zero;
            }
        }

        private void FixedUpdate()
        {
            // Allow turning mid-movement if path is clear
            if (desiredDirection != moveDirection && CanMove(desiredDirection))
            {
                moveDirection = desiredDirection;
            }

            // Move player in current direction
            if (moveDirection != Vector2.zero)
            {
                Vector2 newPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPos);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                // Stop movement when hitting a wall
                moveDirection = Vector2.zero;
            }
        }

        private bool CanMove(Vector2 direction)
        {
            // Cast a short ray to check if wall is blocking path
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, 0.6f, wallLayer);
            return hit.collider == null;
        }
    }
}
