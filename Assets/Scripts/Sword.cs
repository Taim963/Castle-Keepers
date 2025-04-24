using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening.Core.Easing;
using UnityEngine.Events;
using System.Linq;

public class Sword : Weapon
{
    public SwordSO swordSO; // Reference to the SwordSO scriptable object
    public Animator animator; // Reference to the Animator component
    
    private bool isSwinging = false;
    private Collider2D swordCollider;
    private GameManager gameManager; // Reference to the GameManager

    protected override void Awake()
    {
        gameManager = GameManager.instance;
        swordCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        // Get the "Sword Slash" animation clip
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        AnimationClip slashClip = clips.FirstOrDefault(clip => clip.name == "Sword Slash");

        if (slashClip != null)
        {
            // Calculate the speed multiplier needed to match the desired duration
            float speedMultiplier = slashClip.length / swordSO.swingSpeed;
            animator.speed = speedMultiplier;
        }
    }


    protected override void Update()
    {

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
        while (Input.GetMouseButton(0))
        {
            gameManager.onSwordSwing.Invoke(swordSO.cooldown); // Notify GameManager about the sword swing
            animator.SetTrigger("Swing");
            swordCollider.enabled = true; // Enable the sword collider
            yield return new WaitForSeconds(swordSO.cooldown);
            swordCollider.enabled = false; // Disable the sword collider
        }
        isSwinging = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, swordSO.hurtMask))
        {
            AlertGameManagerOnHit(other);
        }
    }

    public void AlertGameManagerOnHit(Collider2D other)
    {
        if (IsColliderInLayerMask(other, swordSO.hurtMask))
        {
            gameManager.onProjectileHit.Invoke(swordSO.damage, swordSO.knockbackForce, other.gameObject, gameObject); // Notify GameManager about the hit
        }
    }

    private bool IsColliderInLayerMask(Collider2D collider, LayerMask layerMask)
    {
        // Use bitwise operations to check if the collider's layer is in the layerMask
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }
}