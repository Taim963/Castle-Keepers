using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    // Navigation
    [Header("Navigation")]
    public Transform target;
    public LayerMask rayCastCollide;
    private NavMeshAgent agent;

    // Health Settings
    [Header("Health Settings")]
    public Health healthBar;
    public int maxHealth = 30;
    private int health;
    private HashSet<GameObject> hitAttacks = new HashSet<GameObject>();

    // Attack Settings
    [Header("Attack Settings")]
    public int damage = 5;
    public float attackRange = 3f;
    public float attackOffset = 1.2f;
    public float attackCooldown = 1f;
    public float knockbackResistance = 0.5f; // 0 = no resistance (full knockback), 1 = full resistance (no knockback)
    public GameObject[] attackPrefab;

    // Effects
    [Header("Effects")]
    public GameObject deathEffect;


    private bool isAttacking = false;

    private void Start()
    {
        // Initialize NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Initialize health
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (target == null || !CanSeeTarget()) GetTarget();
        if (Vector2.Distance(transform.position, target.position) <= attackRange && CanSeeTarget())
        {
            if (isAttacking) return;
            isAttacking = true;
            StartCoroutine(Attack(attackPrefab[0]));
        }
        else
        {
            agent.SetDestination(target.position);
        }
    }

    private void GetTarget()
    {
        // Find the closest target with the "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Get all enemies in the scene
        float closestDistance = Mathf.Infinity; // Set an initially large closest distance
        Transform closestEnemy = null; // Store the closest enemy

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position); // Calculate distance

            if (distance < closestDistance) // Check if this enemy is closer than the previous
            {
                closestDistance = distance;
                closestEnemy = enemy.transform; // Update the closest enemy
            }
        }

        target = closestEnemy; // Assign the closest enemy's transform to the target variable
    }


    private IEnumerator Attack(GameObject attackPrefab)
    {
        while (Vector2.Distance(transform.position, target.position) <= attackRange && CanSeeTarget())
        {
            agent.ResetPath();
            agent.velocity = Vector2.zero;

            PerformAttack(attackPrefab);
            yield return new WaitForSeconds(attackCooldown);
        }


        isAttacking = false;
        agent.SetDestination(target.position);
    }

    private void PerformAttack(GameObject attackPrefab)
    {
        // Calculate direction and offset for the attack spawn position
        Vector2 direction = target.position - transform.position;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 offset = new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * attackOffset;

        // Calculate rotation angle so that the attack faces the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Cast a ray in the direction of the target
        RaycastHit2D hit = Physics2D.Raycast(transform.position, normalizedDirection, attackRange, rayCastCollide);
        Vector3 spawnPosition = hit.point;
        Instantiate(attackPrefab, spawnPosition, Quaternion.Euler(0, 0, angle));
    }


    private bool CanSeeTarget()
    {
        if (target == null) return false;
        // Calculate the direction toward the target and perform a raycast
        Vector2 direction = (target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, attackRange, rayCastCollide);
        Debug.DrawRay(transform.position, direction * attackRange, Color.red);
        return hit.collider != null && hit.collider.transform == target;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hurt1") || other.CompareTag("Hurt2") && !hitAttacks.Contains(other.gameObject))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damageSum);
                ApplyKnockback(projectile.KnockbackForce, transform.position - other.transform.position);
            }
            hitAttacks.Add(other.gameObject);
        }
    }

    // Restored old version of knockback that doesn't stop the agent
    private void ApplyKnockback(float knockbackForce, Vector2 direction)
    {
        // Normalize the direction vector and scale it by the knockback force
        Vector2 knockbackDirection = direction.normalized * knockbackForce;

        // Apply the calculated knockback to the agent's velocity
        agent.velocity = knockbackDirection / knockbackResistance;
    }


    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameObject deathEffectInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
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
