using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ChaserSO
{
    #region // Health Settings
    [Header("Health Settings")]
    public int goldValue = 2;
    #endregion

    #region // Enemy Stats
    [Header("Enemy Stats")]
    public int damage = 5;
    public float attackRange = 3f;
    public float attackOffset = 1.2f;
    public float attackCooldown = 1f;
    #endregion
}