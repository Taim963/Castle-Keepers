using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : EnemyBase
{
    // Flags
    private bool isFirstAttack = true;

    private void Update()
    {
        if (IsReadyToAttack())
        {
            navMeshHandler.StopChasing();
            isAttacking = true;
            StartCoroutine(Attack(attackPrefab[0]));
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    private IEnumerator Attack(GameObject attackPrefab)
    {
        while (Vector2.Distance(targetTransform.position, enemySO.currentTarget) <= enemySO.attackRange && CanSeeTarget())
        {
            CancelInvoke("ResetFirstAttack");

            if (isFirstAttack) yield return new WaitForSeconds(1);
            isFirstAttack = false;

            PerformAttack(attackPrefab);
            yield return new WaitForSeconds(enemySO.attackCooldown);
        }

        Invoke("ResetFirstAttack", 0.5f);
        isAttacking = false;
        navMeshHandler.Chase();
    }

    private void ResetFirstAttack()
    {
        isFirstAttack = true;
    }

    private void PerformAttack(GameObject attackPrefab)
    {
        EnemyAttack attack = attackPrefab.GetComponent<EnemyAttack>();
        attack.baseEnemyDamage = enemySO.damage;



        // Calculate direction and offset for the attack spawn position
        Vector2 direction = enemySO.currentTarget - (Vector2)transform.position;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 offset = new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * enemySO.attackOffset;

        // Calculate rotation angle so that the attack faces the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (!attack.projectileAttack)
        {
            // Perform a melee attack instantiation
            RaycastHit2D hit = Physics2D.Raycast(transform.position, normalizedDirection, enemySO.attackRange, enemySO.rayCastCollide);
            Vector3 spawnPosition = hit.point;
            Instantiate(attackPrefab, spawnPosition, Quaternion.Euler(0, 0, angle));
        }
        else
        {
            // Perform a projectile attack instantiation
            Instantiate(attackPrefab, transform.position + offset, Quaternion.Euler(0, 0, angle));
        }
    }

    protected override bool IsReadyToAttack()
    {
        return base.IsReadyToAttack();
    }
}