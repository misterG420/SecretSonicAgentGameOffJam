using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorScript : MonoBehaviour
{


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("IntroMenu");
    }

  
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }


    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }


    public void QuitApplication()
    {
        Application.Quit();
    }
}
