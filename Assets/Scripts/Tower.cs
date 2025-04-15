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

    [HideInInspector]
    public bool fullyInside = false;

    // === Private State Variables ===
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Transform target;
    private bool isAttacking = false;

    // === Coroutine Handles ===
    private Coroutine wallCheckCoroutine;

    // === Unity Callbacks ===
    private void Start()
    {
        // Any initialization logic you require
    }

    private void Update()
    {
        // Automatically start attacking if at least one enemy is in range and we're not already attacking.
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
            // Start checking if we are fully inside the wall.
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
        }
    }

    private void RemoveEnemyFromRange(Collider2D enemyCollider)
    {
        Enemy enemy = enemyCollider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
        }
    }

    private void StartWallCheck(Collider2D wallCollider)
    {
        if (wallCheckCoroutine != null)
        {
            StopCoroutine(wallCheckCoroutine);
        }
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
        // Run the check on a FixedUpdate cycle to synchronize with Unity's physics engine.
        while (true)
        {
            // First, ensure that the full bounds of the tower are inside the wall.
            if (wallCollider.bounds.Contains(towerCollider.bounds.min) &&
                wallCollider.bounds.Contains(towerCollider.bounds.max))
            {
                // Additionally, verify that there isn’t an overlapping tower nearby.
                if (IsPositionValid())
                {
                    fullyInside = true;
                    yield break; // Position valid—exit the check.
                }
            }

            fullyInside = false;
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        // Continue attacking while there are enemies in range.
        while (enemiesInRange.Count > 0)
        {
            // Set the target using the first enemy in the list.
            target = enemiesInRange[0]?.transform;

            if (target != null)
            {
                TryAttack(target);
            }
            else
            {
                // If the enemy is no longer valid, remove it.
                enemiesInRange.RemoveAt(0);
            }

            yield return new WaitForSeconds(cooldown);
        }

        isAttacking = false;
        target = null;
    }

    private void TryAttack(Transform target)
    {
        // Calculate attack direction and spawn offset.
        Vector2 direction = (Vector2)(target.position - transform.position);
        Vector3 offset = direction.normalized * 0.5f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate the attack prefab facing the target.
        Instantiate(attackPrefab, transform.position + offset, Quaternion.Euler(0f, 0f, angle));
    }

    private bool IsPositionValid()
    {
        // Use OverlapCircleAll to check for nearby towers.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(towerCollider.bounds.center, 0.3f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Tower") && collider.gameObject != gameObject)
            {
                float distance = Vector2.Distance(collider.bounds.center, towerCollider.bounds.center);
                // Ensure towers maintain a minimum spacing.
                if (distance < 1f)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
