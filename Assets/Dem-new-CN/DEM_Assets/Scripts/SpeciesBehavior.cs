using UnityEngine;
using System.Collections;


// attach this as a component of the species prefabs for animals and plants

public class SpeciesBehavior : MonoBehaviour 
{
	// health status, max health = 5, min health = 0
	private bool alive;
	private int health;

	public const int maxHealth = 5;
	public const int injured = 3;
	public const int dead = 0;


	// the initial state
	void Start() {
		this.alive = true;
		this.health = 5;

	}
		
	public void setAlive(bool alive) {
		this.alive = alive;
	}

	public bool getAlive(){
		return this.alive;
	}

	public void Hurt(int damage) {
		damage = health - damage;
		setHealth(damage);
	}

	public void Heal(int healing) {
		healing = health + healing;
		setHealth(healing);
	}

	// makes sure that all new health values are within the max and min range for health
	private void setHealth(int value){

		if (value <= dead)
			health = dead;
		else if (value >= maxHealth)
			health = maxHealth;
		else
			health = value;
	}

	public int getHealth() { 
		return this.health; 
	}

}
