using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(WaitForGameOver(0.5f));
        }
    }


    private IEnumerator WaitForGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameOver();
    }

    private void GameOver()
    {
        GameManager.TriggerGameOver();
        Debug.Log("Game Over() called");
    }
}
