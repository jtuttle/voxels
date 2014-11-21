using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using System.Collections;
using UnityEngine;

public enum GameState {
    WorldCreate, WorldNavigate
}

public class GameController : MonoBehaviour {
    private FiniteStateMachine _fsm;

    protected void Awake() {
        _fsm = new FiniteStateMachine();
        _fsm.AddState(new WorldCreateState());
        _fsm.AddState(new WorldNavigateState());
    }

    protected void Start() {
        _fsm.ChangeState(new FSMTransition(GameState.WorldCreate));
	}
}
