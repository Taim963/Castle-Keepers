using UnityEngine;

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
