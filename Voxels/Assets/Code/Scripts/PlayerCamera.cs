using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	public Player Player;

    public bool Lock;
    public float Angle = 45.0f;
    public float Distance = 20.0f;

	void Start () {
		transform.position = getTarget();
	}
	
	void Update () {
		transform.position = getTarget();
        SmoothFollowPosition();
	}

	private void SmoothFollowPosition() {
		transform.position = Vector3.Lerp(transform.position, getTarget(), Time.deltaTime);
		transform.LookAt(Player.transform);
	}

	private Vector3 getTarget() {
        float opp = Mathf.Sin(Angle * Mathf.Deg2Rad) * Distance;
        float adj = Mathf.Cos(Angle * Mathf.Deg2Rad) * Distance;

		return Player.transform.position + new Vector3(0, adj, -opp);
	}
}
