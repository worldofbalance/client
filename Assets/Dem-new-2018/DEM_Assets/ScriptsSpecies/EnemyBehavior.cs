using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : SpeciesBehavior {

	// distance for an object to be in range for attack or collision avoidance
	public float obstacleRange = 5.0f;


	// to be done in every frame
	void Update() {
		/**
		if (getAlive()) 
		{
			// creat a ray at the same position as the animal and facing in the same direction as the species
			Ray ray = new Ray (transform.position, transform.forward);
			RaycastHit hit;
			// do ray casting to look for a hit, with a radius of 0.75 for the ray
			if (Physics.SphereCast (ray, 0.75f, out hit)) {
				// get the object that was hit by the ray cast
				GameObject hitObject = hit.transform.gameObject;
				// get the needed components from the object that was hit
				// the target returned is null if the object hit does not contain that type of component
				SpeciesBehavior otherSpecies = hitObject.GetComponent<SpeciesBehavior>();

				if (hit.distance < obstacleRange)
				{
					// if object is prey, tell the object to react to being hit
					if ((otherSpecies != null) && (preyList.Contains (otherSpecies.getSpeciesType())))
					{
						otherSpecies.ReactToHit ();
					}

				} // end if distance within obsticle range
			}
		} // end if alive
		**/
	} 


}
