using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This needs to be made a component of the animal prefab.

public class AnimalBehavior : SpeciesBehavior 
{
	// speed of the motion of the wandering animal
	public float speed = 3.0f;
	// distance for an object to be in range for attack or collision avoidance
	public float obstacleRange = 5.0f;
	// for scensing objects ahead
	RaycastHit hit;
	Ray ray;

	// TODO make this a better motion model by making first located enemy, 
	// or the nearest enemy, the next direction of motion ??
	// to be done in every frame
	void Update() {

		if (getAlive()) {
			// always move the species forward, even if it will need to turn
			transform.Translate (0, 0, speed * Time.deltaTime);
			// creat a ray at the same position as the animal and facing in the same direction as the species
			ray = new Ray (transform.position, transform.forward);

			// do ray casting to look for a hit, with a radius of 0.75 for the ray
			if (Physics.SphereCast (ray, 0.75f, out hit)) {
				// get the object that was hit by the ray cast
				GameObject hitObject = hit.transform.gameObject;
				// get the needed components from the object that was hit
				// the target returned is null if the object hit does not contain that type of component
				SpeciesBehavior target = hitObject.GetComponent<SpeciesBehavior>();

				if (hit.distance < obstacleRange)
				{
					// if object is prey, tell the object to react to being hit
					if (target != null) {
						if (preyList.Contains (hitObject.tag)) {
							target.ReactToHit ();
						} else {
							// If the object hit was not a species of prey, and it is within the
							// distance for collision avoidance, then avoid the obstical
							// by turning in a random direction.
							float angle = Random.Range (-110, 110);
							transform.Rotate (0, angle, 0);
						}
					}

				} // end if distance within obsticle range
			}
		} // end if alive
	} 


}
