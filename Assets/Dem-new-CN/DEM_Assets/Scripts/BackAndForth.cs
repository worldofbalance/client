using UnityEngine;
using System.Collections;

public class BackAndForth : MonoBehaviour {

	public float speed = 3.0f;
	// maximum positions of the back and forth motion of the game object
	public float maxZ = 16.0f;
	public float minZ = -16.0f;
	// the current direction of motion of the gaame object
	private int _direction = 1;

	// Update is called once per frame
	void Update() {
		// move the game object
		transform.Translate(0, 0, _direction * speed * Time.deltaTime);

		bool bounced = false;
		if (transform.position.z > maxZ || transform.position.z < minZ) {
			// reverse the direction of motion if the maximum has been reached
			_direction = -_direction;
			bounced = true;
		}
		// if direction of motion has changed, move the game object again,
		// but in the new direction of motion
		if (bounced) {
			transform.Translate(0, 0, _direction * speed * Time.deltaTime);
		}
	}
}
