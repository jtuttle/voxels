using UnityEngine;
using System.Collections;

public class WorldNavigateState : FSMState {
    public WorldNavigateState()
        : base(GameState.WorldNavigate) {
        
    }
    
    public override void InitState(FSMState prevState) {
        base.InitState(prevState);

    }
    
    public override void EnterState(FSMState prevState) {
        base.EnterState(prevState);

    }
    
    public override void ExitState(FSMTransition nextStateTransition) {
        
        base.ExitState(nextStateTransition);
    }
    
    public override void Dispose() {
        
        base.Dispose();
    }
}
