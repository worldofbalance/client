using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species3DFactory : MonoBehaviour{

	public string[] DietType = {"Omnivore", "Carnivore", "Herbivore", "Plant", "TreeOfLife"};
	public string[] SpeciesType = {"Elephant", "Ants", "Buffalo", "Horse", "Leopard", "Tortoise", "ServalCat", "WildBoar"};
	private string diet;

	// These are for linking to the prefabricated object, which is stored as a file in the game assets.
	public GameObject Elephant, Ants, Buffalo, Horse, Leopard, Tortoise, ServalCat, WildBoar;
	public GameObject plantPrefab;


	public GameObject getPlant()
	{
		GameObject plant = Instantiate(plantPrefab);
		// Prefab for plant already contains PlantBehavior and Plant tag.
		return plant;
	}


	// Create a new instance of a species from the prefab file,
	// and set its species type and list of prey.
	public GameObject getAnimal(string speciesType, bool isEnemy)
	{
		string species = speciesType;
		ArrayList prey = new ArrayList ();
		GameObject animal = setAnimalPrefab(species);
		diet = getSpeciesDietType (species);
		prey = setAnimalPrey (diet);
		SpeciesBehavior behavior;

		if (isEnemy) {
			animal.AddComponent<EnemyBehavior> ();
			behavior = animal.GetComponent<EnemyBehavior> ();
			animal.AddComponent<EnemyAINavigation> ();
			animal.tag = "Enemy";
			prey.Add ("TreeOfLife");
		} else {
			behavior = animal.AddComponent<PlayerAnimalBehavior> ();
			animal.tag = "Defender";
		}

		behavior.setSpecies (species);
		behavior.setDietType (diet);
		behavior.setPreyList (prey);

		return animal;
	}



	// Create a new instance of a species from the prefab file,
	// and set its species type and list of prey.
	public GameObject getRandomAnimal(bool isEnemy)
	{
		Random.InitState((int)(System.DateTime.Now.Ticks));
		int numSpecies = SpeciesType.Length;
		string species = SpeciesType[Random.Range(0, numSpecies)];
		ArrayList prey = new ArrayList ();
		GameObject animal = setAnimalPrefab(species);
		diet = getSpeciesDietType (species);
		prey = setAnimalPrey (diet);
		SpeciesBehavior behavior;

		if (isEnemy) {
			animal.AddComponent<EnemyBehavior> ();
			behavior = animal.GetComponent<EnemyBehavior> ();
			animal.AddComponent<EnemyAINavigation> ();
			animal.tag = "Enemy";
			prey.Add ("TreeOfLife");
		} else {
			behavior = animal.AddComponent<PlayerAnimalBehavior> ();
			animal.tag = "Defender";
		}
			
		behavior.setSpecies (species);
		behavior.setDietType (diet);
		behavior.setPreyList (prey);

		return animal;
	}
		

	// choices are Elephant, Ants, Buffalo, Horse, Leopard, Tortoise, ServalCat, WildBoar
	private GameObject setAnimalPrefab(string species)
	{
		// default is a nice bright red omnivore so it can eat anything
		GameObject animal = Instantiate<GameObject>(Ants);

		// must be a better way, need to look up finding prefab by name, but loading
		// would require using a resourse folder or path that could change
		switch (species) 
		{
		case "Elephant":
			animal = Instantiate (Elephant) as GameObject;
			break;
		case "Ants":
			animal = Instantiate (Ants) as GameObject;
			break;
		case "Buffalo":
			animal = Instantiate (Buffalo) as GameObject;
			break;
		case "Horse":
			animal = Instantiate (Horse) as GameObject;
			break;
		case "Leopard":
			animal = Instantiate (Leopard) as GameObject;
			break;
		case "Tortoise":
			animal = Instantiate (Tortoise) as GameObject;
			break;
		case "ServalCat":
			animal = Instantiate (ServalCat) as GameObject;
			break;
		case "WildBoar":
			animal = Instantiate (WildBoar) as GameObject;
			break;
		}

		return animal;
	}


	// choices are Omnivore, Carnivore, Herbivore, Plant
	private string getSpeciesDietType(string species)
	{
		diet = "Plant";
		// must be a better way, need to look up finding prefab by name, but loading
		// would require using a resourse folder or path that could change
		switch (species) 
		{
		case "Elephant":
		case "Buffalo":
		case "Horse":
		case "Tortoise":
			diet = "Herbivore";
			break;
		case "Ants":
		case "WildBoar":
			diet = "Omnivore";
			break;
		case "Leopard":
		case "ServalCat":
			diet = "Carnivore";
			break;
		}

		return diet;
	}


	/***
	* food web hard coded, needes to be replaced with a species specific database implimentation
	* 
	* Omnivore -> Omnivore, Carnivore, Herbivore, Plant, TreeOfLife
	* Carnivore -> Omnivore, Carnivore, Herbivore
	* Herbivore -> Plant, TreeOfLife
	* Plant -> empty list
	* 
	***/
	public ArrayList setAnimalPrey(string diet)
	{
		ArrayList prey = new ArrayList();

		switch (diet) 
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
