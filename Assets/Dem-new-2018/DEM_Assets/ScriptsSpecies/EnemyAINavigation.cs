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
	private bool treeOfLifeHit;


	// Use this for initialization
	void Start () {		
		treeOfLife = GameObject.Find("TreeOfLife");
		agent = GetComponent<NavMeshAgent>();
		agent.destination = treeOfLife.transform.position;
		treeOfLifeHit = false;
		behavior = this.gameObject.GetComponent<EnemyBehavior>();
	}
		


	// to be done in every frame
	void Update() {
		
		// distance from enemy to the tree of life
		if (treeOfLife != null) {
			distance = Vector3.Distance (agent.transform.position, treeOfLife.transform.position);
		}

		// check if enemy has reached the tree of life
		if (distance <= 7.0 && !treeOfLifeHit) 
		{
			if (agent != null) {
				agent.isStopped = true;
			}

			if (treeOfLife != null) {
				treeOfLifeHit = true;
				treeOfLife.GetComponent<TreeOfLifeBehavior> ().reactToHit ();
			}

			if (behavior != null) {
				EnemyController.numberOfEnemies--;
				behavior.ReactToHit ();
			}
		}

	}




}
