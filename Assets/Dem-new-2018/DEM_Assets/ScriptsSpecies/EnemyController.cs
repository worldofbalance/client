using UnityEngine;
using System.Collections;


// attach this as a component of an empty, invisible, generic game object

public class EnemyController : MonoBehaviour 
{
	// the enemy AI prefab
	private GameObject enemy;
	private GameObject testPlayer;
	// the current number of enemies alive in the game
	public static int numberOfEnemies = 0;
	// The maximum number of enemies on the game board at the same time.
	public static int maxNumberOfEnemies = 20;
	// for the timing of automatic enemy spawinings
	public float timeStep = 4.0f;
	private float oldTime;
	// The edges of the game board, this is used for species placement on the board.
	// Minus 4 to accomidate the walls around the board.
	public Transform dimentionsGround;
	private int groundWidth;
	// for making the enemy species
	private Species3DFactory factory;


	// Use this for initialization
	void Start(){	
		oldTime = Time.time;
		groundWidth = (int)(dimentionsGround.localScale.x / 2) - 4;
		factory = gameObject.GetComponent<Species3DFactory> ();
	}


	// For every frame of the game scene
	// Every so many seconds, spawn a new enemy.
	void Update() 
	{
		if ((numberOfEnemies < maxNumberOfEnemies) && ((Time.time - oldTime) > timeStep))
		{
			numberOfEnemies++;
			// Create a new instance of an enemy based on a random animal species.
			enemy = factory.getRandomAnimal (true);
			placeEnemy (enemy);

			// for testing only
			testPlayer = factory.getRandomAnimal(false);
			placeEnemy (testPlayer);

			oldTime = Time.time;
		}

	}


	// Put the new animal at a point on the edge of the game board.
	// Have the enemy start facing in the direction of the Tree of Life,
	// which is at the center of the game board.
	private void placeEnemy(GameObject enemy){

		int x = groundWidth;
		int z = Random.Range(-groundWidth, groundWidth);
		float angle = 90;		

		if (Random.Range(0, 2) == 0) {
			x = -x;
		}

		enemy.transform.position = new Vector3(x, 0, z);
		enemy.transform.Rotate(0, angle, 0);
	}
		
}
