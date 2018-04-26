using UnityEngine;
using System.Collections;


// attach this as a component of an empty, invisible, generic game object
// changed so that 2 enemies can be in the scene at any time

public class EnemyController : MonoBehaviour 
{
	// for linking to the prefabricated object, which is stored as a file in the game assets
	[SerializeField] private GameObject enemyPrefab;
	private GameObject enemy;
	private float timeStep = 2.0f;
	private float oldTime;
	// the current number of enemies alive in the game
	public static int numberOfEnemies = 0;

	// Use this for initialization
	void Start(){	
		oldTime = Time.time;
	}

	// For every frame of the game scene
	// Every so many seconds, spawn a new enemy, up to max number of enemies.
	void Update() 
	{
		if ((numberOfEnemies < DemSceneConstants.maxNumberOfEnemies) && ((Time.time - oldTime) > timeStep))
		{
			// create a new instance of an enemy 
			enemy = makeNewEnemy ();
			placeEnemy (enemy);
			oldTime = Time.time;
		}
	}


	private GameObject makeNewEnemy() 
	{
		Material material;
		// Create a new instance of an enemy based on the enemy prefab file,
		// and cast its type to game object.
		GameObject enemy = Instantiate(enemyPrefab) as GameObject;
		// Randomly assign the enemy an animal species type.
		SpeciesBehavior behavior = enemy.GetComponent<SpeciesBehavior>();
		if (behavior != null) {
			DemSceneConstants.SpeciesType type = DemSceneConstants.getRandomAnimalType ();
			behavior.setSpeciesType (type);
			behavior.setPreyList (type);
		}
		return enemy;
	}


	// Put the new enemy at a point on the edge of the game board.
	// Have the enemy start facing in the direction of the Tree of Life,
	// which is at the center of the game board.
	private void placeEnemy(GameObject enemy){

		int width = DemSceneConstants.groundWidth / 2;
		int x = width;
		int z = Random.Range(-width, width);
		float angle = 90;		

		if (Random.Range(0, 2) == 0) {
			x = -x;
		}

		enemy.transform.position = new Vector3(x, 1, z);
		enemy.transform.Rotate(0, angle, 0);
	}


}
