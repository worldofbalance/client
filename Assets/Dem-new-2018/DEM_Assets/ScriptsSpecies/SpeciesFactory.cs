using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpeciesFactory : MonoBehaviour
{
	// Omnivore = 0, Carnivore = 1, Herbivore = 2, these are the constants used in the database.
	// Added Plant = 3 and TreeOfLife = 4, for easier programming of collision and attack behaviors.
	public enum SpeciesType {Omnivore, Carnivore, Herbivore, Plant, TreeOfLife};
	public int numAnimalTypes = 3;
	// These are for linking to the prefabricated object, which is stored as a file in the game assets.
	public GameObject animalPrefab;
	public GameObject plantPrefab;
	public GameObject enemyAIPrefab;
	// The materials for the animals.
	public Material mat_EnemyCarnivore, mat_EnemyOmnivore, mat_EnemyHerbivore;
	public Material mat_PlayerCarnivore, mat_PlayerOmnivore, mat_PlayerHerbivore;


	public GameObject getPlant()
	{
		GameObject plant = Instantiate(plantPrefab) as GameObject;
		return plant;
	}


	public SpeciesType getRandomAnimalType()
	{
		System.Array values = System.Enum.GetValues (typeof(SpeciesType));
		// need to cast the enums to an array of ints
		SpeciesType species = (SpeciesType)( ((int[])values)[Random.Range(0,numAnimalTypes)] );
		return species;
	}


	// Create a new instance of a species from the prefab file,
	// and set its species type and list of prey.
	public GameObject getRandomAnimal(bool isEnemy)
	{
		SpeciesType species = getRandomAnimalType();
		ArrayList prey = new ArrayList ();
		prey = setAnimalPrey (species);
		GameObject animal = null;

		if (isEnemy) {
			animal = Instantiate(enemyAIPrefab) as GameObject;
			animal.GetComponent<EnemyBehavior> ().setSpeciesType (species);
			animal.GetComponent<EnemyBehavior> ().setPreyList (prey);
		} else {
			animal = Instantiate(animalPrefab) as GameObject;
			animal.GetComponent<AnimalBehavior> ().setSpeciesType (species);
			animal.GetComponent<AnimalBehavior> ().setPreyList (prey);
		}

		setAnimalMaterial (animal, species, isEnemy);
		return animal;
	}


	/**
	* food web hard coded for testing of code concepts
	* 
	* Omnivore -> Omnivore, Carnivore, Herbivore, Plant, TreeOfLife
	* Carnivore -> Omnivore, Carnivore, Herbivore
	* Herbivore -> Plant, TreeOfLife
	* Plant -> empty list
	* 
	* TODO: Lion, Buffalo, Grass and Herbs, Bush Pig
	* Tree Mouse, Cockroach, Decaying Material, 
	* Trees and Shrubs
	* 
	* **/
	public ArrayList setAnimalPrey(SpeciesFactory.SpeciesType species)
	{
		ArrayList prey = new ArrayList();

		switch (species) 
		{
		case SpeciesFactory.SpeciesType.Carnivore:
			prey.Add (SpeciesType.Omnivore);
			prey.Add (SpeciesType.Carnivore); 
			prey.Add (SpeciesType.Herbivore);
			break;
		case SpeciesFactory.SpeciesType.Omnivore: 
			prey.Add (SpeciesType.Omnivore);
			prey.Add (SpeciesType.Carnivore); 
			prey.Add (SpeciesType.Herbivore);
			prey.Add (SpeciesType.Plant);
			break;
		case SpeciesFactory.SpeciesType.Herbivore:
			prey.Add (SpeciesType.Plant);
			break;
		}

		return prey;
	}


	public void setAnimalMaterial(GameObject animal, SpeciesFactory.SpeciesType species, bool isEnemy)
	{
		if (isEnemy) 
		{
			switch (species) 
			{
			case SpeciesFactory.SpeciesType.Carnivore:
				animal.GetComponent<Renderer>().material = mat_EnemyCarnivore;
				break;
			case SpeciesFactory.SpeciesType.Omnivore: 
				animal.GetComponent<Renderer>().material = mat_EnemyOmnivore;
				break;
			case SpeciesFactory.SpeciesType.Herbivore:
				animal.GetComponent<Renderer>().material = mat_EnemyHerbivore;
				break;
			}
		}
		else 
		{
			switch (species) 
			{
			case SpeciesFactory.SpeciesType.Carnivore:
				animal.GetComponent<Renderer>().material = mat_PlayerCarnivore;
				break;
			case SpeciesFactory.SpeciesType.Omnivore: 
				animal.GetComponent<Renderer>().material = mat_PlayerOmnivore;
				break;
			case SpeciesFactory.SpeciesType.Herbivore:
				animal.GetComponent<Renderer>().material = mat_PlayerHerbivore;
				break;
			}
		}

	}


}
