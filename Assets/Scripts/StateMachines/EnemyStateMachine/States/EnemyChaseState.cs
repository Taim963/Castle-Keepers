using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyStateManager.EnemyState stateKey, EnemyStateManager stateMachine)
        : base(stateKey, stateMachine)
    {
    }

    public override void EnterState()
    {
        // Initialize the chase state
        stateMachine.EnemyScript.Chase();
    }

    public override void ExitState()
    {
        stateMachine.EnemyScript.StopChasing();
    }

    public override EnemyStateManager.EnemyState GetNextState()
    {
        if (stateMachine.EnemyScript.IsTargetInRange())
        {
            return EnemyStateManager.EnemyState.Attack;
        }

        return base.GetNextState();
    }
}
