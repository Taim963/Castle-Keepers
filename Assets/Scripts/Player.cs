using UnityEngine;

public class Player : MonoBehaviour
{
    public Health healthBar;
    public int maxHealth = 100;
    public float speed = 5f;
    public GameObject deathEffect;

    private int health;
    private Rigidbody2D rb;

    void Start()
    {
        //Initilizing
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;
        rb.linearVelocity = movement * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hurt2"))
        {
            EnemyAttack enemyAttack = other.GetComponent<EnemyAttack>();
            
            if (!enemyAttack.hasHit)
            {
                TakeDamage(enemyAttack.damageSum);
                enemyAttack.hasHit = true;
            }
        }

        if (other.CompareTag("Hurt1"))
        {
            EnemyAttack enemyAttack = other.GetComponent<EnemyAttack>();

            if (!enemyAttack.hasHit)
            {
                TakeDamage(enemyAttack.damageSum);
                enemyAttack.hasHit = true;
            }
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
        ParticleSystem.MainModule main = deathEffectInstance.GetComponent<ParticleSystem>().main;
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        main.startColor = spriteRenderer.color;
        deathEffectInstance.transform.localScale = Vector2.Scale(spriteRenderer.transform.localScale, deathEffectInstance.transform.localScale);
    }
}
