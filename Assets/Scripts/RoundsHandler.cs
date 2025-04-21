using System.Collections;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class RoundsInfo
{
    public int roundNumber;         // For reference in the inspector
    public EnemySpawnInfo[] enemiesToSpawn; // Array of enemy spawn info for this round
    public Vector2 spawnDelayRange; // Min and max delay stored as x (min) and y (max)
    public int timesToSpawn;        // How many times to do the spawning
    public bool batchSpawn;
    [ShowIf("batchSpawn")] public float batchSpawnChance; // Chance for enemies to spawn in a batch each spawn loop
    [ShowIf("batchSpawn")] public Vector2 batchRange;     // Min and max amount of enemies for each batch spawn
}


public class RoundsHandler : MonoBehaviour
{
    public TextMeshProUGUI roundText;
    public GameObject spawner;
    public RoundsInfo[] roundsInfos;  // Array of custom rounds info

    private int round = 0;
    private bool roundStarted = false;

    public void StartRound()
    {
        if (!roundStarted)
        {
            round++;
            roundText.text = "Round " + round;

            // Add the enemies to the spawner dynamically
            AddEnemiesToSpawner();

            // Apply custom settings for this round.
            SetCustomRoundSettings();

            // Start enemy spawn coroutine.
            Spawner _Spawner = spawner.GetComponent<Spawner>();
            _Spawner.StartCoroutine(_Spawner.SpawnEnemies());
            roundStarted = true;
        }
    }

    private void SetCustomRoundSettings()
    {
        Spawner _Spawner = spawner.GetComponent<Spawner>();

        // Check whether we have custom settings defined for this round.
        if (round <= roundsInfos.Length)
        {
            RoundsInfo currentRoundInfo = roundsInfos[round - 1];

            // Randomly calculate spawn delay using the Vector2 range
            _Spawner.spawnDelay = Random.Range(currentRoundInfo.spawnDelayRange.x, currentRoundInfo.spawnDelayRange.y);

            // Apply the custom spawn settings
            _Spawner.enemiesToSpawn = currentRoundInfo.timesToSpawn;
            _Spawner.batchSpawnChance = currentRoundInfo.batchSpawn ? currentRoundInfo.batchSpawnChance : 0f;
            _Spawner.batchRange = currentRoundInfo.batchSpawn ? currentRoundInfo.batchRange : Vector2.zero;
        }
        else
        {
            // Fallback behavior: use last defined round settings
            Debug.LogWarning("Custom settings for round " + round + " not defined. Using last custom settings.");
            RoundsInfo lastCustomSettings = roundsInfos[roundsInfos.Length - 1];
            _Spawner.spawnDelay = Random.Range(lastCustomSettings.spawnDelayRange.x, lastCustomSettings.spawnDelayRange.y);
            _Spawner.enemiesToSpawn = lastCustomSettings.timesToSpawn;
            _Spawner.batchSpawnChance = lastCustomSettings.batchSpawn ? lastCustomSettings.batchSpawnChance : 0f;
            _Spawner.batchRange = lastCustomSettings.batchSpawn ? lastCustomSettings.batchRange : Vector2.zero;
        }
    }

    private void AddEnemiesToSpawner()
    {
        Spawner _Spawner = spawner.GetComponent<Spawner>();

        if (round <= roundsInfos.Length)
        {
            RoundsInfo currentRoundInfo = roundsInfos[round - 1];

            // Clear the current enemy list and populate based on the current round
            _Spawner.ClearEnemies();
            foreach (EnemySpawnInfo enemyInfo in currentRoundInfo.enemiesToSpawn)
            {
                _Spawner.AddEnemy(enemyInfo.enemyPrefab, enemyInfo.spawnChance);
            }
        }
        else
        {
            Debug.LogWarning("No enemies defined for this round.");
        }
    }


    public void HandleRoundState(bool lost)
    {
        if (lost)
        {
            Debug.Log("You lost! Game Over!");
            roundText.text = "Game Over!";
            roundStarted = false;
        }
        else if (round == roundsInfos.Length)
        {
            Debug.Log("All rounds completed! You win!");
            roundText.text = "You Win!";
        }
        else
        {
            roundText.text = "Setup time";
            roundStarted = false;
        }
    }
}
