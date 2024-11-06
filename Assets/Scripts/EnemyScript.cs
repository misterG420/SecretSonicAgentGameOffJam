using UnityEngine;
using System;

public class EnemyScript : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float detectionRadius = 5f; // Radius for detecting the player to end the game

    private int currentPatrolIndex = 0;
    private Vector3 targetPosition;
    private bool isMovingToEvent = false;
    private Vector3 eventPosition;
    private Rigidbody2D rb;

    // Event for game over
    public static event Action OnGameOver;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Initial patrol target
        if (patrolPoints.Length > 0)
            targetPosition = patrolPoints[currentPatrolIndex].position;

        // Subscribe to player hit events
        ObstacleCollision.OnPlayerHit += MoveToEventPosition;
    }

    void Update()
    {
        if (isMovingToEvent)
        {
            // Move towards the event position
            MoveTowardsTarget(eventPosition, patrolSpeed);

            // Check if the enemy has reached the event position
            if (Vector3.Distance(transform.position, eventPosition) <= 0.2f)
            {
                isMovingToEvent = false; // Stop moving to event and resume patrol
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // Move towards the current patrol point
        if (Vector3.Distance(transform.position, targetPosition) <= 0.2f)
        {
            // Go to the next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            targetPosition = patrolPoints[currentPatrolIndex].position;
        }

        MoveTowardsTarget(targetPosition, patrolSpeed);
    }

    void MoveTowardsTarget(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    public void MoveToEventPosition(Vector3 playerPosition)
    {
        // Only move to event if within detection range of the obstacle
        if (Vector3.Distance(transform.position, playerPosition) <= detectionRadius)
        {
            isMovingToEvent = true;
            eventPosition = playerPosition;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player detected - Game Over!");
            OnGameOver?.Invoke(); // Trigger game over
            Destroy(collision.gameObject);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events on destroy
        ObstacleCollision.OnPlayerHit -= MoveToEventPosition;
    }

    // Draw detection radius in the editor for game over range
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
