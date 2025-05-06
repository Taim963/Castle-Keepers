using UnityEngine;

[CreateAssetMenu(fileName = "ChaserSO", menuName = "Scriptable Objects/ChaserSO")]
public class ChaserSO : EntitySO
{
    [Header("Chaser Settings")]
    public NavMeshHandler navMeshHandler;
    public LayerMask rayCastCollide;
}
