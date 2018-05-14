using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species3DFactory : MonoBehaviour {

	// Omnivore = 0, Carnivore = 1, Herbivore = 2, these are the constants used in the database.
	// Added Plant = 3 and TreeOfLife = 4, for easier programming of collision and attack behaviors.
	public enum AnimalType {Omnivore, Carnivore, Herbivore, Plant, TreeOfLife};
	public enum SpeciesType {Elephant, Ants, Buffalo, Horse, Leopard, Tortoise, ServalCat, WildBoar};

	// These are for linking to the prefabricated object, which is stored as a file in the game assets.
	public GameObject Elephant, Ants, Buffalo, Horse, Leopard, Tortoise, ServalCat, WildBoar;
	public GameObject plantPrefab;


	public GameObject getPlant()
	{
		GameObject plant = Instantiate(plantPrefab) as GameObject;
		return plant;
	}


	// returns a randomly chosen animal species type
	private SpeciesType getRandomSpeciesType()
	{
		// need to cast the enums to an array of ints
		System.Array speciesArray = System.Enum.GetValues (typeof(SpeciesType));
		int numAnimals = speciesArray.Length;
		// get a random entry from the array
		SpeciesType species = (SpeciesType)( speciesArray.GetValue(Random.Range(0,numAnimals)) );
		return species;
	}


	// Create a new instance of a species from the prefab file,
	// and set its species type and list of prey.
	public GameObject getRandomAnimal(bool isEnemy)
	{
		SpeciesType species = getRandomSpeciesType ();
		ArrayList prey = new ArrayList ();
		GameObject animal = setAnimalPrefab(species);
		prey = setAnimalPrey (animal.tag);

		if (isEnemy) {
			//EnemyBehavior enemy = new EnemyBehavior ();
			//enemy.setSpeciesType (species);
			//enemy.setPreyList (prey);
			animal.AddComponent<EnemyBehavior> ();
		} else {
			//AnimalBehavior animalBehavior = new AnimalBehavior ();
			//animalBehavior.setSpeciesType (species);
			//animalBehavior.setPreyList (prey);
			animal.AddComponent<AnimalBehavior> ();
		}
			
		return animal;
	}


	// choices are Elephant, Ants, Buffalo, Horse, Leopard, Tortoise, ServalCat, WildBoar
	private GameObject setAnimalPrefab(SpeciesType species)
	{
		// default is a nice bright red omnivore so it can eat anything
		GameObject animal = Instantiate (Ants) as GameObject;

		// must be a better way, need to look up finding prefab by name, but loading
		// would require using a resourse folder or path that could change
		switch (species) 
		{
		case SpeciesType.Elephant:
			animal = Instantiate (Elephant) as GameObject;
			break;
		case SpeciesType.Ants:
			animal = Instantiate (Ants) as GameObject;
			break;
		case SpeciesType.Buffalo:
			animal = Instantiate (Buffalo) as GameObject;
			break;
		case SpeciesType.Horse:
			animal = Instantiate (Horse) as GameObject;
			break;
		case SpeciesType.Leopard:
			animal = Instantiate (Leopard) as GameObject;
			break;
		case SpeciesType.Tortoise:
			animal = Instantiate (Tortoise) as GameObject;
			break;
		case SpeciesType.ServalCat:
			animal = Instantiate (ServalCat) as GameObject;
			break;
		case SpeciesType.WildBoar:
			animal = Instantiate (WildBoar) as GameObject;
			break;
		}

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
	public ArrayList setAnimalPrey(AnimalType animalType)
	{
		ArrayList prey = new ArrayList();
		string preyType = System.Enum.GetName(typeof(AnimalType), animalType);
		prey = setAnimalPrey(preyType);
		return prey;
	}

	public ArrayList setAnimalPrey(string animalType)
	{
		ArrayList prey = new ArrayList();

		switch (animalType) 
		{
		case "Carnivore":
			prey.Add (AnimalType.Omnivore);
			prey.Add (AnimalType.Carnivore); 
			prey.Add (AnimalType.Herbivore);
			break;
		case "Omnivore": 
			prey.Add (AnimalType.Omnivore);
			prey.Add (AnimalType.Carnivore); 
			prey.Add (AnimalType.Herbivore);
			prey.Add (AnimalType.Plant);
			break;
		case "Herbivore":
			prey.Add (AnimalType.Plant);
			break;
		}

		return prey;
	}


}
