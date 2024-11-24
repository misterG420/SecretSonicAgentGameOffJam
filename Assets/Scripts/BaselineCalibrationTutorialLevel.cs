using UnityEngine;
using UnityEngine.UI;

public class BaselineCalibrationTutorialLevel : MonoBehaviour
{
    public Button calibrateButton;
    public float captureTime = 2f;
    private AudioClip microphoneClip;
    private float baselineLoudness = 0f;
    private float captureTimer = 0f;
    private bool isCalibrating = false;

    public OperatorText operatorText; 

    void Start()
    {
        if (calibrateButton != null)
        {
            calibrateButton.onClick.AddListener(StartCalibration);
        }
        else
        {
            Debug.LogError("Calibrate Button not assigned.");
        }

        if (operatorText == null)
        {
            Debug.LogError("OperatorText script reference is missing!");
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
            Microphone.End(null);

            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            int sampleRate = 44100;
            if (maxFreq > 0) sampleRate = Mathf.Clamp(sampleRate, minFreq, maxFreq);

            microphoneClip = Microphone.Start(Microphone.devices[0], true, 1, sampleRate);

            captureTimer = 0f;
            baselineLoudness = 0f;
            isCalibrating = true;

        }
        else
        {
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
            baselineLoudness /= captureTime; 
            PlayerPrefs.SetFloat("BaselineLoudness", baselineLoudness);
            isCalibrating = false;

            Destroy(calibrateButton.gameObject);

            // Notify OperatorText that calibration is complete
            if (operatorText != null)
            {
                operatorText.OnCalibrationComplete();
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
        return (sum / data.Length) * 35f; 
    }
}
