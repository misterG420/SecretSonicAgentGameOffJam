using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerTutorialLevelController : MonoBehaviour
{
    private float baselineLoudness;
    public Slider loudnessSlider;
    public float amplificationFactor = 35f;
    private float revealTimer = 0f;
    public float resetTime = 2f;

    private AudioClip microphoneClip;
    private Coroutine revealWaveCoroutine = null;
    private bool isMapRevealed = false;
    private bool hasInteractedWithOperator = false;

    private float revealRadius = 2f;
    private float revealSpeed = 12f;
    private float revealDelay = 0.02f;

    public Animator playerAnimator;
    private float shoutThreshold = 0.7f;
    public SpriteRenderer playerSpriteRenderer;
    public Sprite originalSprite;

    void Start()
    {

        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }

        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (playerSpriteRenderer != null)
        {
            originalSprite = playerSpriteRenderer.sprite;
        }

        // Disable features initially
        if (loudnessSlider != null)
        {
            loudnessSlider.gameObject.SetActive(false);
        }

        ShowAllMapObjects();
    }

    void Update()
    {
        if (hasInteractedWithOperator)
        {
            DetectSound();

            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Shouting") && stateInfo.normalizedTime >= 1f)
            {
                // Reset the trigger after the shout animation is done playing
                playerAnimator.ResetTrigger("Shout");
            }
        }
    }

    void DetectSound()
    {
        if (!Microphone.IsRecording(null)) StartMicrophone();

        float[] data = new float[256];
        microphoneClip.GetData(data, 0);
        float loudness = GetNormalizedLoudness(data);
        loudnessSlider.value = loudness;

        // Debug log for testing
        Debug.Log($"Loudness: {loudness}, Baseline: {baselineLoudness}, Threshold: {baselineLoudness * shoutThreshold}");

        if (loudness > baselineLoudness * shoutThreshold)
        {
            if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shouting"))
            {
                playerAnimator.SetTrigger("Shout");
                Debug.Log("Shout animation triggered!");
            }
        }

        if (loudness > baselineLoudness * 1.005f)
        {
            Debug.Log("Loudness threshold exceeded! Starting reveal.");

            if (revealWaveCoroutine != null)
            {
                StopCoroutine(revealWaveCoroutine);
            }

            revealWaveCoroutine = StartCoroutine(RevealMapWave());

            Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, revealRadius);
            if (objectsInRange.Length > 0)
            {
                Collider2D closestObject = null;
                float closestDistance = Mathf.Infinity;

                foreach (Collider2D col in objectsInRange)
                {
                    if (col.CompareTag("MapObject"))
                    {
                        float distance = Vector2.Distance(transform.position, col.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestObject = col;
                        }
                    }
                }

                if (closestObject != null)
                {
                    SpriteRenderer renderer = closestObject.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        StartCoroutine(FadeInObject(renderer));
                    }
                }
            }

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

    private void ShowAllMapObjects()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MapObject"))
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = 1f;
                renderer.color = color;
            }
        }
        isMapRevealed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasInteractedWithOperator && collision.CompareTag("Operator"))
        {
            hasInteractedWithOperator = true;
            Debug.Log("Player interacted with Operator! Activating features.");

            // Enable features
            if (loudnessSlider != null)
            {
                loudnessSlider.gameObject.SetActive(true);
            }

            ResetMap(); // Conceal the map
            LoadBaseline();
            StartMicrophone();
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
}
