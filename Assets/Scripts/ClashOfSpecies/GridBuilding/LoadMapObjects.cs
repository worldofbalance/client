using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.IO;

public class LoadMapObjects : MonoBehaviour {

	//Will Read from a file set up as an 2d array of binary values that can be used to
	//get information and build tiled maps

	public GameObject chosenGO;
	public List<GameObject> buildObjects;
	public float tileSize = 5.0f;
	//public TextAsset objectMap;
	private string filePath = "Assets/Scripts/ClashOfSpecies/GridBuilding/mapObjects.txt";
	public int[][] map;
	bool loaded = false;
	//select object
	//remove object
	//show a hovering green tile for positioning item
	// Use this for initialization
	void startBuild() {


		map = createMap(map,45,45);
		map = zeroFillMap (map);

		readFile ();
		instantiateMap ();
	}

	void Update()
	{
		if (!loaded) {
			startBuild ();
			loaded = true;
		}
	}
	//Read from a local file WILL NEED TO CHANGE**
	private void readFile(){	
		//check if the file contains data needed
		//if no map found create a new map
		StreamReader reader = new StreamReader(filePath);
		string file = reader.ReadLine();
		if (file != null) {
			Debug.Log (" File ");
			string[] splitStr = file.Split (' ');
			if (splitStr.Length == map.Length * map [0].Length) {
				for (int i = 0; i < map.Length; i++) {
					for (int j = 0; j < map [i].Length; j++) {
						map [i] [j] = int.Parse (splitStr [(i * map [i].Length) + j]);
						Debug.Log (map[i][j] +" : value ");
					}
				}
			}
		}
		reader.Close();
	}


	//Instantiate the map to the game
	private void instantiateMap(){
		for (int i = 0; i < map.Length; i++) {
			for (int j = 0; j < map [i].Length; j++) {
				if (map [i] [j] != 0) {
					instantiateObject (map [i] [j], i, j);
				}
			}
		}
	}
	private void instantiateObject(int choice, int xPos, int zPos){
		chosenGO = buildObjects[choice -1];
		Transform mapObjects = GameObject.Find ("MapObjects").transform;
		float x = 0.0f;
		float z = 0.0f;
		if (mapObjects != null && chosenGO != null) {
			Vector3 goPosition;

			x = xPos * tileSize;
			z =zPos * tileSize;
			goPosition.x = x;
			goPosition.z = z;
			goPosition.y = 0.0f;
			Transform go = Instantiate(chosenGO.transform, goPosition, Quaternion.identity);
			go.SetParent (mapObjects);
			//go.gameObject.name = "X : "+(int)Mathf.Floor(goPosition.x)+" Z : "+(int)Mathf.Floor(goPosition.z);
		}
	}


	private int[][] createMap(int [][] newMap, int sizeX, int sizeY){
		int [][] createdMap = new int[sizeX][];
		for (int i = 0; i < sizeX; i++) {
			createdMap [i] = new int[sizeY];
		}
		Debug.Log ("Created map");
		return createdMap;

	}
	private int[][] zeroFillMap(int [][] mapToFill){
		for(int i = 0; i < mapToFill.Length; i++){
			for (int j = 0; j < mapToFill[i].Length; j++) {
				mapToFill[i][j] = 0;
			}
		}
		Debug.Log ("Map Filled");
		return mapToFill;
	}

}
