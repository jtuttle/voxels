using UnityEngine;
using System.Collections;

public class ScreenCamera : MonoBehaviour {
    public Player Player;

    public bool Lock;
    public float Angle = 40.0f;
    public float Distance = 40.0f;

    public Rect Bounds { get; private set; }
    
    void Start() {
        
    }
    
    void Update() {
        if(Player != null) {
            Vector3 target = Player.transform.position;

            // horizontal calc
            if(target.x < Bounds.xMin)
                target.x = Bounds.xMin;
            else if(target.x > Bounds.xMax)
                target.x = Bounds.xMax;

            // vertical calc
            if(target.z < Bounds.yMin)
                target.z = Bounds.yMin;
            else if(target.z > Bounds.yMax)
                target.z = Bounds.yMax;

            float opp = Mathf.Sin(Angle * Mathf.Deg2Rad) * Distance;
            float adj = Mathf.Cos(Angle * Mathf.Deg2Rad) * Distance;

            transform.position = target + new Vector3(0, adj, -opp);
        }
    }

    public void UpdateBounds(XY screenCoord) {
        // TODO: un-hardcode this edge value, maybe.
        //Bounds = GameData.World.GetScreenBounds(screenCoord, 25.0f, 13.0f);
    }

    private void SmoothFollowPosition() {
        //transform.position = Vector3.Lerp(transform.position, getTarget(), Time.deltaTime);

        transform.position = getTarget();
        //transform.LookAt(Player.transform);
    }
    
    private Vector3 getTarget() {
        float opp = Mathf.Sin(Angle * Mathf.Deg2Rad) * Distance;
        float adj = Mathf.Cos(Angle * Mathf.Deg2Rad) * Distance;
        
        return Player.transform.position + new Vector3(0, adj, -opp);
    }
}
