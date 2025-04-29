using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static Bullet;

public class Bullet : Hurt
{
    private int damageSum;
    private float knockbackSum;
    private int pierceSum;
    private float speedSum;

    [HideInInspector] public WeaponSO gunSO; // refrence to the gun data

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track enemies, not colliders

    [HideInInspector] public GameManager gameManager; // Reference to the GameManager

    protected override void Start()
    {
        gameManager = GameManager.instance; // Get the GameManager instance

        if (gunSO.bulletType == BulletType.Projectile)
        {
            Invoke("ProjectileDeath", gunSO.lifetime);
        }

        SetVars();
    }

    private void SetVars()
    {
        damageSum = gunSO.bulletDamage + gunSO.damage;
        knockbackSum = gunSO.bulletKnockbackForce + gunSO.knockbackForce;
        pierceSum = gunSO.bulletPierce + gunSO.pierce;
        speedSum = gunSO.bulletSpeed + gunSO.speed;
    }

    protected override void Update()
    {
        if (gunSO.bulletType == BulletType.Projectile)
        { 
            Launch();
        }
        
    }

    private void Launch()
    {
        gameObject.transform.Translate(Vector2.right * speedSum * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, gunSO.hurtMask))
        {
            HandleEnemyCollision(other);
        }

        if (IsColliderInLayerMask(other, gunSO.collideMask))
        {
            ProjectileDeath();
        }
    }

    private void HandleEnemyCollision(Collider2D other)
    {
        if (hitEnemies.Contains(other.gameObject)) return;
        
        if (pierceSum > 0)
        {
            gunSO.pierce--;
            hitEnemies.Add(other.gameObject);
            CreatePierceEffect();
        }
        else
        {
            ProjectileDeath();
        }

        AlertGameManagerOnHit(other);
    }

    public void AlertGameManagerOnHit(Collider2D other)
    {
        if (IsColliderInLayerMask(other, gunSO.hurtMask))
        {
            gameManager.onProjectileHit.Invoke(damageSum, knockbackSum, other.gameObject, gameObject); // Notify GameManager about the hit
        }
    }

    private void ProjectileDeath()
    {
        GameObject hitInstance = Instantiate(gunSO.hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        Destroy(hitInstance, 0.3f);
        Destroy(gameObject);
    }

    private void CreatePierceEffect()
    {
        GameObject hitInstance = Instantiate(gunSO.hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        hitInstance.transform.localScale = Vector2.Scale(hitInstance.transform.localScale, new Vector2(0.5f, 0.5f));    
        Destroy(hitInstance, 0.3f);
    }

    private void ChangeColorAndScale(GameObject hitInstance)
    {
        ParticleSystem.MainModule main = hitInstance.GetComponent<ParticleSystem>().main;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        main.startColor = spriteRenderer.color;
        hitInstance.transform.localScale = Vector2.Scale(spriteRenderer.transform.localScale, hitInstance.transform.localScale);
    }

    private bool IsColliderInLayerMask(Collider2D collider, LayerMask layerMask)
    {
        // Use bitwise operations to check if the collider's layer is in the layerMask
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }
}