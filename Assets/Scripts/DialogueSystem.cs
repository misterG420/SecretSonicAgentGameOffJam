using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("Character Sprites")]
    public GameObject playerSprite;
    public GameObject operativeSprite;

    [Header("Text Fields")]
    public Text playerText;
    public Text operativeText;

    private float letterDelay = 0.05f; // Delay between each letter
    private float dialogueDelay = 3f;  // Delay between each dialogue line

    private Coroutine currentCoroutine;

    private void Start()
    {
        StartCoroutine(DialogueSequence());
    }

    private IEnumerator DialogueSequence()
    {
        yield return ShowDialogue("Player", "This is Secret Sonic Agent.");
        yield return new WaitForSeconds(dialogueDelay);

        yield return ShowDialogue("Operative", "What's the sitation Agent?");
        yield return new WaitForSeconds(dialogueDelay);

        yield return ShowDialogue("Player", "Night vision is broken, I can only use Special Sonar to reveal the enemy compound");
        yield return new WaitForSeconds(dialogueDelay);
        
        yield return ShowDialogue("Operative", "Careful, if you are too loud you may attract enemies!");
        yield return new WaitForSeconds(dialogueDelay);

        yield return ShowDialogue("Player", "Yes, but I need to use it to avoid traps and get to the secret documents to retrieve them!");
        yield return new WaitForSeconds(dialogueDelay);

        yield return ShowDialogue("Operative", "Good luck - the Agency will deny any involvement.");
        yield return new WaitForSeconds(dialogueDelay);

    }

    // Function to start showing a dialogue line
    private IEnumerator ShowDialogue(string character, string dialogue)
    {
        // Stop any currently running text display
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // Activate the correct sprite and text field
        if (character == "Player")
        {
            playerSprite.SetActive(true);
            operativeSprite.SetActive(false);
            playerText.gameObject.SetActive(true);
            operativeText.gameObject.SetActive(false);

            currentCoroutine = StartCoroutine(TypeText(playerText, dialogue));
        }
        else if (character == "Operative")
        {
            playerSprite.SetActive(false);
            operativeSprite.SetActive(true);
            playerText.gameObject.SetActive(false);
            operativeText.gameObject.SetActive(true);

            currentCoroutine = StartCoroutine(TypeText(operativeText, dialogue));
        }


        yield return currentCoroutine;
    }


    private IEnumerator TypeText(Text textField, string dialogue)
    {
        textField.text = ""; 
        foreach (char letter in dialogue)
        {
            textField.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }
    }
}
