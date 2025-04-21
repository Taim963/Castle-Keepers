using UnityEngine;
using DG.Tweening;

public class CameraBehavior : MonoBehaviour
{
    private Camera Camera;
    public Transform Target;
    public Vector2 Offset;

    // Define the boundaries for the camera
    public Vector2 MinBounds; // Bottom-left corner of the bounding box
    public Vector2 MaxBounds; // Top-right corner of the bounding box

    private void Start()
    {
        Camera = Camera.main;
    }

    private void Update()
    {
        if (Target != null)
        {
            // Calculate the target position
            Vector3 targetPosition = (Vector3)(Target.position) + new Vector3(Offset.x, Offset.y, 0);
            targetPosition.z = Camera.transform.position.z; // Keep the camera's Z position constant

            // Clamp the position to ensure the camera stays within the bounds
            targetPosition = ClampCameraPosition(targetPosition);

            // Tween the camera to the target position
            Camera.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    private Vector3 ClampCameraPosition(Vector3 position)
    {
        // Calculate the camera's half-width and half-height based on its orthographic size
        float cameraHalfHeight = Camera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * Camera.aspect;

        // Clamp the camera's position so its edges stay within the bounds
        position.x = Mathf.Clamp(position.x, MinBounds.x + cameraHalfWidth, MaxBounds.x - cameraHalfWidth);
        position.y = Mathf.Clamp(position.y, MinBounds.y + cameraHalfHeight, MaxBounds.y - cameraHalfHeight);

        return position;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the bounding box in the scene view
        Gizmos.color = Color.red;

        // Define the four corners of the bounding box
        Vector3 bottomLeft = new Vector3(MinBounds.x, MinBounds.y, 0);
        Vector3 bottomRight = new Vector3(MaxBounds.x, MinBounds.y, 0);
        Vector3 topLeft = new Vector3(MinBounds.x, MaxBounds.y, 0);
        Vector3 topRight = new Vector3(MaxBounds.x, MaxBounds.y, 0);

        // Draw the lines to form a box
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }

}
