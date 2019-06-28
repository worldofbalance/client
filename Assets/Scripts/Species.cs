using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Species : MonoBehaviour {

	public int species_id { get; set; }
	public string name { get; set; }
	public short organism_type { get; set; }
	public int biomass { get; set; }
	public int size { get; set; }
	public List<GameObject> speciesList = new List<GameObject>();
	public static List<GameObject> otherSpecies = new List<GameObject>();

	public static int xIdx = 0;
	public static int zIdx = 0; 
	public static int step = 2;
	private static int idxMax = 5;
	public static int zoneSize = 40;
	public static int[,] zoneXLocs = new int[zoneSize, zoneSize];
	public static int[,] zoneYLocs = new int[zoneSize, zoneSize];
	
	// Use this for initialization
	void Start() {
	}


	public GameObject CreateAnimal() {

		/*if (speciesList.Count == 0) {
			position = new Vector3(Random.Range(-30.0f, 30.0f), 100, 60);
			
			if (organism_type.Equals("Plant")) {
				position = new Vector3(Random.Range(-30.0f, 30.0f), 100, Random.Range(65, 70));
			}
		} else {
			position = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
			position *= Random.Range(5, 20);
			position += speciesList[0].transform.position;
		}

		RaycastHit hit;
		Debug.DrawRay(position, Vector3.down * 100);

		if (Physics.Raycast(position, Vector3.down, out hit, 100)) {
			position = hit.point;
		}

		position[1] += 1;*/
//		Vector3 position = playerTiles[Random.Range(0, playerTiles.Count - 1)].transform.position;
//		position.Set (position.x + Random.Range(-5f,5f), 5, position.z + Random.Range(-5f,5f)+2.5f);
		// Vector3 position = Vector3.zero;
		Vector3 position = new Vector3(xIdx * step, 0, zIdx * step);
		UpdateIdx ();
		GameObject organism = CreateOrganism(position);
		
		// Assign Leader
		organism.transform.localScale *= 0.50f;    // scaled to fit on one tile
		organism.transform.localScale *= 1.25f;    // default
		if (speciesList.Count == 0) {
//			organism.GetComponent<AI>().alphaLeader = organism;
			// organism.transform.localScale *= 1.25f;    // default
			// organism.transform.localScale *= 0.50f;    // scaled to fit on one tile
//			organism.GetComponent<CapsuleCollider>().enabled = true;
			organism.name = name + " (Alpha)_" + species_id;
		} else {
//			organism.GetComponent<AI>().alphaLeader = speciesList[0];
			organism.name = name + "_" + species_id;
		}

		speciesList.Add(organism);
		WorldController.speciesLocCurrent = false;

		return organism;
	}

	public GameObject createPlant() {
		return null;
	}

	public static void otherSpecie(int zoneX, int zoneY, int speciesId) {
		string name = SpeciesTable.speciesList [speciesId].name;
		int xOff = zoneXLocs[zoneX, zoneY];
		int yOff = zoneYLocs[zoneX, zoneY];
		UpdateOther(zoneX, zoneY);
		float baseX = (zoneY - 20) * 13.85f + (zoneX % 2 == 0 ? 7 : 0) - 1;
		float baseZ = (zoneX - 19) * -11.95f + 3.5f;
		GameObject organism = Instantiate(Resources.Load("Prefabs/Dummy")) as GameObject;
		organism.transform.position = 
			new Vector3 (baseX + xOff * step, 0, baseZ + yOff * step);
		organism.transform.Find("Quad").GetComponent<Renderer>().material.mainTexture = Resources.Load(Constants.TEXTURE_RESOURCES_PATH + "Species/" + name) as Texture;
		organism.transform.localScale *= 0.50f;    // scaled to fit on one tile
		organism.transform.localScale *= 1.25f;    // default
		if (name.Equals("Acacia")) {
			organism.transform.localScale *= 1.75f;
			organism.transform.localScale *= 0.50f;    // scaled to fit on one tile
		}
		otherSpecies.Add(organism);
	}



	
	private GameObject CreateOrganism(Vector3 position) {
//		GameObject organism = Instantiate(WorldController.speciesPrefabs["African Elephant"]) as GameObject;
		GameObject organism = Instantiate(Resources.Load("Prefabs/Dummy")) as GameObject;
		organism.transform.Find("Quad").GetComponent<Renderer>().material.mainTexture = Resources.Load(Constants.TEXTURE_RESOURCES_PATH + "Species/" + name) as Texture;
		organism.transform.position = position;

		if (organism_type.Equals("Animal")) {
//			organism.AddComponent<AnimalAI>();
		}

		if (name.Equals("Acacia")) {
			organism.transform.localScale *= 1.75f;
			organism.transform.localScale *= 0.50f;    // scaled to fit on one tile
		}

		return organism;
	}
	
	public void UpdateSize(int size) {
		this.size = size;
		biomass = (biomass == 0) ? 1500 : biomass;
		int numChange =  Mathf.RoundToInt(size/biomass) - speciesList.Count;
		
		if (numChange > 0) {
			for (int i = 0; i < numChange; i++) {
				switch (organism_type) {
				case 0:
					CreateAnimal();
					break;
				case 1:
					createPlant();
					break;
				}
			}
		} else if (numChange < 0) {
			for (int i = 0; i > numChange; i--) {
				GameObject organism = speciesList[speciesList.Count - 1];
				Destroy (organism);
				speciesList.RemoveAt(speciesList.Count - 1);
			}
		}
	}
	
	public void updateTerritoryPos(Vector3 position) {
		GameObject organism = speciesList[0];
		//organism.GetComponent<AnimalAI>().territoryPos = position;
	}

	public static void UpdateIdx() {
		// Debug.Log ("Species: UpdateIdx(), x,zIdz: " + xIdx + " " + zIdx);
		xIdx = (xIdx + 1) % idxMax;
		if (xIdx == 0) {
			zIdx = (zIdx + 1) % idxMax;
		}
	}

	public static void UpdateOther(int zoneX, int zoneY) {
		int xIdx = (zoneXLocs [zoneX, zoneY] + 1) % idxMax;
		zoneXLocs [zoneX, zoneY] = xIdx;
		if (xIdx == 0) {
			zoneYLocs [zoneX, zoneY] = (zoneYLocs [zoneX, zoneY] + 1) % idxMax;			
		}
	}
}
