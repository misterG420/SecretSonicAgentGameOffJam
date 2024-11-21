using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float vibrationCooldown = 0.5f;
    private float lastVibrationTime = 0f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MapObject") && Time.time >= lastVibrationTime + vibrationCooldown)
        {
            lastVibrationTime = Time.time;

#if UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Document"))
        {
            float timeTaken = Time.time;
            GameManager.TriggerVictory(timeTaken); // Corrected to call directly on the class
        }
    }
}
