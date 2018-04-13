using UnityEngine;
using System.Collections;

// Notes from book "Unity in Action" by Joseph Hocking script downloaded from book website:
// MouseLook rotates the transform based on the mouse delta.
// To make an FPS style character:
// - Create a capsule.// Named the Player
// - Add the MouseLook script to the capsule. // On the Player
//   -> Set the mouse look to use MouseX. (You want to only turn character but not tilt it)
// - Add FPSInput script to the capsule  // On the Player
//   -> A CharacterController component will be automatically added.
//
// - Create a camera. Make the camera a child of the capsule (Player). Position in the head and reset the rotation.
// - Add a MouseLook script to the camera.
//   -> Set the mouse look to use MouseY. (You want the camera to tilt up and down like a head. The character already turns.)

// Notes added to the code below by Cheryl Nielsen for class CSC 831, SFSU, Spring 2018.

[AddComponentMenu("Control Script/Mouse Look")]
public class MouseLook : MonoBehaviour {
	
	// the possible mouse motion directions
	public enum RotationAxes {
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2
	}

	public RotationAxes axes = RotationAxes.MouseXAndY;

	// formerly called the speed in the Spin script, when only one direction was involved
	// these are the rotation speeds
	public float sensitivityHor = 9.0f;
	public float sensitivityVert = 9.0f;
	// maximum motions alowed for vertical rotation around the X-axis, 
	// this prevents the player's vision from going upsidedown,
	// and no need to limit horizontal rotation around the Y-axis
	public float minimumVert = -45.0f;
	public float maximumVert = 45.0f;

	private float _rotationX = 0;

	void Start() {
		// Make the rigid body not change rotation
		Rigidbody body = GetComponent<Rigidbody>();
		if (body != null)
			body.freezeRotation = true;
	}

	void Update() {
		if (axes == RotationAxes.MouseX) {
			// horizontal rotation about the Y-axis, based on forward or backward motion of the mouse
			// (along the mouse's X-axis)
			// This rotates the player and the attached camera, because we set the player to
			// have the value axes = RotationAxes.MouseX for its component copy of this script.
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
		}
		else if (axes == RotationAxes.MouseY) {
			// vertical rotation about the X-axis, based on left or right motion of the mouse
			// (along the mouse's Y-axis)
			_rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
			// limit or clamp the vertical rotation to the maximum and minimum desired amounts
			_rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
			// Create the motion vector as a local motion rotation vector relative to the camera.
			// This rotates only the attached camera, relative to the player, because we set the camera to
			// have the value axes = RotationAxes.MouseY for its component copy of this script.
			transform.localEulerAngles = new Vector3(_rotationX, transform.localEulerAngles.y, 0);
		}
		else {
			// both horizontal and vertical rotation
			// note that it has components of both the above options
			float rotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityHor;

			_rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
			_rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

			transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
		}
	}
}