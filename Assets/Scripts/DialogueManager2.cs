using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager2 : MonoBehaviour
{
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button menuButton;
    [SerializeField] private string[] dialogueLines;

    [SerializeField] private GameObject awardImage;

    [SerializeField] private float typingSpeed = 0.05f;
    private int currentLineIndex = 0;

    private void Start()
    {
        menuButton.gameObject.SetActive(false);
        awardImage.gameObject.SetActive(false);


        if (dialogueLines.Length > 0)
        {
            StartCoroutine(ShowDialogue());
        }
    }

    private IEnumerator ShowDialogue()
    {
        while (currentLineIndex < dialogueLines.Length)
        {
            yield return StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
            currentLineIndex++;
            yield return new WaitForSeconds(4f);
        }

        yield return new WaitForSeconds(4f);
        menuButton.gameObject.SetActive(true);
        awardImage.gameObject.SetActive(true);
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
