using System.Collections;
using System;

public class FSMTransition {
    public Enum NextStateId { get; private set; }
    public bool PushCurrentState { get; private set; }

    public FSMTransition(Enum nextStateId, bool pushCurrentState = false) {
        NextStateId = nextStateId;
        PushCurrentState = pushCurrentState;
    }
}
