using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    public Color flashColor = Color.red;
    private float flashDuration = 0.25f;
    private int flashCount = 3;

    private Renderer objectRenderer;
    private Color originalColor;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("Renderer component not found");
        }

        TriggerFlash();
    }

    public void TriggerFlash()
    {
        if (objectRenderer != null)
        {
            StartCoroutine(FlashCoroutine());
        }
    }

    private System.Collections.IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            objectRenderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            objectRenderer.material.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
}
