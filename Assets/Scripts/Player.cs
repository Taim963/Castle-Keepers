using UnityEngine;
using System.Collections;

public class Player : Entity
{
    public PlayerSO playerSO;
    public override EntitySO entitySO => playerSO;
    public Animator animator;

    private PlayerStateManager stateManager;
    private Rigidbody2D rb;
    private Weapon weaponS;

    private Vector2 movementForce;
    [HideInInspector] public bool canDash = true;
    [HideInInspector] public bool canMove = true;

    protected override void Awake()
    {
        base.Awake();
        stateManager = GetComponent<PlayerStateManager>();
        rb = GetComponent<Rigidbody2D>();
        weaponS = GetComponentInChildren<Weapon>();
    }

    public void Move(Vector2 moveInput)
    {
        if (!canMove) return;
        
        rb.MovePosition(rb.position + moveInput * playerSO.speed * Time.fixedDeltaTime);

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 moveInput = new Vector2(horizontalInput, verticalInput).normalized;

        animator.SetFloat("Vertical", moveInput.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        FlipPlayer(moveInput);
    }

    private void FlipPlayer(Vector2 moveInput)
    {
        if (weaponS.isFiring) return;
        if (moveInput.x > 0.1f)
        {
            // Move right
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < -0.1f)
        {
            // Move left
            transform.localScale = new Vector3(-1, 1, 1);
        }
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
