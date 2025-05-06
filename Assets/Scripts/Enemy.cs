using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Chaser
{
    public EnemySO EnemySO;
    public override ChaserSO ChaserSO => EnemySO;

    private EnemyStateManager stateManager;

    protected override void Awake()
    {
        base.Awake();
        stateManager = GetComponent<EnemyStateManager>();
    }
}
