using UnityEngine;
using System.Collections;

public class Player : Entity
{
    public PlayerSO playerSO;
    public override EntitySO entitySO => playerSO;

    private PlayerStateManager stateManager;
    private Rigidbody2D rb;

    private Vector2 movementForce;
    [HideInInspector] public bool canDash = true;
    [HideInInspector] public bool canMove = true;

    protected override void Awake()
    {
        base.Awake();
        stateManager = GetComponent<PlayerStateManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 moveInput)
    {
        if (!canMove) return;
        if (moveInput.magnitude > 0.1f)
        {
            // Compute the desired target force
            Vector2 targetForce = moveInput * playerSO.speed;

            // Smoothly blend current movementForce to the new targetForce using the acceleration rate
            movementForce = Vector2.MoveTowards(
                movementForce,
                targetForce,
                playerSO.accelerationRate * Time.fixedDeltaTime
            );

            // Apply the movement force
            rb.AddForce(movementForce, ForceMode2D.Force);
        }
        else
        {
            // Decelerate: move the velocity straight toward zero at a fixed speed.
            rb.linearVelocity = Vector2.MoveTowards(
                rb.linearVelocity,
                Vector2.zero,
                playerSO.decelerationRate * Time.fixedDeltaTime
            );
        }

        // Clamp the total velocity to remain within the maximum speed.
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, playerSO.speed);
    }

    public void Dash(Vector2 dashDirection)
    {
        rb.AddForce(dashDirection * playerSO.dashForce, ForceMode2D.Impulse);
        StartCoroutine(DashCooldownRoutine());
    }

    private IEnumerator DashCooldownRoutine()
    {
        canDash = false;
        yield return new WaitForSeconds(playerSO.dashCooldown);
        canDash = true;
    }

    public override void TakeKnockback(float knockbackForce, Vector2 direction, float knockbackStunDuration)
    {
        StartCoroutine(MoveCoolDownRoutine(knockbackStunDuration));
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    public override void TakeKnockback(float knockbackForce, Vector2 direction)
    {
        StartCoroutine(MoveCoolDownRoutine());
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator MoveCoolDownRoutine(float knockbackStunDuration = 0.3f)
    {
        canMove = false;
        yield return new WaitForSeconds(knockbackStunDuration);
        canMove = true;
    }
}
