using UnityEngine;
using UnityEngine.UI;

public class BaselineCalibration : MonoBehaviour
{
    public Button calibrateButton;
    public GameObject startGameButton; // The button to activate after calibration
    public float captureTime = 2f; // Time in seconds to capture baseline loudness
    private AudioClip microphoneClip;
    private float baselineLoudness = 0f;
    private float captureTimer = 0f;
    private bool isCalibrating = false;
    public Text statusText;

    void Start()
    {
        // Ensure the button is connected and set up its listener
        if (calibrateButton != null)
        {
            calibrateButton.onClick.AddListener(StartCalibration);
        }
        else
        {
            Debug.LogError("Calibrate Button not assigned.");
        }

        // Ensure startGameButton is initially inactive
        if (startGameButton != null)
        {
            startGameButton.SetActive(false);
        }
        else
        {
            Debug.LogError("Start Game Button not assigned.");
        }
    }

    void Update()
    {
        if (isCalibrating)
        {
            CaptureBaseline();
        }
    }

    private void StartCalibration()
    {
        if (Microphone.devices.Length > 0)
        {
            Microphone.End(null); // Stop any existing microphone instance

            // Determine microphone capabilities
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            // Use 44100 as default, but clamp within microphone-supported range
            int sampleRate = 44100;
            if (maxFreq > 0) sampleRate = Mathf.Clamp(sampleRate, minFreq, maxFreq);

            Debug.Log($"Using sample rate: {sampleRate} (Min: {minFreq}, Max: {maxFreq}) on platform: {Application.platform}");

            microphoneClip = Microphone.Start(Microphone.devices[0], true, 1, sampleRate);

            captureTimer = 0f;
            baselineLoudness = 0f;
            isCalibrating = true;
            statusText.text = "Calibrating baseline... Please stay silent.";
        }
        else
        {
            statusText.text = "No microphone found! Unable to play";
            Debug.LogError("No microphone devices detected.");
        }
    }

    private void CaptureBaseline()
    {
        float[] data = new float[256];
        microphoneClip.GetData(data, 0);
        float loudness = GetNormalizedLoudness(data);

        captureTimer += Time.deltaTime;
        baselineLoudness += loudness;

        if (captureTimer >= captureTime)
        {
            baselineLoudness /= captureTime; // Calculate average baseline
            PlayerPrefs.SetFloat("BaselineLoudness", baselineLoudness);
            isCalibrating = false;
            statusText.text = "Baseline calibration complete. Ready to start the game.";

            // Activate the start game button once calibration is complete
            if (startGameButton != null)
            {
                startGameButton.SetActive(true);
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
        return (sum / data.Length) * 35f; // Amplification factor for sensitivity
    }
}
