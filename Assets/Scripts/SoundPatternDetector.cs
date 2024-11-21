using UnityEngine;
using UnityEngine.UI;

public class SoundPatternDetector : MonoBehaviour
{
    public Image[] loudnessSquares;  // The squares to represent target loudness levels
    public float[] predefinedPattern;  // Predefined target loudness levels for each square (0-1 range, or any scale)
    public Text feedbackText;  
    public Slider loudnessSlider;

    public Button cheatButton;

    private int currentStep = 0;  // The index of the current square to be activated
    private AudioClip microphoneClip;  // Microphone clip to capture the sound
    private int sampleRate = 44100;  // Sample rate for the microphone input
    public float amplificationFactor = 35f;  // Amplification factor to make the loudness more sensitive
    public float tolerance = 0.05f;  // The tolerance range (positive or negative) around the target loudness

    private float baselineLoudness = 0f;  // Baseline loudness to determine the threshold for sound detection
    private bool isCalibrating = false;  // Flag to indicate if we are in the calibration phase
    private float calibrationTime = 2f;  // Time to capture baseline loudness
    private float captureTimer = 0f;  // Timer to track calibration time
    private float lastLoudness = 0f;  // Last loudness value for smoothing
    private float smoothingFactor = 0.1f;  // Smoothing factor for microphone input (higher values = more smoothing)

    private float thresholdLoudness = 0.05f;  // The threshold above baseline for valid loudness detection
    private float sustainedTime = 0f;  // Time that loudness stays above the threshold to confirm detection

    void Start()
    {
        StartCalibration();

        if (cheatButton != null)
        {
            cheatButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Cheat Button not assigned.");
        }
    }

    void Update()
    {
        // If we're calibrating, keep capturing the baseline loudness
        if (isCalibrating)
        {
            CaptureBaseline();
        }
        else
        {
            // Process the loudness detection if calibration is done
            DetectLoudness();
        }
    }

    private void StartCalibration()
    {
        if (Microphone.devices.Length > 0)
        {
            Microphone.End(null); // Stop any existing microphone instance
            microphoneClip = Microphone.Start(Microphone.devices[0], true, 1, sampleRate);
            captureTimer = 0f;
            baselineLoudness = 0f;
            isCalibrating = true;  // Start calibration process
            feedbackText.text = "Calibrating baseline... Please stay silent.";
        }
        else
        {
            feedbackText.text = "No microphone found!";
        }
    }

    private void CaptureBaseline()
    {
        float[] data = new float[256];
        microphoneClip.GetData(data, 0);
        float loudness = GetNormalizedLoudness(data);

        captureTimer += Time.deltaTime;
        baselineLoudness += loudness;

        if (captureTimer >= calibrationTime)
        {
            baselineLoudness /= calibrationTime;  // Calculate average baseline
            PlayerPrefs.SetFloat("BaselineLoudness", baselineLoudness);
            isCalibrating = false;
            feedbackText.text = "Baseline calibration complete. Ready to start detecting loudness.";

        }
    }

    private void DetectLoudness()
    {
        // Only process if there are squares to check (i.e., currentStep < loudnessSquares.Length)
        if (currentStep < loudnessSquares.Length)
        {
            Image currentSquare = loudnessSquares[currentStep];
            float targetLoudness = predefinedPattern[currentStep];

            float loudness = GetMicrophoneLoudness();

            // Apply smoothing to the loudness value
            loudness = Mathf.Lerp(lastLoudness, loudness, smoothingFactor);
            lastLoudness = loudness;

            loudness *= amplificationFactor;  

            // Check if loudness is above baseline and threshold
            if (loudness > baselineLoudness + thresholdLoudness)
            {
                loudnessSlider.value = Mathf.Clamp01(loudness);

                sustainedTime += Time.deltaTime;

                // If the loudness has been above the threshold for long enough, proceed
                if (sustainedTime > 0.5f)  // 0.5 seconds to confirm sustained loudness
                {
                    if (loudness < targetLoudness - tolerance)
                    {
                        currentSquare.color = Color.green;  // Too quiet
                        feedbackText.text = "Too quiet!";
                    }
                    else if (loudness > targetLoudness + tolerance)
                    {
                        currentSquare.color = Color.red;  // Too loud
                        feedbackText.text = "Too loud!";
                    }
                    else
                    {
                        currentSquare.color = Color.white;  // Perfect loudness
                        feedbackText.text = "Perfect level - 1 unlocked!";
                        currentSquare.gameObject.SetActive(false);  // Hide the square once matched
                        currentStep++;  // Move to the next target square
                    }
                    if (currentStep >= loudnessSquares.Length)
                    {
                        UnlockCheat();
                    }
                    sustainedTime = 0f;  // Reset sustained time
                }
            }
            else
            {
                sustainedTime = 0f;
                loudnessSlider.value = Mathf.Lerp(loudnessSlider.value, 0f, 0.1f);  // Slowly decrease slider
            }
        }
    }

    private void UnlockCheat()
    {
        // Enable the cheat button after the pattern is completed
        if (cheatButton != null)
        {
            cheatButton.gameObject.SetActive(true);
            feedbackText.text = "Cheat unlocked! Press the button to activate cheat.";
            
            PlayerPrefs.SetInt("hasAccessToLevelSelector", 1); // 1 for unlocked, 0 for locked
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("Cheat Button not assigned.");
        }
    }

    private float GetMicrophoneLoudness()
    {
        float[] data = new float[256];
        microphoneClip.GetData(data, 0);
        return GetNormalizedLoudness(data);
    }

    private float GetNormalizedLoudness(float[] data)
    {
        float sum = 0f;
        foreach (float sample in data)
        {
            sum += Mathf.Abs(sample);
        }
        return (sum / data.Length) * 35f;  // Amplify the sensitivity
    }
}
