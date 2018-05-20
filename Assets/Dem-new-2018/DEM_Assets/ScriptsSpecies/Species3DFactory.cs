﻿using System.Collections;
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
		GameObject animal = setAnimalPrefab(species) as GameObject;
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
	public GameObject setAnimalPrefab(string species)
	{
		switch (species) 
		{
		case "Elephant":
			return Instantiate (Elephant) as GameObject;
			break;
		case "Ants":
			return Instantiate (Ants) as GameObject;
			break;
		case "Buffalo":
			return Instantiate (Buffalo) as GameObject;
			break;
		case "Horse":
			return Instantiate (Horse) as GameObject;
			break;
		case "Leopard":
			return Instantiate (Leopard) as GameObject;
			break;
		case "Tortoise":
			return Instantiate (Tortoise) as GameObject;
			break;
		case "ServalCat":
			return Instantiate (ServalCat) as GameObject;
			break;
		case "WildBoar":
			return Instantiate (WildBoar) as GameObject;
			break;
		}

		//return animal;
		return Instantiate (Ants) as GameObject;
	}


	// choices are Omnivore, Carnivore, Herbivore, Plant
	public string getSpeciesDietType(string species)
	{
		switch (species) 
		{
		case "Elephant":
		case "Buffalo":
		case "Horse":
		case "Tortoise":
			return "Herbivore";
			break;
		case "Ants":
		case "WildBoar":
		case "Wild Boar":
		case "Wild":
			return "Omnivore";
			break;
		case "Leopard":
		case "ServalCat":
		case "Serval Cat":
		case "Serval":
			return "Carnivore";
			break;
		}

		return "Plant";
	}


	/***
	* very simplified food web hard coded, needes to be replaced 
	* with a species specific database implimentation
	* 
	* Omnivore -> eat all plants and all animals
	* Carnivore -> eat all animals
	* Herbivore -> eat all plants
	* Plant -> empty list
	* 
	***/
	public ArrayList setAnimalPrey(string diet)
	{
		ArrayList prey = new ArrayList();

		switch (diet) 
		{
		case "Carnivore": // eat all animals
			prey.Add ("Elephant");
			prey.Add ("Buffalo"); 
			prey.Add ("Horse");
			prey.Add ("Tortoise");
			prey.Add ("Ants");
			prey.Add ("WildBoar");
			prey.Add ("Wild Boar");
			prey.Add ("Wild");
			prey.Add ("Leopard");
			prey.Add ("ServalCat");
			prey.Add ("Serval Cat");
			prey.Add ("Serval");
			break;
		case "Omnivore": // eat all animals and plants
			prey.Add ("Elephant");
			prey.Add ("Buffalo"); 
			prey.Add ("Horse");
			prey.Add ("Tortoise");
			prey.Add ("Ants");
			prey.Add ("WildBoar");
			prey.Add ("Wild Boar");
			prey.Add ("Wild");
			prey.Add ("Leopard");
			prey.Add ("ServalCat");
			prey.Add ("Serval Cat");
			prey.Add ("Serval");
			prey.Add ("Plant");
			break;
		case "Herbivore": // eat all plants
			prey.Add ("Plant");
			break;
		}

		return prey;
	}



}
