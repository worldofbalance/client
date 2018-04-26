using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DemSceneConstants : MonoBehaviour
{
	// Omnivore = 0, Carnivore = 1, Herbivore = 2, these are the constants used in the database.
	// Added Plant = 3 and TreeOfLife = 4, for easier programming of collision and attack behaviors.
	public enum SpeciesType {Omnivore, Carnivore, Herbivore, Plant, TreeOfLife};
	// The edges of the game board, this is used for species placement on the board.
	// Minus 4 to accomidate the walls around the board.
	public static int FloorZWidth;// = (int)(GameObject.Find("Ground").transform.localScale.z) - 4;
	public static int FloorXWidth;// = (int)(GameObject.Find("Ground").transform.localScale.x) - 4;
	// The maximum number of enemies to have on the game board at the same time.
	public static int MaxNumberOfEnemies = 20; // make 50


	public static SpeciesType getRandomAnimalType()
	{
		System.Array values = System.Enum.GetValues (typeof(SpeciesType));
		// limit to animal types only
		int max = values.Length - 2;
		// Omnivore = 0, Carnivore = 1, Herbivore = 2
		SpeciesType specie = (SpeciesType)(((int[])values)[Random.Range(0,max)]);
		return specie;
	}
		
	// Use this for initialization
	void Start () {
		FloorZWidth = (int)(GameObject.Find("Ground").transform.localScale.z) - 4;
		FloorXWidth = (int)(GameObject.Find("Ground").transform.localScale.x) - 4;
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
	public static ArrayList setSpeciesPreyHardCoded(DemSceneConstants.SpeciesType species)
	{
		ArrayList prey = new ArrayList();

		switch (species) 
		{
		case DemSceneConstants.SpeciesType.Omnivore:
			prey.Add (SpeciesType.Omnivore);
			prey.Add (SpeciesType.Carnivore); 
			prey.Add (SpeciesType.Herbivore);
			prey.Add (SpeciesType.Plant);
			break;
		case DemSceneConstants.SpeciesType.Carnivore: 
			prey.Add (SpeciesType.Omnivore);
			prey.Add (SpeciesType.Carnivore); 
			prey.Add (SpeciesType.Herbivore);
			break;
		case DemSceneConstants.SpeciesType.Herbivore:
			prey.Add (SpeciesType.Plant);
			break;
		case DemSceneConstants.SpeciesType.Plant:
			// do nothing return an empty list
			break;
		}

		return prey;
	}
}
