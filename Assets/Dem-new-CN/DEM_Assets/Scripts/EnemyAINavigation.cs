using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// make this a component of the Enemy Animal AI prefab

public class EnemyAINavigation : MonoBehaviour {

	// set this to the tree of life prefab in Unity Inspector
	public Transform locationTreeOfLife;
	private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		agent.destination = locationTreeOfLife.position;
	}
		
	// to be done in every frame
	void Update() {

		// check if enemy has reached the tree of life
		if (agent.remainingDistance <= agent.stoppingDistance) {
			TreeOfLifeBehavior tree = GetComponent<TreeOfLifeBehavior> ();
			if (tree != null) {
				tree.ReactToHit ();
			}
			SpeciesBehavior behavior = GetComponent<SpeciesBehavior> ();
			if (behavior != null) {
				EnemyController.numberOfEnemies--;
				behavior.ReactToHit ();
			}
		}

	}
}
