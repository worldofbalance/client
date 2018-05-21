using UnityEngine;

using System.Collections;
using System.Collections.Generic;
namespace CW{
public class Species : MonoBehaviour {

	public int species_id { get; set; }
	public string name { get; set; }
	public short organism_type { get; set; }
	public int biomass { get; set; }
	public int size { get; set; }
	public List<GameObject> speciesList = new List<GameObject>();
	
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
		Vector3 position = Vector3.zero;
		GameObject organism = CreateOrganism(position);
		
		// Assign Leader
		if (speciesList.Count == 0) {
//			organism.GetComponent<AI>().alphaLeader = organism;
			organism.transform.localScale *= 1.25f;
//			organism.GetComponent<CapsuleCollider>().enabled = true;
			organism.name = name + " (Alpha)_" + species_id;
		} else {
//			organism.GetComponent<AI>().alphaLeader = speciesList[0];
			organism.name = name + "_" + species_id;
		}

		speciesList.Add(organism);

		return organism;
	}

	public GameObject createPlant() {
		return null;
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
}
}