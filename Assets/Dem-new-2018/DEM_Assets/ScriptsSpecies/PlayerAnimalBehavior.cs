using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This needs to be made a component of the animal prefab.

public class PlayerAnimalBehavior : SpeciesBehavior 
{
	public NavMeshAgent playerAgent, enemyAgent;
	// the current target enemy to be navigated to for attack
	private GameObject nearestEnemy = null;
	private float distance;
	private bool hit;

	// the initial state
	void Start() {
		playerAgent = GetComponent<NavMeshAgent>();
		playerAgent.speed = playerAgent.speed * 2.0f;
	}


	// to be done in every frame
	// If the nearest enemy is prey attack, otherwise defend by hopefully hearding it away insted
	void Update() {
		
		string diet;

		if (playerAgent != null && nearestEnemy != null && !hit) 
		{
			// only attack and kill this enemy if it is the correct prey type for this animal
			//diet = nearestEnemy.GetComponent<SpeciesBehavior> ().getDietType();
			//if (preyList.Contains (diet)) {
				// get distance to the enemy
				distance = Vector3.Distance (playerAgent.transform.position, nearestEnemy.transform.position);
				// check if enemy has been reached
				if (distance <= attackBehavior.attackDistance) 
				{
					enemyAgent = nearestEnemy.GetComponent<NavMeshAgent> ();
					if (enemyAgent != null) {
						enemyAgent.isStopped = true;
					}

					playerAgent.isStopped = true;
					hit = true;
					nearestEnemy.GetComponent<SpeciesBehavior> ().ReactToHit ();

				}
			//}
		} 
		else if (playerAgent != null && nearestEnemy == null) 
		{
			playerAgent.isStopped = false;
			hit = false;
			// if no nearest enemy, find a new enemy target
			//nearestEnemy = attackBehavior.findNearestNeighbor (dietType, gameObject.transform.position);
			nearestEnemy = attackBehavior.findNearestEnemy(gameObject.transform.position);
			if (nearestEnemy != null) {
				playerAgent.SetDestination (nearestEnemy.transform.position);
			}

		} else {
			Debug.Log ("OOPS player agent is NULL\n");
		}


	} 
		


}
