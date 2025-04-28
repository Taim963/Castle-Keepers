using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateManager.PlayerState stateKey, PlayerStateManager stateMachine)
        : base(stateKey, stateMachine)
    {
    }

    public override void UpdateState()
    {
        // Idle state doesn't need to do anything
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        // Transition to move state if there's any movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            return PlayerStateManager.PlayerState.Move;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && player.canDash)
        {
            return PlayerStateManager.PlayerState.Dash;
        }

        return StateKey; // Stay in idle state
    }
}
