using UnityEngine;

public abstract class PlayerBaseState : BaseState<PlayerStateManager.PlayerState>
{
    protected PlayerStateManager stateMachine;
    protected Player player;

    public PlayerBaseState(PlayerStateManager.PlayerState stateKey, PlayerStateManager stateMachine)
        : base(stateKey)
    {
        this.stateMachine = stateMachine;
        this.player = stateMachine.PlayerScript;
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

    public override PlayerStateManager.PlayerState GetNextState()
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
