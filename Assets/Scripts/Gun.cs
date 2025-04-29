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
    private Entity Holder; // Reference to the entity holding the gun

    #region // Item rotation
    public Transform center; // The center point around which the item rotates
    public bool followMouse = true; // makes the Item rotate according to the mouse position, mainly for ranged weapons
    public bool followMouseAroundTarget = true; // makes the Item rotate around the center according to the mouse position, mainly for ranged weapons
    public float horizontalRadius = 5f;
    public float verticalRadius = 3f;
    #endregion

    protected override void Awake()
    {
        // Apply initial random spread.
        float randomAngle = Random.Range(-gunSO.randomSpread, gunSO.randomSpread);
        spreadRotation = Quaternion.Euler(0, 0, randomAngle);

        // Determine how many line renderers we need.
        int numRenderers = gunSO.bulletFireType == bulletFireType.Burst ? gunSO.bulletsPerBurst * 2: 1;
        lineRenderers = new LineRenderer[numRenderers];

        // Instantiate line renderers from the prefab defined in BulletSO.
        for (int i = 0; i < numRenderers; i++)
        {
            // Instantiate the prefab as a child of this gun.
            GameObject lineObj = Instantiate(gunSO.LineRendererPrefab, transform);
            lineObj.name = "LineRenderer_" + i;

            // We assume the prefab already has a configured LineRenderer.
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.enabled = false;
            lineRenderers[i] = lr;
        }

        Holder = GetComponentInParent<Entity>();
    }

    protected override void Update()
    {
        if (followMouse) FollowMouseRotation();
        if (followMouseAroundTarget) RotateAround();
    }

    #region // Firing logic
    public override void TryFire()
    {
        if (!isFiring)
        {
            if (gunSO.bulletFireType == bulletFireType.SingleShot)
                StartCoroutine(SingleShotFire());
            else if (gunSO.bulletFireType == bulletFireType.Burst)
                StartCoroutine(BurstFire());
        }
    }

    
    private IEnumerator SingleShotFire()
    {
        isFiring = true;
        while (Input.GetMouseButton(0))
        {
            yield return new WaitForSeconds(gunSO.preFireCooldown);
            Quaternion finalRotation = transform.rotation;
            if (gunSO.randomSpread > 0)
            {
                GetRandomBulletSpread(ref finalRotation);
            }

            if (gunSO.bulletType == BulletType.Projectile)
                InstantiateProjectile(finalRotation);
            else
                InstantiateHitScan(finalRotation);

            Vector2 selfKnockbackDirection = finalRotation * Vector2.right;
            Holder.TakeKnockback(gunSO.selfKnockbackForce, -selfKnockbackDirection);
            yield return new WaitForSeconds(gunSO.cooldown);
        }
        isFiring = false;
    }

    private IEnumerator BurstFire()
    {
        isFiring = true;

        Quaternion finalRotation = transform.rotation;
        while (Input.GetMouseButton(0))
        {
            yield return new WaitForSeconds(gunSO.preFireCooldown);
            for (int i = 0; i < gunSO.bulletsPerBurst; i++)
            {
                if (gunSO.randomSpread > 0)
                {
                    GetRandomBulletSpread(ref finalRotation);
                }

                if (gunSO.bulletType == BulletType.Projectile)
                    InstantiateProjectile(finalRotation);
                else
                    InstantiateHitScan(finalRotation);

                // Only wait if burstCooldown is greater than 0
                if (gunSO.burstCooldown > 0)
                {
                    Vector2 selfKnockbackDirection = finalRotation * Vector2.right;
                    Holder.TakeKnockback(gunSO.selfKnockbackForce, -selfKnockbackDirection);

                    yield return new WaitForSeconds(gunSO.burstCooldown);
                }
            }

            if (gunSO.burstCooldown <= 0)
            {
                Vector2 selfKnockbackDirection = finalRotation * Vector2.right;
                Holder.TakeKnockback(gunSO.selfKnockbackForce, -selfKnockbackDirection);
            }

            yield return new WaitForSeconds(gunSO.cooldown);
        }
        isFiring = false;
    }


    private void InstantiateProjectile(Quaternion finalRotation)
    {
        // Simply instantiate the projectile prefab.
        Instantiate(gunSO.projectilePrefab, transform.position, finalRotation);
        GameObject bulletObj = Instantiate(gunSO.projectilePrefab, transform.position, finalRotation);
        if (bulletObj.TryGetComponent<Bullet>(out var bulletComponent))
        {
            bulletComponent.gunSO = gunSO;
            // Add any other bullet setup here
        }
    }

    private void InstantiateHitScan(Quaternion finalRotation)
    {
        Vector2 direction = finalRotation * Vector2.right;

        // Combine collide and hurt layer masks.
        LayerMask combinedMask = gunSO.collideMask | gunSO.hurtMask;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, gunSO.range, combinedMask);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // Default endpoint is at maximum range.
        Vector2 endPoint = (Vector2)transform.position + direction * gunSO.range;
        int hitCount = 0;

        foreach (RaycastHit2D hit in hits)
        {
            bool isCollidable = (gunSO.collideMask.value & (1 << hit.collider.gameObject.layer)) != 0;
            bool canBeHurt = (gunSO.hurtMask.value & (1 << hit.collider.gameObject.layer)) != 0;

            if (canBeHurt)
            {
                hitCount++;
                Debug.Log($"Hit hurtable object: {hit.collider.name}");

                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.OnHurtCollide(gunSO.damage, gunSO.knockbackForce, hit.collider.gameObject, gameObject);

                // If pierce is 0, then we should hit only one target.
                if (hitCount > gunSO.pierce + gunSO.bulletPierce)
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
    #endregion

    #region // Weapon rotation logic
    private void FollowMouseRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 direction = worldPos - transform.position;

        // Calculate angle and rotate around Z-axis for 2D
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }


    private void RotateAround()
    {
        if (center != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 direction = worldPos - center.position;

            float angle = Mathf.Atan2(direction.y, direction.x);
            float x = Mathf.Cos(angle) * horizontalRadius;
            float y = Mathf.Sin(angle) * verticalRadius;

            Vector3 offset = new Vector3(x, y, 0);
            transform.position = center.position + offset;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (center != null)
        {
            Gizmos.color = Color.green;
            const int segments = 50;
            Vector3[] points = new Vector3[segments + 1];

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2 * Mathf.PI / segments;
                float x = Mathf.Cos(angle) * horizontalRadius;
                float y = Mathf.Sin(angle) * verticalRadius;
                points[i] = center.position + new Vector3(x, y, 0);
            }

            for (int i = 0; i < segments; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
    }
    #endregion
}
