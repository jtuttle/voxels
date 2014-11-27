using System;

public class WorldScreenChangeTransition : FSMTransition {
    public IntVector2 NextScreenCoords { get; private set; }

    public WorldScreenChangeTransition(IntVector2 nextScreenCoords, bool pushCurrentState = false) 
        : base(GameState.WorldScreenChangeState, pushCurrentState) {

        NextScreenCoords = nextScreenCoords;
    }
}
