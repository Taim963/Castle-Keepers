using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 100;
    public GameObject deathEffect;

    private int health;
    private HashSet<GameObject> hitAttacks = new HashSet<GameObject>();

    void Start()
    {
        //Initilizing
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hurt1") && !hitAttacks.Contains(other.gameObject))
        {
            EnemyAttack projectile = other.GetComponent<EnemyAttack>();
            if (projectile != null)
            {
                TakeDamage(projectile.damageSum);
            }
            hitAttacks.Add(other.gameObject);
        }
        if (other.CompareTag("Hurt2") && !hitAttacks.Contains(other.gameObject))
        {
            EnemyAttack projectile = other.GetComponent<EnemyAttack>();
            if (projectile != null)
            {
                TakeDamage(projectile.damageSum);
            }
            hitAttacks.Add(other.gameObject);
        }
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
        // Ensure the deathEffectInstance has a ParticleSystem component
        ParticleSystem particleSystem = deathEffectInstance.GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("No ParticleSystem component found on the deathEffectInstance!");
            return;
        }

        // Ensure the current GameObject has a SpriteRenderer component
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer component found on the current GameObject!");
            return;
        }

        // Safely access and modify the ParticleSystem's MainModule
        ParticleSystem.MainModule main = particleSystem.main;
        main.startColor = spriteRenderer.color;

        // Adjust the scale of the death effect instance
        deathEffectInstance.transform.localScale = Vector2.Scale(spriteRenderer.transform.localScale, deathEffectInstance.transform.localScale);
    }

}