using UnityEngine;
using System.Collections.Generic;

public class Slash : Hurt
{
    public SlashSO slashSO; // Reference to the SlashSO scriptable object   

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track enemies, not colliders

    [HideInInspector] public int baseWeaponDamage;
    [HideInInspector] public float baseWeaponKnockbackForce;
    [HideInInspector] public Vector2 baseWeaponKnockbackDirection;

    [HideInInspector] public GameManager gameManager; // Reference to the GameManager

    protected override void Start()
    {
        gameManager = GameManager.instance; // Get the GameManager instance

        Invoke("ProjectileDeath", slashSO.lifetime);
    }

    protected override void Update()
    {
        if (slashSO.slashType == SlashType.Moving)
        {
            Launch();
        }
    }

    private void Launch()
    {
        gameObject.transform.Translate(Vector2.right * slashSO.speed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, slashSO.hurtMask))
        {
            HandleEnemyCollision(other);
        }
    }

    private void HandleEnemyCollision(Collider2D other)
    {
        if (hitEnemies.Contains(other.gameObject)) return;
  
        hitEnemies.Add(other.gameObject);
        CreatePierceEffect();

        AlertGameManagerOnHit(other);
    }

    public void AlertGameManagerOnHit(Collider2D other)
    {
        if (IsColliderInLayerMask(other, slashSO.hurtMask))
        {
            gameManager.onProjectileHit.Invoke(baseWeaponDamage, baseWeaponKnockbackForce, other.gameObject, gameObject); // Notify GameManager about the hit
        }
    }

    private void ProjectileDeath()
    {
        GameObject hitInstance = Instantiate(slashSO.hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        Destroy(hitInstance, 0.3f);
        Destroy(gameObject);
    }

    private void ChangeColorAndScale(GameObject hitInstance)
    {
        ParticleSystem.MainModule main = hitInstance.GetComponent<ParticleSystem>().main;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        main.startColor = spriteRenderer.color;
        hitInstance.transform.localScale = Vector2.Scale(spriteRenderer.transform.localScale, hitInstance.transform.localScale);
    }

    private void CreatePierceEffect()
    {
        GameObject hitInstance = Instantiate(slashSO.hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        hitInstance.transform.localScale = Vector2.Scale(hitInstance.transform.localScale, new Vector2(0.5f, 0.5f));
        Destroy(hitInstance, 0.3f);
    }

    private bool IsColliderInLayerMask(Collider2D collider, LayerMask layerMask)
    {
        // Use bitwise operations to check if the collider's layer is in the layerMask
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }
}
