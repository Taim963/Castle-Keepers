using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateManager.PlayerState stateKey, PlayerStateManager stateMachine)
        : base(stateKey, stateMachine)
    {
    }

    public override void UpdateState()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveInput.Normalize(); // Normalize to prevent faster diagonal movement

        player.Move(moveInput);
    }

    public override void ExitState()
    {
        player.Move(Vector2.zero); // Stop movement when exiting the state
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            return PlayerStateManager.PlayerState.Idle;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && player.canDash)
        {
            return PlayerStateManager.PlayerState.Dash;
        }

        return StateKey;
    }
}
