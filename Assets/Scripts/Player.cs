using UnityEngine;

public class Player : Entity
{
    public PlayerSO playerSO;
    public override EntitySO entitySO => playerSO;

    private PlayerStateManager stateManager;

    protected override void Awake()
    {
        base.Awake();
        stateManager = GetComponent<PlayerStateManager>();
    }
}