using UnityEngine;
using UnityEngine.UI;

public class SoundPatternDetector : MonoBehaviour
{
    public Image[] loudnessSquares;  // The squares to represent target loudness levels
    public float[] predefinedPattern;  // Predefined target loudness levels for each square (0-1 range, or any scale)
    private int currentStep = 0;  // The index of the current square to be activated

    private AudioClip microphoneClip;  // Microphone clip to capture the sound
    private int sampleRate = 44100;  // Sample rate for the microphone input

    private void Start()
    {
        // Initialize microphone input and start capturing sound
        int minFreq, maxFreq;
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
        microphoneClip = Microphone.Start(null, true, 1, maxFreq);  // Start recording with the max frequency of the microphone device
        while (Microphone.GetPosition(null) <= 0) { }  // Wait until the microphone has started capturing sound
    }

    private void Update()
    {
        // Only process if there are squares to check (i.e., currentStep < loudnessSquares.Length)
        if (currentStep < loudnessSquares.Length)
        {
            // Get the current square and its target loudness
            Image currentSquare = loudnessSquares[currentStep];
            float targetLoudness = predefinedPattern[currentStep];

            // Get microphone data and calculate the loudness (average of samples)
            float loudness = GetMicrophoneLoudness();

            // Check if the current loudness matches the target loudness
            if (loudness < targetLoudness)
            {
                currentSquare.color = Color.green;  // Input loudness is lower than target (green)
            }
            else if (loudness > targetLoudness)
            {
                currentSquare.color = Color.red;  // Input loudness is higher than target (red)
            }
            else
            {
                currentSquare.color = Color.white;  // Exact match (white)

                // If the loudness matches the target, move to the next square
                currentSquare.gameObject.SetActive(false);  // Hide the square when it's matched
                currentStep++;  // Move to the next square
            }
        }
    }

    // Function to get the loudness of the microphone input
    private float GetMicrophoneLoudness()
    {
        float[] samples = new float[256];  // Array to hold the samples from the microphone
        AudioListener.GetOutputData(samples, 0);  // Get the audio data from the microphone

        // Calculate the average loudness (sum of absolute values of samples)
        float sum = 0f;
        foreach (float sample in samples)
        {
            sum += Mathf.Abs(sample);  // Sum up absolute values (to avoid negative values)
        }
        return sum / samples.Length;  // Return the average loudness
    }
}
