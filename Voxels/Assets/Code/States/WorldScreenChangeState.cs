using Holoville.HOTween;
using UnityEngine;

public class WorldScreenChangeState : FSMState {
    private IntVector2 _nextScreenCoords;

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

        _camera = Camera.main;
        _player = GameData.Player;

        // Disable camera and player movement scripts.
        _camera.GetComponent<ScreenCamera>().enabled = false;
        _player.GetComponent<CharacterController>().enabled = false;

        // Set up tween to move camera across screen edge.
        // TODO: there's a noticable snap when transitioning because the camera
        // isn't being tweened far enough to keep up with the player's position.
        Vector3 newCameraPos = GetNewCameraPosition(_camera, _nextScreenCoords);

        TweenParms cameraParms = new TweenParms();
        cameraParms.Prop("position", newCameraPos);
        cameraParms.Ease(EaseType.Linear);

        HOTween.To(_camera.transform, 1, cameraParms);

        // Set up tween to move player across screen edge.
        Vector3 newPlayerPos = GetNewPlayerPosition(_player, _nextScreenCoords);

        TweenParms playerParms = new TweenParms();
        playerParms.Prop("position", newPlayerPos);
        playerParms.Ease(EaseType.Linear);
        playerParms.OnComplete(OnTweenComplete);

        HOTween.To(_player.transform, 1, playerParms);
    }
    
    public override void ExitState(FSMTransition nextStateTransition) {
        GameData.CurrentScreen = GameData.World.GetScreen(_nextScreenCoords);
        _camera.GetComponent<ScreenCamera>().UpdateBounds(_nextScreenCoords);

        // Re-enable camera and player movement scripts.
        _camera.GetComponent<ScreenCamera>().enabled = true;
        _player.GetComponent<CharacterController>().enabled = true;

        _nextScreenCoords = null;
        _camera = null;
        _player = null;

        base.ExitState(nextStateTransition);
    }
    
    public override void Update() {

    }
    
    public override void Dispose() {

        base.Dispose();
    }

    // Move player to one chunk farther than the screen edge.
    private Vector3 GetNewPlayerPosition(Player player, IntVector2 nextScreenCoords) {
        Vector2 distanceToEdge = GetDistanceToEdge(player.transform, nextScreenCoords);

        // Coordinate difference will be used to mask vector dimensions.
        Vector2 coordDiff = (nextScreenCoords - GameData.CurrentScreen.Coords).ToVector2();

        // For now, we'll just place the player one chunk away from the screen edge.
        WorldConfig worldConfig = GameData.World.Config;
        Vector2 chunkSize = 2 * new Vector2(worldConfig.ChunkSize, worldConfig.ChunkSize);
        chunkSize.Scale(coordDiff);
        
        return player.transform.position + new Vector3(distanceToEdge.x + chunkSize.x, 0, distanceToEdge.y + chunkSize.y);
    }

    // Reflect camera position across the edge of the screen.
    private Vector3 GetNewCameraPosition(Camera camera, IntVector2 nextScreenCoords) {
        Vector2 distanceToEdge = GetDistanceToEdge(camera.transform, nextScreenCoords);

        return camera.transform.position + new Vector3(2 * distanceToEdge.x, 0, 2 * distanceToEdge.y);
    }

    private Vector2 GetDistanceToEdge(Transform transform, IntVector2 nextScreenCoords) {
        World world = GameData.World;
        IntVector2 currentScreenCoords = GameData.CurrentScreen.Coords;

        // Coordinate difference will be used to mask vector dimensions.
        Vector2 coordDiff = (nextScreenCoords - currentScreenCoords).ToVector2();

        Vector3 pos = transform.position;

        // Calculate absolute value of distance to center.
        Vector2 center = world.GetScreenCenter(currentScreenCoords);
        Vector2 distanceToCenter = new Vector2(pos.x, pos.z) - center;
        distanceToCenter = new Vector2(Mathf.Abs(distanceToCenter.x), Mathf.Abs(distanceToCenter.y));
        
        Vector2 halfScreenDimensions = world.GetScreenDimensions() / 2;

        // Use coordinate different to mask the desired dimensions.
        halfScreenDimensions.Scale(coordDiff);
        distanceToCenter.Scale(coordDiff);

        return halfScreenDimensions - distanceToCenter;
    }

    private void OnTweenComplete() {
        ExitState(new FSMTransition(null));
    }
}
