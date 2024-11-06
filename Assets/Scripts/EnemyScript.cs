using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Transform[] patrolPoints;   // Points the enemy will patrol between
    public float patrolSpeed = 2f;     // Speed while patrolling
    public float chaseSpeed = 4f;      // Speed when chasing the player
    public float detectionRadius = 5f; // Radius to detect the player

    private int currentPatrolIndex = 0;  // Current patrol point index
    private Transform player;           // Reference to the player
    private bool isChasing = false;     // Whether the enemy is chasing the player
    private Vector3 targetPosition;     // Target position for movement
    private Rigidbody2D rb;             // Reference to the Rigidbody2D for movement

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D for physics-based movement
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player

        if (patrolPoints.Length > 0)
        {
            targetPosition = patrolPoints[currentPatrolIndex].position;
        }

        // Subscribe to the OnPlayerHit event from ObstacleCollision
        ObstacleCollision.OnPlayerHit += StartChasing;
    }

    void Update()
    {
        if (!isChasing)
        {
            // Patrol behavior
            Patrol();
        }
        else
        {
            // If chasing, move toward the player
            ChasePlayer();
        }
    }

    void Patrol()
    {
        // Move towards the current patrol point
        if (Vector3.Distance(transform.position, targetPosition) <= 0.2f)
        {
            // Update to the next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            targetPosition = patrolPoints[currentPatrolIndex].position;
        }

        MoveTowardsTarget(targetPosition, patrolSpeed);
    }

    void ChasePlayer()
    {
        // Move towards the player
        MoveTowardsTarget(player.position, chaseSpeed);
    }

    void MoveTowardsTarget(Vector3 target, float speed)
    {
        // Calculate the direction to the target
        Vector3 direction = (target - transform.position).normalized;

        // Move the enemy using Rigidbody2D to avoid relying on NavMesh
        rb.velocity = direction * speed;
    }

    public void StartChasing(Vector3 obstaclePosition)
    {
        // Check if the enemy is within the detection radius of the obstacle
        float distanceToObstacle = Vector3.Distance(transform.position, obstaclePosition);

        if (distanceToObstacle <= detectionRadius)
        {
            isChasing = true;
            Debug.Log(gameObject.name + " is now chasing the player.");
        }
    }

    // Optional: Unsubscribe from the event if needed (e.g., if the enemy is destroyed)
    void OnDestroy()
    {
        ObstacleCollision.OnPlayerHit -= StartChasing;
    }
}
