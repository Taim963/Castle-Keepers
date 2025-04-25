using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateManager.PlayerState stateKey, PlayerStateManager stateMachine, GameObject player)
        : base(stateKey, stateMachine, player) { }

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

        return StateKey; // Stay in idle state
    }
}
