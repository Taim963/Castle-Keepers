using UnityEngine;
using System.Collections;

public class RotateAroundTarget : MonoBehaviour
{
    public Transform center;
    public bool followMouse = true;
    public bool followMouseAroundTarget = true;
    public float horizontalRadius = 5f;
    public float verticalRadius = 3f;

    private void Update()
    {
        if (followMouse) FollowMouseRotation();
        if (followMouseAroundTarget) RotateAround();
    }

    public void OnClickRotateTarget(float timeRotated)
    {
        StartCoroutine(RotateTargetCoroutine(timeRotated));
    }

    private IEnumerator RotateTargetCoroutine(float timeRotated)
    {
        // Store original position
        Vector3 originalPosition = transform.position;
        float elapsedTime = 0f;

        // Calculate starting angle based on current position
        Vector2 currentOffset = transform.position - center.position;
        float startAngle = Mathf.Atan2(currentOffset.y / verticalRadius, currentOffset.x / horizontalRadius);

        // Temporarily disable mouse following
        bool wasFollowingMouse = followMouseAroundTarget;
        followMouseAroundTarget = false;

        // Make one full rotation in the specified time
        while (elapsedTime < timeRotated)
        {
            elapsedTime += Time.deltaTime;
            float completionRatio = elapsedTime / timeRotated;

            // Calculate current angle (is one full rotation)
            float currentAngle = startAngle + (Mathf.PI * 2 * completionRatio);

            // Calculate position on ellipse
            float x = Mathf.Cos(currentAngle) * horizontalRadius;
            float y = Mathf.Sin(currentAngle) * verticalRadius;

            // Update position
            transform.position = center.position + new Vector3(x, y, 0);

            // If we're also following mouse rotation, update the rotation
            if (followMouse)
            {
                Vector3 direction = transform.position - center.position;
                float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle));
            }

            yield return null;
        }

        // Return to original position with a small lerp
        float returnTime = 0.2f; // Time to return to original position
        float returnTimer = 0f;
        Vector3 currentPos = transform.position;

        while (returnTimer < returnTime)
        {
            returnTimer += Time.deltaTime;
            float t = returnTimer / returnTime;

            // Smooth lerp back to original position
            transform.position = Vector3.Lerp(currentPos, originalPosition, t);

            yield return null;
        }

        // Ensure we're exactly at the original position
        transform.position = originalPosition;

        // Restore mouse following if it was enabled
        followMouseAroundTarget = wasFollowingMouse;
    }


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
}
