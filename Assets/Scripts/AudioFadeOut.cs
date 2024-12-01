using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoftAudioFadeOutFromCurrentVolume : MonoBehaviour
{
    private AudioSource audioSource;
    public float fadeStartTime = 24f;
    private float fadeDuration = 8f;
    private bool isFading = false;
    private float fadeTimer = 0f;
    private float initialVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initialVolume = audioSource.volume;
    }

    private void Update()
    {
        if (audioSource.isPlaying && !isFading && audioSource.time >= fadeStartTime)
        {
            isFading = true;
            fadeTimer = 0f;
        }

        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float fadeProgress = fadeTimer / fadeDuration;
            audioSource.volume = Mathf.Lerp(initialVolume, 0f, fadeProgress);

            if (fadeTimer >= fadeDuration)
            {
                audioSource.volume = 0f;
                audioSource.Stop();
                isFading = false;
            }
        }
    }
}
