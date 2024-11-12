using UnityEngine;

public class GunObstacle : MonoBehaviour
{
    public Transform[] gunSprites; // Assign gun sprite transforms in the inspector
    public float lineWidth = 0.1f;
    public LayerMask playerLayer; // Layer to check for the player in the raycast

    private LineRenderer[] lineRenderers;

    void Start()
    {
        // Initialize line renderers based on the number of gun sprites
        lineRenderers = new LineRenderer[gunSprites.Length];
        for (int i = 0; i < gunSprites.Length; i++)
        {
            GameObject lineObj = new GameObject("LineRenderer" + i);
            lineObj.transform.parent = transform;

            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.tag = "Enemy";
            lineRenderer.enabled = false;

            lineRenderers[i] = lineRenderer;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has collided with the GunObstacle
        if (collision.gameObject.CompareTag("Player"))
        {
            ActivateLineRenderersAndRaycasts(collision.transform);
        }
    }

    private void ActivateLineRenderersAndRaycasts(Transform playerTransform)
    {
        for (int i = 0; i < gunSprites.Length; i++)
        {
            // Enable and set positions for each line renderer
            lineRenderers[i].enabled = true;
            lineRenderers[i].SetPosition(0, gunSprites[i].position);
            lineRenderers[i].SetPosition(1, playerTransform.position);

            // Perform a raycast from each gun sprite to the player
            Vector2 direction = (playerTransform.position - gunSprites[i].position).normalized;
            float distance = Vector2.Distance(gunSprites[i].position, playerTransform.position);

            RaycastHit2D hit = Physics2D.Raycast(gunSprites[i].position, direction, distance, playerLayer);

            // If the raycast hits the player, trigger the game over function
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                GameOver();
                break; // Only trigger game over once
            }
        }
    }

    private void GameOver()
    {
        GameManager.TriggerGameOver();
        Debug.Log("Game Over: Player hit by raycast from gun obstacle!");
    }
}
