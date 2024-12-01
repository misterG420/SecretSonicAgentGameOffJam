using UnityEngine;
using UnityEngine.UI;

public class AlphaLerp : MonoBehaviour
{
    private Image imageRenderer;
    private float lerpDuration = 1.3f;
    private float lerpTimer = 0f;
    private bool isLerping = true;

    private void Start()
    {
        imageRenderer = GetComponent<Image>();
        if (imageRenderer != null)
        {
            Color color = imageRenderer.color;
            color.a = 0f;
            imageRenderer.color = color;
        }
    }

    private void Update()
    {
        if (isLerping && imageRenderer != null)
        {
            lerpTimer += Time.deltaTime;
            float lerpProgress = lerpTimer / lerpDuration;
            Color color = imageRenderer.color;
            color.a = Mathf.Lerp(0f, 1f, lerpProgress);
            imageRenderer.color = color;

            if (lerpTimer >= lerpDuration)
            {
                color.a = 1f;
                imageRenderer.color = color;
                isLerping = false;
            }
        }
    }
}
