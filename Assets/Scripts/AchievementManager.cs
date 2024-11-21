using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [Header("UI References")]
    public Image achievementImage;
    public Text levelTimeText;
    public Text bestTimeText;

    [Header("Achievement Settings")]
    public float achievementBenchmarkTime = 60f; 
    public Sprite achievementSprite; 

    private float bestTime;

    void OnEnable()
    {

        GameManager.OnVictory += OnLevelComplete;
    }

    void OnDisable()
    {

        GameManager.OnVictory -= OnLevelComplete;
    }

    void Start()
    {

        bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue); // Default to max if no time is saved
    }

    public void OnLevelComplete(float timeTaken)
    {
        // Update the level time UI
        levelTimeText.text = $"Time: {timeTaken:F2} seconds";
        bestTimeText.text = $"Best Time: {bestTime:F2} seconds";

        // Check if the time taken is less than the best time and update
        if (timeTaken < bestTime)
        {
            bestTime = timeTaken;
            PlayerPrefs.SetFloat("BestTime", bestTime); // Save the new best time
        }

        // Check if the time taken is below the achievement benchmark
        if (timeTaken <= achievementBenchmarkTime)
        {
            // Unlock the achievement and display the achievement sprite
            achievementImage.sprite = achievementSprite;
            achievementImage.enabled = true;
        }
        else
        {
            achievementImage.enabled = false; // Hide achievement icon if no achievement
        }
    }
}
