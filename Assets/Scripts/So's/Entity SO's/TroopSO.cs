using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "TroopSO", menuName = "Scriptable Objects/TroopSO")]
public class TroopSO : ScriptableObject
{
    #region // Navigation
    [Header("Navigation")]
    public LayerMask rayCastCollide; // LayerMask for raycast collision
    #endregion

    #region // Health Settings
    [Header("Health Settings")]
    public int maxHealth = 30;
    public int goldValue = 2;
    #endregion

    #region // Enemy Stats
    [Header("Enemy Stats")]
    public int damage = 5;
    public float attackRange = 3f;
    public float attackOffset = 1.2f;
    public float attackCooldown = 1f;
    public float knockbackResistance = 0.7f; // 0 = no resistance (full knockback), 1 = full resistance (no knockback)
    #endregion

    #region // Effects
    [Header("Effects")]
    public GameObject deathEffect;
    #endregion
}
