using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnCheatsButtonPressed()
    {
       int hasAccess = PlayerPrefs.GetInt("hasAccessToLevelSelector", 0); // Default to 0 if not found


        if (hasAccess == 1)
        {
            // Player has access, load Sceneselectro cheat screen
            SceneManager.LoadScene("CheatCodeLevelSelector"); 
        }
        else
        {
            SceneManager.LoadScene("CheatCodeUnlockLevel"); 
        }
    }
}
