using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoalManager : MonoBehaviour {

	public Terrain terrain;
	public GameObject fishPrefab;
	public Vector3 goal1;
	public Vector3 goal2;
	public Vector3 goalActual;
	public int waterLevel;
	public Vector3 swimLimits = new Vector3(5,5,5);
	private float timer = 5;
	public int numFish = 1;
	public GameObject[] allFish;
	[Header("Fish Settings")]
	[Range(0.0f,5.0f)]
	public float minSpeed = 1f;
	[Range(0.0f,5.0f)]
	public float maxSpeed = 1.5f;
	public static float terrainOffset;
	
	

	// Use this for initialization
	void Start () {
		terrainOffset = terrain.transform.position.y;
		allFish = new GameObject[numFish];
		for(int i =0; i < numFish; i++)
		{
			Vector3 pos = this.transform.position + new Vector3(Random.Range(-swimLimits.x,swimLimits.x),
																Random.Range(-swimLimits.y,swimLimits.y),
																Random.Range(-swimLimits.z,swimLimits.z));
			if (pos.y < (Terrain.activeTerrain.SampleHeight(pos)+terrainOffset))
			{
				pos.y = Terrain.activeTerrain.SampleHeight(pos)+terrainOffset;
			}
			if (pos.y > waterLevel)
			{
				pos.y = waterLevel;
			}


			allFish[i] = (GameObject) Instantiate (fishPrefab,pos,Quaternion.identity);
			allFish[i].GetComponent<Shoal>().manager = this;
	}
}
	
	// Update is called once per frame
	void Update () {
		timer -=Time.deltaTime;
		if(timer<= 0){
			NewGoal();
			
			timer = (Random.Range(4,7));

			if (Random.Range(0,3) == 0)
			{
				goalActual = goal1;
			}
			else
			{
				goalActual = goal2;
			}
		}
	}

	void NewGoal()
	{
		goal1 = this.transform.position + new Vector3(Random.Range(-swimLimits.x,swimLimits.x),Random.Range(terrainOffset,waterLevel),Random.Range(-swimLimits.z,swimLimits.z));															
		goal2 = this.transform.position + new Vector3(Random.Range(-swimLimits.x,swimLimits.x),Random.Range(terrainOffset,waterLevel),Random.Range(-swimLimits.z,swimLimits.z));	
	}
}
