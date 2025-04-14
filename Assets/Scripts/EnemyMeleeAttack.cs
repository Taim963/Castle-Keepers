using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class EnemyMeleeAttack : MonoBehaviour
{
    public int damage = 5;
    public GameObject hitEffect;
    public LayerMask collisionMask;

    [HideInInspector] public int damageSum;
    [HideInInspector] public int baseEnemyDamage;
    [HideInInspector] public bool hasHit = false;
    private HashSet<GameObject> hitPlayers = new HashSet<GameObject>();
    [HideInInspector] public EnemyMeleeAttack script;

    private void Start()
    {
        script = GetComponent<EnemyMeleeAttack>();

        damageSum = baseEnemyDamage + damage;

        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, collisionMask) && !hitPlayers.Contains(other.gameObject))
        {
            hitPlayers.Add(other.gameObject);
        }
    }

    private bool IsColliderInLayerMask(Collider2D collider, LayerMask layerMask)
    {
        // Use bitwise operations to check if the collider's layer is in the layerMask
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }
}
