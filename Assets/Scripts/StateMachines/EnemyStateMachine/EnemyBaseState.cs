using UnityEngine;

public class EnemyBaseState : BaseState<EnemyStateManager.EnemyState>
{
    protected EnemyStateManager stateMachine;
    protected Enemy enemy;

    public EnemyBaseState(EnemyStateManager.EnemyState stateKey, EnemyStateManager stateMachine)
        : base(stateKey)
    {
        this.stateMachine = stateMachine;
        this.enemy = stateMachine.EnemyScript;
    }

    public override void EnterState()
    {
        //Debug.Log($"Entering {StateKey} state");
    }

    public override void ExitState()
    {
        //Debug.Log($"Exiting {StateKey} state");
    }

    // Default implementations; override in child classes as needed
    public override void UpdateState()
    {
        //Debug.Log($"Updating {StateKey} state");
    }

    public override EnemyStateManager.EnemyState GetNextState()
    {
        //Debug.Log($"Checking transitions from {StateKey} state");
        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"{StateKey} state received OnTriggerEnter2D");
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log($"{StateKey} state received OnTriggerExit2D");
    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log($"{StateKey} state received OnTriggerStay2D");
    }
}
