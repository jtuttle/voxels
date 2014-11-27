using System;
using System.Collections.Generic;

public abstract class FSMState {
    //public delegate void StateExitDelegate(Enum nextStateId);
    //public event StateExitDelegate OnStateExit = delegate { };

    public Enum StateId { get; private set; }
    public FSMTransition NextStateTransition { get; private set; }

    public FSMState(Enum stateId) {
        StateId = stateId;
    }

    public virtual void InitState(FSMTransition transition) { }

    public virtual void EnterState(FSMTransition transition) {
        NextStateTransition = null;
    }

    public virtual void ExitState(FSMTransition nextStateTransition) {
        //OnStateExit(nextStateId);

        if(nextStateTransition == null)
            throw new Exception("State transition can not be null.");

        NextStateTransition = nextStateTransition;
    }

    public virtual void Update() { }
    public virtual void OnGUI() { }
    public virtual void Dispose() { }
}
