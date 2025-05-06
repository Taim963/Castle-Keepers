using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    public EnemySO EnemySO;
    public override EntitySO entitySO => EnemySO;

    private EnemyStateManager stateManager;
}
