using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float lastLoudness;
    private float silenceDuration = 0f;
    public float resetTime = 1f; // Time to wait before resetting alpha (in seconds)
    private const string mapLayerName = "MapLayer"; // The name of your map layer
    private AudioClip microphoneClip; // Store the microphone audio clip

    void Start()
    {
        StartMicrophone();
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
            RevealMap(loudness);
            lastLoudness = loudness;

            if (loudness < 0.01f) // Define a threshold for silence
            {
                silenceDuration += Time.deltaTime;
                if (silenceDuration >= resetTime)
                {
                    ResetMap();
                    silenceDuration = 0f; // Reset the duration after resetting the map
                }
            }
            else
            {
                silenceDuration = 0f; // Reset silence timer if sound is detected
            }
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
        // Lower the threshold for loudness to reveal the map
        float revealThreshold = 0.005f; // Adjust this value to change sensitivity

        if (loudness > revealThreshold) // Change this threshold if needed
        {
            // Find all objects on the MapLayer and reveal them
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MapObject")) // Ensure your map objects are tagged appropriately
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    color.a = 1; // Set alpha to 1 to make the object visible
                    renderer.color = color;
                }
            }
        }
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
}


