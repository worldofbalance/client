using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// make this a component of the Enemy Animal AI prefab

public class EnemyAINavigation : MonoBehaviour {

	private EnemyBehavior behavior;
	public NavMeshAgent agent;
	private float distance;
	private GameObject treeOfLife;
	private Vector3 treeOfLifeLocation;
	private bool treeOfLifeHit;
	// distance for an tree of life or prey to be in range for attack
	public float attackDistance = 7.0f;


	// Use this for initialization
	void Start () {		
		treeOfLife = GameObject.Find("TreeOfLife");
		treeOfLifeHit = false;
		behavior = this.gameObject.GetComponent<EnemyBehavior>();
		agent = GetComponent<NavMeshAgent>();

		if (agent != null && treeOfLife != null) {
			treeOfLifeLocation = treeOfLife.transform.position;
			agent.destination = treeOfLifeLocation;
		}

	}		


	// to be done in every frame
	void Update() {

		if (agent != null && !treeOfLifeHit && treeOfLife != null) {			
			// distance from enemy to the tree of life
			distance = Vector3.Distance (agent.transform.position, treeOfLifeLocation);
		
			// check if enemy has reached the tree of life
			if (distance <= attackDistance && !treeOfLifeHit) {
				agent.isStopped = true;
				treeOfLifeHit = true;
				treeOfLife.GetComponent<TreeOfLifeBehavior> ().reactToHit ();
				if (behavior != null) {
					EnemyController.numberOfEnemies--;
					behavior.ReactToHit ();
				}
			}
		} 
			
	}
		

}
