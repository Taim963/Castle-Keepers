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
}
