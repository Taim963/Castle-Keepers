using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Timeline;

public class EnemyBase : MonoBehaviour
{
    public EnemySO enemySO;

    protected bool isAttacking = false;

    protected Transform targetTransform; // The transform of the target
    public Health healthBar;
    public NavMeshHandler navMeshHandler;
    public GameObject[] attackPrefab;

    protected virtual void Start()
    {
        // register enemy in GameManager
        GameManager.instance.RegisterEnemy(gameObject);

        // Initialize health
        healthBar = GetComponentInChildren<Health>();
        enemySO.health = enemySO.maxHealth;
        healthBar.SetMaxHealth(enemySO.maxHealth);

        // Initialize NavMeshAgent
        navMeshHandler.Chase();
        targetTransform.position = enemySO.currentTarget;
    }

    protected virtual void TakeDamage(int damage, float knockback, Vector2 direction)
    {
        enemySO.health -= damage;
        healthBar.SetHealth(enemySO.health);
        TakeKnockback(knockback, direction);

        if (enemySO.health <= 0)
        {
            Die();
        }
    }

    protected virtual void TakeKnockback(float knockbackForce, Vector2 direction)
    {
        // Normalize the direction vector and scale it by the knockback force
        Vector2 knockbackDirection = direction.normalized * knockbackForce;

        // Apply the calculated knockback to the agent's velocity
        navMeshHandler.agent.velocity = knockbackDirection / enemySO.knockbackResistance;
    }

    protected virtual void Die()
    {
        GameManager.instance.OnEnemyDeath(gameObject, enemySO.goldValue);
        GameObject deathEffectInstance = Instantiate(enemySO.deathEffect, transform.position, Quaternion.identity);
        ChangeColorAndScale(deathEffectInstance);
        Destroy(deathEffectInstance, 0.6f);
        Destroy(gameObject);
    }

    protected virtual void ChangeColorAndScale(GameObject deathEffectInstance)
    {
        ParticleSystem.MainModule main = deathEffectInstance.GetComponent<ParticleSystem>().main;
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        main.startColor = spriteRenderer.color;
        deathEffectInstance.transform.localScale = Vector2.Scale(spriteRenderer.transform.localScale, deathEffectInstance.transform.localScale);
    }

    protected virtual bool IsReadyToAttack()
    {
        return Vector2.Distance(targetTransform.position, enemySO.currentTarget) <= enemySO.attackRange &&
            !isAttacking &&
            CanSeeTarget();
    }

    protected virtual bool CanSeeTarget()
    {
        // Calculate the direction toward the target and perform a raycast
        Vector2 direction = (enemySO.currentTarget - (Vector2)targetTransform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(targetTransform.position, direction, enemySO.attackRange, enemySO.rayCastCollide);
        Debug.DrawRay(targetTransform.position, direction * enemySO.attackRange, Color.red);
        return hit.collider != null && hit.collider.transform == targetTransform;
    }

    // Set up through unity events
    public virtual void ChangeTarget()
    {
        navMeshHandler.target = enemySO.currentTarget;
        targetTransform.position = enemySO.currentTarget;
    }
}