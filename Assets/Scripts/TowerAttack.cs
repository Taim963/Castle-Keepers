using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class TowerAttack : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 5;
    public int pierce = 0;
    public float KnockbackForce = 1;
    public float lifetime = 2f;
    public GameObject hitEffectPrefab;
    public LayerMask collisionMask;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track enemies, not colliders


    void Start()
    {
        Invoke("ProjectileDeath", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        Launch();
    }

    private void Launch()
    {
        gameObject.transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, collisionMask) && !hitEnemies.Contains(other.gameObject))
        {
            if (pierce > 0)
            {
                pierce--;
                hitEnemies.Add(other.gameObject);
                CreatePierceEffect();
            }
            else
            {
                ProjectileDeath();
            }
        }

        if (other.CompareTag("Wall"))
        {
            ProjectileDeath();
        }
    }

    private void ProjectileDeath()
    {
        GameObject hitInstance = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        ChangeColorAndScale(hitInstance);
        Destroy(hitInstance, 0.3f);
        Destroy(gameObject);
    }

    private void CreatePierceEffect()
    {
        GameObject hitInstance = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
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

    private bool IsColliderInLayerMask(Collider2D collider, LayerMask layerMask)
    {
        // Use bitwise operations to check if the collider's layer is in the layerMask
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }
}
