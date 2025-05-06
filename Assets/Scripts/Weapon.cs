using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public bool isFiring = false;
    protected virtual void Awake() { }

    protected virtual void Update() { }

    public virtual void TryFire() { }
}
