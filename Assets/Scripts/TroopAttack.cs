using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class TroopAttack : MonoBehaviour
{
    public int damage = 20;
    public float knockbackForce = 2f;
    public GameObject hitEffect;
    public LayerMask collisionMask;

    private List<GameObject> hitEnemies = new List<GameObject>();
    [HideInInspector] public bool hasHit = false;

    private void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsColliderInLayerMask(other, collisionMask) && !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);
        }
    }

    private bool IsColliderInLayerMask(Collider2D collider, LayerMask layerMask)
    {
        // Use bitwise operations to check if the collider's layer is in the layerMask
        return ((1 << collider.gameObject.layer) & layerMask) != 0;
    }
}
