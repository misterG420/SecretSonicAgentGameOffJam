using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject victoryCanvas;

    public static event Action OnGameOver;
    public static event Action<float> OnVictory; // Changed to pass time for victory

    private GameObject player;
    private CircleCollider2D playerCollider;
    private float startTime;

    private void OnEnable()
    {
        OnGameOver += ActivateGameOverCanvas;
        OnVictory += ActivateVictoryCanvas;
    }

    private void OnDisable()
    {
        OnGameOver -= ActivateGameOverCanvas;
        OnVictory -= ActivateVictoryCanvas;
    }

    private void Start()
    {
        gameOverCanvas.SetActive(false);
        victoryCanvas.SetActive(false);

        StartLevel();

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerCollider = player.GetComponent<CircleCollider2D>();

            if (playerCollider == null)
            {
                playerCollider = player.AddComponent<CircleCollider2D>();
            }
            playerCollider.radius = 0.22f;
        }
    }

    public static void TriggerGameOver()
    {
        OnGameOver?.Invoke();
    }

    public static void TriggerVictory(float timeTaken)
    {
        OnVictory?.Invoke(timeTaken); // Pass the time taken to the event subscribers
    }

    private void ActivateGameOverCanvas()
    {
        if (player != null)
        {
            Destroy(player);
        }

        gameOverCanvas.SetActive(true);
    }

    private void ActivateVictoryCanvas(float timeTaken)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        if (player != null && playerCollider != null)
        {
            Destroy(playerCollider);
        }

        victoryCanvas.SetActive(true);


    }

    public void CompleteLevel()
    {
        float timeTaken = Time.time - startTime;
        TriggerVictory(timeTaken); 
    }


    public void StartLevel()
    {
        startTime = Time.time;
    }
}
