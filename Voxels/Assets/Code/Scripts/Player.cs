using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private CharacterController controller;

	private float speed = 20.0f;

	private Vector3 moveDirection = Vector3.zero;
	private Vector3 forward = Vector3.zero;
	private Vector3 right = Vector3.zero;

	// Use this for initialization
	void Start () {
		controller = gameObject.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		Vector3 movement = Vector3.zero;

		if(h != 0 || v != 0) {
			movement += new Vector3(h * Time.deltaTime, 0, v * Time.deltaTime);
			transform.rotation = Quaternion.LookRotation(movement);
		}

		movement += new Vector3(0, -0.05f, 0); // gravity

		controller.Move(movement * speed);
	}
}
