using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float detectionRadius = 5f;
    public GameObject alertIcon; // GameObject to show when not patrolling

    private int currentPatrolIndex = 0;
    private Vector3 targetPosition;
    private Vector3 eventPosition;
    private Rigidbody2D rb;
    private bool isMovingToEvent = false;
    private Vector3 previousDirection; // To store the previous movement direction
    private bool isReturning = false;  // Flag to indicate the enemy is returning
    private Transform player; // Reference to the player
    private bool isShowingAlert = false; // Flag to track if alert icon is being shown

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag

        if (patrolPoints.Length > 0)
            targetPosition = patrolPoints[currentPatrolIndex].position;

        // Start patrol
        MoveToTarget(targetPosition);

        // Hide alert icon initially
        alertIcon.SetActive(false);

        // Subscribe to the event
        ObstacleCollision.OnPlayerHit += MoveToEventPosition;
    }

    void Update()
    {
        if (isMovingToEvent)
        {
            ShowAlert();
            MoveToTarget(eventPosition);
        }
        else if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            // Move toward the player if within detection radius
            ShowAlert();
            MoveToTarget(player.position);
        }
        else if (isReturning)
        {
            // Move the enemy in the opposite direction temporarily
            MoveToTarget(transform.position + previousDirection * -1);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // If near the patrol point, move to the next one
        if (Vector3.Distance(transform.position, targetPosition) <= 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            targetPosition = patrolPoints[currentPatrolIndex].position;
            MoveToTarget(targetPosition);
        }
    }

    void MoveToTarget(Vector3 target)
    {
        // Move normally towards the target
        Vector3 direction = (target - transform.position).normalized;
        rb.velocity = direction * patrolSpeed;

        // Store the direction the enemy is moving in
        previousDirection = direction;
    }

    public void MoveToEventPosition(Vector3 playerPosition)
    {
        if (Vector3.Distance(transform.position, playerPosition) <= detectionRadius)
        {
            isMovingToEvent = true;
            eventPosition = playerPosition;
        }
    }

    void ShowAlert()
    {
        if (!isShowingAlert)
        {
            isShowingAlert = true;
            alertIcon.SetActive(true);
            Invoke(nameof(HideAlert), 2f); // Hide after 2 seconds
        }
    }

    void HideAlert()
    {
        alertIcon.SetActive(false);
        isShowingAlert = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit a "MapObject"
        if (collision.gameObject.CompareTag("MapObject"))
        {
            Debug.Log("Hit MapObject, returning to previous position...");
            isReturning = true; // Start returning to the direction we came from

            // We reverse the direction to go back
            MoveToTarget(transform.position + previousDirection * -1); // Move in the opposite direction

            // After some time (optional), stop returning and continue patrol
            Invoke(nameof(StopReturning), 1f); // 1 second return time
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player detected - Game Over!");
            GameManager.TriggerGameOver();
            Destroy(collision.gameObject);
        }
    }

    void StopReturning()
    {
        isReturning = false; // Stop returning after the specified time
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        ObstacleCollision.OnPlayerHit -= MoveToEventPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
