using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // SO
    public EnemySO enemySO;

    // Flags
    private bool isFirstAttack = true;
    private bool isAttacking = false;
    private Bullet bullet;

    // References
    public NavMeshHandler navMeshHandler; // Reference to the NavMeshHandler script
    public GameObject attackPrefab;      // Array of attack prefabs
    public HealthBar healthBar;
    public int health;

    // Instead of targetTransform we now use currentTarget as a simple position
    private Vector2 currentTarget;
    public List<GameObject> attackers; // List of troops attacking the enemy 

    private void Start()
    {
        bullet = attackPrefab.GetComponent<Bullet>();

        // Register enemy in GameManager
        GameManager.instance.RegisterEnemy(gameObject);
        GameManager.instance.onProjectileHit.AddListener(OnHurtCollide);

        // Initialize health
        healthBar = GetComponentInChildren<HealthBar>();
        health = enemySO.maxHealth;
        healthBar.SetMaxHealth(enemySO.maxHealth);

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
        // Use enemy's position instead of a target transform.
        while (Vector2.Distance(transform.position, navMeshHandler.target) <= enemySO.attackRange && CanSeeTarget())
        {
            CancelInvoke("ResetFirstAttack");
            navMeshHandler.StopChasing();

            if (isFirstAttack)
                yield return new WaitForSeconds(1);
            isFirstAttack = false;

            PerformAttack(attackPrefab);
            yield return new WaitForSeconds(enemySO.attackCooldown);
        }

        Invoke("ResetFirstAttack", 0.5f);
        isAttacking = false;
        navMeshHandler.Chase();
    }

    private void ResetFirstAttack()
    {
        isFirstAttack = true;
    }

    private void PerformAttack(GameObject attackPrefab)
    {
        // Calculate direction and offset for the attack spawn position
        Vector2 direction = navMeshHandler.target - (Vector2)transform.position;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 offset = new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * enemySO.attackOffset;

        // Calculate rotation angle so that the attack faces the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //if (bullet.bulletSO.speed <= 0)
        //{
        //    // Perform a melee attack instantiation with a raycast, using the enemy's position as the origin.
        //    RaycastHit2D hit = Physics2D.Raycast(transform.position, normalizedDirection, enemySO.attackRange, enemySO.rayCastCollide);
        //    Vector3 spawnPosition = hit.point;
        //    Instantiate(attackPrefab, spawnPosition, Quaternion.Euler(0, 0, angle));
        //}
        //else
        //{
        //    // Perform a projectile attack instantiation
        //    Instantiate(attackPrefab, transform.position + offset, Quaternion.Euler(0, 0, angle));
        //}
    }

    private bool IsReadyToAttack()
    {
        // Check if the enemy is close enough to the target, is not already attacking, and can see the target.
        return Vector2.Distance(transform.position, navMeshHandler.target) <= enemySO.attackRange &&
               !isAttacking &&
               CanSeeTarget();
    }

    protected virtual bool CanSeeTarget()
    {
        // Use the enemy's own position for the raycast.
        Vector2 startPos = transform.position;
        Vector2 direction = (navMeshHandler.target - startPos).normalized;

        // Perform the raycast
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, enemySO.attackRange, enemySO.rayCastCollide);
        Debug.DrawRay(startPos, direction * enemySO.attackRange, Color.red);

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


    public virtual void ChangeTarget()
    {
        // Simply update our current target position from the nav mesh handler.
        currentTarget = navMeshHandler.target;
    }

    public void OnHurtCollide(int damage, float knockbackForce, GameObject thisEnemy, GameObject projectile)
    {
        if (thisEnemy != gameObject) return;

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
        navMeshHandler.agent.velocity = knockbackDirection / enemySO.knockbackResistance;
    }

    private void Die()
    {
        GameManager.instance.OnEnemyDeath(gameObject, enemySO.goldValue);
        GameObject deathEffectInstance = Instantiate(enemySO.deathEffect, transform.position, Quaternion.identity);
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
