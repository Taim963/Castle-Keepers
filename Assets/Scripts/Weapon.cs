using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int damage = 5;
    public float cooldown = 0.3f;

    private bool isFiring = false;


    private void Awake()
    {
        Projectile projectile = projectilePrefab.GetComponent<Projectile>();
        projectile.baseWeaponDamage = damage;
    }

    public void TryFire()
    {
        if (!isFiring) 
        {
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        isFiring = true; 
        while (Input.GetMouseButton(0)) 
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, gameObject.transform.rotation);
            yield return new WaitForSeconds(cooldown);
        }
        isFiring = false; 
    }
}