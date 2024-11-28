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

    public void LoadLevel5()
    {
        SceneManager.LoadScene("Level5");
    }

    public void LoadLevel6()
    {
        SceneManager.LoadScene("Level6");
    }

    public void LoadStoryOutroLevel()
    {
        SceneManager.LoadScene("StoryOutroLevel");
    }

    
    public void LoadLevelSelector()
    {
        SceneManager.LoadScene("CheatCodeLevelSelector");
    }

    public void LoadStoryOutroLevelGoodSide()
    {
        SceneManager.LoadScene("StoryOutroLevelGoodSide");
    }

    public void LoadStoryOutroLevelBadSide()
    {
        SceneManager.LoadScene("StoryOutroLevelDarkSide");
    }


    public void QuitApplication()
    {
        Application.Quit();
    }
}
