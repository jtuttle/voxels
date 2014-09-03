using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	public Player Player;

	void Start () {
		transform.position = getTarget();
	}
	
	void Update () {
		transform.position = getTarget();
	}

	private void SmoothFollowPosition() {
		transform.position = Vector3.Lerp(transform.position, getTarget(), Time.deltaTime);
		transform.LookAt(Player.transform);
	}

	private Vector3 getTarget() {
		return Player.transform.position + new Vector3(0, 35.0f, -30.0f);
	}
}
