using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UnityEvent onEnemyDeath; // Event triggered when an enemy dies.
    public UnityEvent onAllEnemiesDead; // Event triggered when all enemies are dead.
    public UnityEvent onPlayerDeath; // Event triggered when the player dies.
    public UnityEvent onCastleDestroyed; // Event triggered when the castle is destroyed.
    public UnityEvent<int, float, GameObject, GameObject> onProjectileHit;

    private HashSet<GameObject> enemiesAlive = new HashSet<GameObject>(); // Track all alive enemies.
    private GameObject player; // Reference to the player object.
    private GameObject castle; // Reference to the castle object.

    private PlayerInteractions playerInteractions; // Reference to PlayerInteractions.

    #region // Singleton
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject); // Destroy duplicate GameManager instance.
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes.
        }
    }

    private void Start()
    {
        // Cache PlayerInteractions reference.
        playerInteractions = FindFirstObjectByType<PlayerInteractions>();
    }
    #endregion

    #region // Enemy Management
    public void RegisterEnemy(GameObject enemy)
    {
        enemiesAlive.Add(enemy);
    }

    public void OnEnemyDeath(GameObject enemy, int goldValue)
    {
        if (enemiesAlive.Contains(enemy))
        {
            enemiesAlive.Remove(enemy);
            onEnemyDeath.Invoke(); // Notify subscribers that an enemy has died.

            // Add gold to the player.
            if (playerInteractions != null)
            {
                playerInteractions.AddGold(goldValue);
            }

            // Check if all enemies are dead.
            if (AreAllEnemiesDead())
            {
                Debug.Log("All enemies are dead! Triggering victory event.");
                onAllEnemiesDead.Invoke(); // Notify subscribers that all enemies are dead.
            }
        }
    }

    public bool AreAllEnemiesDead()
    {
        return enemiesAlive.Count == 0;
    }
    #endregion

    #region // Player Management
    public void RegisterPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnPlayerDeath()
    {
        if (player != null)
        {
            onPlayerDeath.Invoke(); // Notify subscribers that the player has died.
        }
    }
    #endregion

    #region // Castle Management
    public void RegisterCastle()
    {
        castle = GameObject.FindGameObjectWithTag("Castle");
    }
    public void OnCastleDestroyed()
    {
        if (castle != null)
        {
            onCastleDestroyed.Invoke(); // Notify subscribers that the castle has been destroyed.
        }
    }
    #endregion

    #region // Camera zoom

    #endregion
}