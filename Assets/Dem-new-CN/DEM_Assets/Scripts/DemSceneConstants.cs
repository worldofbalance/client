using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemSceneConstants 
{
	// Omnivore = 0, Carnivore = 1, Herbivore = 2, the constants used in the database
	public enum AnimalType {Omnivore, Carnivore, Herbivore};
	// the edges of the game board 
	public const int MinEdge = -48;
	public const int MaxEdge = 48;

}
