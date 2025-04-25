using UnityEngine;

public abstract class PlayerBaseState : BaseState<PlayerStateManager.PlayerState>
{
    protected PlayerStateManager stateMachine;

    public PlayerBaseState(PlayerStateManager.PlayerState stateKey, PlayerStateManager stateMachine, GameObject Player)
        : base(stateKey)
    {
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        Debug.Log($"Entering {StateKey} state");
    }

    public override void ExitState()
    {
        Debug.Log($"Entering {StateKey} state");
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        Debug.Log($"Entering {StateKey} state");
        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Entering {StateKey} state");
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Entering {StateKey} state");
    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"Entering {StateKey} state");
    }

    public override void UpdateState()
    {
        Debug.Log($"Entering {StateKey} state");
    }
}