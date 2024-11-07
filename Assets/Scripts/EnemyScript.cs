using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float detectionRadius = 5f;
    public float avoidanceRadius = 1f; // Radius to avoid obstacles

    private int currentPatrolIndex = 0;
    private Vector3 targetPosition;
    private bool isMovingToEvent = false;
    private Vector3 eventPosition;
    private Rigidbody2D rb;
    private float waitTime = 2f; // Wait time at event position
    private bool isWaiting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (patrolPoints.Length > 0)
            targetPosition = patrolPoints[currentPatrolIndex].position;

        ObstacleCollision.OnPlayerHit += MoveToEventPosition;
    }

    void Update()
    {
        if (isMovingToEvent)
        {
            if (!isWaiting)
                MoveTowardsTargetWithAvoidance(eventPosition, patrolSpeed);

            if (Vector3.Distance(transform.position, eventPosition) <= 0.2f && !isWaiting)
            {
                isWaiting = true;
                Invoke(nameof(ReturnToPatrol), waitTime); // Wait before returning to patrol
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (Vector3.Distance(transform.position, targetPosition) <= 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            targetPosition = patrolPoints[currentPatrolIndex].position;
        }

        MoveTowardsTargetWithAvoidance(targetPosition, patrolSpeed);
    }

    void MoveTowardsTargetWithAvoidance(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        Vector3 avoidanceDirection = Vector3.zero;
        int obstacleLayerMask = LayerMask.GetMask("MapObject");

        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius, obstacleLayerMask);
        foreach (Collider2D obstacle in obstacles)
        {
            Vector3 obstaclePos = new Vector3(obstacle.transform.position.x, obstacle.transform.position.y, 0);
            Vector3 toObstacle = transform.position - obstaclePos;
            avoidanceDirection += toObstacle.normalized / toObstacle.sqrMagnitude;
        }

        Vector3 moveDirection = (direction + avoidanceDirection).normalized;
        rb.velocity = moveDirection * speed;
    }

    public void MoveToEventPosition(Vector3 playerPosition)
    {
        if (Vector3.Distance(transform.position, playerPosition) <= detectionRadius)
        {
            isMovingToEvent = true;
            eventPosition = playerPosition;
            isWaiting = false; // Reset waiting status
        }
    }

    void ReturnToPatrol()
    {
        isMovingToEvent = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player detected - Game Over!");
            GameManager.TriggerGameOver();
            Destroy(collision.gameObject);
        }
    }

    void OnDestroy()
    {
        ObstacleCollision.OnPlayerHit -= MoveToEventPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
    }
}
