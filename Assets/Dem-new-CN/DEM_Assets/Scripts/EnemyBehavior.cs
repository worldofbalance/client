using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : SpeciesBehavior {

	// Enenmy purpose is to attack the Tree of Life, 
	// so enemy goal is the location of the Tree of Life.
	Vector3 TreeOfLifeLocation;
	// speed of the motion of the wandering animal
	public float speed = 3.0f;
	// distance for an object to be in range for attack or collision avoidance
	public float obstacleRange = 5.0f;


	// Use this for initialization
	void Start() 
	{
		this.setAlive(true);
		this.setHealth (5);
		preyList = new ArrayList();
		preyList.Add (DemSceneConstants.SpeciesType.TreeOfLife);
		// set as goal the location of TreeOfLife
		TreeOfLifeLocation = new Vector3();
		TreeOfLifeLocation = GameObject.Find ("TreeOfLife").transform.position;
	}


	// to be done in every frame
	void Update() {

		if (getAlive()) 
		{
			SearchForTheTreeOfLife ();
			// always move the species forward, even if it will need to turn
			transform.Translate (0, 0, speed * Time.deltaTime);
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
						if (otherSpecies.getSpeciesType() == DemSceneConstants.SpeciesType.TreeOfLife)
						{
							ObsticleAvoidance();
							transform.Translate (0, 0, 10);
						}
					}
					else 
					{
						// If the object hit was not a species of prey, and it is within the
						// distance for collision avoidance
						ObsticleAvoidance();
					}

				} // end if distance within obsticle range
			}
		} // end if alive
	} // end function Update()
		

	// find the tree of life
	public void SearchForTheTreeOfLife() 
	{
		// find direction to the Tree of Life
		Vector3 targetDirection = TreeOfLifeLocation - transform.position;
		float angle = Vector3.Angle (targetDirection, transform.forward);
		// turn towards the Tree of Life
		transform.Rotate (0, angle, 0);
		// always move the species forward, even if it will need to turn
		// transform.Translate (0, 0, speed * Time.deltaTime);
	}


	// avoid the obstical by turning in a random direction.
	public void ObsticleAvoidance()
	{
		float angle = Random.Range (-110, 110);
		transform.Rotate (0, angle, 0);
	}

}
