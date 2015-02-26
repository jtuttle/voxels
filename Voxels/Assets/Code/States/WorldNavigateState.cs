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

        //_navBounds = GameData.CurrentScreen.NavBounds;
    }
    
    public override void ExitState(FSMTransition nextStateTransition) {

        base.ExitState(nextStateTransition);
    }

    public override void Update() {
        Vector3 playerPos = _player.transform.position;

        if(!_navBounds.Contains(new Vector2(playerPos.x, playerPos.z))) {
            XY nextScreenCoords = GetTransitionCoords();
            ExitState(new WorldScreenChangeTransition(nextScreenCoords, true));
        }
    }
    
    public override void Dispose() {
        _player = null;
        
        base.Dispose();
    }

    private XY GetTransitionCoords() {
        Vector3 playerPos = _player.transform.position;

        //XY currentCoords = GameData.CurrentScreen.Coords;
        XY shiftCoord = null;

        if(playerPos.x <= _navBounds.xMin) {
            shiftCoord = new XY(-1, 0);
        } else if(playerPos.x >= _navBounds.xMax) {
            shiftCoord = new XY(1, 0);
        } else if(playerPos.y <= _navBounds.yMin) {
            shiftCoord = new XY(0, -1);
        } else if(playerPos.y >= _navBounds.yMax) {
            shiftCoord = new XY(0, 1);
        }

        //return currentCoords + shiftCoord;
        return shiftCoord;
    }
}
