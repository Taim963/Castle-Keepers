using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    // Navigation
    [Header("Navigation")]
    private Vector2 target;         // Closest point on the enemy collider
    private Transform location;     // The enemy transform from which we got the closest point
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
    public bool rangeAttack = false;
    public int damage = 5;
    public float attackRange = 3f;
    public float attackOffset = 1.2f;
    public float attackCooldown = 1f;
    public float knockbackResistance = 0.5f; // 0 = no resistance, 1 = full resistance
    public GameObject[] attackPrefab;

    // Effects
    [Header("Effects")]
    public GameObject deathEffect;

    private bool isAttacking = false;

    private void Awake()
    {
        
    }

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
        GetTarget();

        if (Vector2.Distance(transform.position, target) <= attackRange && CanSeeTarget())
        {
            if (isAttacking) return;
            isAttacking = true;
            StartCoroutine(Attack(attackPrefab[0]));
        }
        else
        {
            agent.SetDestination(target);
        }
    }

    private void GetTarget()
    {
        // Find all enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Vector2 bestPoint = Vector2.zero;
        Transform bestEnemyTransform = null;

        // For each enemy get its collider's closest point to this troop and choose the closest one.
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider == null)
                continue;

            // Get the closest point on the enemy's collider
            Vector2 candidatePoint = enemyCollider.ClosestPoint(transform.position);
            float distance = Vector2.Distance(transform.position, candidatePoint);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestPoint = candidatePoint;
                bestEnemyTransform = enemy.transform;
            }
        }

        target = bestPoint;
        location = bestEnemyTransform;
    }

    private IEnumerator Attack(GameObject attackPrefab)
    {
        while (Vector2.Distance(transform.position, target) <= attackRange && CanSeeTarget())
        {
            agent.ResetPath();
            agent.velocity = Vector2.zero;

            PerformAttack(attackPrefab);
            yield return new WaitForSeconds(attackCooldown);
        }

        isAttacking = false;
        agent.SetDestination(target);
    }

    private void PerformAttack(GameObject attackPrefab)
    {
        // Calculate the attack direction and offset for spawning the attack prefab
        Vector2 direction = target - (Vector2)transform.position;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 offset = new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * attackOffset;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (!rangeAttack)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, normalizedDirection, attackRange, rayCastCollide);
            Vector3 spawnPosition = hit.point;
            Instantiate(attackPrefab, spawnPosition, Quaternion.Euler(0, 0, angle));
        }
        else
        {
            Instantiate(attackPrefab, transform.position + offset, Quaternion.Euler(0, 0, angle));
        }
    }

    private bool CanSeeTarget()
    {
        if (location == null)
            return false;

        Vector2 direction = (target - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, attackRange, rayCastCollide);
        Debug.DrawRay(transform.position, direction * attackRange, Color.red);
        return hit.collider != null && hit.collider.transform == location;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Hurt1") || other.CompareTag("Hurt2")) && !hitAttacks.Contains(other.gameObject))
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

    private void ApplyKnockback(float knockbackForce, Vector2 direction)
    {
        Vector2 knockbackDirection = direction.normalized * knockbackForce;
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
        deathEffectInstance.transform.localScale = Vector2.Scale(
            spriteRenderer.transform.localScale,
            deathEffectInstance.transform.localScale
        );
    }
}
