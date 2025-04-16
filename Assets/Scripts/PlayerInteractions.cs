using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class TowerInfo
{
    public string towerName;
    public GameObject towerPrefab;
    public int cost;
}

[System.Serializable]
public class TroopInfo
{
    public string troopName;
    public GameObject troopPrefab;
    public int cost;
}

public class PlayerInteractions : MonoBehaviour
{
    public int gold = 20;
    public TextMeshProUGUI goldText;
    private int kills = 0;
    public TextMeshProUGUI killsText;

    // Use custom info objects just like in your rounds script.
    public TowerInfo[] towerInfos;
    public TroopInfo[] troopInfos;

    //lists to store the placed towers and troops.
    private List<GameObject> placedTowers = new List<GameObject>();
    private List<GameObject> placedTroops = new List<GameObject>();

    // Tracks the current tower being placed, if any.
    private GameObject currentPlacingTower;
    private Tower currentTower;  // The Tower component on the tower prefab.

    // Collider references for the currently placing tower.
    private CircleCollider2D bigTriggerCollider;
    private CircleCollider2D smallTriggerCollider;

    // Flag to track placement state.
    private bool isPlacing = false;

    private void Start()
    {
        UpdateGoldUI();
    }

    private void Update()
    {
        //update tower's position to follow the mouse while in placing state.
        if (isPlacing && currentPlacingTower != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPlacingTower.transform.position = mousePos;
        }
    }

    public void StartPlacingTower(int index)
    {
        // If already placing a tower, cancel the current placement.
        if (isPlacing)
        {
            Destroy(currentPlacingTower);
            isPlacing = false;
            return;
        }

        // Check if enough gold is available.
        if (gold >= towerInfos[index].cost)
        {
            StartCoroutine(SelectTower(index));
        }
        else
        {
            Debug.Log("Not enough gold to place this tower.");
        }
    }

    public void StartPlacingTroop(int index)
    {
        if (gold >= troopInfos[index].cost)
        {
            StartCoroutine(SelectTroop(index));
        }
        else
        {
            Debug.Log("Not enough gold for this troop.");
        }
    }

    private IEnumerator SelectTower(int index)
    {
        // Instantiate the tower from our custom TowerInfo at an initial position.
        currentPlacingTower = Instantiate(towerInfos[index].towerPrefab, Vector2.zero, Quaternion.identity);
        currentTower = currentPlacingTower.GetComponent<Tower>();

        if (currentTower == null)
        {
            Debug.LogError("The tower prefab does not have a Tower component!");
            yield break;
        }

        isPlacing = true;

        // Wait until the player clicks the left mouse button and the tower is fully inside a wall.
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) && currentTower.fullyInside);

        // If the pointer is over a UI element, cancel the placement.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Destroy(currentPlacingTower);
            isPlacing = false;
            yield break;
        }

        GetColliders(currentPlacingTower);
        PlaceTower();

        gold -= towerInfos[index].cost;
        UpdateGoldUI();
    }

    private IEnumerator SelectTroop(int index)
    {
        GameObject troop = Instantiate(troopInfos[index].troopPrefab, new Vector2(0, -6), Quaternion.identity);
        placedTroops.Add(troop);

        gold -= troopInfos[index].cost;
        UpdateGoldUI();

        yield break;
    }

    private void PlaceTower()
    {
        // Set the exclusion layers for the colliders.
        if (bigTriggerCollider != null)
            bigTriggerCollider.excludeLayers = LayerMask.GetMask("Enemy Attacks");
        if (smallTriggerCollider != null)
            smallTriggerCollider.excludeLayers = 0;

        placedTowers.Add(currentPlacingTower);
        // Reset placement trackers.
        isPlacing = false;
        currentPlacingTower = null;
        currentTower = null;
    }

    private void GetColliders(GameObject towerObj)
    {
        CircleCollider2D[] colliders = towerObj.GetComponents<CircleCollider2D>();
        if (colliders.Length < 2)
        {
            Debug.LogError("Not enough colliders on the tower prefab!");
            return;
        }

        // Choose the collider with the larger radius.
        if (colliders[0].radius > colliders[1].radius)
        {
            bigTriggerCollider = colliders[0];
            smallTriggerCollider = colliders[1];
        }
        else
        {
            bigTriggerCollider = colliders[1];
            smallTriggerCollider = colliders[0];
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = "Gold:\n" + gold;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
    }

    public void UpdateKillsGUI()
    {
        kills += 1;
        killsText.text = "Kills:\n" + kills;
    }
}