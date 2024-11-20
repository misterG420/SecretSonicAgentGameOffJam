using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class EnemyScript : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public float detectionRadius = 5f;
    public GameObject alertIcon;

    private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private Vector3 targetPosition;
    private Vector3 eventPosition;
    private Rigidbody2D rb;
    private bool isMovingToEvent = false;
    private Vector3 previousDirection;
    private bool isReturning = false;
    private Transform player;
    private bool isShowingAlert = false;

    // New variables for wobble effect
    private float wobbleSpeed = 0.25f; // Time for one full wobble cycle 
    private float wobbleAmount = 9f;   // How much to rotate (-9 to 9 degrees)
    private bool isWobbling = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player by tag (optional)

        // Find patrol points
        Transform p1 = GameObject.Find("P1")?.transform;
        Transform p2 = GameObject.Find("P2")?.transform;

        if (p1 != null && p2 != null)
        {
            patrolPoints = new Transform[] { p1, p2 };
            targetPosition = patrolPoints[currentPatrolIndex].position;
        }
        else
        {
            Debug.LogWarning("EnemyScript: P1 and/or P2 not found in the scene!");
        }

        // Start patrol if patrol points are valid
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            MoveToTarget(targetPosition);
        }

        // Hide alert icon initially
        alertIcon.SetActive(false);

        // Start wobble effect
        StartCoroutine(WobbleEffect());

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

            // stop returning and continue patrol
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
        ObstacleCollision.OnPlayerHit -= MoveToEventPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // Coroutine to handle the wobble effect
    IEnumerator WobbleEffect()
    {
        while (isWobbling)
        {
            float timeElapsed = 0f;
            float startRotation = transform.rotation.eulerAngles.z;
            float endRotation = startRotation + wobbleAmount;

            while (timeElapsed < wobbleSpeed)
            {
                float t = timeElapsed / wobbleSpeed;
                float zRotation = Mathf.Lerp(startRotation, endRotation, t);
                transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Swap directions and wobble back
            timeElapsed = 0f;
            startRotation = transform.rotation.eulerAngles.z;
            endRotation = startRotation - wobbleAmount;

            while (timeElapsed < wobbleSpeed)
            {
                float t = timeElapsed / wobbleSpeed;
                float zRotation = Mathf.Lerp(startRotation, endRotation, t);
                transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
