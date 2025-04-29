using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    #region // Weapon Stats
    [Header("Weapon stats")]
    public int damage;
    public float knockbackForce;
    public int pierce;
    public float selfKnockbackForce;
    public float cooldown;
    public float preFireCooldown;
    public bool hasAltFire;
    public bool hasSpecial;
    #endregion

    #region // Bullet Stats
    [Header("Bullet Stats")]
    public bulletFireType bulletFireType;
    public BulletType bulletType;

    public GameObject projectilePrefab;

    public float randomSpread = 0.1f; // Random spread for bullets direction
    [ShowIf("bulletFireType", bulletFireType.Burst)] public int bulletsPerBurst = 3;
    [ShowIf("bulletFireType", bulletFireType.Burst)] public float burstCooldown = 0f;

    // bullet type dependant stats

    // For projectile bullets
    [ShowIf("bulletType", BulletType.Projectile)] public float speed = 10f;
    [ShowIf("bulletType", BulletType.Projectile)] public float lifetime = 2f;

    // For hitscan bullets
    [ShowIf("bulletType", BulletType.HitScan)] public float range = 10f;
    [ShowIf("bulletType", BulletType.HitScan)] public GameObject LineRendererPrefab; // For optimization

    public GameObject hitEffectPrefab;
    public LayerMask collideMask;
    public LayerMask hurtMask;

    // Additional bullet damage, knockback, and pierce, if needed. or for equiping a different type of bullet, etc...
    [Header("Additional bullet Stats")]
    public int bulletDamage;
    public float bulletKnockbackForce;
    public int bulletPierce;
    [ShowIf("bulletType", BulletType.Projectile)] public float bulletSpeed;
    #endregion
}

public enum bulletFireType
{
    SingleShot,
    Burst,
}
public enum BulletType
{
    HitScan,
    Projectile
}