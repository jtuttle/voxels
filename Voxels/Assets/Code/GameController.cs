﻿using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using System.Collections;
using UnityEngine;

public enum GameState {
    WorldName, WorldCreate, WorldNavigate, WorldScreenChange
}

public class GameController : MonoBehaviour {
    private FiniteStateMachine _fsm;

    protected void Awake() {
        _fsm = new FiniteStateMachine();
        _fsm.AddState(new WorldNameState());
        _fsm.AddState(new WorldCreateState());
        _fsm.AddState(new WorldNavigateState());
        _fsm.AddState(new WorldScreenChangeState());
    }

    protected void Start() {
        _fsm.ChangeState(new FSMTransition(GameState.WorldName));
	}

    protected void Update() {
        if(_fsm != null)
            _fsm.Update();
    }

    protected void OnGUI() {
        if(_fsm != null)
            _fsm.OnGUI();
    }
}
