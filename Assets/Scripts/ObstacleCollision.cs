using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    public float detectionRadius = 5f; // The radius to detect nearby enemies when the player hits this obstacle

    // Define a delegate and event for broadcasting the obstacle's event
    public delegate void PlayerHitEvent(Vector3 playerPosition);
    public static event PlayerHitEvent OnPlayerHit;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Capture the player's position when the collision occurs
            Vector3 playerPosition = collision.transform.position;

            // Invoke the event for all listeners, passing the player's position
            OnPlayerHit?.Invoke(playerPosition);

            // Broadcast to nearby enemies
            BroadcastToEnemies(playerPosition);
        }
    }

    void BroadcastToEnemies(Vector3 playerPosition)
    {
        // Detect all colliders within the obstacle's detection radius
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        // Loop through colliders and check if they belong to enemies
        foreach (var collider in collidersInRange)
        {
            if (collider.CompareTag("Enemy"))
            {
                // Command each enemy in range to move to the player's collision position
                collider.GetComponent<EnemyScript>().MoveToEventPosition(playerPosition);
            }
        }
    }

    
}
