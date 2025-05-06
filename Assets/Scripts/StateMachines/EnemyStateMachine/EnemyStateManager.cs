using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class EnemyStateManager : StateManager<EnemyStateManager.EnemyState>
{
    [HideInInspector] public Enemy EnemyScript { get; private set; }
    [HideInInspector] public EnemySO EnemySO { get; private set; }
    [HideInInspector] public GameObject Enemy { get; private set; }

    private void Awake()
    {
        // GetComponent calls—make sure these components exist on the GameObject!
        EnemyScript = GetComponent<Enemy>();
        if (EnemyScript == null)
        {
            Debug.LogError("Enemy component not found on the GameObject");
        }
        else
        {
            EnemySO = EnemyScript.EnemySO;
        }

        Enemy = gameObject;

        // Initialize the state machine with plain C# state objects
        states = new Dictionary<EnemyState, BaseState<EnemyState>>
        {
            { EnemyState.Idle, new EnemyIdleState(EnemyState.Idle, this) },
            // Add other states as needed.
        };

        // Default state
        currentState = states[EnemyState.Idle];
    }

    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Hit,
        Dead,
    }
}
