using UnityEngine;
using System.Collections;

public class WorldNameState : FSMState {
    private string _worldName = "Voxlandia";

    public WorldNameState()
        : base(GameState.WorldName) {
    
    }

    public override void OnGUI() {
        base.OnGUI();

        float hCenter = Screen.width / 2;
        float vCenter = Screen.height / 2;

        _worldName = GUI.TextField(new Rect(hCenter - 100, vCenter - 20, 200, 20), _worldName);

        if(GUI.Button(new Rect(hCenter - 50, vCenter + 20, 100, 30), "Create World")) {
            OnCreateWorldClick();
        }
    }

    private void OnCreateWorldClick() {
        ExitState(new WorldCreateTransition(_worldName));
    }
}
