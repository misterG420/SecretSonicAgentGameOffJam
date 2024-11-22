using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OperatorText : MonoBehaviour
{
    public Text displayText; 
    public GameObject endButton; 
    public float letterDelay = 0.02f; 
    [TextArea(3, 10)]
    public string textToDisplay; 

    private void Start()
    {
        if (endButton != null)
        {
            endButton.SetActive(false); 
        }

        if (displayText != null)
        {
            displayText.text = ""; 
            StartCoroutine(TypeText());
        }
        else
        {
            Debug.LogError("No displayText assigned to OperatorText script!");
        }
    }

    private IEnumerator TypeText()
    {
        foreach (char letter in textToDisplay)
        {
            displayText.text += letter; 
            yield return new WaitForSeconds(letterDelay);
        }

        if (endButton != null)
        {
            endButton.SetActive(true); 
        }
    }
}
