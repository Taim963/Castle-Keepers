using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 5;
    public int pierce = 0;
    public float KnockbackForce = 1;
    public float lifetime = 2f;
    public GameObject hitEffectPrefab;
    public LayerMask collisionMask;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track enemies, not colliders

    [HideInInspector] public int baseWeaponDamage;
    [HideInInspector] public int damageSum;

    private void Start()
    {
        damageSum = baseWeaponDamage + damage;
        Invoke("ProjectileDeath", lifetime);
    }

    private void Update()
    {
        Launch();
    }

    private void Launch()
    {
        gameObject.transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, collisionMask) && !hitEnemies.Contains(other.gameObject))
        {
            if (pierce > 0)
            {
                pierce--;
                hitEnemies.Add(other.gameObject);
                CreatePierceEffect();
            }
            else
            {
                ProjectileDeath();
            }
        }

        if (other.CompareTag("Wall") || other.CompareTag("Castle"))
        {
            ProjectileDeath();
        }
    }

    private void ProjectileDeath()
    {
        GameObject hitInstance = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        Destroy(hitInstance, 0.3f);
        Destroy(gameObject);
    }

    private void CreatePierceEffect()
    {
        GameObject hitInstance = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        hitInstance.transform.localScale = Vector2.Scale(hitInstance.transform.localScale, new Vector2(0.5f, 0.5f));    
        Destroy(hitInstance, 0.3f);
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


    private bool IsColliderInLayerMask(Collider2D collider, LayerMask layerMask)
    {
        // Use bitwise operations to check if the collider's layer is in the layerMask
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }
}
