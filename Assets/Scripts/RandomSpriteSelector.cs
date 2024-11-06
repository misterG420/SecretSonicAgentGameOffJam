using UnityEngine;

public class RandomSpriteSelector : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites; // Array to hold possible sprites
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Get the SpriteRenderer component attached to the object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if there are sprites in the array
        if (sprites.Length > 0)
        {
            // Select a random sprite from the array
            int randomIndex = Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[randomIndex];
        }
        else
        {
            Debug.LogWarning("No sprites assigned to the RandomSpriteSelector.");
        }
    }
}
