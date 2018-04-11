using UnityEngine;
using System.Collections;

// attach this as a component of the species prefabs for carnivore, omnivore, herbivore

public class SpeciesAttack : MonoBehaviour 
{
	// speed of the motion of the wandering enemy
	// too fast so reducing speed of enemy from 5.0f to 3.0f
	public float speed = 3.0f;
	// distance for an obsticle to be in range for avoidance reaction by the enemy
	public float obstacleRange = 5.0f;
	// health status, max health = 5, min health = 0
	private bool alive;
	private int health;
	private const int maxHealth = 5;
	private const int minHealth = 0;

	// the initial state
	void Start() {
		this.alive = true;
		this.health = 5;
	}


	// to be done in every frame
	void Update() {

		if (alive) {
			// always move the species forward, even if it will need to turn
			transform.Translate (0, 0, speed * Time.deltaTime);
			// creat a ray at the same position as the enemy and facing in the same direction as the species
			Ray ray = new Ray (transform.position, transform.forward);
			RaycastHit hit;
			// do ray casting to look for a hit, with a radius of 0.75 for the ray
			if (Physics.SphereCast (ray, 0.75f, out hit)) {
				// get the object that was hit by the ray cast
				GameObject hitObject = hit.transform.gameObject;
				// get the reactive target component from the object that was hit
				// the target returned is null if the object hit does not contain that type of component
				ReactiveTarget target = hitObject.GetComponent<ReactiveTarget> ();

				if ((target != null) && (hit.distance < obstacleRange)){
					// if object is prey, tell the object to react to being hit
					// TODO add test for prey
					target.ReactToHit ();
				} 
				else if (hit.distance < obstacleRange) {
					// If the object hit was not a species of prey, (like if it is a wall),
					// and if the object hit is within the distance for obsticle avoidance reaction,
					// then rotate by randomly +/- 110 degrees horizontally about the Y-axis.
					float angle = Random.Range (-110, 110);
					transform.Rotate (0, angle, 0);
				}
			}
		}
	}


	// This shows where an object was hit by the ray at point pos,
	// by creating a sphere at that point for one second.
	private IEnumerator SphereIndicator(Vector3 pos) {
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = pos;

		// pause this function for one second, so that the user can see the sphere 
		// indicating where the ray hit an object, and meanwhile keep the game playing,
		// then come back to this function to continue to the next line in one second.
		yield return new WaitForSeconds(1);

		// destroy the temporary sphere object
		Destroy(sphere);
	}


	public void setAlive(bool alive) {
		this.alive = alive;
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

		if (value <= minHealth)
			health = minHealth;
		else if (value >= maxHealth)
			health = maxHealth;
		else
			health = value;
	}

	public int getHealth() { 
		return this.health; 
	}

}
