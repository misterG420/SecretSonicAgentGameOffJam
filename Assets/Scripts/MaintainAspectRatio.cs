using UnityEngine;

public class MaintainAsectRatio : MonoBehaviour
{
    private void Awake()
    {
        // Lock  to Portrait
        Screen.orientation = ScreenOrientation.Portrait;

        //Enforce resolution for Windows builds
#if UNITY_STANDALONE
        SetResolutionForWindows();
#endif
    }

    private void SetResolutionForWindows()
    {
        // Set a fixed portrait resolution for standalone builds
        int portraitWidth = 1080;
        int portraitHeight = 2408;
        Screen.SetResolution(portraitWidth, portraitHeight, false);
    }
}
