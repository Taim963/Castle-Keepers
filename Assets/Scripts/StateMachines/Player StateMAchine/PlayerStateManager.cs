using UnityEngine;
using System.Collections.Generic;

public class PlayerStateManager : StateManager<PlayerStateManager.PlayerState>
{
    #region // Variables
    [HideInInspector] public Player PlayerScript { get; private set; }
    [HideInInspector] public Rigidbody2D Rb { get; private set; }
    [HideInInspector] public PlayerSO PlayerSO { get; private set; }
    [HideInInspector] public GameObject Player { get; private set; }
    #endregion

    private void Awake()
    {
        #region // Get references
        PlayerScript = GetComponent<Player>();
        PlayerSO = PlayerScript.playerSO;
        Rb = GetComponent<Rigidbody2D>();
        Player = gameObject;
        #endregion

        #region // Initialize the state machine and send context
        states = new Dictionary<PlayerState, BaseState<PlayerState>>
        {
            { PlayerState.Idle, new PlayerIdleState(PlayerState.Idle, this, gameObject) },
            { PlayerState.Move, new PlayerMoveState(PlayerState.Move, this, gameObject) }
        };
        #endregion

        // Default state
        currentState = states[PlayerState.Idle];
    }

    public enum PlayerState
    {
        Idle,
        Move,
        Attack,
        Dash,
        hit,
        Dead,
    }
}