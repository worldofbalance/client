using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.IO;

public class WorldController : MonoBehaviour {

	private GameObject globalObject;
	private GameState gs;
	private Graph graph;
	private Rect logoutConfirmRect, foodWebRect;
	private int confirmWidth = 300;
	private int confirmHeight = 200;
	private Dictionary<int, int> results = new Dictionary<int, int>();
	public static bool speciesLocCurrent = false;
	private Texture2D bgTexture;
	private bool confirmPopUp = false;
	private Dictionary<int, int> speciesList = new Dictionary<int, int> ();
	private List<int> keyList;
	private int foodWebWidth, foodWebHeight, foodWebWidthP, foodWebHeightP, foodWebDPI;
	private int FWWWidth, FWWHeight;
	private int imageByteCount, segCount, segCounter;
	public static int FOOD_WEB_BLOCK_SIZE = 32000; // Must match value in GameServer 
	public static float FOOD_WEB_FRACTION = 0.5f;  // Fraction of screen w,h taken by food web image
	private string speciesStr, configStr;
	private byte[] imageContents;
	private bool foodWebImageExists;
	private Texture2D fwTexture = null;


  void Awake() {
    try {
      Game.networkManager.Send(WorldProtocol.Prepare(), ProcessWorld);
    } catch (Exception) {
    }

    globalObject = GameObject.Find ("Global Object");
	gs = globalObject.GetComponent<GameState> ();
	logoutConfirmRect = new Rect ((Screen.width - confirmWidth) / 2, 
			(Screen.height - confirmHeight) / 2, confirmWidth, confirmHeight);
	bgTexture = Resources.Load<Texture2D> (Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
	confirmPopUp = false;
	foodWebImageExists = false;
  }
  
  // Use this for initialization
  void Start() {
		foodWebWidth = (int) ((Screen.width / Screen.dpi) * FOOD_WEB_FRACTION);
		foodWebHeight = (int) ((Screen.height / Screen.dpi) * FOOD_WEB_FRACTION);
		foodWebDPI = (int)Screen.dpi; 
		foodWebWidthP = foodWebWidth * foodWebDPI;
		foodWebHeightP = foodWebHeight * foodWebDPI;
		FWWWidth = foodWebWidthP + 140;
		FWWHeight = foodWebHeightP + 120;
		foodWebRect = new Rect ((Screen.width - FWWWidth) / 2, 
			(Screen.height - FWWHeight) / 2, FWWWidth, FWWHeight);
		Debug.Log ("WorldController: foodWebWidth, foodWebHeight = " + foodWebWidth + " " + foodWebHeight);
		Debug.Log ("WorldController: Screen.dpi, foodWebDPI = " + Screen.dpi + " " + foodWebDPI);

    Game.StartEnterTransition ();
    if (GameState.world != null) {
      LoadComponents();
	  GameState.UpdateSpDisplay ();
    }
  }

  // Update is called once per frame
  void Update() {
  }

  void OnGUI() {
	if (!speciesLocCurrent) {
	  UpdateSpeciesLoc ();
	}
	
    // displays Food Web button
	/*if (GUI.Button (new Rect (200, Screen.height - 145f, 80, 30), "Food Web") && !foodWebImageExists) {
        Debug.Log("food web button, food web image does not exist");
        short action = 2;
	    Game.networkManager.Send (SpeciesActionProtocol.Prepare (action), ProcessSpeciesAction2);
	}*/
			
	if (GUI.Button (new Rect (200, Screen.height - 45f, 80, 30), "Logout")) {
	  confirmPopUp = true;
	}

	if (confirmPopUp) {
	  GUI.Window (Constants.CONFIRM_LOGOUT, logoutConfirmRect, 
			MakeConfirmDeleteWindow, "confirm logout", GUIStyle.none);
	}

	if (foodWebImageExists) {
	  GUI.Window (Constants.FOOD_WEB_VIEW, foodWebRect, MakeFoodWebWindow, "food Web view", GUIStyle.none);
            Debug.Log("food web image exists");
	}
  }
  

	void MakeFoodWebWindow(int id) {
        Debug.Log("make food web window method");
        Functions.DrawBackground(new Rect(0, 0, FWWWidth, FWWHeight), bgTexture);
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = 18;

		GUI.DrawTexture(new Rect(70, 60, foodWebWidthP, foodWebHeightP), fwTexture, ScaleMode.ScaleToFit);

		GUI.Label(new Rect((FWWWidth - 150)/2, 40, 150, 30), "Food Web Display", style);

		if (GUI.Button (new Rect (FWWWidth/2 - 50, FWWHeight - 60, 100, 30), "Close")) {
			foodWebImageExists = false;
		}
	}


	void MakeConfirmDeleteWindow(int id) {
		Functions.DrawBackground(new Rect(0, 0, confirmWidth, confirmHeight), bgTexture);
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = 16;

		GUI.Label(new Rect((confirmWidth - 200)/2, 50, 200, 30), "Confirm Logout", style);

		if (GUI.Button (new Rect (40, confirmHeight - 70, 60, 30), "YES")) {
			Game.networkManager.Send (LogoutProtocol.Prepare ((short) 1), ProcessLogout);
			Debug.Log ("sent logout message");
		}

		if (GUI.Button (new Rect (confirmWidth - 100, confirmHeight - 70, 60, 30), "NO")) {
			confirmPopUp = false;
		}			
	}


	public void ProcessLogout (NetworkResponse response)
	{
		ResponseLogout args = response as ResponseLogout;
		Debug.Log ("inside process logout");
		Debug.Log ("Logout details: type, status, playerId: " + args.type + " " + args.status + " " + args.playerId);
		if (args.status == 0) {
			Debug.Log ("logout successful");
		} else {
			Debug.Log ("login failed, server message = " + args.status);
		}

		Game.SwitchScene("Login");
	}


	public void ProcessSpeciesAction2 (NetworkResponse response)
	{
        Debug.Log("ProcessSpeciesAction2 start");
		ResponseSpeciesAction args = response as ResponseSpeciesAction;
		short action = args.action;
		short status = args.status;
		if ((action != 2) || (status != 0)) {
			Debug.Log ("ResponseSpeciesAction2 unexpected result");
			Debug.Log ("action, status = " + action + " " + status);
		}
		speciesList = args.speciesList;
		Debug.Log ("WorldController, ProcessSpeciesAction2, size = " + speciesList.Count);
		if (speciesList.Count == 0) {
			return;
		}

		keyList = new List<int>(speciesList.Keys);
		// If the player has no species, cannot generate food web graph
		if (keyList.Count == 0) {
			return;
		}
		keyList.Sort();
		speciesStr = "" + keyList [0];
		for (int i = 1; i < keyList.Count; i++) {
			speciesStr += " " + keyList [i];
		}

		string fileName = "foodweb_" + speciesStr.Replace (" ", "-") + ".png";
		if (File.Exists (fileName)) {
			DrawTexture (fileName);
			return;
		}
		configStr = " --figsize " + foodWebWidth + " " + foodWebHeight + " "
				+ " --dpi " + foodWebDPI;
		action = 8;
		Debug.Log ("WorldController, :config:species: = :" + configStr + ":" + speciesStr + ":");
		Game.networkManager.Send (SpeciesActionProtocol.Prepare 
			(action, configStr, speciesStr), ProcessFWImage);
	}


  public void ProcessFWImage(NetworkResponse response) {
    Debug.Log("ProcessFWImage start");
	ResponseSpeciesAction args = response as ResponseSpeciesAction;
	Debug.Log ("WorldController, ProcessFWImage, byteCount = " + args.byteCount);
	imageByteCount = args.byteCount;
	if (imageByteCount > 0) {
	  short action = 9;
	  imageContents = new byte[imageByteCount];
	  segCounter = 0;
	  segCount = ((imageByteCount - 1) / FOOD_WEB_BLOCK_SIZE) + 1;
	  for (int i = 0; i < segCount; i++) {
		Game.networkManager.Send (SpeciesActionProtocol.Prepare 
				(action, configStr, speciesStr, i * FOOD_WEB_BLOCK_SIZE), ProcessFWImage2);
	  }
    }
  }

  public void ProcessFWImage2(NetworkResponse response) {
        Debug.Log("ProcessFWImage2 start");
	ResponseSpeciesAction args = response as ResponseSpeciesAction;
	Debug.Log ("WorldController, ProcessFWImage2, startByte, byteCount = " 
		+ args.startByte + " " + args.byteCount);
	for (int i = 0; i < args.byteCount; i++) {
	  imageContents [i + args.startByte] = args.fileContents [i];
	}
	segCounter++;
	if (segCount == segCounter) {
	  string fileName = "foodweb_" + speciesStr.Replace (" ", "-") + ".png";
	  using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create))) {
		writer.Write(imageContents);
	  }
	  DrawTexture (fileName);
	}
  }
  

  void DrawTexture(String filePath) {
        Debug.Log("DrawTexture start");
	byte[] fileData;
	if (File.Exists(filePath)) {
	  fileData = File.ReadAllBytes(filePath);
	  fwTexture = new Texture2D(2, 2);
	  fwTexture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
	  foodWebImageExists = true;
            Debug.Log("foodWebImageExists = true");
        }
    }


  public void ProcessWorld(NetworkResponse response) {
    ResponseWorld args = response as ResponseWorld;

    if (args.status == 0) {
      GameState.world = args.world;

      SwitchToTileSelect(1);

      LoadComponents();

	  // This is to run simulation of the user's ecosystem upon login
	  // Debug.Log("WorldController: Send PredictionProtocol");
	  // Game.networkManager.Send (PredictionProtocol.Prepare (), ProcessPrediction);
    }
  }

  void LoadComponents() {
    GameObject gObject = GameObject.Find("Global Object");
    
    if (gObject != null) {
      if (gObject.GetComponent<EcosystemScore>() == null) {
        gObject.AddComponent<EcosystemScore>();
      }
      
      if (gObject.GetComponent<GameResources>() == null) {
        gObject.AddComponent<GameResources>();
      }
      
      if (gObject.GetComponent<Clock>() == null) {
        gObject.AddComponent<Clock>();
      }
      
      if (gObject.GetComponent<Chat>() == null) {
        gObject.AddComponent<Chat>();
      }
      
      if (GameObject.Find("Cube").GetComponent<Shop>() == null) {
        GameObject.Find("Cube").AddComponent<Shop>();
      }
	
	  if (GameObject.Find("Cube").GetComponent<Graph>() == null) {
		GameObject.Find("Cube").AddComponent<Graph>();
	  }
	  graph = GameObject.Find("Cube").GetComponent<Graph>();
    }
  }
  
  public void SwitchToTileSelect(int numTilesOwned) {
    //If player owns no tiles, they will need to pick a new home tile
    if (numTilesOwned == 0) {
      GameObject.Find("MapCamera").GetComponent<MapCamera>().FirstTileProcess(true);
      gameObject.AddComponent<TileSelectGUI>();
    }
  }


	public void ProcessPrediction(NetworkResponse response) {
		ResponsePrediction args = response as ResponsePrediction;
		// Debug.Log("WorldController, ProcessPrediction: status = " + args.status);
		if (args.status == 0) {
			Dictionary<int, Species> speciesList = gs.speciesListSave;
			results = args.results;
			foreach (KeyValuePair<int, int> entry in results) {
				// Debug.Log("WorldController, ProcessPrediction: k/v:" + entry.Key + " " + entry.Value);
				if (speciesList.ContainsKey (entry.Key)) {
					speciesList [entry.Key].biomass += entry.Value;
					// Debug.Log("WorldController, ProcessPrediction: new value:" + speciesList [entry.Key].biomass);
				} else {
					// Debug.Log("WorldController, ProcessPrediction: Could not find key:" + entry.Key);
				}
			}
		}
	}
		
	void UpdateSpeciesLoc() {
		Species.xIdx = 0;
		Species.zIdx = 0;
		int player_id = GameState.player.GetID ();
		GameObject zoneObject = GameObject.Find ("Map").GetComponent<Map> ().FindPlayerOwnedTile (player_id);
		if (zoneObject == null) {
			speciesLocCurrent = true;
			return;
		}
		Zone zone = zoneObject.GetComponent<Zone> ();
		float baseX = (zone.column - 20) * 13.85f + (zone.row % 2 == 0 ? 7 : 0) - 1;
		float baseZ = (zone.row - 19) * -11.95f + 3.5f;
		// Debug.Log ("WorldController: zone.row, zone.column: " + zone.row + " " + zone.column);
		GameObject gO = GameObject.Find ("Global Object");
		// if (gO == null) {
		//	Debug.Log ("WorldController: gO is null");
		// }
		Dictionary<int, Species> sL = gO.GetComponent<GameState> ().speciesListSave;
		// if (sL == null) {
		//	Debug.Log ("WorldController: sL is null");
		// }
		// Debug.Log ("WorldController: sL count = " + sL.Count);
		foreach (KeyValuePair<int, Species> entry in sL) {
			Species species = entry.Value;
			// Debug.Log ("WorldController: species assignment");
			// if (species == null) {
			// 	Debug.Log ("WorldController: species == null");
			// }

			List<GameObject> organisms = new List<GameObject> ();
			bool organismCreated = false;
			foreach (GameObject organism in species.speciesList) {
				// Debug.Log ("WorldController: organism assignment");
				// if (organism == null) {
				//	Debug.Log ("WorldController: organism == null");
				// }
				organisms.Add (organism);
				/*
				organism.transform.position = 
						new Vector3 (baseX + Species.xIdx * Species.step, 0, baseZ + Species.zIdx * Species.step);
				Species.UpdateIdx ();
				*/
			}
			for (int i = 0; i < organisms.Count; i++) {
				if (!organismCreated) {
					if (organisms[i] == null) {
						// Debug.Log ("WorldController: organisms, organism == null");
						int tX = Species.xIdx;
						int tZ = Species.zIdx;
						Destroy (organisms [i]);
						organisms[i] = species.CreateAnimal ();
						organismCreated = true;
						Species.xIdx = tX;
						Species.zIdx = tZ;
					}
					organisms[i].transform.position = 
						new Vector3 (baseX + Species.xIdx * Species.step, 0, baseZ + Species.zIdx * Species.step);
					Species.UpdateIdx ();
				}
			}
		}
		speciesLocCurrent = true;
	}
}
