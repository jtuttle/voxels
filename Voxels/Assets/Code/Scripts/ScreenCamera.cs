using UnityEngine;
using System.Collections;

public class ScreenCamera : MonoBehaviour {
	public Player Player;

    public bool Lock;
    public float Angle = 45.0f;
    public float Distance = 20.0f;

	void Start () {
		
	}
	
	void Update () {
        float opp = Mathf.Sin(Angle * Mathf.Deg2Rad) * Distance;
        float adj = Mathf.Cos(Angle * Mathf.Deg2Rad) * Distance;

        transform.position = new Vector3(0, opp, -adj);
        transform.LookAt(Vector3.zero);
	}
}
