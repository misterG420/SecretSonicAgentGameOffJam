using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    public float detectionRadius = 5f; // The radius of the circle to detect nearby enemies

    // Define a delegate and event for broadcasting
    public delegate void PlayerHitEvent(Vector3 obstaclePosition);
    public static event PlayerHitEvent OnPlayerHit;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Invoke the event for all listeners, passing the obstacle's position
            OnPlayerHit?.Invoke(transform.position);

            // Broadcast event to enemies within the detection radius
            BroadcastToEnemies();
        }
    }

    void BroadcastToEnemies()
    {
        // Find all colliders within the detection radius using OverlapCircle
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        // Iterate through all detected colliders and check if they are enemies
        foreach (var collider in collidersInRange)
        {
            // Check if the collider belongs to an enemy (assuming enemies are tagged "Enemy")
            if (collider.CompareTag("Enemy"))
            {
                // Example of triggering some action on each enemy
                Debug.Log("Enemy detected in range: " + collider.name);

                // Instead of calling ReactToObstacle, we now handle it in a different way
                // For instance, start chasing the player:
                collider.GetComponent<EnemyScript>().StartChasing(transform.position);
            }
        }
    }

    // Gizmos to visualize the detection radius in the editor
    void OnDrawGizmos()
    {
        // Set the color of the Gizmo
        Gizmos.color = Color.red;

        // Draw a wire sphere at the obstacle's position to represent the detection radius
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
