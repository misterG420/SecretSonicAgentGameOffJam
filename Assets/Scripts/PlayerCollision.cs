using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float vibrationCooldown = 0.5f; // Cooldown in seconds before another vibration
    private float lastVibrationTime = 0f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has the tag "MapObject"
        if (collision.gameObject.CompareTag("MapObject") && Time.time >= lastVibrationTime + vibrationCooldown)
        {
            Debug.Log("Player collided with a MapObject!");

#if UNITY_ANDROID
            Handheld.Vibrate(); // Trigger a single short vibration
#endif
            lastVibrationTime = Time.time; // Reset cooldown timer
        }
    }
}
