using UnityEngine;

public class CanvasAudioManager : MonoBehaviour
{
    public AudioClip victorySound;
    public AudioClip lossSound;

    public GameObject victoryCanvas;
    public GameObject gameOverCanvas;

    private AudioSource audioSource;
    private bool victorySoundPlayed = false;
    private bool lossSoundPlayed = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (victoryCanvas != null && victoryCanvas.activeSelf && !victorySoundPlayed)
        {
            PlaySound(victorySound);
            victorySoundPlayed = true;
        }

        if (gameOverCanvas != null && gameOverCanvas.activeSelf && !lossSoundPlayed)
        {
            PlaySound(lossSound);
            lossSoundPlayed = true;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
