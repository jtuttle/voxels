using UnityEngine;
using System.Collections;

public class WorldScreenChangeState : FSMState {
    private IntVector2 _nextScreenCoords;

    public WorldScreenChangeState()
        : base(GameState.WorldScreenChangeState) {

    }

    public override void InitState(FSMTransition transition) {
        base.InitState(transition);

    }
    
    public override void EnterState(FSMTransition transition) {
        base.EnterState(transition);

        _nextScreenCoords = (transition as WorldScreenChangeTransition).NextScreenCoords;

        Debug.Log(_nextScreenCoords);
    }
    
    public override void ExitState(FSMTransition nextStateTransition) {
        
        base.ExitState(nextStateTransition);
    }
    
    public override void Update() {

    }
    
    public override void Dispose() {

        base.Dispose();
    }
}
