using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    public TroopSO troopSO; // Reference to the ScriptableObject

    private bool isAttacking = false;

    // References
    public NavMeshHandler navMeshHandler; // Reference to the NavMeshHandler script
    public GameObject attackPrefab;      // Array of attack prefabs
    public Health healthBar;
    public int health;

    // Instead of targetTransform we now use currentTarget as a simple position
    private Vector2 currentTarget;
    private Projectile projectile;

    private void Start()
    {
        projectile = attackPrefab.GetComponent<Projectile>();
        projectile.baseWeaponDamage = troopSO.damage;

        // Register enemy in GameManager
        GameManager.instance.RegisterEnemy(gameObject);
        GameManager.instance.onProjectileHit.AddListener(OnProjectileCollide);

        // Initialize health
        healthBar = GetComponentInChildren<Health>();
        health = troopSO.maxHealth;
        healthBar.SetMaxHealth(troopSO.maxHealth);

        // Initialize NavMeshAgent chasing; update our current target position from the handler.
        navMeshHandler.Chase();
        currentTarget = navMeshHandler.target;
    }

    private void Update()
    {
        if (IsReadyToAttack())
        {
            navMeshHandler.StopChasing();
            isAttacking = true;
            StartCoroutine(Attack(attackPrefab));
        }
    }

    private IEnumerator Attack(GameObject attackPrefab)
    {
        Enemy enemy = navMeshHandler.chosenTarget.GetComponent<Enemy>();

        enemy.attackers.Add(gameObject);
        // Use enemy's position instead of a target transform.
        while (Vector2.Distance(transform.position, navMeshHandler.target) <= troopSO.attackRange && CanSeeTarget())
        {
            navMeshHandler.StopChasing();

            PerformAttack(attackPrefab);
            yield return new WaitForSeconds(troopSO.attackCooldown);
        }

        isAttacking = false;
        navMeshHandler.Chase();
    }

    private void PerformAttack(GameObject attackPrefab)
    {
        // Calculate direction and offset for the attack spawn position
        Vector2 direction = navMeshHandler.target - (Vector2)transform.position;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 offset = new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * troopSO.attackOffset;

        // Calculate rotation angle so that the attack faces the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (projectile.speed <= 0)
        {
            // Perform a melee attack instantiation with a raycast, using the enemy's position as the origin.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, normalizedDirection, troopSO.attackRange, troopSO.rayCastCollide);
            Vector3 spawnPosition = hit.point;
            Instantiate(attackPrefab, spawnPosition, Quaternion.Euler(0, 0, angle));
        }
        else
        {
            // Perform a projectile attack instantiation
            Instantiate(attackPrefab, transform.position + offset, Quaternion.Euler(0, 0, angle));
        }
    }

    // Check if the enenmy is being attacked by 3 or more troops by checking the list attackers from the enemy script, if so, change the target to a different one with the highest priority and closest to troop
    private void SwitchTargets()
    {
        Enemy enemy = navMeshHandler.chosenTarget?.GetComponent<Enemy>();

        // Ensure we have a valid target and it's being overwhelmed
        if (enemy == null || enemy.attackers.Count < 3) return;

        GameObject newTarget = null;
        float closestDistance = Mathf.Infinity;
        int highestPriority = int.MaxValue;

        // Loop through potential targets based on priority stored in the dictionary
        foreach (var entry in navMeshHandler.targetInfo)
        {
            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(entry.Key);

            foreach (GameObject potential in potentialTargets)
            {
                Enemy potentialEnemy = potential.GetComponent<Enemy>();
                if (potentialEnemy == null) continue; // Ensure it's a valid enemy

                // Ensure we're not picking another overwhelmed target
                if (potentialEnemy.attackers.Count >= 3) continue;

                float distance = Vector2.Distance(transform.position, potential.transform.position);

                // Select the best target based on priority first, then distance
                if (entry.Value < highestPriority ||
                    (entry.Value == highestPriority && distance < closestDistance))
                {
                    highestPriority = entry.Value;
                    closestDistance = distance;
                    newTarget = potential;
                }
            }
        }

        // If we found a better target, update it
        if (newTarget != null)
        {
            navMeshHandler.chosenTarget = newTarget;
            navMeshHandler.target = newTarget.transform.position;
            navMeshHandler.onTragetUpdate.Invoke();
        }
    }



    private bool IsReadyToAttack()
    {
        // Check if the enemy is close enough to the target, is not already attacking, and can see the target.
        return Vector2.Distance(transform.position, navMeshHandler.target) <= troopSO.attackRange &&
               !isAttacking &&
               CanSeeTarget();
    }

    protected virtual bool CanSeeTarget()
    {
        // Use the enemy's own position for the raycast.
        Vector2 startPos = transform.position;
        Vector2 direction = (navMeshHandler.target - startPos).normalized;

        // Perform the raycast
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, troopSO.attackRange, troopSO.rayCastCollide);
        Debug.DrawRay(startPos, direction * troopSO.attackRange, Color.red);

        // If we hit a collider, find the closest point on its surface relative to the enemy position.
        if (hit.collider != null)
        {
            Vector2 closestPoint = hit.collider.ClosestPoint(startPos); // Closest point on the collider
            const float tolerance = 0.5f; // Distance tolerance for validation

            // Verify if the closest point is close enough to the target position
            if (Vector2.Distance(closestPoint, navMeshHandler.target) <= tolerance)
            {
                return true;
            }
        }
        return false;
    }

    // called when the navMeshHandler updates the target
    public virtual void ChangeTarget()
    {
        // Simply update our current target position from the nav mesh handler.
        currentTarget = navMeshHandler.target;

        Enemy enemy = navMeshHandler.chosenTarget.GetComponent<Enemy>();
        enemy.attackers.Remove(gameObject);
        if (enemy.attackers.Count >= 3) SwitchTargets();
    }

    public void OnProjectileCollide(int damage, float knockbackForce, GameObject thisTroop, GameObject projectile)
    {
        if (thisTroop != gameObject) return;

        TakeDamage(damage);
        TakeKnockback(knockbackForce, transform.position - projectile.transform.position);
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void TakeKnockback(float knockbackForce, Vector2 direction)
    {
        // Normalize the direction vector and scale it by the knockback force
        Vector2 knockbackDirection = direction.normalized * knockbackForce;

        // Apply the calculated knockback to the NavMeshAgent's velocity
        navMeshHandler.agent.velocity = knockbackDirection / troopSO.knockbackResistance;
    }

    private void Die()
    {
        GameManager.instance.OnEnemyDeath(gameObject, troopSO.goldValue);
        GameObject deathEffectInstance = Instantiate(troopSO.deathEffect, transform.position, Quaternion.identity);
        ChangeColorAndScale(deathEffectInstance);
        Destroy(deathEffectInstance, 0.6f);
        Destroy(gameObject);
    }

    private void ChangeColorAndScale(GameObject deathEffectInstance)
    {
        ParticleSystem.MainModule main = deathEffectInstance.GetComponent<ParticleSystem>().main;
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        main.startColor = spriteRenderer.color;
        deathEffectInstance.transform.localScale = Vector2.Scale(spriteRenderer.transform.localScale, deathEffectInstance.transform.localScale);
    }
}