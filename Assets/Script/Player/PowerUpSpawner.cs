using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs; 
    public float respawnDelay = 5f; 

    private GameObject currentSpawnedPowerUp; 
    private float timer;
    private bool isWaitingToRespawn = false;

    void Start()
    {
        SpawnRandomPowerUp();
    }

    void Update()
    {
        if (currentSpawnedPowerUp != null) return;

        if (!isWaitingToRespawn)
        {
            isWaitingToRespawn = true;
            timer = respawnDelay; 
        }

        if (isWaitingToRespawn)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SpawnRandomPowerUp();
                isWaitingToRespawn = false;
            }
        }
    }

    void SpawnRandomPowerUp()
    {
        if (powerUpPrefabs != null && powerUpPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            currentSpawnedPowerUp = Instantiate(powerUpPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }
}