using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;
    public bool hitScanWeapon = false;
    public bool burstWeapon = false;

    public int damage = 5;
    [ShowIf("burstWeapon")] public int bulletsPerBurst = 3;
    [ShowIf("burstWeapon")] public float burstCooldown = 0.1f;
    public float cooldown = 0.3f;
    public float randomSpread = 0.1f; // Random spread for projectile direction

    private bool isFiring = false;
    Quaternion spreadRotation;

    private void Awake()
    {
        Projectile projectile = projectilePrefab.GetComponent<Projectile>();
        projectile.baseWeaponDamage = damage;
        float randomAngle = Random.Range(-randomSpread, randomSpread);
        spreadRotation = Quaternion.Euler(0, 0, randomAngle);
    }

    public void TryFire()
    {
        if (!isFiring) 
        {
            if (!burstWeapon)
            {
                StartCoroutine(Fire());
            }
            else
            {
                StartCoroutine(BurstFire());
            }
            
        }
    }

    private IEnumerator Fire()
    {
        isFiring = true;
        while (Input.GetMouseButton(0))
        {
            Quaternion finalRotation = transform.rotation; // Default rotation

            // Only apply spread rotation if randomSpread is greater than 0
            if (randomSpread > 0)
            {
                float randomAngle = Random.Range(-randomSpread, randomSpread);
                Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
                finalRotation *= spreadRotation;
            }

            // Instantiate projectile with calculated rotation
            GameObject projectile = Instantiate(projectilePrefab, transform.position, finalRotation);
            yield return new WaitForSeconds(cooldown);
        }
        isFiring = false;
    }

    private IEnumerator BurstFire()
    {
        isFiring = true;
        while (Input.GetMouseButton(0))
        {
            for (int i = 0; i < bulletsPerBurst; i++)
            {
                Quaternion finalRotation = transform.rotation; // Default rotation

                // Only apply spread rotation if randomSpread is greater than 0
                if (randomSpread > 0)
                {
                    float randomAngle = Random.Range(-randomSpread, randomSpread);
                    Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
                    finalRotation *= spreadRotation;
                }

                // Instantiate projectile with calculated rotation
                GameObject projectile = Instantiate(projectilePrefab, transform.position, finalRotation);
                yield return new WaitForSeconds(burstCooldown);
            }
            yield return new WaitForSeconds(cooldown);
        }
        isFiring = false;
    }

}