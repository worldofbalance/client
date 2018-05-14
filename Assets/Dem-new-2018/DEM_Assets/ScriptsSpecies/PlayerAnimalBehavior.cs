using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This needs to be made a component of the animal prefab.

public class PlayerAnimalBehavior : SpeciesBehavior 
{
	// distance for an enemy prey to be in range for attack
	public float attackDistance = 15.0f;
	private int gameBordWidth = 200;
	public NavMeshAgent agent;
	private GameObject nearestEnemy;
	private float distance;

	// the current target enemy to be navigated to for attack
	GameObject targetEnemy = null;

	// the initial state
	void Start() {
		agent = GetComponent<NavMeshAgent>();
		agent.speed = agent.speed + 5;
	}


	// TODO make this a better motion model by making first located enemy, 
	// or the nearest enemy, the next direction of motion ??
	// to be done in every frame
	void Update() {

		if (agent != null && targetEnemy != null) {			
			// get distance to the enemy
			distance = Vector3.Distance (agent.transform.position, targetEnemy.transform.position);

			// check if enemy has been reached
			if (distance <= attackDistance) {
				targetEnemy.GetComponent<NavMeshAgent> ().isStopped = true;
				targetEnemy.GetComponent<SpeciesBehavior> ().ReactToHit ();
			}
		} 
		else if(agent != null)
		{
			findNearestEnemy ();
		}


	} 
		

	// finds the nearest enemy and makes them the new navmesh agent target for this player animal object
	// based partially on code from github.com/Brackeys/Tower-Defence-Tutorial
	//     /tree/master/Tower%20Defence%20Unity%20Project/Assets/Scripts/Turret.cs
	public void findNearestEnemy()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		float minDistance = gameBordWidth/2;
		float distance = 0;
		string diet;

		foreach (GameObject enemy in enemies) 
		{
			distance = Vector3.Distance (transform.position, enemy.transform.position);

			if (distance < minDistance) 
			{
				//diet = nearestEnemy.GetComponent<SpeciesBehavior> ().getDietType();

				// only call this an enemy target if it is the correct prey type
				//if (preyList.Contains (diet)) {
					nearestEnemy = enemy;
					minDistance = distance;
				//}
			}
		}

		if (nearestEnemy != null && agent != null) 
		{
			agent.SetDestination (nearestEnemy.transform.position);
		}
			
	}


}
