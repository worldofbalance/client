using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this needs to be made a component of the TreeOfLife object

public class TreeOfLifeBehavior : SpeciesBehavior 
{
	private Material material;
	public Material healthyTreeOfLife;
	public Material injuredTreeOfLife;
	public Material deadTreeOfLife;

	// Update is called once per frame
	// Changes the look of the Tree of Life based on its health state.
	void Update () {

		int treeHealth = getHealth();

		if ((treeHealth <= injured) && (treeHealth > dead)) {
			// This changes the appearance of Tree of Life as a visual clue
			// to the player that the Tree is at risk of dying soon.
			material = GetComponent<Renderer>().material;
			material = injuredTreeOfLife;
		}

		if (treeHealth <= dead) {
			// Make the Tree of Life look dead.
			material = GetComponent<Renderer>().material;
			material = deadTreeOfLife;
			// Start a coroutine Die to let the object react to being hit
			StartCoroutine(Die());
			// TODO need to notify game that game is over due to death of Tree of Life
		}
	}


	// Override, Tree of Life does not dies instantly.
	// Instead, the tree of life reduces health untill dead state has been reached.
	public void ReactToHit() {
		Hurt (1);
	}



}
