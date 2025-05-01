using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletWeaponSO", menuName = "Scriptable Objects/BulletWeaponSO")]
public class BulletWeaponSO : WeaponSO
{
    public enum BulletFireType
    {
        SingleShot,
        Burst,
    }

    public enum BulletType
    {
        HitScan,
        Projectile
    }

    protected virtual bool hasBullet { get; set; } = true;

    #region // Bullet Stats
    [Header("Bullet Stats")]
    [ShowIf("hasBullet")] public BulletFireType bulletFireType;  // Note: Changed from bulletFireType to BulletFireType
    [ShowIf("hasBullet")] public BulletType bulletType;

    [ShowIf("hasBullet")] public GameObject projectilePrefab;
    [ShowIf(EConditionOperator.And, "hasBullet", "IsHitScan")] public GameObject LineRendererPrefab;
    [ShowIf("hasBullet")] public GameObject hitEffectPrefab;

    [ShowIf("hasBullet")] public float randomSpread = 0.1f;
    [ShowIf(EConditionOperator.And, "hasBullet", "IsBurstFire")] public int bulletsPerBurst = 3;
    [ShowIf("bulletFireType", BulletFireType.Burst)] public float burstCooldown = 0f;

    // bullet type dependant stats

    // For projectile bullets
    [ShowIf(EConditionOperator.And, "hasBullet", "IsProjectile")] public float speed = 10f;
    [ShowIf(EConditionOperator.And, "hasBullet", "IsProjectile")] public float lifetime = 2f;

    // For hitscan bullets
    [ShowIf(EConditionOperator.And, "hasBullet", "IsHitScan")] public float range = 10f;

    // Collision masks
    [ShowIf("hasBullet")] public LayerMask collideMask;
    [ShowIf("hasBullet")] public LayerMask hurtMask;

    [Header("Additional bullet Stats")]
    [ShowIf("hasBullet")] public int bulletDamage;
    [ShowIf("hasBullet")] public float bulletKnockbackForce;
    [ShowIf("hasBullet")] public int bulletPierce;
    [ShowIf(EConditionOperator.And, "hasBullet", "IsProjectile")] public float bulletSpeed;
    #endregion

    private bool IsBurstFire() => bulletFireType == BulletFireType.Burst;
    private bool IsHitScan() => bulletType == BulletType.HitScan;
    private bool IsProjectile() => bulletType == BulletType.Projectile;
}
