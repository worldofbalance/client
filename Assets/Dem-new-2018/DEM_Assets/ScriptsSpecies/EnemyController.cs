using UnityEngine;
using System.Collections;


// attach this as a component of an empty, invisible, generic game object
// changed so that 2 enemies can be in the scene at any time

public class EnemyController : MonoBehaviour 
{
	// for linking to the prefabricated object, which is stored as a file in the game assets
	public GameObject enemyPrefab;
	private GameObject enemy;
	// the current number of enemies alive in the game
	public static int numberOfEnemies;
	// The maximum number of enemies on the game board at the same time.
	public static int maxNumberOfEnemies = 10;
	// for the timing of automatic enemy spawinings
	private float timeStep = 2.0f;
	private float oldTime;
	// The edges of the game board, this is used for species placement on the board.
	// Minus 4 to accomidate the walls around the board.
	public Transform dimentionsGround;
	private int groundWidth;
	// for making the enemy species
	private SpeciesFactory factory;


	// Use this for initialization
	void Start(){	
		oldTime = Time.time;
		groundWidth = (int)(dimentionsGround.localScale.x / 2) - 4;
		numberOfEnemies = 0;
		factory = GameObject.Find("MasterController").GetComponent<SpeciesFactory>();
	}


	// For every frame of the game scene
	// Every so many seconds, spawn a new enemy.
	void Update() 
	{
		if ((numberOfEnemies < maxNumberOfEnemies) && ((Time.time - oldTime) > timeStep))
		{
			// Create a new instance of an enemy based on a random animal species.
			enemy = factory.getRandomAnimal (true);
			placeEnemy (enemy);
			numberOfEnemies++;
			oldTime = Time.time;
		}
	}


	// Put the new enemy at a point on the edge of the game board.
	// Have the enemy start facing in the direction of the Tree of Life,
	// which is at the center of the game board.
	private void placeEnemy(GameObject enemy){

		int x = groundWidth;
		int z = Random.Range(-groundWidth, groundWidth);
		float angle = 90;		

		if (Random.Range(0, 2) == 0) {
			x = -x;
		}

		enemy.transform.position = new Vector3(x, 1, z);
		enemy.transform.Rotate(0, angle, 0);
	}

		
}
