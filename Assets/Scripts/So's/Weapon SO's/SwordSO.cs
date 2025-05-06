using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordSO", menuName = "Scriptable Objects/SwordSO")]
public class SwordSO : WeaponSO
{
    public enum SwordSwingType
    {
        Slash,
        Stab,
    }

    #region // Sword Type
    [Header("Sword Type")]
    public SwordSwingType swordSwingType;
    #endregion

    #region // Sword Stats
    [Header("Sword Stats")]
    public float swingSpeed;
    public LayerMask hurtMask;
    #endregion
}