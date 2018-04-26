
using UnityEngine;
using System.Collections;

public class ClashCameraControl : MonoBehaviour {
	public float speed = 5.0f;
	const float defaultCameraHeight = 20.0f;
	const float defaultFOV = 75.0f;
	//const float maxFOV = 110.0f;
	//const float minFOV = 60.0f;
	const float minY = 6.0f;
	const float maxY = 110.0f;
	const float minX = 88.0f;
	const float maxX = 360.0f;
	const float minZ = 0.0f;
	const float maxZ = 450.0f;
	float fov;
	Vector3 terrainBoundaries;

	// Use this for initialization
	void Start () {
		//transform.rotation = Quaternion.Euler (45, 0, 0);
		Camera.main.fieldOfView = defaultFOV;
		transform.position = new Vector3(transform.position.x, defaultCameraHeight, transform.position.z);
		Camera.main.transform.localPosition = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		moveX ();
		moveY ();
		moveZ ();
		rotateCam ();
	}

	//Seperate functions to move X Pos, Y pos , Z pos and rotate camera
	void moveX(){
		//check X pos
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			if( transform.position.x <= maxX)
				transform.position += new Vector3 (speed*Time.deltaTime, 0, 0);
		}
		if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
			if( transform.position.x >= minX)
				transform.position += new Vector3(-speed*Time.deltaTime,0,0);
		}
	}
	void moveY(){
		if(Input.GetKey(KeyCode.Q)) {
			if( transform.position.y <= maxY)
				transform.position += new Vector3(0,speed*Time.deltaTime,0);
		}
		if(Input.GetKey(KeyCode.E)) {
			if( transform.position.y >= minX)
				transform.position += new Vector3(0,-speed*Time.deltaTime,0);
		}
	}
	void moveZ(){
		if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			if( transform.position.z <= maxZ)
				transform.position += new Vector3(0,0,speed*Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
			if( transform.position.z >= minZ)
				transform.position += new Vector3(0,0,-speed*Time.deltaTime);
		}
	}
	void rotateCam(){
		if (Input.GetKey(KeyCode.X)) {
			transform.Rotate(new Vector3 (0,speed*Time.deltaTime, 0));
		}
		if(Input.GetKey(KeyCode.Z)) {
			transform.Rotate(new Vector3(0,-speed*Time.deltaTime,0));
		}
	}
}
