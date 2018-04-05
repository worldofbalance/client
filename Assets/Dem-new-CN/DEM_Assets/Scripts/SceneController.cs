using UnityEngine;
using System.Collections;

// attach this as a component of an empty, invisible, generic game object
// changed so that 2 enemies can be in the scene at any time

public class SceneController : MonoBehaviour 
{
	// for linking to the prefabricated object, which is stored as a file in the game assets
	[SerializeField] private GameObject enemyPrefab;
	// the current enemy instance in the game scene
	private GameObject _enemyOne;
	private GameObject _enemyTwo;

	// For every frame of the game scene, if the enemy has died, spawn a new enemy.
	// If the enemy == null, the enemy has died, or the game has just started.
	void Update() 
	{
		if (_enemyOne == null) 
		{
			// create a new instance of an enemy based on the enemyPrefab file,
			// and cast its type to game object
			_enemyOne = Instantiate(enemyPrefab) as GameObject;
			// put the new enemy at the starting location of point (0, 1, 0);
			// change to put the enemy at a random starting location point;
			int x = Random.Range(-24, 24);
			int z = Random.Range(-24, 24);
			_enemyOne.transform.position = new Vector3(x, 1, z);
			// have the enemy start facing in any random horizontal direction
			float angle = Random.Range(0, 360);
			_enemyOne.transform.Rotate(0, angle, 0);
		}

		if (_enemyTwo == null) 
		{
			// create a new instance of an enemy based on the enemyPrefab file,
			// and cast its type to game object
			_enemyTwo = Instantiate(enemyPrefab) as GameObject;
			// put the new enemy at the starting location of point (0, 1, 0);
			// change to put the enemy at a random starting location point;
			int x = Random.Range(-24, 24);
			int z = Random.Range(-24, 24);
			_enemyTwo.transform.position = new Vector3(x, 1, z);
			// have the enemy start facing in any random horizontal direction
			float angle = Random.Range(0, 360);
			_enemyTwo.transform.Rotate(0, angle, 0);
		}

	}

}
