using UnityEngine;
using System.Collections;

// This needs to be made a component of the player.
// lets the player react to being hit with a fireball from the enemy,
// by losing health points.

public class PlayerCharacter : MonoBehaviour {
	private int _health;

	// when the player is initialized (created), set its health value to 5
	void Start() {
		_health = 5;
	}

	// changes the health value by the damage amount, and then displays
	// the new health value to the console, which is only viewable in the Unity IDE
	public void Hurt(int damage) {
		_health -= damage;
		Debug.Log("Player Health: " + _health);
	}

}
