using UnityEngine;

public class MaintainAspectRatio : MonoBehaviour
{

    public static MaintainAspectRatio Instance;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject); 
            return;
        }


        Instance = this;

        DontDestroyOnLoad(gameObject);

        Screen.orientation = ScreenOrientation.Portrait;

        //  resolution for Windows builds
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
