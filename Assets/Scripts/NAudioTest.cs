using UnityEngine;
using NAudio.CoreAudioApi; 

public class NAudioTest : MonoBehaviour
{
    void Start()
    {
        var deviceEnumerator = new MMDeviceEnumerator();
        Debug.Log($"Devices found: {deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).Count}");
    }
}
