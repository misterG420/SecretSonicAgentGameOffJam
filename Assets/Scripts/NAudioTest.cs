using System;
using NAudio.CoreAudioApi;
using UnityEngine;

public class NAudioTest : MonoBehaviour
{
    void Start()
    {
        CheckHeadphones();
    }

    private void CheckHeadphones()
    {
        try
        {

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

  
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            bool headphonesPluggedIn = false;


            foreach (var device in devices)
            {
                Debug.Log($"Device found: {device.FriendlyName}");

                if (device.FriendlyName.ToLower().Contains("headphone"))
                {
                    headphonesPluggedIn = true;
                    Debug.Log("Headphones are plugged in.");
                    break;
                }
            }

            if (!headphonesPluggedIn)
            {
                Debug.LogWarning("No headphones detected.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in NAudioTest: {ex.Message}");
        }
    }
}
