using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OperatorText : MonoBehaviour
{
    public Text operatorText;
    public Button calibrateButton;
    public Slider loudnessSlider;
    public string[] linesOfText;

    private int currentLine = 0;
    private bool isDisplayingText = false;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        calibrateButton.gameObject.SetActive(false);
        loudnessSlider.gameObject.SetActive(false);
    }

    public void StartTyping()
    {
        if (!isDisplayingText)
        {
            StartCoroutine(TypeText());
        }
    }

    private IEnumerator TypeText()
    {
        isDisplayingText = true;

        foreach (string line in linesOfText)
        {
            operatorText.text = "";
            foreach (char letter in line.ToCharArray())
            {
                operatorText.text += letter;
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(2f);

            currentLine++;
        }

        if (currentLine >= linesOfText.Length)
        {
            calibrateButton.gameObject.SetActive(true);
            loudnessSlider.gameObject.SetActive(true);

            if (player != null)
            {
                player.GetComponent<PlayerTutorialSoundAbilityScript>().enabled = true;
            }
            else
            {
                Debug.LogError("Player not found!");
            }
        }
    }
}
