using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {

	void Awake() {
		NetworkManager.Send(WorldProtocol.Prepare(), ProcessWorld);
	}
	
	// Use this for initialization
	void Start() {
		Game.StartEnterTransition ();

		if (GameState.world != null) {
			LoadComponents();
		}
	}

	// Update is called once per frame
	void Update() {
		
	}

	void OnGUI() {
		MiniGamesConfig config = gameObject.GetComponent<MiniGamesConfig>();
		if (config.Ecosystem) {
			if (GUI.Button(new Rect(10, 10, 120, 30), "Ecosystem")) {
				Camera.main.GetComponent<MapCamera>().Move(GameState.player.GetID());
			}
		}

		if (config.ClashOfSpecies) {
			if (GUI.Button (new Rect (140, 10, 130, 30), "Clash of Species")) {
				gameObject.AddComponent <ClashOfSpeciesGUI>(); //Single Player
			}
		}

		if (config.DontEatMe) {
			if (GUI.Button (new Rect (10, 50, 120, 30), "Don't Eat Me")) {
				gameObject.AddComponent <DontEatMeGUI>(); // Single Player
			}
		}

		if (config.Convergence) {
			if (GUI.Button (new Rect (10, 90, 120, 30), "Convergence")) {
				gameObject.AddComponent <ConvergeGUI>(); //Single player
			}
		}

		if (config.MultiplayerGames) {
			if (GUI.Button (new Rect (140, 50, 130, 30), "Multiplayer Games")) {
				gameObject.AddComponent <MultiplayerGames>();
			}		
		}		
	}
	
	public void ProcessWorld(NetworkResponse response) {
		ResponseWorld args = response as ResponseWorld;

		if (args.status == 0) {
			GameState.world = args.world;

			SwitchToTileSelect(1);

			LoadComponents();
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
		}
	}
	
	public void SwitchToTileSelect(int numTilesOwned) {
		//If player owns no tiles, they will need to pick a new home tile
		if (numTilesOwned == 0) {
			GameObject.Find("MapCamera").GetComponent<MapCamera>().FirstTileProcess(true);
			gameObject.AddComponent<TileSelectGUI>();
		}
	}
}
