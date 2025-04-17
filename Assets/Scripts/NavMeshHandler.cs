using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshHandler : MonoBehaviour
{
    [HideInInspector] public Vector2 target;
    [HideInInspector] public Transform location;
    [HideInInspector] public NavMeshAgent agent;
    public string targetTag = "Castle";
    public LayerMask rayCastCollide;

    protected virtual void Start()
    {
        // Initialize NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
}
