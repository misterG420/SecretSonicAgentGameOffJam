using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string levelName; 
        public float[] achievementTimes; 
        public Sprite[] achievementSprites; 
    }

    public List<LevelData> levels;
    public GameObject victoryScreen;
    public Image[] achievementIcons; 
    public Text levelTimeText; 
    public Text bestTimeText;

    private Dictionary<string, float> bestTimes = new Dictionary<string, float>();
    private string currentLevel;
    private float startTime;

    void Start()
    {

        foreach (var level in levels)
        {
            if (PlayerPrefs.HasKey(level.levelName))
            {
                bestTimes[level.levelName] = PlayerPrefs.GetFloat(level.levelName);
            }
            else
            {
                bestTimes[level.levelName] = float.MaxValue; // Default to max time
            }
        }
    }

    public void StartLevel(string levelName)
    {
        currentLevel = levelName;
        startTime = Time.time; // rreset
    }

    public void EndLevel()
    {
        if (string.IsNullOrEmpty(currentLevel)) return;

        float timeTaken = Time.time - startTime;

        // ppdate  best time  current t is faster
        if (timeTaken < bestTimes[currentLevel])
        {
            bestTimes[currentLevel] = timeTaken;
            PlayerPrefs.SetFloat(currentLevel, timeTaken); 
        }

        ShowVictoryScreen(currentLevel, timeTaken);
        currentLevel = null; // Reset current level
        //Question: Will the time sdtop if not victory screen but game over screen?
    }

    private void ShowVictoryScreen(string levelName, float timeTaken)
    {
        victoryScreen.SetActive(true);

        levelTimeText.text = $"Time: {timeTaken:F2} seconds";
        bestTimeText.text = $"Best Time: {bestTimes[levelName]:F2} seconds";

        // ffind the level data for the current level
        LevelData levelData = levels.Find(level => level.levelName == levelName);

        // check achievemeents
        for (int i = 0; i < achievementIcons.Length; i++)
        {
            if (i < levelData.achievementTimes.Length && timeTaken <= levelData.achievementTimes[i])
            {
                achievementIcons[i].sprite = levelData.achievementSprites[i];
                achievementIcons[i].enabled = true;
            }
            else
            {
                achievementIcons[i].enabled = false;
            }
        }
    }

    public void ResetBestTimes()
    {
        foreach (var level in levels)
        {
            PlayerPrefs.DeleteKey(level.levelName);
            bestTimes[level.levelName] = float.MaxValue;
        }
    }
}
