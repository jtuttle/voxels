using Holoville.HOTween;
using UnityEngine;

// The WorldScreenChangeState is responsible for handling a transition between
// world screens. This includes tweening the player and camera, and creating or
// destroying screens where necessary.

public class WorldScreenChangeState : FSMState {
    private WorldScreenManager _worldScreenManager;
    private BoundedTargetCamera _camera;
    private GameObject _player;

    public WorldScreenChangeState()
        : base(GameState.WorldScreenChange) {

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

        XY coordDelta = (transition as WorldScreenChangeTransition).CoordDelta;

        CreateScreens(GameData.CurrentScreenCoord, coordDelta);

        // Disable camera and player movement scripts.
        _camera.GetComponent<BoundedTargetCamera>().enabled = false;
        _player.GetComponent<CharacterController>().enabled = false;

        // Tween player one half chunk into next screen.
        Vector3 newPlayerPos = GetNewPlayerPosition(coordDelta);
        
        TweenParms playerParms = new TweenParms();
        playerParms.Prop("position", newPlayerPos);
        playerParms.Ease(EaseType.Linear);
        playerParms.OnComplete(OnTweenComplete, coordDelta);
        
        HOTween.To(_player.transform, 1, playerParms);

        // Tween camera to bound of next screen.
        Vector3 newCameraPos = GetNewCameraPosition(coordDelta);

        TweenParms cameraParms = new TweenParms();
        cameraParms.Prop("position", newCameraPos);
        cameraParms.Ease(EaseType.Linear);

        HOTween.To(_camera.transform, 1, cameraParms);
    }
    
    public override void ExitState(FSMTransition nextStateTransition) {
        // Re-enable camera and player movement scripts.
        _camera.GetComponent<BoundedTargetCamera>().enabled = true;
        _player.GetComponent<CharacterController>().enabled = true;

        base.ExitState(nextStateTransition);
    }

    public override void Dispose() {
        _camera = null;
        _player = null;

        base.Dispose();
    }

    // IDEA: To prevent lag here, we could try pushing coordinates onto a 
    // queue in the order that they'll appear to the player, then check
    // that queue on each update loop and create one per update. This should
    // prevent lag while crossing screen boundaries. It will matter much more
    // when we're creating trees / enemies / etc.
    private void CreateScreens(XY oldCoord, XY coordDelta) {
        // north
        if(coordDelta.Y == 1) {
            for(int x = oldCoord.X - 1; x <= oldCoord.X + 1; x++)
                _worldScreenManager.CreateScreen(new XY(x, oldCoord.Y + 3));
        // east
        } else if (coordDelta.X == 1) {
            for(int y = oldCoord.Y - 1; y <= oldCoord.Y + 2; y++)
                _worldScreenManager.CreateScreen(new XY(oldCoord.X + 2, y));
        // south
        } else if(coordDelta.Y == -1) {
            for(int x = oldCoord.X - 1; x <= oldCoord.X + 1; x++)
                _worldScreenManager.CreateScreen(new XY(x, oldCoord.Y - 2));
        // west
        } else if(coordDelta.X == -1) {
            for(int y = oldCoord.Y - 1; y <= oldCoord.Y + 2; y++)
                _worldScreenManager.CreateScreen(new XY(oldCoord.X - 2, y));
        }  
    }

    private void DestroyScreens(XY oldCoord, XY coordDelta) {
        // north
        if(coordDelta.Y == 1) {
            for(int x = oldCoord.X - 1; x <= oldCoord.X + 1; x++)
                _worldScreenManager.DestroyScreen(new XY(x, oldCoord.Y - 1));
            // east
        } else if (coordDelta.X == 1) {
            for(int y = oldCoord.Y - 1; y <= oldCoord.Y + 2; y++)
                _worldScreenManager.DestroyScreen(new XY(oldCoord.X - 1, y));
            // south
        } else if(coordDelta.Y == -1) {
            for(int x = oldCoord.X - 1; x <= oldCoord.X + 1; x++)
                _worldScreenManager.DestroyScreen(new XY(x, oldCoord.Y + 2));
            // west
        } else if(coordDelta.X == -1) {
            for(int y = oldCoord.Y - 1; y <= oldCoord.Y + 2; y++)
                _worldScreenManager.DestroyScreen(new XY(oldCoord.X + 1, y));
        }  
    }

    // Move player one half chunk's length onto the next screen.
    private Vector3 GetNewPlayerPosition(XY coordDelta) {
        Vector3 playerPos = _player.transform.position;
        float halfChunkSize = 0.5f;

        return playerPos + new Vector3(coordDelta.X * halfChunkSize,
                                       0,
                                       coordDelta.Y * halfChunkSize);
    }

    private Vector3 GetNewCameraPosition(XY coordDelta) {
        XY nextCoord = GameData.CurrentScreenCoord + coordDelta;

        Rect nextCameraBounds = 
            _worldScreenManager.GetScreenCameraBounds(nextCoord);

        Vector3 camPos = _camera.transform.position;

        float newX, newZ;

        if(coordDelta.X == -1)
            newX = nextCameraBounds.xMax;
        else if(coordDelta.X == 1)
            newX = nextCameraBounds.xMin;
        else
            newX = camPos.x;

        float adj = Mathf.Sin(_camera.Angle * Mathf.Deg2Rad) * _camera.Distance;

        if(coordDelta.Y == -1)
            newZ = nextCameraBounds.yMax - adj;
        else if(coordDelta.Y == 1)
            newZ = nextCameraBounds.yMin - adj;
        else
            newZ = camPos.z;

        return new Vector3(newX, camPos.y, newZ);
    }

    private void OnTweenComplete(TweenEvent data) {
        XY coordDelta = (XY)data.parms[0];

        DestroyScreens(GameData.CurrentScreenCoord, coordDelta);

        // Update current screen coordinate.
        GameData.CurrentScreenCoord = GameData.CurrentScreenCoord + coordDelta;

        ExitState(new FSMTransition(null));
    }
}
