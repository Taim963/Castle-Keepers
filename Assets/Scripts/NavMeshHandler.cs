using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class TargetEntry
{
    public string targetTag;
    public int priority; // Lower number = higher priority
}

public class NavMeshHandler : MonoBehaviour
{
    [SerializeField] private List<TargetEntry> targetList = new List<TargetEntry>(); // Serialized for Unity Inspector
    public Dictionary<string, int> targetInfo = new Dictionary<string, int>(); // Runtime Dictionary
    public UnityEvent onTragetUpdate;

    [HideInInspector] public Vector2 target;
    public NavMeshAgent agent;
    public GameObject chosenTarget;

    protected virtual void Start()
    {
        // Initialize NavMeshAgent
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Convert serialized list to dictionary at runtime
        foreach (var entry in targetList)
        {
            if (!targetInfo.ContainsKey(entry.targetTag))
            {
                targetInfo[entry.targetTag] = entry.priority;
            }
        }
    }

    public void Chase()
    {
        GetTarget();
        StartCoroutine(UpdateDestination());
    }

    public void StopChasing()
    {
        StopCoroutine(UpdateDestination());
        agent.ResetPath();
        agent.velocity = Vector2.zero;
    }

    private IEnumerator UpdateDestination()
    {
        while (true)
        {
            GetTarget();
            agent.SetDestination(target);
            yield return null;
        }
    }

    private void GetTarget()
    {
        if (targetInfo.Count == 0)
        {
            Debug.LogWarning("No TargetInfo provided.");
            return;
        }

        chosenTarget = null;
        int bestPriority = int.MaxValue;
        float bestDistance = Mathf.Infinity;
        Vector2 closestPoint = Vector2.zero;

        // Loop through each entry in the dictionary
        foreach (KeyValuePair<string, int> entry in targetInfo)
        {
            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(entry.Key);

            if (potentialTargets.Length == 0)
                continue;

            // Evaluate each candidate
            foreach (GameObject potential in potentialTargets)
            {
                Collider2D potentialCollider = potential.GetComponent<Collider2D>();

                if (potentialCollider == null)
                {
                    Debug.LogWarning($"GameObject {potential.name} does not have a Collider2D.");
                    continue;
                }

                // Find the closest point on the collider to this object
                Vector2 pointOnCollider = potentialCollider.ClosestPoint(transform.position);
                float distance = Vector2.Distance(transform.position, pointOnCollider);

                // Select target based on priority first, then distance
                if (entry.Value < bestPriority ||
                    (entry.Value == bestPriority && distance < bestDistance))
                {
                    bestPriority = entry.Value;
                    bestDistance = distance;
                    chosenTarget = potential;
                    closestPoint = pointOnCollider;
                }
            }
        }

        if (chosenTarget != null)
        {
            target = closestPoint;
            onTragetUpdate.Invoke();
        }
        else
        {
            Debug.LogWarning("No valid target found based on the TargetInfo entries.");
        }
    }
}
