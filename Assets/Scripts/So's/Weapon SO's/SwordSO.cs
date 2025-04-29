using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordSO", menuName = "Scriptable Objects/SwordSO")]
public class SwordSO : WeaponSO
{
    #region // Sword Type
    [Header("Sword Type")]
    public SwordSwingType swordSwingType;
    public bool hasBullet;
    #endregion

    #region // Sword Stats
    [Header("Sword Stats")]
    public float swingSpeed;
    #endregion
}

public enum SwordSwingType
{
    Slash,
    Stab,
}