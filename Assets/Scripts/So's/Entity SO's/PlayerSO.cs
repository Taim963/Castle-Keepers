using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : EntitySO
{
    public float accelerationRate = 50f; // Acceleration speed
    public float decelerationRate = 50f; // Deceleration speed
    public float dashForce = 10f; // Force applied during a dash
    public float dashDuration = 0.2f; // Duration of the dash
    public float dashCooldown = 1f; // Cooldown time for dashing
}
