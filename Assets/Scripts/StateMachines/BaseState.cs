using UnityEngine;
using System;

public abstract class BaseState<EState> where EState : Enum
{
    public EState StateKey { get; private set; }
    public BaseState(EState key)
    {
        StateKey = key;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract EState GetNextState();
    public virtual void OnTriggerEnter2D(Collider2D other) { }
    public virtual void OnTriggerExit2D(Collider2D other) { }
    public virtual void OnTriggerStay2D(Collider2D other) { }
}
