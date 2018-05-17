using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this needs to be made a component of the TreeOfLife object

public class TreeOfLifeBehavior : MonoBehaviour 
{
	private Renderer render;
	public Material healthyTreeOfLife;
	public Material injuredTreeOfLife;
	public Material deadTreeOfLife;

	// health status
	private bool alive;
	public static int treeHealth;
	private const int maxHealth = 6;
	private const int injured = 3;
	private const int dead = 0;

	// the initial state
	void Start() {
		this.alive = true;
		treeHealth = maxHealth;
		render = GetComponent<Renderer> ();
	}

	// Update is called once per frame
	// Changes the look of the Tree of Life based on its health state.
	void Update () {

	}

	public void reactToHit() {
		treeHealth--;

		if (treeHealth <= dead) {
			// Make the Tree of Life look dead.
			render.material = deadTreeOfLife;
			setAlive(false);
			// Start a coroutine Die to let the object react to being hit
			StartCoroutine (Die ());
			// TODO need to notify game that game is over due to death of Tree of Life
		} 
		else if (treeHealth <= injured) {
			// This changes the appearance of Tree of Life as a visual clue
			// to the player that the Tree is at risk of dying soon.
			render.material = injuredTreeOfLife;
		}
	}
		
	public IEnumerator Die() {
		// The object reacts to being hit by falling over,
		this.transform.Rotate(-75, 0, 0);
		// and then laying dead for 1.5 seconds, while the function yields control,
		// so that the game keeps on playing.
		yield return new WaitForSeconds(1.5f);
		// After 1.5 seconds, the dead object is destroyed, so it leaves the game.
		Destroy(this.gameObject);
	}
		
	public void setAlive(bool alive) {
		this.alive = alive;
	}

	public bool getAlive(){
		return this.alive;
	}
}
