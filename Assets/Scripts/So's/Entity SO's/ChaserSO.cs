using UnityEngine;

[CreateAssetMenu(fileName = "ChaserSO", menuName = "Scriptable Objects/ChaserSO")]
public class ChaserSO : EntitySO
{
    [Header("Chaser Settings")]
    public LayerMask rayCastCollide;
    public float attackRange;
}
