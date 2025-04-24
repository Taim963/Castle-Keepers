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

    // We’ll use an array of LineRenderers so that burst fire can show multiple tracers at once.
    private LineRenderer[] lineRenderers;
    private int currentLineIndex = 0;

    protected override void Awake()
    {
        // Get the bullet component from the projectile prefab.
        bullet = gunSO.projectilePrefab.GetComponent<Bullet>();
        bullet.baseWeaponDamage = gunSO.damage;
        bullet.baseWeaponKnockback = gunSO.knockbackForce;

        // Apply initial random spread.
        float randomAngle = Random.Range(-gunSO.randomSpread, gunSO.randomSpread);
        spreadRotation = Quaternion.Euler(0, 0, randomAngle);

        // Determine how many line renderers we need.
        int numRenderers = gunSO.weaponType == WeaponType.Burst ? gunSO.bulletsPerBurst : 1;
        lineRenderers = new LineRenderer[numRenderers];

        // Instantiate line renderers from the prefab defined in BulletSO.
        for (int i = 0; i < numRenderers; i++)
        {
            // Instantiate the prefab as a child of this gun.
            GameObject lineObj = Instantiate(bullet.bulletSO.LineRendererPrefab, transform);
            lineObj.name = "LineRenderer_" + i;

            // We assume the prefab already has a configured LineRenderer.
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.enabled = false;
            lineRenderers[i] = lr;
        }
    }

    public override void TryFire()
    {
        if (!isFiring)
        {
            if (gunSO.weaponType == WeaponType.SingleShot)
                StartCoroutine(SingleShotFire());
            else if (gunSO.weaponType == WeaponType.Burst)
                StartCoroutine(BurstFire());
        }
    }

    private IEnumerator SingleShotFire()
    {
        isFiring = true;
        while (Input.GetMouseButton(0))
        {
            Quaternion finalRotation = transform.rotation;
            if (gunSO.randomSpread > 0)
            {
                GetRandomBulletSpread(ref finalRotation);
            }

            if (bullet.bulletSO.bulletType == BulletType.Projectile)
                InstantiateProjectile(finalRotation);
            else
                InstantiateHitScan(finalRotation);

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
                Quaternion finalRotation = transform.rotation;
                if (gunSO.randomSpread > 0)
                {
                    GetRandomBulletSpread(ref finalRotation);
                }

                if (bullet.bulletSO.bulletType == BulletType.Projectile)
                    InstantiateProjectile(finalRotation);
                else
                    InstantiateHitScan(finalRotation);

                yield return new WaitForSeconds(gunSO.burstCooldown);
            }
            yield return new WaitForSeconds(gunSO.cooldown);
        }
        isFiring = false;
    }

    private void InstantiateProjectile(Quaternion finalRotation)
    {
        // Simply instantiate the projectile prefab.
        Instantiate(gunSO.projectilePrefab, transform.position, finalRotation);
    }

    private void InstantiateHitScan(Quaternion finalRotation)
    {
        Vector2 direction = finalRotation * Vector2.right;

        // Combine collide and hurt layer masks.
        LayerMask combinedMask = bullet.bulletSO.collideMask | bullet.bulletSO.hurtMask;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, bullet.bulletSO.range, combinedMask);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // Default endpoint is at maximum range.
        Vector2 endPoint = (Vector2)transform.position + direction * bullet.bulletSO.range;
        int hitCount = 0;

        foreach (RaycastHit2D hit in hits)
        {
            bool isCollidable = (bullet.bulletSO.collideMask.value & (1 << hit.collider.gameObject.layer)) != 0;
            bool canBeHurt = (bullet.bulletSO.hurtMask.value & (1 << hit.collider.gameObject.layer)) != 0;

            if (canBeHurt)
            {
                hitCount++;
                Debug.Log($"Hit hurtable object: {hit.collider.name}");

                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.OnProjectileCollide(gunSO.damage, gunSO.knockbackForce, hit.collider.gameObject, gameObject);

                // If pierce is 0, then we should hit only one target.
                if (hitCount > bullet.bulletSO.pierce)
                {
                    endPoint = hit.point;
                    break;
                }
            }
            else if (isCollidable)
            {
                endPoint = hit.point;
                Debug.Log($"Hit collidable object: {hit.collider.name}");
                break;
            }
        }

        // Use the next available LineRenderer.
        LineRenderer lr = lineRenderers[currentLineIndex];
        currentLineIndex = (currentLineIndex + 1) % lineRenderers.Length;
        StartCoroutine(DrawLine(transform.position, endPoint, lr));
    }

    private IEnumerator DrawLine(Vector2 startPoint, Vector2 endPoint, LineRenderer lr)
    {
        // Adjust these values to change how the tracer looks.
        float tracerLength = 1f;    // How long the tracer "tail" is.
        float tracerSpeed = 200f;   // How fast the tracer moves (units per second).

        lr.enabled = true;
        Vector2 direction = (endPoint - startPoint).normalized;
        float totalDistance = Vector2.Distance(startPoint, endPoint);
        float distanceTraveled = 0f;

        while (distanceTraveled < totalDistance)
        {
            distanceTraveled += tracerSpeed * Time.deltaTime;
            distanceTraveled = Mathf.Min(distanceTraveled, totalDistance);

            Vector2 currentPosition = startPoint + (direction * distanceTraveled);
            Vector2 tracerStart = currentPosition - (direction * tracerLength);

            lr.SetPosition(0, tracerStart);
            lr.SetPosition(1, currentPosition);

            yield return null;
        }
        lr.enabled = false;
    }

    private void GetRandomBulletSpread(ref Quaternion finalRotation)
    {
        float randomAngle = Random.Range(-gunSO.randomSpread, gunSO.randomSpread);
        Quaternion spreadRot = Quaternion.Euler(0, 0, randomAngle);
        finalRotation *= spreadRot;
    }
}
