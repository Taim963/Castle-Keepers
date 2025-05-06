using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateManager.EnemyState stateKey, EnemyStateManager stateMachine)
        : base(stateKey, stateMachine)
    {
    }

    public override void UpdateState()
    {
        // Idle state doesn't need to do anything
    }

    public override EnemyStateManager.EnemyState GetNextState()
    {
        if (stateMachine.targetInfo)
        {
            return EnemyStateManager.EnemyState.Chase;
        }

        return StateKey; // Stay in idle state
    }
}
