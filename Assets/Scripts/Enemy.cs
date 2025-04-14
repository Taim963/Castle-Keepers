using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
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
    private HashSet<GameObject> hitProjectiles = new HashSet<GameObject>();

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
    private bool isFirstAttack = true;

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
        if (target == null) return;

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

    private IEnumerator Attack(GameObject attackPrefab)
    {
        while (Vector2.Distance(transform.position, target.position) <= attackRange && CanSeeTarget())
        {
            CancelInvoke("ResetFirstAttack");
            agent.ResetPath();
            agent.velocity = Vector2.zero;

            if (isFirstAttack) yield return new WaitForSeconds(1);
            isFirstAttack = false;

            PerformAttack(attackPrefab);
            yield return new WaitForSeconds(attackCooldown);
        }

        Invoke("ResetFirstAttack", 0.5f);
        isAttacking = false;
        agent.SetDestination(target.position);
    }

    private void ResetFirstAttack()
    {
        isFirstAttack = true;
    }

    private void PerformAttack(GameObject attackPrefab)
    {
        EnemyMeleeAttack attack = attackPrefab.GetComponent<EnemyMeleeAttack>();
        if (attack == null)
        {
            EnemyRangedAttack attack2 = attackPrefab.GetComponent<EnemyRangedAttack>();
            attack2.baseEnemyDamage = damage;
        }
        else
            attack.baseEnemyDamage = damage;



        // Calculate direction and offset for the attack spawn position
        Vector2 direction = target.position - transform.position;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 offset = new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * attackOffset;

        // Calculate rotation angle so that the attack faces the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate the attack prefab
        Instantiate(attackPrefab, transform.position + offset, Quaternion.Euler(0, 0, angle));
    }

    private bool CanSeeTarget()
    {
        // Calculate the direction toward the target and perform a raycast
        Vector2 direction = (target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, attackRange, rayCastCollide);
        Debug.DrawRay(transform.position, direction * attackRange, Color.red);
        return hit.collider != null && hit.collider.transform == target;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile") && !hitProjectiles.Contains(other.gameObject))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damageSum);
                ApplyKnockback(projectile.KnockbackForce, transform.position - other.transform.position);

            }
            else
            {
                TowerAttack towerAttack = other.GetComponent<TowerAttack>();
                if (towerAttack != null)
                {
                    TakeDamage(towerAttack.damage);
                    ApplyKnockback(towerAttack.KnockbackForce, transform.position - other.transform.position);

                }
            }
            hitProjectiles.Add(other.gameObject);
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
