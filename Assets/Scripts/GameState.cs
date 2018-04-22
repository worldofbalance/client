/*
 * This class is created and persists throughout the game session. It contains several important 
 * static objects. 
 */

using UnityEngine;

using System.Collections.Generic;
using System;

public class GameState : MonoBehaviour
{
	public static Account account { get; set; }

	public static Player player { get; set; }

	public static World world { get; set; }

	public static Ecosystem ecosystem { get; set; }

	private int month;

	public Dictionary<int, Species> speciesList { get; set; }

	// speciesListSave does not get destroyed or changed by Convergence / MC
	public Dictionary<int, Species> speciesListSave { get; set; }

	public static CSVObject csvList { get; set; }

	public static int matchID { get; set; }

	static List<SpData> spDatas { get; set; }

	private bool sLSaveFlag = false;

	// Holds environment score & high score to keep these for when player returns from game
	public static int envScore { get; set; }
	public static int envHighScore { get; set; } 

	
	// Use this for initialization
	void Awake ()
	{
		speciesList = new Dictionary<int, Species> ();
		speciesListSave = new Dictionary<int, Species> ();
		spDatas = new List<SpData>();

//
//		Game.networkManager.Send(
//			NetworkCode.CHART,
//			ProcessChart
//		);

		Game.networkManager.Listen (
			NetworkCode.ECOSYSTEM,
			ProcessEcosystem
		);

		Game.networkManager.Listen (
			NetworkCode.SPECIES_CREATE,
			ProcessSpeciesCreate
		);

		Game.networkManager.Listen (
			NetworkCode.SPECIES_INFO,
			ProcessSpeciesInfo
		);

		string csv = ",1,2,3,4,5,6,7,8,9,10,11,12\nTree Mouse,5000.0,4348.049640655518,3498.2125759124756,2587.291717529297,1743.8188791275024,1097.4494218826294,643.5683965682983,354.99313473701477,176.78679525852203,91.19854867458344,40.83816707134247,17.714429646730423\nFlies,5000.0,3011.394739151001,1221.3971614837646,257.5061619281769,1.336263376288116,1.0892114987726131E-6,1.3877543862325648E-10,7.128640401399624E-13,2.956730960663441E-14,7.831051207937521E-15,0,\nDwarf Puddle Frog,5000.0,5539.774417877197,5473.539352416992,4501.90544128418,3035.2282524108887,1794.481635093689,979.7311425209045,500.2158284187317,226.70242190361023,109.04326289892197,44.129449874162674,17.338458448648453";
//		csv = ",1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25\nEcosystem Score,100,200,300,400,500,400,300,200,300,400,500,600,700,800,900,1000,2000,1200,600,500,400,700,900,1000,1100";

		csv = ",1,2,3,4,5\nSpecies A,5000,4300,3500,2600,1700\nSpecies B,2500,4000,4100,5000,1000\nSpecies C,1000,900,850,600,100";
		GameState.csvList = Functions.ParseCSV (csv);
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnDestroy ()
	{
		Game.networkManager.Ignore (
			NetworkCode.ECOSYSTEM,
			ProcessEcosystem
		);

		Game.networkManager.Ignore (
			NetworkCode.SPECIES_CREATE,
			ProcessSpeciesCreate
		);

		Game.networkManager.Ignore (
			NetworkCode.SPECIES_INFO,
			ProcessSpeciesInfo
		);
	}
	
	public void ProcessEcosystem (NetworkResponse response)
	{
		ResponseEcosystem args = response as ResponseEcosystem;
		if (args.status == 0) {
			GameState.ecosystem = args.ecosystem;
		}
	}

	public void ProcessSpeciesInfo (NetworkResponse response)
	{
		ResponseSpeciesInfo args = response as ResponseSpeciesInfo;
		// Debug.Log ("GameState, ProcessSpeciesInfo: received message");
		// Debug.Log ("ZoneX, ZoneY = " + args.zoneX + " " + args.zoneY);
		List<int> tList = args.speciesIds;
		// Debug.Log ("Species id count = " + tList.Count);
		SpData spData = null;
		SpData spData2;
		for (int idx = 0; idx < spDatas.Count; idx++) {
			spData2 = spDatas [idx];
			if ((spData2.zoneX == args.zoneX) && (spData2.zoneY == args.zoneY)) {
				spData = spData2;
				break;
			}
		}
		if (spData == null) {
			spData = new SpData();
			spData.zoneX = args.zoneX;
			spData.zoneY = args.zoneY;
			spData.spIds = new List<int>();
			spDatas.Add(spData);
		}

		for (int idx = 0; idx < tList.Count; idx++) {
			// Debug.Log(tList [idx]);
			Boolean addFlag = true;
			for (int idxL = 0; idxL < spData.spIds.Count; idxL++) {
				if (spData.spIds [idxL] == tList [idx]) {
					addFlag = false;
					break;
				}
			}
			if (addFlag) {
				spData.spIds.Add(tList [idx]);
				Species.otherSpecie (args.zoneX, args.zoneY, tList [idx]);
			}
		}
		// Debug.Log ("");
	}


	public static void UpdateSpDisplay() {
		// Debug.Log("Entered GameState: UpdateSpDisplay(), count = " + spDatas.Count);
		if (spDatas.Count > 0) {
			Species.zoneXLocs = new int[Species.zoneSize, Species.zoneSize];
			Species.zoneYLocs = new int[Species.zoneSize, Species.zoneSize];
			for (int i = 0; i < spDatas.Count; i++) {
				int zX = spDatas[i].zoneX;
				int zY = spDatas[i].zoneY;
				List<int> spList = spDatas[i].spIds;
				for (int j = 0; j < spList.Count; j++) {
					Species.otherSpecie (zX, zY, spList[j]);
				}
			}
		}
	}
		
	
	public void ProcessSpeciesCreate (NetworkResponse response)
	{
		ResponseSpeciesCreate args = response as ResponseSpeciesCreate;

		SpeciesData species = null;

		if (SpeciesTable.speciesList.ContainsKey (args.species_id)) {
			species = SpeciesTable.speciesList [args.species_id];
		}

		if (species == null) {
			Debug.LogError ("Failed to create Species #" + args.species_id);
			return;
		}

		CreateSpecies (args.group_id, args.biomass, args.name, species, true);
	}

	// This is used to update the memory species list when you buy from ShopCartPanel
	public void PurchaseSpecies (int group_id, int species_id, int biomass) {
		if (speciesList.ContainsKey (species_id)) {
			speciesList [species_id].biomass += biomass;
			speciesListSave [species_id].biomass += biomass;
		} else {
			SpeciesData speciesData = new SpeciesData (species_id);
			speciesData.organism_type = SpeciesTable.speciesList[species_id].organism_type;
			string name = SpeciesTable.speciesList [species_id].name;
			CreateSpecies (group_id, biomass, name, speciesData, true);
		}
	}

	public void CreateSpecies (int group_id, int biomass, string name, SpeciesData sdata, bool flag)
	{
		sLSaveFlag = flag;
		CreateSpecies (group_id, biomass, name, sdata);
		sLSaveFlag = false;
	}
		
	public void CreateSpecies (int group_id, int biomass, string name, SpeciesData sdata)
	{
//		if (speciesList.ContainsKey(species_id)) {
//			UpdateSpecies(species_id, biomass);
//		} else {
		// Species species = gameObject.AddComponent<Species> ();
		Species species = new Species();
		// Species speciesSave = new Species();
		species.species_id = sdata.species_id;
		species.name = name;
		species.organism_type = sdata.organism_type;
		species.biomass = biomass;

		/*
		speciesSave.species_id = sdata.species_id;
		speciesSave.name = name;
		speciesSave.organism_type = sdata.organism_type;
		speciesSave.biomass = biomass;
		*/
	
		GameObject organism = species.CreateAnimal ();
		// GameObject organismSave = speciesSave.CreateAnimal ();

		Dictionary<int, GameObject> zoneList = null;
		if (GameObject.Find ("Local Object")) {
			if (GameObject.Find ("Local Object").GetComponent<EcosystemController> ()) {
				zoneList = GameObject.Find ("Local Object").GetComponent<EcosystemController> ().zoneList;
			}
		}
		if (zoneList != null) {
			int zone_id = new List<int> (zoneList.Keys) [UnityEngine.Random.Range (0, zoneList.Count)];

			organism.transform.position = zoneList [zone_id].transform.position + new Vector3 (0, 0, 0);
			// organismSave.transform.position = zoneList [zone_id].transform.position + new Vector3 (-1000, 0, -1000);
		}
		speciesList [species.species_id] = species;
		if (sLSaveFlag) {
			speciesListSave [species.species_id] = species;
		}

		if (zoneList != null) {
			GameObject.Find ("Global Object").GetComponent<EcosystemScore> ().Calculate ();
		}
//			Game.networkManager.Send(
//				ShopActionProtocol.Prepare(),
//			);
//		}
	}
	
	public void UpdateSpecies (int species_id, int size)
	{
		Species species = speciesList [species_id];
		species.UpdateSize (size);
	}
	
	public void ProcessChart (NetworkResponse response)
	{
		ResponseChart args = response as ResponseChart;
	}
	
	public Species GetSpeciesGroup (int group_id)
	{
		return speciesList.ContainsKey (group_id) ? speciesList [group_id] : null;
	}
}

public class SpData {
	public int zoneX { get; set; }
	public int zoneY { get; set; }
	public List<int> spIds { get; set; }
}
