using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordSO", menuName = "Scriptable Objects/SwordSO")]
public class SwordSO : ScriptableObject
{
    #region // Sword Type
    [Header("Sword Type")]
    public SwordType swordType;
    public bool hasBullet;
    public bool hasAltFire = false;
    public bool hasSpecial = false;
    #endregion

    #region // Bullet Prefab
    [Header("Bullet Prefab")]
    [ShowIf("hasBullet")] public GameObject projectilePrefab;
    #endregion

    #region // Sword Stats
    [Header("Sword Stats")]
    public int damage = 10; // Damage dealt per bullet
    public float knockbackForce = 1; // Knockback force applied to the target
    public float swingSpeed = 1; // Multiplicative, default speed and animation duration is 1 
    public float cooldown = 0.3f;
    public LayerMask hurtMask;
    #endregion
}

public enum SwordType
{
    Slash,
    Stab,
}