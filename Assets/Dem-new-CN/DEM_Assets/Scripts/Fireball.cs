using UnityEngine;
using System.Collections;

// for desired behavior to trigger automatically on collision, the fireballs must have:
// a physics ridgedbody component added, 
// checked is trigger on the sphere collider component, 
// uncheck use gravity for the ridgedbody so it is not effected by gravity,
// and this script must be added as a component of the fireball object

public class Fireball : MonoBehaviour {
	// the speed of motion of a fireball
	public float speed = 10.0f;
	// the damage to health that is caused by a fireball when it hits a player
	public int damage = 1;

	// updates the horizontal motion of a fireball for each frame, 
	// adjusted by the Time.deltaTime frame rate of the user's screen
	void Update() {
		transform.Translate(0, 0, speed * Time.deltaTime);
	}
		
	// the behavior triggered by a collision
	void OnTriggerEnter(Collider other) {
		// get the player character component of the object that was hit by this fireball
		PlayerCharacter player = other.GetComponent<PlayerCharacter>();
		// if the hit object has a player character, call its Hurt fuction
		if (player != null) {
			player.Hurt(damage);
		}
		// this fireball is used up, so get rid of it
		Destroy(this.gameObject);
	}
}
