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

		if (playerAgent != null && nearestEnemy != null && !hit) 
		{
			// only attack and kill this enemy if it is the correct prey type for this animal
			string targetSpecies = nearestEnemy.GetComponent<SpeciesBehavior> ().getSpecies();
			if (preyList.Contains (targetSpecies)) {
					// get distance to the enemy
					distance = Vector3.Distance (playerAgent.transform.position, nearestEnemy.transform.position);
					// check if enemy has been reached
					if (distance <= attackBehavior.attackDistance) {
						//Debug.Log ("attaking the enemy\n");
						enemyAgent = nearestEnemy.GetComponent<NavMeshAgent> ();
						if (enemyAgent != null) {
							enemyAgent.isStopped = true;
						}

						playerAgent.isStopped = true;
						hit = true;
						EnemyController.numberOfEnemies--;
						nearestEnemy.GetComponent<SpeciesBehavior> ().ReactToHit ();

					}
			} 
		} 
		else if (playerAgent != null && nearestEnemy == null) 
		{
			// if no nearest enemy, find a new enemy target
			//Debug.Log ("Need to get new enemy target\n");
			playerAgent.isStopped = false;
			hit = false;
	
				// find nearest enemy or plants
				if(dietType.Equals("Herbivore")) {
					nearestEnemy = attackBehavior.findNearestPlant(gameObject.transform.position);
					if(nearestEnemy != null)
						playerAgent.SetDestination (nearestEnemy.transform.position);
				} else {
					nearestEnemy = attackBehavior.findNearestEnemy(gameObject.transform.position);
					if(nearestEnemy != null)
						playerAgent.SetDestination (nearestEnemy.transform.position);
				}

		} else {
			//Debug.Log ("OOPS player agent is NULL\n");
		}

	} // end Update()
		
}
