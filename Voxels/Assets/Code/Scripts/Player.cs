using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public ModifyTerrain ModifyTerrain;

	private CharacterController controller;

	private float speed = 20.0f;

	private Vector3 moveDirection = Vector3.zero;
	private Vector3 forward = Vector3.zero;
	private Vector3 right = Vector3.zero;

	// Use this for initialization
	protected void Start() {
		controller = gameObject.GetComponent<CharacterController>();
	}

	protected void Update() {
		if(Input.GetButtonDown("Jump"))
			Attack();
	}

	// Update is called once per frame
	protected void FixedUpdate() {
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

	private void Attack() {
		Ray ray = new Ray(transform.position, transform.forward);
		int distance = 8;
		RaycastHit hit;

		Debug.DrawLine(ray.origin, ray.origin + (ray.direction * distance), Color.green, 2);

		if(Physics.Raycast(ray, out hit, distance)) {
			ModifyTerrain.ReplaceRectangularBox(hit.point, new Vector3(6, 7, 6), (byte)0);

            GameObject cubeEffectGO = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/CubeEffect"));
            cubeEffectGO.transform.position = hit.point;

            CubeEffect cubeEffect = cubeEffectGO.GetComponent<CubeEffect>();
            
            cubeEffect.Player = this;
            cubeEffect.Spawn(new Color(0.66f, 0.45f, 0.26f), 1);
		}
	}
}
