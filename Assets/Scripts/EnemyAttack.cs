using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class EnemyAttack : MonoBehaviour
{
    public bool projectileAttack = false;
    public int damage = 5;
    [ShowIf("projectileAttack")] public float lifeTime = 0.5f;
    [ShowIf("projectileAttack")] public float speed = 15f;
    [ShowIf("projectileAttack")] public int pierce = 3;
    public LayerMask collisionMask;
    public GameObject hitEffect;

    [HideInInspector] public int damageSum;
    [HideInInspector] public int baseEnemyDamage;
    [HideInInspector] public bool hasHit = false;
    private HashSet<GameObject> hitPlayers = new HashSet<GameObject>();

    private void Start()
    {
        damageSum = baseEnemyDamage + damage;

        if (projectileAttack) Invoke("ProjectileDeath", lifeTime);
        else Invoke("ProjectileDeath", 0.5f);
    }

    private void Update()
    {
        if (projectileAttack) gameObject.transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, collisionMask) && !hitPlayers.Contains(other.gameObject))
        {
            if (pierce > 0  && projectileAttack)
            {
                pierce--;
                hitPlayers.Add(other.gameObject);
                CreatePierceEffect();
            }
            else
            {
                ProjectileDeath();
            }
        }

        if (other.CompareTag("Wall") || other.CompareTag("Castle"))
        {
            ProjectileDeath();
        }
    }

    private bool IsColliderInLayerMask(Collider2D collider, LayerMask layerMask)
    {
        // Use bitwise operations to check if the collider's layer is in the layerMask
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }

    private void ProjectileDeath()
    {
        GameObject hitInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        Destroy(hitInstance, 0.3f);
        Destroy(gameObject);
    }

    private void CreatePierceEffect()
    {
        GameObject hitInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        hitInstance.transform.localScale = Vector2.Scale(hitInstance.transform.localScale, new Vector2(0.5f, 0.5f));
        Destroy(hitInstance, 0.3f);
    }

    private void ChangeColorAndScale(GameObject hitInstance)
    {
        ParticleSystem.MainModule main = hitInstance.GetComponent<ParticleSystem>().main;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        main.startColor = spriteRenderer.color;
        hitInstance.transform.localScale = Vector2.Scale(spriteRenderer.transform.localScale, hitInstance.transform.localScale);
    }
}
