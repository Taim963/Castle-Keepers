using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class TargetInfo
{
    public string targetTag;
    public int targetPriority; // Priority for selecting the target, smaller value = higher priority
}

public class NavMeshHandler : MonoBehaviour
{
    public TargetInfo[] targetInfo; // Reference to the TargetInfo object
    //instead of the code above, create a dictionary array with a string for and a priority for a key (lower number = higher priority).
    public UnityEvent onTragetUpdate;
    [HideInInspector] public Vector2 target; // The enemy transform from which we got the closest point
    public NavMeshAgent agent;

    protected virtual void Start()
    {
        // Initialize NavMeshAgent
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
        if (targetInfo == null || targetInfo.Length == 0)
        {
            Debug.LogWarning("No TargetInfo provided.");
            return;
        }

        GameObject chosenTarget = null;
        int bestPriority = int.MaxValue;
        float bestDistance = Mathf.Infinity;
        Vector2 closestPoint = Vector2.zero;

        // Loop through each TargetInfo entry
        foreach (TargetInfo info in targetInfo)
        {
            if (string.IsNullOrEmpty(info.targetTag))
            {
                Debug.LogWarning("A TargetInfo entry has an empty targetTag.");
                continue;
            }

            // Retrieve all objects that match the tag from this TargetInfo
            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(info.targetTag);

            if (potentialTargets.Length == 0)
                continue; // No objects found under this tag

            // Evaluate each candidate
            foreach (GameObject potential in potentialTargets)
            {
                Collider2D potentialCollider = potential.GetComponent<Collider2D>();

                if (potentialCollider == null)
                {
                    Debug.LogWarning($"GameObject {potential.name} does not have a Collider2D.");
                    continue;
                }

                // Find the closest point on the collider to this object (e.g., the enemy)
                Vector2 pointOnCollider = potentialCollider.ClosestPoint(transform.position);
                float distance = Vector2.Distance(transform.position, pointOnCollider);

                // Use targetPriority as the primary selector (smaller value is better)
                // and distance as a tiebreaker
                if (info.targetPriority < bestPriority ||
                    (info.targetPriority == bestPriority && distance < bestDistance))
                {
                    bestPriority = info.targetPriority;
                    bestDistance = distance;
                    chosenTarget = potential;
                    closestPoint = pointOnCollider;
                }
            }
        }

        if (chosenTarget != null)
        {
            target = closestPoint; // Store the closest point on the chosen target's collider
            onTragetUpdate.Invoke();
        }
        else
        {
            Debug.LogWarning("No valid target found based on the TargetInfo entries.");
        }
    }
}
