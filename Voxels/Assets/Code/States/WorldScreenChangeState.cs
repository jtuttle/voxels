using Holoville.HOTween;
using UnityEngine;

public class WorldScreenChangeState : FSMState {
    private IntVector2 _nextScreenCoords;
    private Vector2 _coordDifference;

    private Camera _camera;
    private Player _player;

    public WorldScreenChangeState()
        : base(GameState.WorldScreenChangeState) {

    }

    public override void InitState(FSMTransition transition) {
        base.InitState(transition);

    }
    
    public override void EnterState(FSMTransition transition) {
        base.EnterState(transition);

        _nextScreenCoords = (transition as WorldScreenChangeTransition).NextScreenCoords;
        _coordDifference = (_nextScreenCoords - GameData.CurrentScreen.Coords).ToVector2();

        _camera = Camera.main;
        _player = GameData.Player;

        // Disable camera and player movement scripts.
        _camera.GetComponent<ScreenCamera>().enabled = false;
        _player.GetComponent<CharacterController>().enabled = false;

        // Set up tween to move player across screen edge.
        Vector3 newPlayerPos = GetNewPlayerPosition(_player);
        
        TweenParms playerParms = new TweenParms();
        playerParms.Prop("position", newPlayerPos);
        playerParms.Ease(EaseType.Linear);
        playerParms.OnComplete(OnTweenComplete);
        
        HOTween.To(_player.transform, 1, playerParms);

        // Set up tween to move camera across screen edge.
        Vector3 newCameraPos = GetNewCameraPosition(_camera, newPlayerPos);

        TweenParms cameraParms = new TweenParms();
        cameraParms.Prop("position", newCameraPos);
        cameraParms.Ease(EaseType.Linear);

        HOTween.To(_camera.transform, 1, cameraParms);
    }
    
    public override void ExitState(FSMTransition nextStateTransition) {
        // Update game state stuff.
        GameData.CurrentScreen = GameData.World.GetScreen(_nextScreenCoords);
        _camera.GetComponent<ScreenCamera>().UpdateBounds(_nextScreenCoords);

        // Re-enable camera and player movement scripts.
        _camera.GetComponent<ScreenCamera>().enabled = true;
        _player.GetComponent<CharacterController>().enabled = true;

        _nextScreenCoords = null;
        _coordDifference = Vector2.zero;
        _camera = null;
        _player = null;

        base.ExitState(nextStateTransition);
    }

    public override void Dispose() {

        base.Dispose();
    }

    // Move player to one chunk farther than the screen edge.
    private Vector3 GetNewPlayerPosition(Player player) {
        Vector2 distanceToEdge = GetDistanceToEdge(player.transform);

        // For now, we'll just place the player one chunk away from the screen edge.
        WorldConfig worldConfig = GameData.World.Config;
        Vector2 chunkSize = 2 * new Vector2(worldConfig.ChunkSize, worldConfig.ChunkSize);
        chunkSize.Scale(_coordDifference);
        
        return player.transform.position + new Vector3(distanceToEdge.x + chunkSize.x, 0, distanceToEdge.y + chunkSize.y);
    }

    // New camera position needs to match player's.
    private Vector3 GetNewCameraPosition(Camera camera, Vector3 newPlayerPos) {
        Vector3 cameraPos = camera.transform.position;

        if(_coordDifference.x != 0)
            cameraPos = new Vector3(newPlayerPos.x, cameraPos.y, cameraPos.z);
        else
            cameraPos = new Vector3(cameraPos.x, cameraPos.y, newPlayerPos.z);

        return cameraPos;
    }

    private Vector2 GetDistanceToEdge(Transform transform) {
        World world = GameData.World;
        IntVector2 currentScreenCoords = GameData.CurrentScreen.Coords;

        Vector3 pos = transform.position;

        // Calculate absolute value of distance to center.
        Vector2 center = world.GetScreenCenter(currentScreenCoords);
        Vector2 distanceToCenter = new Vector2(pos.x, pos.z) - center;
        distanceToCenter = new Vector2(Mathf.Abs(distanceToCenter.x), Mathf.Abs(distanceToCenter.y));
        
        Vector2 halfScreenDimensions = world.GetScreenDimensions() / 2;

        // Use coordinate different to mask the desired dimensions.
        halfScreenDimensions.Scale(_coordDifference);
        distanceToCenter.Scale(_coordDifference);

        return halfScreenDimensions - distanceToCenter;
    }

    private void OnTweenComplete() {
        ExitState(new FSMTransition(null));
    }
}
