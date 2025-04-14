using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    private GameObject[] towers = new GameObject[100];
    private int value = 0;

    private bool placed = true;
    private CircleCollider2D bigTriggerColliderr;
    private CircleCollider2D smallTriggerCollider;

    private Tower tower;

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!placed)
        {
            towers[value].transform.position = mousePos;
        }
    }

    public void StartPlacingTower(int index)
    {
        if (!placed)
            Destroy(towers[value]); // Destroy previous tower if still in placement mode
        else
            StartCoroutine(SelectTower(index));
    }

    public IEnumerator SelectTower(int index)
    {
        // Instantiate the tower at an initial position.
        towers[value] = Instantiate(towerPrefabs[index], Vector2.zero, Quaternion.identity);
        tower = towers[value].GetComponent<Tower>();
        placed = false;

        // Wait until the left mouse button is pressed AND the tower is fully inside a wall.
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) && tower.fullyInside);

        GetColliders();
        PlaceTower();
    }

    private void PlaceTower()
    {
        // Set exclude layers for the colliders as needed
        bigTriggerColliderr.excludeLayers = LayerMask.GetMask("Enemy Attacks");
        smallTriggerCollider.excludeLayers = 0;
        placed = true;
        value++;
    }

    private void GetColliders()
    {
        // The tower is expected to have 2 CircleCollider2D components.
        CircleCollider2D[] colliders = towers[value].GetComponents<CircleCollider2D>();
        if (colliders.Length < 2)
        {
            Debug.LogError("Not enough colliders on the tower prefab!");
            return;
        }

        // Assign based on radius
        if (colliders[0].radius > colliders[1].radius)
        {
            bigTriggerColliderr = colliders[0];
            smallTriggerCollider = colliders[1];
        }
        else
        {
            bigTriggerColliderr = colliders[1];
            smallTriggerCollider = colliders[0];
        }
    }
}
