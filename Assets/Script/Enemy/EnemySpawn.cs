using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab; 
    public float spawnInterval = 2f; 
    public int maxEnemyCount = 5; 

    private float timer; 

    void Start()
    {
        timer = spawnInterval; 
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (GetCurrentEnemyCount() < maxEnemyCount)
            {
                SpawnEnemy(); 
            }
            timer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }

    int GetCurrentEnemyCount()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}