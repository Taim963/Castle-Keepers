using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;

[System.Serializable]
public class TargetEntry
{
    public string targetTag;
    public int priority; // Lower number = higher priority
}

    public class Chaser : Entity
{
    public virtual ChaserSO ChaserSO => (ChaserSO)entitySO;
    public override EntitySO entitySO => ChaserSO;

    [SerializeField] private List<TargetEntry> targetList = new List<TargetEntry>();
    private Dictionary<string, int> targetInfo = new Dictionary<string, int>();

    private NavMeshAgent agent;
    private GameObject currentTarget;
    private bool isChasing;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();

        foreach (var entry in targetList)
        {
            if (!targetInfo.ContainsKey(entry.targetTag))
            {
                targetInfo[entry.targetTag] = entry.priority;
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void Chase()
    {
        if (!isChasing)
        {
            isChasing = true;
            StartCoroutine(ChaseRoutine());
        }
    }

    public void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            StopAllCoroutines();
            agent.ResetPath();
            agent.velocity = Vector2.zero;
            currentTarget = null;
        }
    }

    private IEnumerator ChaseRoutine()
    {
        while (isChasing)
        {
            UpdateTarget();
            if (currentTarget != null)
            {
                Vector2 targetPoint = GetClosestPointOnTarget();
                agent.SetDestination(targetPoint);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void UpdateTarget()
    {
        if (targetInfo.Count == 0) return;

        currentTarget = null;
        int bestPriority = int.MaxValue;
        float bestDistance = Mathf.Infinity;

        foreach (var entry in targetInfo)
        {
            foreach (GameObject potential in GameObject.FindGameObjectsWithTag(entry.Key))
            {
                if (!potential.TryGetComponent<Collider2D>(out var collider)) continue;

                Vector2 pointOnCollider = collider.ClosestPoint(transform.position);
                float distance = Vector2.Distance(transform.position, pointOnCollider);

                if (entry.Value < bestPriority ||
                    (entry.Value == bestPriority && distance < bestDistance))
                {
                    bestPriority = entry.Value;
                    bestDistance = distance;
                    currentTarget = potential;
                }
            }
        }
    }

    private Vector2 GetClosestPointOnTarget()
    {
        if (currentTarget == null || !currentTarget.TryGetComponent<Collider2D>(out var collider))
            return transform.position;

        return collider.ClosestPoint(transform.position);
    }

    public bool IsTargetInRange()
    {
        if (currentTarget == null) return false;

        Vector2 closestPoint = GetClosestPointOnTarget();
        float distance = Vector2.Distance(transform.position, closestPoint);
        return distance <= ChaserSO.attackRange && CanSeeTarget(closestPoint);
    }

    private bool CanSeeTarget(Vector2 targetPoint)
    {
        if (currentTarget == null) return false;

        Vector2 directionToTarget = (targetPoint - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToTarget,
            ChaserSO.attackRange,
            ChaserSO.rayCastCollide
        );

        return hit.collider != null && hit.collider.gameObject == currentTarget;
    }
}
