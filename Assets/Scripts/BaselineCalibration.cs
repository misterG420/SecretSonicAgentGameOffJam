using UnityEngine;
using UnityEngine.UI;

public class BaselineCalibration : MonoBehaviour
{
    public Button calibrateButton;
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
            microphoneClip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
            captureTimer = 0f;
            baselineLoudness = 0f;
            isCalibrating = true;
            statusText.text = "Calibrating baseline... Please stay silent.";
        }
        else
        {
            statusText.text = "No microphone found!";
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
