using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This needs to be made a component of the animal prefab.

public class PlayerAnimalBehavior : SpeciesBehavior 
{
	// distance for an enemy prey to be in range for attack
	public float attackDistance = 7.0f;
	private int gameBordWidth = 200;
	public NavMeshAgent playerAgent, enemyAgent;
	// the current target enemy to be navigated to for attack
	private GameObject nearestEnemy = null;
	private float distance;

	// the initial state
	void Start() {
		playerAgent = GetComponent<NavMeshAgent>();
		playerAgent.speed = playerAgent.speed * 1.5f;
	}


	// TODO make this a better motion model by making first located enemy, 
	// or the nearest enemy, the next direction of motion ??
	// to be done in every frame
	void Update() {

		if (playerAgent != null && nearestEnemy != null) {
			Debug.Log ("player and enemy NOT null\n");
			// get distance to the enemy
			distance = Vector3.Distance (playerAgent.transform.position, nearestEnemy.transform.position);

			// check if enemy has been reached
			if (distance <= attackDistance) {
				enemyAgent = GetComponent<NavMeshAgent> ();
				enemyAgent.velocity = Vector3.zero;
				enemyAgent.GetComponent<NavMeshAgent> ().isStopped = true;
				playerAgent.velocity = Vector3.zero;
				playerAgent.GetComponent<NavMeshAgent> ().isStopped = true;
				nearestEnemy.GetComponent<SpeciesBehavior> ().ReactToHit ();
			}
		} else if (playerAgent != null && nearestEnemy == null) {
			Debug.Log ("player NOT null, enemy is NULL \n");
			nearestEnemy = findNearestEnemy ();
		} else {
			Debug.Log ("OOPS player agent is NULL\n");
		}


	} 
		

	// finds the nearest enemy and makes them the new navmesh agent target for this player animal object
	// based partially on code from github.com/Brackeys/Tower-Defence-Tutorial
	//     /tree/master/Tower%20Defence%20Unity%20Project/Assets/Scripts/Turret.cs
	public GameObject findNearestEnemy()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		float minDistance = gameBordWidth/2;
		float distance = 0;
		string diet;
		nearestEnemy = null;

		Debug.Log ("initial min distance = " + minDistance + "\n");

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

		Debug.Log ("min distance player to ai = " + minDistance + "\n");

		if (nearestEnemy != null && playerAgent != null) 
		{
			playerAgent.SetDestination (nearestEnemy.transform.position);
		}

		return nearestEnemy;
			
	}


}
