using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species3DFactory : MonoBehaviour {

	public string[] AnimalType = {"Omnivore", "Carnivore", "Herbivore", "Plant", "TreeOfLife"};
	public enum SpeciesType {Elephant, Ants, Buffalo, Horse, Leopard, Tortoise, ServalCat, WildBoar};

	// These are for linking to the prefabricated object, which is stored as a file in the game assets.
	public GameObject Elephant, Ants, Buffalo, Horse, Leopard, Tortoise, ServalCat, WildBoar;
	public GameObject plantPrefab;

	// needed for setting location in prefab enemy AI
	public Transform locationTreeOfLife;


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
		Random.InitState((int)(System.DateTime.Now.Ticks));
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
			animal.AddComponent<EnemyBehavior> ().setPreyList(prey);
			animal.AddComponent<EnemyAINavigation> ();
		} else {
			animal.AddComponent<AnimalBehavior> ().setPreyList (prey);
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
	public ArrayList setAnimalPrey(string animalType)
	{
		ArrayList prey = new ArrayList();

		switch (animalType) 
		{
		case "Carnivore":
			prey.Add ("Omnivore");
			prey.Add ("Carnivore"); 
			prey.Add ("Herbivore");
			break;
		case "Omnivore": 
			prey.Add ("Omnivore");
			prey.Add ("Carnivore"); 
			prey.Add ("Herbivore");
			prey.Add ("Plant");
			break;
		case "Herbivore":
			prey.Add ("Plant");
			break;
		}

		return prey;
	}


}
