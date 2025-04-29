using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletSO", menuName = "Scriptable Objects/BulletSO")]
public class BulletSO : ScriptableObject
{
    public int pierce { get; set; }
    public float speed { get; set; }
    public float lifetime { get; set; }
    public float range { get; set; }


    public GameObject LineRendererPrefab;
}
