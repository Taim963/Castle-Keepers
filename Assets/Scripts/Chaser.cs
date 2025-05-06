using UnityEngine;

public class Chaser : Entity
{
    public ChaserSO ChaserSO;
    public override EntitySO entitySO => ChaserSO;
    private EnemyStateManager stateManager;
    protected override void Awake()
    {
        base.Awake();
    }
}
