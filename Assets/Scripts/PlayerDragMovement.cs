using UnityEngine;

public class PlayerDragMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private bool isMoving = false;

    private float wobbleTimer = 0f;       // Timer for wobble
    private float wobbleDuration = 0.25f;   // Time to complete a full wobble cycle
    private bool isDragging = false;     // For touch input
    private Vector2 targetPosition;      // For touch input

    void Update()
    {
        // Check movement and handle controls
        HandleKeyboardInput();
        HandleTouchInput();

        // Apply wobble effect if moving
        if (isMoving)
        {
            ApplyWobbleEffect();
        }
        else
        {
            ResetRotation(); // Stop wobbling when not moving
        }
    }

    void HandleKeyboardInput()
    {
        // Get WASD input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection != Vector2.zero)
        {
            isMoving = true;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            isMoving = false;
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                targetPosition = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                targetPosition = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }

            if (isDragging)
            {
                isMoving = true;
                Vector2 currentPosition = transform.position;
                Vector2 direction = (targetPosition - currentPosition).normalized;

                transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    void ApplyWobbleEffect()
    {
        wobbleTimer += Time.deltaTime;

        float wobbleAngle = Mathf.Lerp(-7f, 7f, Mathf.PingPong(wobbleTimer / wobbleDuration, 1));
        transform.rotation = Quaternion.Euler(0, 0, wobbleAngle);
    }

    void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        wobbleTimer = 0f; // Reset wobble timer
    }
}
