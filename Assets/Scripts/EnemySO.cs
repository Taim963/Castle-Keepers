using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "EnemySO",menuName = "Enemy Logic")]
public abstract class EnemySO : ScriptableObject
{
    #region // Navigation
    [Header("Navigation")]
    public NavMeshHandler navMeshHandler;
    #endregion

    #region // Health Settings
    [Header("Health Settings")]
    public Health healthBar;
    public int maxHealth = 30;
    public int goldValue = 2;
    [HideInInspector] public int health;
    private HashSet<GameObject> hitAttacks = new HashSet<GameObject>();
    #endregion

    #region // Attack Settings
    [Header("Attack Settings")]
    public int damage = 5;
    public float attackRange = 3f;
    public float attackOffset = 1.2f;
    public float attackCooldown = 1f;
    public float knockbackResistance = 0.5f; // 0 = no resistance (full knockback), 1 = full resistance (no knockback)
    public GameObject[] attackPrefab;
    #endregion

    #region // Effects
    [Header("Effects")]
    public GameObject deathEffect;
    #endregion
}