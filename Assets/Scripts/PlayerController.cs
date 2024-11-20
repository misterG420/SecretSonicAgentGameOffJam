using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private float baselineLoudness;
    public Slider loudnessSlider;
    public float amplificationFactor = 35f;
    private float revealTimer = 0f;
    public float resetTime = 2f;

    private AudioClip microphoneClip;
    private Coroutine revealWaveCoroutine = null;
    private bool isMapRevealed = false;

    private float revealRadius = 2f;
    private float revealSpeed = 2.6f;
    private float revealDelay = 0.04f;

    public Animator playerAnimator; // Reference to the player's Animator
    private float shoutThreshold = 0.7f;
    public SpriteRenderer playerSpriteRenderer;
    public Sprite originalSprite;

    void Start()
    {
       
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>(); // Get the Animator component if not set
        }

        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer if not set
        }

        if (playerSpriteRenderer != null)
        {
            originalSprite = playerSpriteRenderer.sprite; // Store the original sprite
        }
        ResetMap();
        LoadBaseline();
        StartMicrophone();
        ResetSprite();

    }

    private void LoadBaseline()
    {
        if (PlayerPrefs.HasKey("BaselineLoudness"))
        {
            baselineLoudness = PlayerPrefs.GetFloat("BaselineLoudness");
        }
        else
        {
            Debug.LogError("Baseline not calibrated! Please run the calibration scene first.");
        }
    }

    private void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            Microphone.End(null); // Stop any existing microphone instance
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            int sampleRate = 44100;
            if (maxFreq > 0) sampleRate = Mathf.Clamp(44100, minFreq, maxFreq);

            Debug.Log($"Using sample rate: {sampleRate}, MinFreq: {minFreq}, MaxFreq: {maxFreq}");
            microphoneClip = Microphone.Start(Microphone.devices[0], true, 1, sampleRate);
        }
        else
        {
            Debug.LogError("No microphone detected on this device.");
        }
    }

    void Update()
    {
        DetectSound();
    }

    private void DetectSound()
    {
        if (!Microphone.IsRecording(null)) StartMicrophone();

        float[] data = new float[256];
        microphoneClip.GetData(data, 0);
        float loudness = GetNormalizedLoudness(data);
        loudnessSlider.value = loudness;

        Debug.Log($"Loudness: {loudness}, Baseline: {baselineLoudness}, Threshold: {baselineLoudness * 1.005f}");

        if (loudness > baselineLoudness * shoutThreshold)
        {
            if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shouting")) // Check if already shouting
            {
                playerAnimator.SetTrigger("Shout");
                Debug.Log("Shout animation should play!");
                ResetSprite();
            }
        }

        if (loudness > baselineLoudness * 1.005f)
        {
            Debug.Log("Loudness threshold exceeded!");

            if (revealWaveCoroutine != null)
            {
                StopCoroutine(revealWaveCoroutine);
            }
            revealWaveCoroutine = StartCoroutine(RevealMapWave());
            revealTimer = resetTime;
        }

        if (revealTimer > 0)
        {
            revealTimer -= Time.deltaTime;
        }
        else if (isMapRevealed)
        {
            ResetMap();
        }
    }

    public void ResetSprite()
    {
        if (playerSpriteRenderer != null && originalSprite != null)
        {
            playerSpriteRenderer.sprite = originalSprite;
        }
    }


    private float GetNormalizedLoudness(float[] data)
    {
        float sum = 0f;
        for (int i = 0; i < data.Length; i++)
        {
            sum += Mathf.Abs(data[i]);
        }
        return sum / data.Length * amplificationFactor;
    }

    private IEnumerator RevealMapWave()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, revealRadius);

        Debug.Log($"Objects in range: {objectsInRange.Length}");

        List<Collider2D> sortedObjects = new List<Collider2D>(objectsInRange);
        sortedObjects.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position)
                             .CompareTo(Vector2.Distance(transform.position, b.transform.position)));

        foreach (Collider2D col in sortedObjects)
        {
            if (col.CompareTag("MapObject"))
            {
                SpriteRenderer renderer = col.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    StartCoroutine(FadeInObject(renderer));
                }
            }
            yield return new WaitForSeconds(revealDelay);
        }
        isMapRevealed = true;
    }

    private IEnumerator FadeInObject(SpriteRenderer renderer)
    {
        Color color = renderer.color;
        while (color.a < 1f)
        {
            color.a += revealSpeed * Time.deltaTime;
            renderer.color = color;
            yield return null;
        }
    }

    private void ResetMap()
    {
        if (revealWaveCoroutine != null)
        {
            StopCoroutine(revealWaveCoroutine);
            revealWaveCoroutine = null;
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MapObject"))
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = 0;
                renderer.color = color;
            }
        }
        isMapRevealed = false;
    }
}
