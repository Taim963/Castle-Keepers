using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "GunSO", menuName = "Scriptable Objects/GunSO")]
public class GunSO : ScriptableObject
{
    #region // Gun Type
    [Header("Gun Type")]
    public WeaponType weaponType;
    public bool hasAltFire = false;
    public bool hasSpecial = false;
    #endregion

    #region // Bullet Prefab
    [Header("Bullet Prefab")]
    public GameObject projectilePrefab;
    #endregion

    #region // Gun Stats
    [Header("Gun Stats")]
    public int damage = 10; // Damage dealt per bullet
    public float knockbackForce = 1; // Knockback force applied to the target
    public float selfKnockbackForce = 1; // Knockback force applied to the player
    [ShowIf("weaponType", WeaponType.Burst)] public int bulletsPerBurst = 3;
    [ShowIf("weaponType", WeaponType.Burst)] public float burstCooldown = 0.1f;
    public float cooldown = 0.3f;
    public float preFireCooldown = 0f; // Time before the gun starts firing
    public float randomSpread = 0.1f; // Random spread for bullets direction
    #endregion
}

public enum WeaponType
{
    SingleShot,
    Burst,
}