using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
	/**
	public float enemyDistance;
	public GameObject attackTarget;
	bool attacking;
	public bool hasAttackTarget;
	public string enemyTag;
	public int attack_range = 5;
	public int attack_speed = 5;
	public int enemyHealthPoints = 5;

	// Use this for initialization
	void Start () {
		attacking = false;
		hasAttackTarget = true;
	}


	// Update is called once per frame
	void Update () {
		
	}

	public void setTarget(GameObject go) {
		this.attackTarget = go;
	}

	
	// Update is called once per frame
	void Update () {
		if(attackTarget != null) 
			GetComponent<NavMeshAgent>().destination = attackTarget.transform.position;
		
		if (hasAttackTarget && attackTarget != null) {
			//attack if it has an enemy
			enemyDistance = Vector3.Distance (enemy.transform.position, transform.position);
			if (enemyDistance <= attack_range) {
				if (!attacking) {
					Invoke ("ApplyDamage", attack_speed);	//do damage every attackInt seconds
					attacking = true;
				}
			}
			if(enemyHealthPoints <= 0) {
				Destroy(attackTarget);
				ai.SetTarget(null);
				attackTarget = null;
				hasAttackTarget = false;
			}
		} else {
			//search for an enemy
			this.SetEnemy(FindClosestEnemy(enemyTag));
		}
	}

	public void SetEnemy(GameObject enemy) {
		this.attackTarget = enemy;
		setTarget (enemy);
		hasAttackTarget = true;
	}
		

	public GameObject FindClosestEnemy(string enemyTag) {
		GameObject[] enemies;
		enemies = GameObject.FindGameObjectsWithTag (enemyTag);
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 ourPosition = transform.position;

		if (enemies.Length == 0)
			return closest;

		foreach (GameObject enemy in enemies) {
			Vector3 distanceDifference = (enemy.transform.position - ourPosition);
			float currentDistance = distanceDifference.sqrMagnitude;
			if (currentDistance < distance) {
				closest = enemy;
				distance = currentDistance;
			}
		}
		return closest;
	}
	***/
}
