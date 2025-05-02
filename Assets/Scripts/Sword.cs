using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Sword : Weapon
{
    public SwordSO swordSO;
    public Animator animator;

    private Entity Holder; // Reference to the entity holding the gun

    private bool isSwinging = false;
    private Collider2D swordCollider;
    private GameManager gameManager;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private Quaternion spreadRotation;
    private LineRenderer[] lineRenderers;
    private int currentLineIndex = 0;

    public Transform holder;
    public float horizontalRadius = 5f;
    public float verticalRadius = 3f;

    // Cache the animation clip and actual duration
    private AnimationClip slashAnimation;
    private float slashDuration;

    protected override void Awake()
    {
        // Cache components
        gameManager = GameManager.instance;
        swordCollider = GetComponent<Collider2D>();

        // Cache animation data
        SetupAnimation();

        Holder = GetComponentInParent<Entity>();
    }

    private void SetupAnimation()
    {
        // Find the slash animation once
        slashAnimation = animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(clip => clip.name == "Sword Slash");

        if (slashAnimation != null)
        {
            // Calculate and cache the speed multiplier and duration
            float speedMultiplier = slashAnimation.length / swordSO.swingSpeed;
            animator.speed = speedMultiplier;
            slashDuration = slashAnimation.length / animator.speed;
        }
        else
        {
            Debug.LogWarning("Sword Slash animation not found!");
        }
    }

    public override void TryFire()
    {
        if (!isSwinging)
        {
            StartCoroutine(SwingSword());
        }
    }

    private IEnumerator SwingSword()
    {
        isSwinging = true;

        WaitForSeconds preFireWait = new WaitForSeconds(swordSO.preFireCooldown);
        WaitForSeconds cooldownWait = new WaitForSeconds(swordSO.cooldown);
        WaitForSeconds slashWait = new WaitForSeconds(slashDuration);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        while (Input.GetMouseButton(0))
        {
            if (swordSO.preFireCooldown > 0)
            {
                yield return preFireWait;
            }

            animator.SetTrigger("Swing");

            swordCollider.enabled = true;
            OnClickRotate(mousePos);
            yield return slashWait;
            ResetRotation();
            swordCollider.enabled = false;

            transform.rotation = Quaternion.Euler(0, 0, 0);

            hitEnemies.Clear();
            yield return cooldownWait;
        }
        isSwinging = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Combine layer check and HashSet check in one if statement
        if (((1 << other.gameObject.layer) & swordSO.hurtMask) != 0
            && hitEnemies.Add(other.gameObject)) // Add returns false if already present
        {
            gameManager.onProjectileHit.Invoke(
                swordSO.damage,
                swordSO.knockbackForce,
                other.gameObject,
                gameObject
            );
        }
    }

    private void OnClickRotate(Vector2 mousePos)
    {
        // Calculate direction from center to mouse
        Vector2 directionToMouse = (mousePos - (Vector2)holder.position).normalized;

        // Calculate position on ellipse
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x);
        float x = holder.position.x + (horizontalRadius * Mathf.Cos(angle));
        float y = holder.position.y + (verticalRadius * Mathf.Sin(angle));

        // Set position
        transform.position = new Vector3(x, y, transform.position.z);

        // Calculate rotation to look at mouse with X axis
        float rotationAngle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, 0, (rotationAngle * Mathf.Rad2Deg) - 90f);  // Subtract 90 degrees
    }

    private void ResetRotation()
    {
        // Get mouse position in world space
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate direction from center to mouse
        Vector2 directionToMouse = (mousePos - (Vector2)holder.position).normalized;

        // Calculate position on ellipse
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x);
        float x = holder.position.x + (horizontalRadius * Mathf.Cos(angle));
        float y = holder.position.y + (verticalRadius * Mathf.Sin(angle));

        // Calculate rotation to look at mouse with X axis
        float rotationAngle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, 0, (rotationAngle * Mathf.Rad2Deg) - 90f);  // Subtract 90 degrees
    }


    private void OnDrawGizmosSelected()
    {
        if (holder != null)
        {
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
    }
}
