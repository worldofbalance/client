﻿using UnityEngine;
using System.Collections;

public class ClashNavMeshAI : MonoBehaviour {
	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if(target != null) 
			//GetComponent<UnityEngine.AI.NavMeshAgent>().destination = target.transform.position;
	}

	public void SetTarget(GameObject go) {
		this.target = go;
	}
}
