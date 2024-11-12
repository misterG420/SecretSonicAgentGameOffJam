using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject victoryCanvas;

    public static event Action OnGameOver;
    public static event Action OnVictory;

    private GameObject player; 

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
        if (player != null)
        {
            Destroy(player);
        }

        victoryCanvas.SetActive(true);
    }
}
