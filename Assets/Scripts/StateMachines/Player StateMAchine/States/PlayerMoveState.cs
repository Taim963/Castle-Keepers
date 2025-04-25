using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private Rigidbody2D rb;
    private PlayerSO playerSO;

    public PlayerMoveState(PlayerStateManager.PlayerState stateKey, PlayerStateManager stateMachine, GameObject player)
        : base(stateKey, stateMachine, player)
    {
        // Get references in constructor
        this.rb = stateMachine.Rb;
        this.playerSO = stateMachine.PlayerSO;
    }

    public override void UpdateState()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;
        rb.linearVelocity = movement * playerSO.speed;
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            return PlayerStateManager.PlayerState.Idle;
        }

        return StateKey;
    }
}
