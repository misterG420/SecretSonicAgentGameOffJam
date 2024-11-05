using UnityEngine;
using UnityEngine.UI; 

public class PlayerController : MonoBehaviour
{
    private float lastLoudness;
    private float baselineVolume = 0f; 
    private const float visibilityDuration = 2f; // Time to keep objects visible after activation (in seconds)
    private const string mapLayerName = "MapLayer"; 
    private AudioClip microphoneClip; // Store the microphone audio clip
    private const float baselineCalibrationTime = 1f; // Time to calibrate the baseline

    public Slider loudnessSlider; 
    public float sensitivity = 0.001f; 

    void Start()
    {
        StartMicrophone();
        StartCoroutine(CalibrateBaseline());
    }

    private void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            // Start recording from the microphone without looping
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

        // Check if the microphone is currently recording
        if (position > 0)
        {
            microphoneClip.GetData(data, 0); // Get audio data from the microphone clip
            float loudness = GetAverageVolume(data);
            UpdateLoudnessIndicator(loudness); // Update the slider with loudness
            RevealMap(loudness);
            lastLoudness = loudness;
        }
    }

    private float GetAverageVolume(float[] data)
    {
        float sum = 0f;
        for (int i = 0; i < data.Length; i++)
        {
            sum += Mathf.Abs(data[i]);
        }
        return sum / data.Length;
    }

    private void RevealMap(float loudness)
    {
        // Lower the threshold to make the system more sensitive
        float activationThreshold = baselineVolume + sensitivity; 

        if (loudness > activationThreshold) // Check if current loudness exceeds the new sensitive baseline
        {
            // Find all objects on the MapLayer and reveal them
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

            // Start a coroutine to reset the map after a delay of 2 seconds
            StartCoroutine(ResetMapAfterDelay());
        }
    }

    private System.Collections.IEnumerator ResetMapAfterDelay()
    {
        // Wait for the specified visibility duration before resetting the map
        yield return new WaitForSeconds(visibilityDuration);
        ResetMap();
    }

    private void ResetMap()
    {
        // Reset all objects on the MapLayer
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
    }

    private void UpdateLoudnessIndicator(float loudness)
    {
        // Assuming the loudness is normalized between 0 and 1
        if (loudnessSlider != null)
        {
            loudnessSlider.value = loudness; // Set the slider value based on the current loudness
        }
    }

    private System.Collections.IEnumerator CalibrateBaseline()
    {
        float sum = 0f;
        int samples = 0;
        float[] data = new float[256];

        // Record for a certain duration to establish baseline volume
        float calibrationDuration = 0f;

        while (calibrationDuration < baselineCalibrationTime)
        {
            int position = Microphone.GetPosition(Microphone.devices[0]);
            if (position > 0)
            {
                microphoneClip.GetData(data, 0); // Get audio data from the microphone clip
                sum += GetAverageVolume(data);
                samples++;
            }
            calibrationDuration += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Calculate average volume over the calibration period
        baselineVolume = sum / samples;
    }
}
