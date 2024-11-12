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
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has collided with the GunObstacle");
            ActivateLineRenderersAndRaycasts(collision.transform);
        }
    }

    private void ActivateLineRenderersAndRaycasts(Transform playerTransform)
    {
        for (int i = 0; i < gunSprites.Length; i++)
        {
            lineRenderers[i].enabled = true;
            lineRenderers[i].SetPosition(0, gunSprites[i].position);
            lineRenderers[i].SetPosition(1, playerTransform.position);

                GameOver();
                Debug.Log("Line Renderer hit Player");
        }
    }

    private void GameOver()
    {
        GameManager.TriggerGameOver();
        Debug.Log("Game Over() called");
    }
}
