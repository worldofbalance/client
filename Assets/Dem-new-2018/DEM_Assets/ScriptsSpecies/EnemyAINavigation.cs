using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// make this a component of the Enemy Animal AI prefab

public class EnemyAINavigation : MonoBehaviour {

	private EnemyBehavior enemyBehavior;
	public NavMeshAgent agent, target;
	// the current targets to be navigated to for attack
	private GameObject neighbor, treeOfLife;
	private float distance;
	private Vector3 targetLocation;
	private bool hit;
	// distance for an tree of life or prey to be in range for attack
	public float attackDistance = 6.0f;
	int dice;
	protected AttackBehavior attackBehavior;
	string enemyDiet;

	// Use this for initialization
	void Start () 
	{		
		treeOfLife = GameObject.Find("TreeOfLife");
		hit = false;
		attackBehavior = new AttackBehavior ();
		enemyBehavior = this.gameObject.GetComponent<EnemyBehavior>();
		agent = GetComponent<NavMeshAgent>();

		// generates 0 or 1 or 2
		dice = Random.Range (0, 3);
		enemyDiet = enemyBehavior.getDietType();

		// so most of enemy will attack treeOfLife and some will attack player defenders or plants
		if (agent != null && treeOfLife != null && dice <= 1) {
			targetLocation = treeOfLife.transform.position;
			agent.destination = targetLocation;
		} 

	} // end Start()	
		

	// to be done in every frame
	void Update() {

		if (agent != null && !hit && treeOfLife != null && dice <= 1) {			
				// distance from enemy to the tree of life
				distance = Vector3.Distance (agent.transform.position, targetLocation);

				// check if enemy has reached the tree of life
				if (distance <= attackDistance && !hit) {
					agent.isStopped = true;
					hit = true;
					treeOfLife.GetComponent<TreeOfLifeBehavior> ().reactToHit ();
					EnemyController.numberOfEnemies--;
					enemyBehavior.ReactToHit ();
				}
		} 

		if (agent != null && dice > 1) {
			
			if (neighbor != null)  {				
				// only attack and kill this target if it is the correct prey type for this animal
				string targetSpecies = neighbor.GetComponent<SpeciesBehavior> ().getSpecies ();
				if (enemyBehavior.getPreyList ().Contains (targetSpecies)) {
					// get distance to the enemy
					distance = Vector3.Distance (agent.transform.position, neighbor.transform.position);
					// check if enemy has been reached
					if (distance <= attackBehavior.attackDistance) {
						//Debug.Log ("attaking the player\n");
						target = neighbor.GetComponent<NavMeshAgent> ();
						if (target != null) {
							target.isStopped = true;
						}

						agent.isStopped = true;
						//hit = true;
						neighbor.GetComponent<SpeciesBehavior> ().ReactToHit ();
					}
				}
			} else if (neighbor == null) {
				// if no nearest enemy, find a new enemy target
				//Debug.Log ("Need to get new player target\n");
				agent.isStopped = false;
				//hit = false;

				// find the nearest defender or plant
				// find nearest enemy or plants
				if (enemyDiet.Equals ("Herbivore")) {
					neighbor = attackBehavior.findNearestPlant (gameObject.transform.position);
					if(neighbor != null)
						agent.SetDestination (neighbor.transform.position);
				} else {
					neighbor = attackBehavior.findNearestDefender (gameObject.transform.position);
					if(neighbor != null)
						agent.SetDestination (neighbor.transform.position);
				}

			} else {
				//Debug.Log ("OOPS enemy agent is NULL\n");
			}
		}
		
	} // end Update()


}
