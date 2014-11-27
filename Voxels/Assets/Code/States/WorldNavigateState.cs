using UnityEngine;
using System.Collections;

public class WorldNavigateState : FSMState {
    private Player _player;
    private Rect _navBounds;

    public WorldNavigateState()
        : base(GameState.WorldNavigate) {
        
    }
    
    public override void InitState(FSMTransition transition) {
        base.InitState(transition);

        _player = GameData.Player;
    }
    
    public override void EnterState(FSMTransition transition) {
        base.EnterState(transition);

        _navBounds = GameData.CurrentScreen.NavBounds;
    }
    
    public override void ExitState(FSMTransition nextStateTransition) {

        base.ExitState(nextStateTransition);
    }

    public override void Update() {
        Vector3 playerPos = _player.transform.position;

        if(!_navBounds.Contains(new Vector2(playerPos.x, playerPos.z))) {
            IntVector2 nextScreenCoords = GetTransitionCoords();

            _player.GetComponent<CharacterController>().enabled = false;
            Camera.main.GetComponent<ScreenCamera>().enabled = false;

            ExitState(new WorldScreenChangeTransition(nextScreenCoords));
        }
    }
    
    public override void Dispose() {
        _player = null;
        
        base.Dispose();
    }

    private IntVector2 GetTransitionCoords() {
        Vector3 playerPos = _player.transform.position;

        IntVector2 currentCoords = GameData.CurrentScreen.Coords;
        IntVector2 shiftCoord = null;

        if(playerPos.x <= _navBounds.xMin) {
            shiftCoord = new IntVector2(-1, 0);
        } else if(playerPos.x >= _navBounds.xMax) {
            shiftCoord = new IntVector2(1, 0);
        } else if(playerPos.y <= _navBounds.yMin) {
            shiftCoord = new IntVector2(0, -1);
        } else if(playerPos.y >= _navBounds.yMax) {
            shiftCoord = new IntVector2(0, 1);
        }

        return currentCoords + shiftCoord;
    }
}
