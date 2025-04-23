using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening.Core.Easing;

public class Gun : Weapon
{
    public GunSO gunSO; // Reference to the GunSO scriptable object

    private bool isFiring = false;
    private Quaternion spreadRotation;
    private Bullet bullet;

    protected override void Awake()
    {
        bullet = gunSO.projectilePrefab.GetComponent<Bullet>();
        bullet.baseWeaponDamage = gunSO.damage;
        bullet.baseWeaponKnockback = gunSO.knockbackForce;
        float randomAngle = Random.Range(-gunSO.randomSpread, gunSO.randomSpread);
        spreadRotation = Quaternion.Euler(0, 0, randomAngle);
    }

    public override void TryFire()
    {
        if (!isFiring) 
        {
            if (bullet.bulletSO.bulletType == BulletType.Projectile) HandleProjectileShots();

            if (bullet.bulletSO.bulletType == BulletType.HitScan) HandleHitScanShots();
        }
    }

    private IEnumerator HandleHitScanShots()
    {
        isFiring = true;
        while (Input.GetMouseButton(1)) // Right mouse button for hitscan
        {
            Vector2 fireDirection = transform.right; // Forward direction of the gun
            float range = bullet.bulletSO.range; // Use range from BulletSO

            RaycastHit2D hit = Physics2D.Raycast(transform.position, fireDirection, range, bullet.bulletSO.hurtMask);
            Vector2 hitPoint = hit.collider != null ? hit.point : (Vector2)transform.position + fireDirection * range;

            StartCoroutine(DrawTracer(transform.position, hitPoint));

            if (hit.collider != null)
            {
                bullet.AlertGameManagerOnHit(hit.collider);
            }

            yield return new WaitForSeconds(gunSO.cooldown); // Same cooldown as projectile firing
        }
        isFiring = false;
    }



    // Tracer Coroutine (Fades away after a short time)
    private IEnumerator DrawTracer(Vector2 start, Vector2 end)
    {
        GameObject tracer = new GameObject("Tracer");
        LineRenderer lineRenderer = tracer.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Optional: Add color gradient (looks cooler)
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0f), new GradientColorKey(Color.red, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        lineRenderer.colorGradient = gradient;

        yield return new WaitForSeconds(0.1f); // Short visibility time before disappearing
        Destroy(tracer);
    }


    private void HandleProjectileShots()
    {
        if (gunSO.weaponType == WeaponType.SingleShot)
        {
            StartCoroutine(Fire());
        }
        else
        {
            StartCoroutine(BurstFire());
        }
    }

    private IEnumerator Fire()
    {
        isFiring = true;
        while (Input.GetMouseButton(0))
        {
            Quaternion finalRotation = transform.rotation; // Default rotation

            // Only apply spread rotation if randomSpread is greater than 0
            if (gunSO.randomSpread > 0)
            {
                float randomAngle = Random.Range(-gunSO.randomSpread, gunSO.randomSpread);
                Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
                finalRotation *= spreadRotation;
            }

            // Instantiate projectile with calculated rotation
            GameObject projectile = Instantiate(gunSO.projectilePrefab, transform.position, finalRotation);
            yield return new WaitForSeconds(gunSO.cooldown);
        }
        isFiring = false;
    }

    private IEnumerator BurstFire()
    {
        isFiring = true;
        while (Input.GetMouseButton(0))
        {
            for (int i = 0; i < gunSO.bulletsPerBurst; i++)
            {
                Quaternion finalRotation = transform.rotation; // Default rotation

                // Only apply spread rotation if randomSpread is greater than 0
                if (gunSO.randomSpread > 0)
                {
                    float randomAngle = Random.Range(-gunSO.randomSpread, gunSO.randomSpread);
                    Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
                    finalRotation *= spreadRotation;
                }

                // Instantiate projectile with calculated rotation
                GameObject projectile = Instantiate(gunSO.projectilePrefab, transform.position, finalRotation);
                yield return new WaitForSeconds(gunSO.burstCooldown);
            }
            yield return new WaitForSeconds(gunSO.cooldown);
        }
        isFiring = false;
    }
}