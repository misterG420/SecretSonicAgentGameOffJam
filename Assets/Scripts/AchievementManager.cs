using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchievementManager : MonoBehaviour
{
    [Header("UI References")]
    public Image achievementImage;
    public Text levelTimeText;
    public Text bestTimeText;
    public Text benchmarkTimeText;
    public Text specialAchievementText;

    [Header("Achievement Settings")]
    public float achievementBenchmarkTime = 45;
    public Sprite achievementSprite;

    private float bestTime;
    private string currentSceneName;

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

        currentSceneName = SceneManager.GetActiveScene().name;

        // Load the best time for the current level using the scene name
        bestTime = PlayerPrefs.GetFloat($"BestTime_{currentSceneName}", float.MaxValue);
        bestTime = Mathf.Round(bestTime * 100f) / 100f;
        benchmarkTimeText.text = $"Benchmark: {achievementBenchmarkTime:F2} seconds";
    }

    public void OnLevelComplete(float timeTaken)
    {
        timeTaken = Mathf.Round(timeTaken * 100f) / 100f;
        levelTimeText.text = $"Time: {timeTaken:F2} seconds";
        bestTimeText.text = $"Best Time: {bestTime:F2} seconds";

        if (timeTaken < bestTime)
        {
            bestTime = timeTaken;
            PlayerPrefs.SetFloat($"BestTime_{currentSceneName}", bestTime);
        }

        if (timeTaken <= achievementBenchmarkTime)
        {
            achievementImage.sprite = achievementSprite;
            achievementImage.enabled = true;
            specialAchievementText.enabled = true;
        }
        else
        {
            achievementImage.enabled = false;
            specialAchievementText.enabled = false;
        }
    }
}
