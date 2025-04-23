using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SlashSO", menuName = "Scriptable Objects/SlashSO")]
public class SlashSO : ScriptableObject
{
    #region // Slash type
    [Header("Slash type")]
    public SlashType slashType; // Drop down menu for bullet type
    #endregion

    #region // Slash stats
    [Header("Slash stats")]
    [ShowIf("slashType", SlashType.Moving)] public float speed = 10f;
    public float lifetime = 2f;
    public GameObject hitEffectPrefab;
    public LayerMask hurtMask;
    #endregion
}

public enum SlashType
{
    Moving,
    Stationary
}