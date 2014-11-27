using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FiniteStateMachine {
    public FSMState CurrentState { get; private set; }

    private List<FSMState> _states;
    private Stack<FSMState> _stateStack;

    public FiniteStateMachine() {
        _states = new List<FSMState>();
        _stateStack = new Stack<FSMState>();
    }

    public void AddState(FSMState state) {
        if(HasState(state.StateId))
            throw new Exception("State " + state.StateId.ToString() + " is already defined.");

        _states.Add(state);
    }

    public void RemoveState(Enum stateId) {
        FSMState state = GetState(stateId);

        if(state == null)
            throw new Exception("Could not find state " + stateId.ToString() + " for removal.");

        _states.Remove(state);
    }

    public bool HasState(Enum stateId) {
        return _states.Any(s => s.StateId == stateId);
    }

    public FSMState GetState(Enum stateId) {
        return _states.Find(s => s.StateId.ToString() == stateId.ToString());
    }

    public void ChangeState(FSMTransition stateTransition) {
        Enum nextStateId = stateTransition.NextStateId;

        FSMState prevState = CurrentState;

        if(nextStateId == null) {
            CurrentState = _stateStack.Pop();
        } else {
            FSMState nextState = GetState(nextStateId);

            if(nextState == null)
                throw new Exception("State " + nextStateId.ToString() + " has not been defined.");

            if(stateTransition.PushCurrentState)
                _stateStack.Push(CurrentState);

            CurrentState = nextState;

            CurrentState.InitState(stateTransition);
        }

        Debug.Log("Changed state to: " + CurrentState.StateId.ToString());

        CurrentState.EnterState(stateTransition);

        if(prevState != null && !stateTransition.PushCurrentState)
            prevState.Dispose();
    }

    public void Update() {
        if(CurrentState != null) {
            CurrentState.Update();

            if(CurrentState.NextStateTransition != null)
                ChangeState(CurrentState.NextStateTransition);
        }
    }

    public void OnGUI() {
        if(CurrentState != null)
            CurrentState.OnGUI();
    }
}
