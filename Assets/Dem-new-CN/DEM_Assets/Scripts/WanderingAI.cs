using UnityEngine;
using System.Collections;

// attach this as a component of the enemy object

public class WanderingAI : MonoBehaviour 
{
	// speed of the motion of the wandering enemy
	// too fast so reducing speed of enemy from 5.0f to 3.0f
	public float speed = 3.0f;


	// distance for an obsticle to be in range for avoidance reaction by the enemy
	public float obstacleRange = 5.0f;
	
	[SerializeField] private GameObject fireballPrefab;
	private GameObject _fireball;

	// the current state of an enemy is alive or dead, default value is true for alive
	private bool _alive;

	// the initial state of an enemy when created is true for alive
	void Start() {
		_alive = true;
	}

	// to be done in every frame
	void Update() {
		// if the current state of the enemy is alive
		if (_alive) {
			// always move the enemy forward, even if it will need to turn
			transform.Translate(0, 0, speed * Time.deltaTime);
			// creat a ray at the same position as the enemy and facing in the same direction as the enemy
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hit;
			// do ray casting to look for a hit, with a radius of 0.75 for the ray
			if (Physics.SphereCast(ray, 0.75f, out hit)) {
				// get the object that was hit by the ray cast
				GameObject hitObject = hit.transform.gameObject;
				// Creat a fireball only if the object hit by the ray cast has a player character component, 
				// and if there is not a current fireball
				if (hitObject.GetComponent<PlayerCharacter>()) {
					if (_fireball == null) {
						// create a new instance of a fireball from the prefab file and cast it to a game object type
						_fireball = Instantiate(fireballPrefab) as GameObject;
						// calculate a the next forward position for the fireball
						_fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
						// move the fireball forward to the next position
						_fireball.transform.rotation = transform.rotation;
					}
				}
				// if the object hit was not a player character,
				// and if the object hit is within the distance for an obsticle to be in 
				// range for avoidance reaction by the enemy,
				// then rotate the enemy by randomly +/- 110 degrees horizontally about the Y-axis
				else if (hit.distance < obstacleRange) {
					float angle = Random.Range(-110, 110);
					transform.Rotate(0, angle, 0);
				}
			}
		}
	}

	// allow other scripts to dynamically change the enemy's state of alive or dead
	public void SetAlive(bool alive) {
		_alive = alive;
	}
}
