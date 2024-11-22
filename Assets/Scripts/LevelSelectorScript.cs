using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorScript : MonoBehaviour
{


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadFirstMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTutorialLevel()
    {
        SceneManager.LoadScene("TutorialLevel");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("IntroMenu");
    }

    public void LoadPreLevelInstructions()
    {
        SceneManager.LoadScene("PreLevelInstructions");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }


    public void LoadCheatScreen()
    {
        SceneManager.LoadScene("CheatCodeUnlockLevel");
    }


    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }


    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene("Level4");
    }

    public void LoadLevelSelector()
    {
        SceneManager.LoadScene("CheatCodeLevelSelector");
    }


    public void QuitApplication()
    {
        Application.Quit();
    }
}
