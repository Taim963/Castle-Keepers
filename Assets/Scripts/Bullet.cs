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

    [HideInInspector] public BulletWeaponSO bulletWeaponSO; // refrence to the gun data

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track enemies, not colliders

    [HideInInspector] public GameManager gameManager; // Reference to the GameManager

    protected override void Start()
    {
        gameManager = GameManager.instance; // Get the GameManager instance

        if (bulletWeaponSO.bulletType == BulletWeaponSO.BulletType.Projectile)
        {
            Invoke("ProjectileDeath", bulletWeaponSO.lifetime);
        }

        SetVars();
    }

    private void SetVars()
    {
        damageSum = bulletWeaponSO.bulletDamage + bulletWeaponSO.damage;
        knockbackSum = bulletWeaponSO.bulletKnockbackForce + bulletWeaponSO.knockbackForce;
        pierceSum = bulletWeaponSO.bulletPierce + bulletWeaponSO.pierce;
        speedSum = bulletWeaponSO.bulletSpeed + bulletWeaponSO.speed;
    }

    protected override void Update()
    {
        if (bulletWeaponSO.bulletType == BulletWeaponSO.BulletType.Projectile)
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
        if (IsColliderInLayerMask(other, bulletWeaponSO.hurtMask))
        {
            HandleEnemyCollision(other);
        }

        if (IsColliderInLayerMask(other, bulletWeaponSO.collideMask))
        {
            ProjectileDeath();
        }
    }

    private void HandleEnemyCollision(Collider2D other)
    {
        if (hitEnemies.Contains(other.gameObject)) return;

        if (pierceSum > 0)
        {
            bulletWeaponSO.pierce--;
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
        if (IsColliderInLayerMask(other, bulletWeaponSO.hurtMask))
        {
            gameManager.onProjectileHit.Invoke(damageSum, knockbackSum, other.gameObject, gameObject); // Notify GameManager about the hit
        }
    }

    private void ProjectileDeath()
    {
        GameObject hitInstance = Instantiate(bulletWeaponSO.hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        Destroy(hitInstance, 0.3f);
        Destroy(gameObject);
    }

    private void CreatePierceEffect()
    {
        GameObject hitInstance = Instantiate(bulletWeaponSO.hitEffectPrefab, transform.position, Quaternion.identity);
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