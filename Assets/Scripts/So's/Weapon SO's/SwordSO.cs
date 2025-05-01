using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordSO", menuName = "Scriptable Objects/SwordSO")]
public class SwordSO : BulletWeaponSO
{

    public enum SwordSwingType
    {
        Slash,
        Stab,
    }

    #region // Sword Type
    [Header("Sword Type")]
    public SwordSwingType swordSwingType;
    public new bool hasBullet = false;
    #endregion

    #region // Sword Stats
    [Header("Sword Stats")]
    public float swingSpeed;
    #endregion
}