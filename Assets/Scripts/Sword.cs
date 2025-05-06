using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sword : Weapon
{
    [Header("References")]
    public SwordSO swordSO;
    public Animator animator;
    public Transform holder;

    [Header("Position Settings")]
    public Vector2 defaultLocalPosition = new Vector2(0.5f, 0); // Adjust these values in inspector
    public float defaultLocalRotation = 0f; // Adjust in inspector

    [Header("Swing Settings")]
    public float horizontalRadius = 5f;
    public float verticalRadius = 3f;

    private GameManager gameManager;
    private Collider2D swordCollider;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    // Animation related fields
    private AnimationClip slashAnimation;
    private AnimationClip stabAnimation;
    private float currentAnimationDuration;

    // Cached WaitForSeconds objects
    private WaitForSeconds preFireWait;
    private WaitForSeconds cooldownWait;
    private WaitForSeconds animationWait;

    #region Unity Methods

    protected override void Awake()
    {
        InitializeComponents();
        SetupAnimation();
        InitializeWaitObjects();
        ResetToDefaultPosition();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & swordSO.hurtMask) != 0
            && hitEnemies.Add(other.gameObject))
        {
            gameManager.onProjectileHit.Invoke(
                swordSO.damage,
                swordSO.knockbackForce,
                other.gameObject,
                gameObject
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the swing radius
        DrawSwingRadiusGizmo();

        // Draw the default position
        if (holder != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 defaultPos = holder.TransformPoint(defaultLocalPosition);
            Gizmos.DrawWireSphere(defaultPos, 0.1f);

            // Draw a line to show the default rotation
            Vector3 direction = Quaternion.Euler(0, 0, defaultLocalRotation) * Vector3.right;
            Gizmos.DrawLine(defaultPos, defaultPos + direction * 0.5f);
        }
    }

    #endregion

    #region Initialization Methods

    private void InitializeComponents()
    {
        gameManager = GameManager.instance;
        swordCollider = GetComponentInChildren<Collider2D>();

        if (!gameManager || !swordCollider || !holder)
        {
            Debug.LogError("Missing required components on Sword!");
        }
    }

    private void SetupAnimation()
    {
        slashAnimation = animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(clip => clip.name == "Sword Slash");
        stabAnimation = animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(clip => clip.name == "Sword Stab");

        if (slashAnimation == null || stabAnimation == null)
        {
            Debug.LogError("Required sword animations not found! Please ensure both 'Sword Slash' and 'Sword Stab' animations exist.");
            return;
        }

        UpdateAnimationDuration();
    }

    private void InitializeWaitObjects()
    {
        preFireWait = new WaitForSeconds(swordSO.preFireCooldown);
        cooldownWait = new WaitForSeconds(swordSO.cooldown);
        UpdateAnimationWait();
    }

    #endregion

    #region Animation Methods

    private void UpdateAnimationDuration()
    {
        AnimationClip currentClip = GetCurrentAnimationClip();
        float speedMultiplier = currentClip.length / swordSO.swingSpeed;
        animator.speed = speedMultiplier;
        currentAnimationDuration = currentClip.length / animator.speed;
        UpdateAnimationWait();
    }

    private void UpdateAnimationWait()
    {
        animationWait = new WaitForSeconds(currentAnimationDuration);
    }

    private AnimationClip GetCurrentAnimationClip()
    {
        return swordSO.swordSwingType == SwordSO.SwordSwingType.Slash
            ? slashAnimation
            : stabAnimation;
    }

    private string GetCurrentAnimationTrigger()
    {
        return swordSO.swordSwingType == SwordSO.SwordSwingType.Slash
            ? "Slash"
            : "Stab";
    }

    #endregion

    #region Weapon Behavior

    public override void TryFire()
    {
        if (!isFiring)
        {
            StartCoroutine(SwingSword());
        }
    }

    private IEnumerator SwingSword()
    {
        isFiring = true;
        hitEnemies.Clear();

        while (Input.GetMouseButton(0))
        {
            yield return HandleSingleSwing();
        }

        isFiring = false;
    }

    private IEnumerator HandleSingleSwing()
    {
        if (swordSO.preFireCooldown > 0)
        {
            yield return preFireWait;
        }

        // Trigger animation and enable collision
        animator.SetTrigger(GetCurrentAnimationTrigger());
        swordCollider.enabled = true;

        // Handle swing movement
        OnClickRotate();
        yield return animationWait;

        // Reset position and rotation after swing
        ResetToDefaultPosition();

        // Cleanup after swing
        swordCollider.enabled = false;
        hitEnemies.Clear();

        yield return cooldownWait;
    }

    #endregion

    #region Position and Rotation

    private void OnClickRotate()
    {
        UpdateSwordPosition();
        UpdateSwordRotation();
    }

    private void ResetToDefaultPosition()
    {
        // Reset local position and rotation
        transform.localPosition = defaultLocalPosition;
        transform.localRotation = Quaternion.Euler(0, 0, defaultLocalRotation);
    }

    private void UpdateSwordPosition()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePos - (Vector2)holder.position).normalized;
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x);

        float x = holder.position.x + (horizontalRadius * Mathf.Cos(angle));
        float y = holder.position.y + (verticalRadius * Mathf.Sin(angle));

        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void UpdateSwordRotation()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float rotationAngle = Mathf.Atan2(
            mousePos.y - transform.position.y,
            mousePos.x - transform.position.x
        );
        transform.rotation = Quaternion.Euler(0, 0, (rotationAngle * Mathf.Rad2Deg) - 90f);
    }

    #endregion

    #region Gizmos

    private void DrawSwingRadiusGizmo()
    {
        if (holder == null) return;

        Gizmos.color = Color.green;
        const int segments = 50;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            float x = Mathf.Cos(angle) * horizontalRadius;
            float y = Mathf.Sin(angle) * verticalRadius;
            points[i] = holder.position + new Vector3(x, y, 0);
        }

        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }



    #endregion
}
