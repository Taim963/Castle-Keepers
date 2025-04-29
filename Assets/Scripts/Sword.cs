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

    // Cache the animation clip and actual duration
    private AnimationClip slashAnimation;
    private float slashDuration;

    protected override void Awake()
    {
        // Cache components
        gameManager = GameManager.instance;
        swordCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        // Cache animation data
        SetupAnimation();

        // Apply initial random spread.
        float randomAngle = Random.Range(-swordSO.randomSpread, swordSO.randomSpread);
        spreadRotation = Quaternion.Euler(0, 0, randomAngle);

        // Determine how many line renderers we need.
        int numRenderers = swordSO.bulletFireType == SwordBulletFireType.Burst ? swordSO.bulletsPerBurst * 2 : 1;
        lineRenderers = new LineRenderer[numRenderers];

        // Instantiate line renderers from the prefab defined in BulletSO.
        for (int i = 0; i < numRenderers; i++)
        {
            // Instantiate the prefab as a child of this gun.
            GameObject lineObj = Instantiate(gunSO.LineRendererPrefab, transform);
            lineObj.name = "LineRenderer_" + i;

            // We assume the prefab already has a configured LineRenderer.
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.enabled = false;
            lineRenderers[i] = lr;
        }

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

        while (Input.GetMouseButton(0))
        {
            if (swordSO.preFireCooldown > 0)
            {
                yield return preFireWait;
            }

            animator.SetTrigger("Swing");
            swordCollider.enabled = true;
            yield return slashWait;
            swordCollider.enabled = false;

            hitEnemies.Clear(); // Clear hit enemies after each swing
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
}
