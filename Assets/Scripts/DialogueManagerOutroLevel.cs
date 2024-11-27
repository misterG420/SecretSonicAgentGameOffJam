using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManagerOutroLevel : MonoBehaviour
{
    [SerializeField] private Text dialogueText; 
    [SerializeField] private Button goodSidebutton1;    
    [SerializeField] private Button darkSidebutton2;    
    [SerializeField] private string[] dialogueLines; 

    private int currentLineIndex = 0;

    private void Start()
    {

        goodSidebutton1.gameObject.SetActive(false);
        darkSidebutton2.gameObject.SetActive(false);

        if (dialogueLines.Length > 0)
        {
            StartCoroutine(ShowDialogue());
        }
    }

    private IEnumerator ShowDialogue()
    {
        while (currentLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
            currentLineIndex++;
            yield return new WaitForSeconds(4f); 
        }

        yield return new WaitForSeconds(4f);
        goodSidebutton1.gameObject.SetActive(true);
        darkSidebutton2.gameObject.SetActive(true);
    }
}
