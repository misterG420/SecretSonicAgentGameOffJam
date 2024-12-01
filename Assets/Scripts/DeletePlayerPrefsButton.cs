using UnityEngine;
using UnityEngine.UI;

public class DeletePlayerPrefsButton : MonoBehaviour
{
    public Button deleteButton;

    void Start()
    {
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(DeletePlayerPrefs);
        }
    }

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs have been deleted.");
    }
}
