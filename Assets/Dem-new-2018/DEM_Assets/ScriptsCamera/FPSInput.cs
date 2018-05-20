using UnityEngine;
using System.Collections;

// This is from the text book "Unity in Action: Multiplatform Game Development in C#" by Joseph Hocking.
// With some notes added by Cheryl Nielsen.
// Basic WASD-style movement control (W = up, A = left, S = down, D = right)
// Commented out line demonstrates that transform.Translate instead of charController.Move doesn't have collision detection
// The character controller has the collision behavior already built into it by Unity,
// otherwise the player can walk through the walls.

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour {
	// multiple for the desired speed of the motion of the player
	public float speed = 6.0f;
	// standard earth downward gravity
	public float gravity = -9.8f;
	// the character controller has the collision detection avoidance ability already built into it by Unity
	private CharacterController _charController;

	void Start() {
		// this script is a component of an object (attached to a game object), 
		// get that object's character controller component
		_charController = GetComponent<CharacterController>();
	}

	void Update() {
		// with no collision detection
		//transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime);

		// get input from mouse about horizontal X-axis motion, 
		// and scale the speed by that motion
		float deltaX = Input.GetAxis("Horizontal") * speed;
		// get input from mouse about vertical Z-axis location motion, 
		// and scale the speed by that motion
		float deltaZ = Input.GetAxis("Vertical") * speed;

		// same transform but using deltaX and deltaZ variables with no collision detection
		//transform.Translate(deltaX * Time.deltaTime, 0, deltaZ * Time.deltaTime);

		// the 3D cordinates of the new location for the player
		Vector3 movement = new Vector3(deltaX, 0, deltaZ);
		// clamp or limit the magnitude of the diagnal motion to no more than the
		// speed constant that was set for motion along a single axis
		movement = Vector3.ClampMagnitude(movement, speed);

		// Gravity in the Y-axis direction ensures that the game player stays on the ground,
		// instead of floating up in the air.
		movement.y = gravity;
		// proportion the motion speed by the hardware frame rate so that the speed looks
		// similar on all platforms, otherwise it will be faster on screens with faster framerates,
		// and slower on screens with slower framerates
		movement *= Time.deltaTime;
		// transform the movement vector from local to global coordinates
		movement = transform.TransformDirection(movement);
		// tell the character controller to move by that vector amount, 
		// because the controller is a component of the player, this will move the player also
		_charController.Move(movement);
	}
}
