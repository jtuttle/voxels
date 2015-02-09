using UnityEngine;
using System.Collections;

public class WorldCreateTransition : FSMTransition {
    public string WorldName { get; private set; }

    public WorldCreateTransition(string worldName) 
        : base(GameState.WorldCreate, false) {
        
        WorldName = worldName;
    }
}
