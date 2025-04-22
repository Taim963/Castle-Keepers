using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Sword : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int damage = 5;
    public float cooldown = 0.3f;
    private bool isFiring = false;

    public void TryFire()
    {
        if (!isFiring)
        {            
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        isFiring = true;
        while (Input.GetMouseButton(0))
        {
            
            yield return new WaitForSeconds(cooldown);
        }
        isFiring = false;
    }
}