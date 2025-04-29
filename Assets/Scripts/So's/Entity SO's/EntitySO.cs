using UnityEngine;

[CreateAssetMenu(fileName = "EntitySO", menuName = "Scriptable Objects/EntitySO")]
public class EntitySO : ScriptableObject
{
    #region // Stats
    [Header("Stats")]
    public int maxHealth;
    public float speed;
    public float KnockbackResistance; // 0 = no resistance (full knockback), 1 = full resistance (no knockback)
    public GameObject deathEffect;
    #endregion
}
