using System;
using NAudio.CoreAudioApi;
using UnityEngine;

public class NAudioTest : MonoBehaviour
{
    void Start()
    {
        try
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            if (devices.Count > 0)
            {
                Debug.Log("Audio devices detected:");
                foreach (var device in devices)
                {
                    Debug.Log(device.FriendlyName);
                }
            }
            else
            {
                Debug.LogWarning("No active audio devices found.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize audio device enumerator: {ex.Message}");
        }
    }
}
