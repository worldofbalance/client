using UnityEngine;
using System.Collections;


// attach this as a component of an empty, invisible, generic game object
// changed so that 2 enemies can be in the scene at any time

public class EnemyController : MonoBehaviour 
{
	// for linking to the prefabricated object, which is stored as a file in the game assets
	[SerializeField] private GameObject enemyPrefab;
	// the current enemy instance in the game scene
	private GameObject enemyOne, enemyTwo;
	private bool next;


	// For every frame of the game scene, if the enemy has died, spawn a new enemy.
	// If the enemy == null, the enemy has died, or the game has just started.
	void Update() 
	{
		next = false;
		if (enemyOne == null) 
		{
			// create a new instance of an enemy based on the enemyPrefab file,
			// and cast its type to game object
			enemyOne = Instantiate(enemyPrefab) as GameObject;
			placeEnemy (enemyOne);
		}

		next = true;
		if (enemyTwo == null) 
		{
			// create a new instance of an enemy based on the enemyPrefab file,
			// and cast its type to game object
			enemyTwo = Instantiate(enemyPrefab) as GameObject;
			placeEnemy (enemyTwo);
		}

	}


	// Put the new enemy at a point on the edge of the game board.
	// Have the enemy start facing in the direction of the Tree of Life,
	// which is at the center of the game board.
	private void placeEnemy(GameObject enemy){
		
		int x;
		float angle;

		if (next) {
			x = DemSceneConstants.MaxEdge;
			angle = 0;
		}
		else {
			x = DemSceneConstants.MinEdge;
			angle = 360;
		}
		
		int z = Random.Range(DemSceneConstants.MinEdge, DemSceneConstants.MaxEdge);
		enemy.transform.position = new Vector3(x, 1, z);
		enemy.transform.Rotate(0, angle, 0);
	}

}
