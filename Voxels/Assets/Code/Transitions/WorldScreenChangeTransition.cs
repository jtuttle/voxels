﻿using System;

public class WorldScreenChangeTransition : FSMTransition {
    public XY NextScreenCoords { get; private set; }

    public WorldScreenChangeTransition(XY nextScreenCoords, bool pushCurrentState = false) 
        : base(GameState.WorldScreenChange, pushCurrentState) {

        NextScreenCoords = nextScreenCoords;
    }
}
