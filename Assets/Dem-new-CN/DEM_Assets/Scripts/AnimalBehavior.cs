using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This needs to be made a component of the animal prefab.

public class AnimalBehavior : SpeciesBehavior 
{
	DemSceneConstants.AnimalType animalType;
	// speed of the motion of the wandering animal
	public float speed = 3.0f;
	// distance for an object to be in range for attack or collision avoidance
	public float obstacleRange = 5.0f;

	// to be done in every frame
	void Update() {

		if (getAlive()) {
			// always move the species forward, even if it will need to turn
			transform.Translate (0, 0, speed * Time.deltaTime);
			// creat a ray at the same position as the enemy and facing in the same direction as the species
			Ray ray = new Ray (transform.position, transform.forward);
			RaycastHit hit;
			// do ray casting to look for a hit, with a radius of 0.75 for the ray
			if (Physics.SphereCast (ray, 0.75f, out hit)) {
				// get the object that was hit by the ray cast
				GameObject hitObject = hit.transform.gameObject;
				// get the reactive target component from the object that was hit
				// the target returned is null if the object hit does not contain that type of component
				ReactiveTarget target = hitObject.GetComponent<ReactiveTarget> ();

				if ((target != null) && (hit.distance < obstacleRange)){
					// if object is prey, tell the object to react to being hit
					// TODO add test for prey
					target.ReactToHit ();
				} 
				else if (hit.distance < obstacleRange) {
					// If the object hit was not a species of prey, (like if it is a wall),
					// and if the object hit is within the distance for obsticle avoidance reaction,
					// then rotate by randomly +/- 110 degrees horizontally about the Y-axis.
					float angle = Random.Range (-110, 110);
					transform.Rotate (0, angle, 0);
				}
			}
		}
	}


	public void setAnimalType(DemSceneConstants.AnimalType type){
		animalType = type;
	}

	public DemSceneConstants.AnimalType getAnimalType() {
		return animalType;
	}

}
