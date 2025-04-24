using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletSO", menuName = "Scriptable Objects/BulletSO")]
public class BulletSO : ScriptableObject
{
    #region // Bullet type
    [Header("Bullet type")]
    public BulletType bulletType; // Drop down menu for bullet type
    #endregion

    #region // Bullet stats
    [Header("Bullet stats")]
    public int pierce = 0;
    [ShowIf("bulletType", BulletType.Projectile)] public float speed = 10f;
    [ShowIf("bulletType", BulletType.Projectile)] public float lifetime = 2f;
    [ShowIf("bulletType", BulletType.HitScan)] public float range = 10f;
    [ShowIf("bulletType", BulletType.HitScan)] public GameObject LineRendererPrefab; // For optimization
    public GameObject hitEffectPrefab;
    public LayerMask collideMask;
    public LayerMask hurtMask;
    #endregion
}

public enum BulletType
{
    HitScan,
    Projectile
}