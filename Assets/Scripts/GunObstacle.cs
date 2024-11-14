using UnityEngine;
using System.Collections; // Add this to resolve IEnumerator issue

public class GunObstacle : MonoBehaviour
{
    public Transform[] gunSprites;
    public float lineWidth = 0.1f;
    public LayerMask playerLayer;

    private LineRenderer[] lineRenderers;

    void Start()
    {
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

            // Set the color of the line renderer to #D62828 (hex color)
            if (ColorUtility.TryParseHtmlString("#D62828", out Color color))
            {
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
            }


            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

            lineRenderers[i] = lineRenderer;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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


            StartCoroutine(WaitForGameOver(0.5f));
        }
    }

    private IEnumerator WaitForGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameOver();
    }

    private void GameOver()
    {
        GameManager.TriggerGameOver();
        Debug.Log("Game Over() called");
    }
}
