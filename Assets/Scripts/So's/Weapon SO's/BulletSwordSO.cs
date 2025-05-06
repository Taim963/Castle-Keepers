using UnityEngine;

[CreateAssetMenu(fileName = "BulletSwordSO", menuName = "Scriptable Objects/BulletSwordSO")]
public class BulletSwordSO : BulletWeaponSO
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

    #region // Sword bullet stats
    [Header("Sword Bullet Stats")]
    public float cooldown;
    #endregion
}
