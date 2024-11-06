using UnityEngine;

public class PlayerDragMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    private Vector2 targetPosition;
    private bool isDragging = false;

    void Update()
    {
        // Handle input (touch or mouse)
        HandleInput();

        // Move and rotate the player towards the target position
        MovePlayer();
    }

    void HandleInput()
    {
        if (Input.touchCount > 0) // For mobile devices
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            if (touch.phase == TouchPhase.Began) // If touch begins
            {
                isDragging = true;
                targetPosition = Camera.main.ScreenToWorldPoint(touch.position); // Convert touch position to world space
            }
            else if (touch.phase == TouchPhase.Moved) // If touch moves
            {
                targetPosition = Camera.main.ScreenToWorldPoint(touch.position); // Update target position
            }
            else if (touch.phase == TouchPhase.Ended) // If touch ends
            {
                isDragging = false;
            }
        }
        else if (Input.GetMouseButtonDown(0)) // For desktop (mouse)
        {
            isDragging = true;
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convert mouse position to world space
        }
        else if (Input.GetMouseButtonUp(0)) // If mouse is released
        {
            isDragging = false;
        }
    }

    void MovePlayer()
    {
        if (isDragging)
        {
            // Move towards the target position
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Rotate the player to face the target position
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Player rotates around the z-axis
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
