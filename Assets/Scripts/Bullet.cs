using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static Bullet;

public class Bullet : Hurt
{
    public BulletSO bulletSO; // Reference to the Bullet ScriptableObject

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track enemies, not colliders

    [HideInInspector] public int baseWeaponDamage;
    [HideInInspector] public float baseWeaponKnockback;

    [HideInInspector] public GameManager gameManager; // Reference to the GameManager

    protected override void Start()
    {
        gameManager = GameManager.instance; // Get the GameManager instance

        if (bulletSO.bulletType == BulletType.Projectile)
        {
            Invoke("ProjectileDeath", bulletSO.lifetime);
        } 
    }

    protected override void Update()
    {
        if (bulletSO.bulletType == BulletType.Projectile)
        { 
            Launch();
        }
        
    }

    private void Launch()
    {
        gameObject.transform.Translate(Vector2.right * bulletSO.speed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, bulletSO.hurtMask))
        {
            HandleEnemyCollision(other);
        }

        if (IsColliderInLayerMask(other, bulletSO.collideMask))
        {
            ProjectileDeath();
        }
    }

    private void HandleEnemyCollision(Collider2D other)
    {
        if (hitEnemies.Contains(other.gameObject)) return;
        
        if (bulletSO.pierce > 0)
        {
            bulletSO.pierce--;
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
        if (IsColliderInLayerMask(other, bulletSO.hurtMask))
        {
            gameManager.onProjectileHit.Invoke(baseWeaponDamage, baseWeaponKnockback, other.gameObject, gameObject); // Notify GameManager about the hit
        }
    }

    private void ProjectileDeath()
    {
        GameObject hitInstance = Instantiate(bulletSO.hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        Destroy(hitInstance, 0.3f);
        Destroy(gameObject);
    }

    private void CreatePierceEffect()
    {
        GameObject hitInstance = Instantiate(bulletSO.hitEffectPrefab, transform.position, Quaternion.identity);
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