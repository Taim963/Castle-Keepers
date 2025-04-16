using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class RoundsInfo
{
    public int roundNumber;         // For reference in the inspector
    public float spawnDelay;        // Custom delay for this round
    public int enemiesToSpawn;      // Custom enemy count for this round
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
            _Spawner.spawnDelay = currentRoundInfo.spawnDelay;
            _Spawner.enemiesToSpawn = currentRoundInfo.enemiesToSpawn;
        }
        else
        {
            // Fallback behavior: if no custom settings, could either repeat the last defined round or implement a scaling logic.
            Debug.LogWarning("Custom settings for round " + round + " not defined. Using last custom settings.");
            RoundsInfo lastCustomSettings = roundsInfos[roundsInfos.Length - 1];
            _Spawner.spawnDelay = lastCustomSettings.spawnDelay;
            _Spawner.enemiesToSpawn = lastCustomSettings.enemiesToSpawn;
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
