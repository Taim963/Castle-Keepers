using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    public EnemySO enemySO;

    protected virtual void Start()
    {
        // register enemy in GameManager
        GameManager.instance.RegisterEnemy(gameObject);

        // Initialize health
        enemySO.health = enemySO.maxHealth;
        enemySO.healthBar.SetMaxHealth(enemySO.maxHealth);
    }
}
