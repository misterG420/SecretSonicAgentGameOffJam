using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject victoryCanvas;

    public static event Action OnGameOver;
    public static event Action OnVictory;

    private GameObject player;
    private CircleCollider2D playerCollider;

    private AchievementManager achievementManager;
    private bool levelInProgress = false;

    private void OnEnable()
    {
        OnGameOver += HandleGameOver;
        OnVictory += HandleVictoryCondition;
        Debug.Log("Subscribed HandleVictoryCondition to OnVictory");
    }

    private void OnDisable()
    {
        OnGameOver -= HandleGameOver;
        OnVictory -= HandleVictoryCondition;
        Debug.Log("Unsubscribed HandleVictoryCondition from OnVictory");
    }

    private void Start()
    {
        gameOverCanvas.SetActive(false);
        victoryCanvas.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerCollider = player.GetComponent<CircleCollider2D>() ?? player.AddComponent<CircleCollider2D>();
            playerCollider.radius = 0.22f;
        }
        achievementManager = FindObjectOfType<AchievementManager>();

        Debug.Log($"GameManager initialized. OnVictory subscribers: {OnVictory?.GetInvocationList().Length ?? 0}");
    }

    public void StartLevel(string levelName)
    {
        if (achievementManager != null)
        {
            achievementManager.StartLevel(levelName);
        }
        levelInProgress = true;
        Debug.Log("Level started, levelInProgress set to true");
    }

    public static void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        Debug.Log("TriggerGameOver() called");
    }

    public static void TriggerVictory()
    {
        OnVictory?.Invoke();
        Debug.Log($"TriggerVictory() called. OnVictory subscribers: {OnVictory?.GetInvocationList().Length ?? 0}");
    }

    private void HandleGameOver()
    {
        if (player != null)
        {
            Destroy(player);
        }

        gameOverCanvas.SetActive(true);
        levelInProgress = false;
        Debug.Log("Game Over handled");
    }

    private void HandleVictoryCondition()
    {
        Debug.Log($"HandleVictoryCondition called. Level in progress: {levelInProgress}");
        if (!levelInProgress) return;

        if (player != null && playerCollider != null)
        {
            Destroy(playerCollider);
        }

        if (achievementManager != null)
        {
            achievementManager.EndLevel();
        }

        victoryCanvas.SetActive(true);
        levelInProgress = false;
        Debug.Log("Victory condition handled");
    }
}
