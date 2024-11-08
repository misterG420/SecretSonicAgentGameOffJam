using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float baselineLoudness = 0f;
    private float silenceDuration = 0f;
    public float resetTime = 2f; // Time to keep objects visible after activation
    private AudioClip microphoneClip;
    public Slider loudnessSlider;
    public float amplificationFactor = 30f; // Sensitivity control
    private bool isMapRevealed = false;
    private float revealTimer = 0f;
    private bool isBaselineSet = false;
    private float baselineCaptureTime = 2f; // Time to capture baseline loudness
    private float baselineTimer = 0f;

    void Start()
    {
        StartMicrophone();
        SetMapObjectsAlpha(0); // Set all map objects to alpha 0 by default
    }

    private void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
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

            // Capture baseline loudness during the initial period
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

            // Check if loudness exceeds baseline threshold by 1%
            if (loudness > baselineLoudness * 1.01f)
            {
                RevealMap();
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

    private void RevealMap()
    {
        if (!isMapRevealed)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MapObject"))
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    color.a = 1; // Set alpha to 1 to make the object visible
                    renderer.color = color;
                }
            }
            isMapRevealed = true;
        }
    }

    private void ResetMap()
    {
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
}
