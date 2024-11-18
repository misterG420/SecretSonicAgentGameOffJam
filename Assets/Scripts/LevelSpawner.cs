using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab; 
    public Transform spawnLocation; 

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnLocation != null)
        {
            Instantiate(enemyPrefab, spawnLocation.position, spawnLocation.rotation);
        }
        else
        {
            Debug.LogWarning("LevelSpawner: Missing enemyPrefab or spawnLocation.");
        }
    }
}
