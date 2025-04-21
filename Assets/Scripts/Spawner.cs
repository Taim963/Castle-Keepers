using System;
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
    public float batchSpawnChance = 0f; // Chance for batch spawning
    public Vector2 batchRange = Vector2.zero;

    [HideInInspector] public int spawnCount = 0;
    [HideInInspector] public bool allEnemiesSpawned = false;

    public void ClearEnemies()
    {
        enemySpawnInfos = new EnemySpawnInfo[0];
    }

    public void AddEnemy(GameObject enemyPrefab, float spawnChance)
    {
        // Add new enemy with its spawn chance
        EnemySpawnInfo newEnemyInfo = new EnemySpawnInfo
        {
            enemyPrefab = enemyPrefab,
            spawnChance = spawnChance
        };

        Array.Resize(ref enemySpawnInfos, enemySpawnInfos.Length + 1);
        enemySpawnInfos[enemySpawnInfos.Length - 1] = newEnemyInfo;
    }


    public IEnumerator SpawnEnemies()
    {
        while (true)
        {
            allEnemiesSpawned = false;
            yield return new WaitForSeconds(spawnDelay);

            // Choose a random edge of the rectangle (0 = top, 1 = bottom, 2 = left, 3 = right)
            int side = UnityEngine.Random.Range(0, 4);
            float spawnX = 0f;
            float spawnY = 0f;

            switch (side)
            {
                case 0: // Top
                    spawnX = UnityEngine.Random.Range(-halfWidth, halfWidth);
                    spawnY = halfHeight;
                    break;
                case 1: // Bottom
                    spawnX = UnityEngine.Random.Range(-halfWidth, halfWidth);
                    spawnY = -halfHeight;
                    break;
                case 2: // Left
                    spawnX = -halfWidth;
                    spawnY = UnityEngine.Random.Range(-halfHeight, halfHeight);
                    break;
                case 3: // Right
                    spawnX = halfWidth;
                    spawnY = UnityEngine.Random.Range(-halfHeight, halfHeight);
                    break;
            }

            Vector3 spawnPosition = spawnAreaCenter.position + new Vector3(spawnX, spawnY, 0);

            // Handle batch spawning
            if (batchSpawnChance > 0f && UnityEngine.Random.value <= batchSpawnChance)
            {
                int batchCount = UnityEngine.Random.Range((int)batchRange.x, (int)batchRange.y + 1);
                for (int i = 0; i < batchCount; i++)
                {
                    SpawnEnemy(spawnPosition);
                }
            }
            else
            {
                // Spawn a single enemy
                SpawnEnemy(spawnPosition);
            }

            spawnCount++;

            if (spawnCount >= enemiesToSpawn)
            {
                Debug.Log("All enemies spawned.");
                spawnCount = 0; // Reset spawn count for the next round
                allEnemiesSpawned = true;
                yield break;
            }
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        if (enemySpawnInfos.Length > 0)
        {
            GameObject enemyPrefab = GetRandomEnemyPrefab();
            Instantiate(enemyPrefab, position, Quaternion.identity);
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        float totalChance = 0f;
        foreach (EnemySpawnInfo info in enemySpawnInfos)
        {
            totalChance += info.spawnChance;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalChance);

        foreach (EnemySpawnInfo info in enemySpawnInfos)
        {
            if (randomValue < info.spawnChance)
            {
                return info.enemyPrefab;
            }
            randomValue -= info.spawnChance;
        }

        return enemySpawnInfos[0].enemyPrefab; // Fallback
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
