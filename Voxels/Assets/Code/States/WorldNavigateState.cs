using UnityEngine;
using System.Collections;

// The WorldNavigateState is responsible for updating the camera's bounds to 
// match those of the current screen and listening for when the player crosses 
// the edge of the current screen.

public class WorldNavigateState : FSMState {
    private WorldScreenManager _worldScreenManager;
    private GameObject _player;
    private BoundedTargetCamera _camera;

    private Rect _navBounds;

    public WorldNavigateState()
        : base(GameState.WorldNavigate) {
        
    }
    
    public override void InitState(FSMTransition transition) {
        base.InitState(transition);

        _worldScreenManager = GameObject.Find("WorldScreenManager").
            GetComponent<WorldScreenManager>();

        _camera = Camera.main.GetComponent<BoundedTargetCamera>();
    }
    
    public override void EnterState(FSMTransition transition) {
        base.EnterState(transition);

        if(_player == null)
            _player = GameObject.Find("Player");

        XY screenCoord = GameData.CurrentScreenCoord;

        _navBounds = _worldScreenManager.GetScreenBounds(screenCoord);
        _camera.Bounds = _worldScreenManager.GetScreenCameraBounds(screenCoord);
    }
    
    public override void ExitState(FSMTransition nextStateTransition) {

        base.ExitState(nextStateTransition);
    }

    public override void Update() {
        Vector3 playerPos = _player.transform.position;

        if(!_navBounds.Contains(new Vector2(playerPos.x, playerPos.z))) {
            //XY currentCoord = GameData.CurrentScreenCoord;
            XY coordDelta = GetCoordDelta();
            ExitState(new WorldScreenChangeTransition(coordDelta, true));
        }
    }
    
    public override void Dispose() {
        _player = null;
        
        base.Dispose();
    }

    private Rect GetRectWithBounds(Rect rect, float top, float bottom, 
                                   float left, float right) {
       
        return new Rect(rect.xMin + left,
                        rect.yMin + bottom,
                        rect.width - left - right,
                        rect.height - top - bottom);
    }

    private XY GetCoordDelta() {
        Vector3 playerPos = _player.transform.position;

        XY coordDelta = null;

        if(playerPos.x <= _navBounds.xMin)
            coordDelta = new XY(-1, 0);
        else if(playerPos.x >= _navBounds.xMax)
            coordDelta = new XY(1, 0);
        else if(playerPos.z <= _navBounds.yMin)
            coordDelta = new XY(0, -1);
        else if(playerPos.z >= _navBounds.yMax)
            coordDelta = new XY(0, 1);

        return coordDelta;
    }
}
