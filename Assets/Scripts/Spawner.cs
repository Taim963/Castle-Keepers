using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    [Tooltip("Relative spawn chance (e.g., 1 = common; 0.2 = rare)")]
    public float spawnChance = 1f;
}


public class Spawner : MonoBehaviour
{
    public Transform spawnAreaCenter;  // Center of the rectangle spawn area
    public float halfWidth = 20f;        // Half-width of the rectangle
    public float halfHeight = 10f;       // Half-height of the rectangle
    public EnemySpawnInfo[] enemySpawnInfos;
    public float spawnDelay = 2f;        // Time delay between spawns
    public int enemiesToSpawn = 10;

    private int spawnCount = 0;
    private bool roundStarted = false;

    public void StartSpawning()
    {
        if (!roundStarted)
        { 
            StartCoroutine(SpawnEnemies());
            roundStarted = true;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            // Choose a random edge of the rectangle (0 = top, 1 = bottom, 2 = left, 3 = right)
            int side = Random.Range(0, 4);
            float spawnX = 0f;
            float spawnY = 0f;

            switch (side)
            {
                case 0: // Top
                    spawnX = Random.Range(-halfWidth, halfWidth);
                    spawnY = halfHeight;
                    break;
                case 1: // Bottom
                    spawnX = Random.Range(-halfWidth, halfWidth);
                    spawnY = -halfHeight;
                    break;
                case 2: // Left
                    spawnX = -halfWidth;
                    spawnY = Random.Range(-halfHeight, halfHeight);
                    break;
                case 3: // Right
                    spawnX = halfWidth;
                    spawnY = Random.Range(-halfHeight, halfHeight);
                    break;
            }

            Vector3 spawnPosition = spawnAreaCenter.position + new Vector3(spawnX, spawnY, 0);

            // Pick enemy prefab based on weighted spawn chance.
            GameObject enemyPrefab = GetRandomEnemyPrefab();
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            ++spawnCount;

            if (spawnCount >= enemiesToSpawn)
            {
                Debug.Log("All enemies spawned.");
                roundStarted = false;
                spawnCount = 0; // Reset spawn count for the next round
                yield break;
            }
        }
    }

    // This method uses weighted random selection to return an enemy prefab.
    private GameObject GetRandomEnemyPrefab()
    {
        // First, compute the total weight from all spawn chances.
        float totalChance = 0f;
        foreach (EnemySpawnInfo info in enemySpawnInfos)
        {
            totalChance += info.spawnChance;
        }

        // Get a random value between 0 and total weight.
        float randomValue = Random.Range(0f, totalChance);

        // Iterate through the enemy list, subtracting chance until threshold is reached.
        foreach (EnemySpawnInfo info in enemySpawnInfos)
        {
            if (randomValue < info.spawnChance)
            {
                return info.enemyPrefab;
            }
            randomValue -= info.spawnChance;
        }

        // Fallback - in case something goes wrong, return the first enemy.
        return enemySpawnInfos[0].enemyPrefab;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnAreaCenter != null)
        {
            Gizmos.color = Color.green;

            Vector3 topLeft = spawnAreaCenter.position + new Vector3(-halfWidth, halfHeight, 0);
            Vector3 topRight = spawnAreaCenter.position + new Vector3(halfWidth, halfHeight, 0);
            Vector3 bottomRight = spawnAreaCenter.position + new Vector3(halfWidth, -halfHeight, 0);
            Vector3 bottomLeft = spawnAreaCenter.position + new Vector3(-halfWidth, -halfHeight, 0);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}
