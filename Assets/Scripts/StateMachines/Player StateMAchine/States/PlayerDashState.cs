using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    private float dashDuration = 0.2f; // Duration of the dash state
    private float dashTimer;
    private Vector2 dashDirection;

    // Store layer indices for re-use.
    private int playerLayer;
    private int hurtLayer;
    private int enemyLayer;

    public PlayerDashState(PlayerStateManager.PlayerState stateKey, PlayerStateManager stateMachine)
        : base(stateKey, stateMachine)
    {
    }

    public override void EnterState()
    {
        dashDuration = player.playerSO.dashDuration;
        dashTimer = dashDuration;
        GetDashDirection();
        // Disable collisions with harmful layers
        SetCollisionIgnore(true);
        player.Dash(dashDirection);
    }

    public override void UpdateState()
    {
        dashTimer -= Time.deltaTime;
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        // Once dash duration is over, re-enable collisions and transition state.
        if (dashTimer <= 0f)
        {
            SetCollisionIgnore(false);
            return PlayerStateManager.PlayerState.Idle; // Or transition to another appropriate state
        }
        return StateKey;
    }

    private void GetDashDirection()
    {
        dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        dashDirection.Normalize();

        // Default to mouse direction if no input
        if (dashDirection.magnitude < 0.1f)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dashDirection = (mousePos - (Vector2)player.transform.position).normalized;
        }
    }

    private void SetCollisionIgnore(bool ignore)
    {
        // Ensure we get the proper layer indices
        playerLayer = player.gameObject.layer;
        hurtLayer = LayerMask.NameToLayer("Hurt");
        enemyLayer = LayerMask.NameToLayer("Enemies");

        Physics2D.IgnoreLayerCollision(playerLayer, hurtLayer, ignore);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, ignore);
    }

    public override void ExitState()
    {
        // Optionally, re-enable collisions if for some reason they aren't already set
        SetCollisionIgnore(false);
    }

    // Optional overrides for triggers, etc.
    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerExit2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) { }
}
