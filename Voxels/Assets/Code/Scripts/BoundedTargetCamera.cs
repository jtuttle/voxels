using System.Collections;
using UnityEngine;

public class BoundedTargetCamera : MonoBehaviour {
    public GameObject Target;

    public bool Lock;
    public float Angle = 40.0f;
    public float Distance = 40.0f;

    public Rect Bounds { get; set; }

    void Update() {
        if(Target == null) return;

        Vector3 targetPos = Target.transform.position;

        // horizontal calc
        if(targetPos.x < Bounds.xMin)
            targetPos.x = Bounds.xMin;
        else if(targetPos.x > Bounds.xMax)
            targetPos.x = Bounds.xMax;

        // vertical calc
        if(targetPos.z < Bounds.yMin)
            targetPos.z = Bounds.yMin;
        else if(targetPos.z > Bounds.yMax)
            targetPos.z = Bounds.yMax;

        float opp = Mathf.Sin(Angle * Mathf.Deg2Rad) * Distance;
        float adj = Mathf.Cos(Angle * Mathf.Deg2Rad) * Distance;

        transform.position = targetPos + new Vector3(0, adj, -opp);
        transform.LookAt(targetPos);
    }
}
