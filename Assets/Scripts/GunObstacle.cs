using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GunObstacle : MonoBehaviour
{
    public Transform gun1; // Reference to the first gun child object
    public Transform gun2; // Reference to the second gun child object
    public float lineWidth = 0.1f; // Width of the line renderer
    public float detectionRadius = 5f; // Detection radius (if needed)

    private LineRenderer lineRenderer;
    private BoxCollider2D lineCollider;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.tag = "Enemy";
        lineRenderer.enabled = false;

        // Set up the collider to cover the line renderer's path
        lineCollider = gameObject.AddComponent<BoxCollider2D>();
        lineCollider.isTrigger = true;
        lineCollider.enabled = false; // Initially disabled until the player collision
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Enable and set up the line renderer between guns and player
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, gun1.position);
            lineRenderer.SetPosition(1, collision.transform.position);

            // Enable and position the collider to match the line renderer
            Vector2 lineCenter = (gun1.position + collision.transform.position) / 2;
            float lineLength = Vector2.Distance(gun1.position, collision.transform.position);
            lineCollider.enabled = true;
            lineCollider.size = new Vector2(lineLength, lineWidth);
            lineCollider.offset = transform.InverseTransformPoint(lineCenter);
            lineCollider.transform.rotation = Quaternion.FromToRotation(Vector3.right, collision.transform.position - gun1.position);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
        }
    }
}
