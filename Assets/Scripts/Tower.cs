using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // === Public Configuration ===
    [Header("Tower Settings")]
    public float cooldown = 1f;
    public GameObject attackPrefab;

    [Header("Collision Settings")]
    public Collider2D towerCollider; // Assigned in Inspector

    [HideInInspector] public bool fullyInside = false;

    // === Private State Variables ===
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Transform target;
    private bool isAttacking = false;

    // === Coroutines ===
    private Coroutine wallCheckCoroutine;

    // === Unity Callbacks ===
    private void Start()
    {

    }

    private void Update()
    {
        // Automatically start attacking if conditions are met
        if (enemiesInRange.Count > 0 && target == null && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            AddEnemyToRange(other);
        }
        else if (other.CompareTag("Wall"))
        {
            StartWallCheck(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            RemoveEnemyFromRange(other);
        }
        else if (other.CompareTag("Wall"))
        {
            StopWallCheck();
        }
    }

    // === Core Logic ===
    private void AddEnemyToRange(Collider2D enemyCollider)
    {
        Enemy enemy = enemyCollider.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
            Debug.Log("Enemy added to range.");
        }
    }

    private void RemoveEnemyFromRange(Collider2D enemyCollider)
    {
        Enemy enemy = enemyCollider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
            Debug.Log("Enemy removed from range.");
        }
    }

    private void StartWallCheck(Collider2D wallCollider)
    {
        if (wallCheckCoroutine != null)
            StopCoroutine(wallCheckCoroutine);

        wallCheckCoroutine = StartCoroutine(IsFullyInside(wallCollider));
    }

    private void StopWallCheck()
    {
        fullyInside = false;
        if (wallCheckCoroutine != null)
        {
            StopCoroutine(wallCheckCoroutine);
            wallCheckCoroutine = null;
        }
    }

    private IEnumerator IsFullyInside(Collider2D wallCollider)
    {
        while (true)
        {
            if (wallCollider.bounds.Contains(towerCollider.bounds.min) &&
                wallCollider.bounds.Contains(towerCollider.bounds.max) &&
                IsPositionValid())
            {
                fullyInside = true;
                Debug.Log("Tower is fully inside the wall.");
                yield break;
            }
            else
            {
                fullyInside = false;
            }
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        while (enemiesInRange.Count > 0)
        {
            // Set the target to the first enemy in range
            target = enemiesInRange[0].transform;

            if (target != null)
            {
                TryAttack(target);
            }
            else
            {
                // Remove null targets
                enemiesInRange.RemoveAt(0);
            }

            yield return new WaitForSeconds(cooldown);
        }

        isAttacking = false;
        target = null;
    }

    private void TryAttack(Transform target)
    {
        Debug.Log("Fire!");
        Vector2 direction = (Vector2)(target.position - transform.position);
        Vector3 offset = direction.normalized * 0.5f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate attack prefab
        Instantiate(
            attackPrefab,
            transform.position + offset,
            Quaternion.Euler(0f, 0f, angle)
        );
        Debug.Log($"Attacking {target.name}");
    }

    private bool IsPositionValid()
    {
        // Get all colliders within the circle
        Collider2D[] colliders = Physics2D.OverlapCircleAll(towerCollider.bounds.center, 0.3f);

        // Iterate through the colliders
        foreach (Collider2D collider in colliders)
        {
            // Check if the collider is tagged as "Tower" and is not the current object
            if (collider.gameObject.CompareTag("Tower") && collider.gameObject != this.gameObject)
            {
                // Calculate the distance to the current object
                float distance = Vector2.Distance(collider.bounds.center, towerCollider.bounds.center);

                // If the distance is less than 1.5, return false
                if (distance < 1.5f)
                {
                    return false;
                }
            }
        }

        // If all conditions are met, return true
        return true;
    }

}
