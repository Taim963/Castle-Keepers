using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : StateManager<PlayerStateManager.PlayerState>
{
    [HideInInspector] public Player PlayerScript { get; private set; }
    [HideInInspector] public Rigidbody2D Rb { get; private set; }
    [HideInInspector] public PlayerSO PlayerSO { get; private set; }
    [HideInInspector] public GameObject Player { get; private set; }

    private void Awake()
    {
        // GetComponent calls—make sure these components exist on the GameObject!
        PlayerScript = GetComponent<Player>();
        if (PlayerScript == null)
        {
            Debug.LogError("Player component not found on the GameObject");
        }
        else
        {
            PlayerSO = PlayerScript.playerSO;
        }

        Rb = GetComponent<Rigidbody2D>();
        if (Rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the GameObject");
        }
        Player = gameObject;

        // Initialize the state machine with plain C# state objects
        states = new Dictionary<PlayerState, BaseState<PlayerState>>
        {
            { PlayerState.Idle, new PlayerIdleState(PlayerState.Idle, this) },
            { PlayerState.Move, new PlayerMoveState(PlayerState.Move, this) },
            { PlayerState.Dash, new PlayerDashState(PlayerState.Dash, this) }
            // Add other states as needed.
        };

        // Default state
        currentState = states[PlayerState.Idle];
    }

    public enum PlayerState
    {
        Idle,
        Move,
        Attack,
        Dash,
        Hit,
        Dead,
    }
}
