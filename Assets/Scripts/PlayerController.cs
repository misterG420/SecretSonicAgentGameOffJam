using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private float baselineLoudness = 0f;
    private float silenceDuration = 0f;
    public float resetTime = 2f; // Time to keep objects visible after activation
    private AudioClip microphoneClip;
    public Slider loudnessSlider;
    public float amplificationFactor = 35f; // Sensitivity control
    private bool isMapRevealed = false;
    private float revealTimer = 0f;
    private bool isBaselineSet = false;
    private float baselineCaptureTime = 2f; // Time to capture baseline loudness
    private float baselineTimer = 0f;

    private float revealRadius = 2f; // Radius within which map objects are revealed
    private float revealSpeed = 1.6f; // Speed of reveal wave
    private float revealDelay = 0.04f; // Delay between revealing each layer



    void Start()
    {
        StartMicrophone();
        ResetState();
    }


    private void ResetState()
    {
        isMapRevealed = false;
        isBaselineSet = false;
        baselineLoudness = 0f;
        baselineTimer = 0f;
        revealTimer = 0f;
        SetMapObjectsAlpha(0); // Hide map objects initially
    }


    private void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            Microphone.End(null); // Stop any existing microphone instance
            microphoneClip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
        }
    }


    private void Update()
    {
        DetectSound();
    }

    private void DetectSound()
    {
        float[] data = new float[256];
        int position = Microphone.GetPosition(Microphone.devices[0]);

        if (position > 0)
        {
            microphoneClip.GetData(data, 0);
            float loudness = GetNormalizedLoudness(data);
            UpdateLoudnessSlider(loudness);

            if (!isBaselineSet)
            {
                baselineTimer += Time.deltaTime;

                baselineLoudness += loudness; // Sum loudness for averaging
                if (baselineTimer >= baselineCaptureTime)
                {
                    baselineLoudness /= baselineCaptureTime; // Calculate average loudness
                    isBaselineSet = true;
                }
                return; // Do nothing until the baseline is set
            }

            if (loudness > baselineLoudness * 1.005f)
            {
                StartCoroutine(RevealMapWave());
                revealTimer = resetTime; // Reset the timer to 2 seconds
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
    }

    private float GetNormalizedLoudness(float[] data)
    {
        float sum = 0f;
        for (int i = 0; i < data.Length; i++)
        {
            sum += Mathf.Abs(data[i]);
        }
        float avgLoudness = sum / data.Length;
        return avgLoudness * amplificationFactor;
    }

    private void UpdateLoudnessSlider(float loudness)
    {
        loudnessSlider.value = loudness;
    }

    private IEnumerator RevealMapWave()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, revealRadius);

        // Sort objects by distance to create a wave effect
        List<Collider2D> sortedObjects = new List<Collider2D>(objectsInRange);
        sortedObjects.Sort((a, b) =>
            Vector2.Distance(transform.position, a.transform.position)
            .CompareTo(Vector2.Distance(transform.position, b.transform.position)));

        // Reveal each object with a slight delay based on distance
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
        StopAllCoroutines(); // Stop any ongoing reveal coroutines
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MapObject"))
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = 0; // Set alpha to 0 to make the object invisible
                renderer.color = color;
            }
        }
        isMapRevealed = false;
    }

    private void SetMapObjectsAlpha(float alpha)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MapObject"))
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = alpha; // Set the alpha to the specified value
                renderer.color = color;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, revealRadius);
    }
}
