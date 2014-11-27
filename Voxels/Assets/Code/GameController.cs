using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using System.Collections;
using UnityEngine;

public enum GameState {
    WorldCreate, WorldNavigate, WorldScreenChangeState
}

public class GameController : MonoBehaviour {
    private FiniteStateMachine _fsm;

    protected void Awake() {
        _fsm = new FiniteStateMachine();
        _fsm.AddState(new WorldCreateState());
        _fsm.AddState(new WorldNavigateState());
        _fsm.AddState(new WorldScreenChangeState());
    }

    protected void Start() {
        _fsm.ChangeState(new FSMTransition(GameState.WorldCreate));
	}

    protected void Update() {
        if(_fsm != null)
            _fsm.Update();
    }
}
