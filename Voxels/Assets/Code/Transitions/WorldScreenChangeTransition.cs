using System;

public class WorldScreenChangeTransition : FSMTransition {
    public XY CoordDelta { get; private set; }

    public WorldScreenChangeTransition(XY coordDelta, bool pushCurrentState = false) 
        : base(GameState.WorldScreenChange, pushCurrentState) {

        CoordDelta = coordDelta;
    }
}
