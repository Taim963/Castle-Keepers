using UnityEngine;

public class Entity : MonoBehaviour
{
    protected EntitySO _entitySO;  // backing field
    public virtual EntitySO entitySO => _entitySO;  // virtual property

    private int health;
    private HealthBar healthBar;

    protected virtual void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        health = entitySO.maxHealth;
        healthBar.SetMaxHealth(health);
    }

    protected virtual void Start()
    {

    }
    public virtual void OnHurtCollide(int damage, float knockbackForce, GameObject thisEnemy, GameObject projectile)
    {
        if (thisEnemy != gameObject) return;

        TakeDamage(damage);
        TakeKnockback(knockbackForce, transform.position - projectile.transform.position);
    }

    public virtual void TakeDamage(int damage)
    {
        health = HealthBar.TakeDamage(health, damage);
        healthBar.SetHealth(health);
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void TakeKnockback(float knockbackForce, Vector2 direction, float knockbackStunDuration)
    {
        // Very different compared to enemies and the player, so, no default implementation here.
    }

    public virtual void TakeKnockback(float knockbackForce, Vector2 direction)
    { 

    }

    protected virtual void Die()
    {
        GameObject deathEffectInstance = Instantiate(entitySO.deathEffect, transform.position, Quaternion.identity);
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
}
