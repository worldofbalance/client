using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This needs to be made a component of the plant prefab.
// This lets the species and enemy interact.

public class PlantCharacter : MonoBehaviour {
	private int _health;

	// when the plant species is initialized (created), set its health value to 5
	void Start() {
		_health = 5;
	}

	// changes the health value by the damage amount, and then displays
	// the new health value to the console, which is only viewable in the Unity IDE
	public void Hurt(int damage) {
		_health -= damage;
		Debug.Log("Plant Health: " + _health);
	}

}
