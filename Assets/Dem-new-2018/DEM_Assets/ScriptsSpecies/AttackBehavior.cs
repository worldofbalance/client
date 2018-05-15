using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackBehavior {

	// distance for prey to be in range for attack
	public float attackDistance = 5.0f;
	public int gameBordWidth = 200;


	// finds the nearest neighbor animal or plant based on basic food web with final choices
	// of acceptable food made by the calling function when desired
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


	// finds the nearest enemy AI animal only
	// with no regard for diet of the attacker, and no plants as targets
	public GameObject findNearestEnemy(Vector3 attackerPosition)
	{
		GameObject nearest = null;
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		nearest = findNearestTarget (enemies, attackerPosition);
		return nearest;
	}


	// finds the nearest defender player animal 
	// with no regard for diet the attacker, and no plants as targets
	public GameObject findNearestDefender(Vector3 attackerPosition)
	{
		GameObject nearest = null;
		GameObject[] defenders = GameObject.FindGameObjectsWithTag ("Defender");
		nearest = findNearestTarget (defenders, attackerPosition);
		return nearest;
	}


	// finds the nearest plant only, no animals,
	// with no regard for diet of the attacker
	public GameObject findNearestPlant(Vector3 attackerPosition)
	{
		GameObject nearest = null;
		GameObject[] plants = GameObject.FindGameObjectsWithTag ("Plant");
		nearest = findNearestTarget (plants, attackerPosition);
		return nearest;
	}


	// finds the nearest target species from the given array of target species,
	// with no regard for diet of the attacker
	public GameObject findNearestTarget(GameObject[] targets, Vector3 attackerPosition)
	{
		float minDistance = gameBordWidth;
		float distance = 0;
		GameObject nearest = null;

		foreach (GameObject target in targets) 
		{
			distance = Vector3.Distance (attackerPosition, target.transform.position);

			if (distance < minDistance) 
			{
				nearest = target;
				minDistance = distance;
			}
		}

		return nearest;
	}



	// finds all the animals and plants on the game board
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
