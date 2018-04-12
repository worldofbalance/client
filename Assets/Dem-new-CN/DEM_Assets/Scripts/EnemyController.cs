using UnityEngine;
using System.Collections;


// attach this as a component of an empty, invisible, generic game object
// changed so that 2 enemies can be in the scene at any time

public class EnemyController : MonoBehaviour 
{
	// for linking to the prefabricated object, which is stored as a file in the game assets
	[SerializeField] private GameObject enemyPrefab;
	// the current enemy instances in the game scene
	private ArrayList enemies;
	private GameObject enemy;
	private bool next = true;

	// Use this for initialization
	void Start() 
	{
		makeNewEnemy();
	}

	// For every frame of the game scene, if the enemy has died, spawn a new enemy.
	// TODO spawn a new enemy up to max number of enemies every so many seconds.
	void Update() 
	{
		if (enemy == null) 
		{
			makeNewEnemy ();
			placeEnemy (enemy);
		}

		/**
		if (enemies.Count < DemSceneConstants.MaxNumberOfEnemies) 
		{
			// create a new instance of an enemy based on the enemyPrefab file,
			// and cast its type to game object
			enemy = Instantiate(enemyPrefab) as GameObject;
			placeEnemy (enemy);
			enemies.Add (enemy);
		}
		**/
	}


	private void makeNewEnemy() 
	{
		Material material;
		// Create a new instance of an enemy based on the enemy prefab file,
		// and cast its type to game object.
		enemy = Instantiate(enemyPrefab) as GameObject;
		// Randomly assign the enemy an animal species type.
		SpeciesBehavior behavior = enemy.GetComponent<SpeciesBehavior>();
		DemSceneConstants.SpeciesType type = DemSceneConstants.getRandomAnimalType ();
		behavior.setSpeciesType (type);
		behavior.setPreyList (type);
	}


	// Put the new enemy at a point on the edge of the game board.
	// Have the enemy start facing in the direction of the Tree of Life,
	// which is at the center of the game board.
	private void placeEnemy(GameObject enemy){
		
		int x = DemSceneConstants.FloorXWidth/2;
		int z = DemSceneConstants.FloorZWidth/2;
		float angle = 90;		
		int z_random = Random.Range(-z, z);
		int x_random = x;

		if (Random.Range(0, 2) == 0) {
			x_random = -x;
		}

		enemy.transform.position = new Vector3(x, 1, z);
		enemy.transform.Rotate(0, angle, 0);
	}


}
