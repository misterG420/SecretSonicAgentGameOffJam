using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject victoryCanvas;

    public static event Action OnGameOver;
    public static event Action OnVictory;

    private GameObject player;
    private CircleCollider2D playerCollider;

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

    public static void TriggerVictory()
    {
        OnVictory?.Invoke();
    }

    private void ActivateGameOverCanvas()
    {
        if (player != null)
        {
            Destroy(player);
        }

        gameOverCanvas.SetActive(true);
    }

    private void ActivateVictoryCanvas()
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
}
