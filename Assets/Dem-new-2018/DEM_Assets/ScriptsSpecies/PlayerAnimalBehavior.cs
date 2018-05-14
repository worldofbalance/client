using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This needs to be made a component of the animal prefab.

public class PlayerAnimalBehavior : SpeciesBehavior 
{
	// distance for an enemy prey to be in range for attack
	public float attackDistance = 7.0f;
	// for finding nearest enemy
	public Transform gameBord;
	private int gameBordWidth;
	public NavMeshAgent agent;

	// the current target enemy to be navigated to for attack
	GameObject targetEnemy = null;

	// the initial state
	void Start() {
		gameBordWidth = (int)(gameBord.localScale.x);
		agent = GetComponent<NavMeshAgent>();
	}

	// TODO make this a better motion model by making first located enemy, 
	// or the nearest enemy, the next direction of motion ??
	// to be done in every frame
	void Update() {

		float distance;
		string enemyDietType;

		if (getAlive ()) 
		{
			if (targetEnemy == null) {
				targetEnemy = findNearestEnemy ();
				if (targetEnemy != null) {
					enemyDietType = targetEnemy.GetComponent<SpeciesBehavior> ().getDietType();
					if (preyList.Contains (enemyDietType)) {
						agent.destination = targetEnemy.transform.position;
					} 
					else {
						targetEnemy = null;
					}
				}
			}
			else
			{				
				// get distance to the enemy
				distance = Vector3.Distance (agent.transform.position, targetEnemy.transform.position);

				// check if enemy has been reached
				if (distance <= attackDistance && agent != null && targetEnemy != null) 
				{
					agent.isStopped = true;
					targetEnemy.GetComponent<NavMeshAgent> ().isStopped = true;
					targetEnemy.GetComponent<SpeciesBehavior> ().ReactToHit ();
				}
			}
		} 

	} 




	// finds the nearest enemy and makes them the new navmesh agent target for this player animal object
	// based partially on code from github.com/Brackeys/Tower-Defence-Tutorial
	//     /tree/master/Tower%20Defence%20Unity%20Project/Assets/Scripts/Turret.cs
	public GameObject findNearestEnemy()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		float minDistance = 10 * gameBordWidth;
		float distance = 0;
		GameObject nearestEnemy = null;

		foreach (GameObject enemy in enemies) 
		{
			distance = Vector3.Distance (transform.position, enemy.transform.position);
			if (distance < minDistance) {
				minDistance = distance;
				nearestEnemy = enemy;
			}
		}

		return nearestEnemy;
	}


}
