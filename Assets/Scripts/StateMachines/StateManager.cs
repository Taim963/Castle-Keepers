using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> states = new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState> currentState;

    protected bool isTransitioningState = false;

    private void Start()
    {
        currentState.EnterState();
    }

    private void Update()
    {
        EState nextStateKey = currentState.GetNextState();

        if (nextStateKey.Equals(currentState.StateKey))
        {
            currentState.UpdateState();
        }
        else if (!isTransitioningState)
        {
            TransitionToState(nextStateKey);
        }
        
    }

    public void TransitionToState(EState stateKey)
    {
        isTransitioningState = true;
        currentState.ExitState();
        currentState = states[stateKey];
        currentState.EnterState();
        isTransitioningState = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        currentState.OnTriggerEnter2D(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        currentState.OnTriggerExit2D(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        currentState.OnTriggerStay2D(other);
    }
}