using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this needs to be made a component of the TreeOfLife object

public class TreeOfLifeBehavior : MonoBehaviour 
{
	private const int injured = 3;
	private const int dead = 0;
	private int health;

	private Material material;
	public Material healthyTreeOfLife;
	public Material injuredTreeOfLife;
	public Material deadTreeOfLife;

	// Update is called once per frame
	// Changes the look of the Tree of Life based on its health state.
	void Update () {

		if ((health <= injured) && (health > dead)) {
			material = GetComponent<Renderer>().material;
			material = injuredTreeOfLife;
		}

		if (health <= dead) {
			// Let the Tree of Life animate dying and then be destroyed.
			StartCoroutine(Die());
		}
	}

	// Overide because Tree of Life does not dies instantly.
	// Instead, the tree of life reduces health untill dead state has been reached.
	public void ReactToHit() {
		health--;
	}
		

	// Overide to add specific Tree of Life animations.
	private IEnumerator Die() {
		// Make the Tree of Life look dead.
		material = GetComponent<Renderer>().material;
		material = deadTreeOfLife;
		// The Tree of Life falls over on its side.
		this.transform.Rotate(-90, 0, 0);
		// It then lays dead for 2 seconds, while the function yields control,
		// so that the game keeps on playing.
		yield return new WaitForSeconds(2.0f);
		// After 1.5 seconds, the dead object is destroyed, so it leaves the game.
		Destroy(this.gameObject);
	}
}
