using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackBehavior {

	// distance for prey to be in range for attack
	public float attackDistance = 6.0f;
	public int gameBordWidth = 200;


	// finds the nearest neighbor animal or plant based on basic food web
	// more exclusions are needed by the calling fuction to match food web correctly 
	public GameObject findNearestNeighbor(string diet, Vector3 attackerPosition)
	{
		float minDistance = gameBordWidth;
		float distance = 0;
		GameObject nearest = null;
		GameObject[] neighbors = null;
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		GameObject[] plants = GameObject.FindGameObjectsWithTag ("Plant");

		switch (diet) 
		{
		case "Carnivore":
			neighbors = enemies;
			break;
		case "Herbivore":
			neighbors = plants;
			break;
		case "Omnivore":
			neighbors = new GameObject[enemies.Length + plants.Length];
			enemies.CopyTo (neighbors, 0);
			plants.CopyTo (neighbors, enemies.Length);
			break;
		}

		foreach (GameObject neighbor in neighbors) 
		{
			distance = Vector3.Distance (attackerPosition, neighbor.transform.position);

			if (distance < minDistance) 
			{
				nearest = neighbor;
				minDistance = distance;
			}
		}

		return nearest;
	}


	// finds the nearest enemy AI animals only
	public GameObject findNearestEnemy(Vector3 attackerPosition)
	{
		float minDistance = gameBordWidth;
		float distance = 0;
		GameObject nearest = null;
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");

		foreach (GameObject enemy in enemies) 
		{
			distance = Vector3.Distance (attackerPosition, enemy.transform.position);

			if (distance < minDistance) 
			{
				nearest = enemy;
				minDistance = distance;
			}
		}

		return nearest;
	}


	public ArrayList getAllNeighbors()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		GameObject[] defenders = GameObject.FindGameObjectsWithTag ("Defender");
		GameObject[] plants = GameObject.FindGameObjectsWithTag ("Plant");

		ArrayList allNeighbors = new ArrayList();
		allNeighbors.AddRange (plants);
		allNeighbors.AddRange (enemies);
		allNeighbors.AddRange (defenders);

		return allNeighbors;
	}


}
