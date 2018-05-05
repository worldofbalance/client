using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Carnivore : ClashBattleUnit {

	//Awake, Start, and Update are identical to ClashBattleUnit (Omnivore)

    void Awake (){
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponent<Animator> ();
        controller = GameObject.Find ("Battle Menu").GetComponent<ClashBattleController> ();
    }

    void Start (){
		//Set variables according to species data
		name = species.name;
		currentHealth += species.hp;
		damage += species.attack;
		timeBetweenAttacks = 100f / species.attackSpeed;
		type = species.type.ToString ();
		if (agent != null) {
			agent.speed += species.moveSpeed / 20.0f;
			agent.stoppingDistance = stoppingDistance;
		}			
    }

    void Update (){
		//Find a target
		targetTimer += Time.deltaTime;
		if (targetTimer >= 0.25f && !isDead) {
			findTarget();
			if (target) {
				if (!target.isDead) {
					agent.SetDestination (target.transform.position);
					targetTimer = 0f;
				}
			}
		}
		//Attack if there is a target
		if (!isDead && target) {
			if (!target.isDead) {
				timer += Time.deltaTime;
				if (timer >= timeBetweenAttacks)
					Attack ();
			}
		}
    } 
	//End of Update

	// Sort by invading/defending -> sort by species type -> get closest target for each type -> set target
    protected override void findTarget () {
		float minDistance = Mathf.Infinity;
		float dist = 0;

		// Sorts all species in the scene by invading and defending species then
		// sorts by species type in to their respective list (e.g. omnivore -> omnivoreList)
		SortSpecies();

		animalList.AddRange(carnivoreList); //combines the omnivoreList and carnivoreList in to animalList
		animalList.AddRange(omnivoreList);
		// Priority Targeting: favoritePrey > herbivores > omnivores/carnivores > obstacles
		if (obstacleList.Count > 0) {
			minDistance = findClosestTarget (obstacleList);
			target = tempTarget;
		}
		if (animalList.Count > 0) {
			dist = findClosestTarget (animalList);
			// This 'if' is so that defending units don't go wandering out too far
			if (dist <= 30.0f || gameObject.tag == "InvadingSpecies") {
				if (dist <= 15.0f || dist <= minDistance) {
					minDistance = dist;
					target = tempTarget;
				}
			}
		}
		if (herbivoreList.Count > 0) {
			dist = findClosestTarget (herbivoreList);
			if (dist <= 30.0f || gameObject.tag == "InvadingSpecies") {
				if (dist <= 15.0f || dist <= minDistance) {
					minDistance = dist;
					target = tempTarget;
				}
			}
		}
		if (favoritePreyList.Count > 0) {
			dist = findClosestTarget (favoritePreyList);
			if (dist <= 30.0f || gameObject.tag == "InvadingSpecies") {
				if (dist <= 15.0f || dist <= minDistance) {
					minDistance = dist;
					target = tempTarget;
				}
			}
		}
		//End Priority Targeting
		
//		 Set anim to walk
		 anim.SetTrigger ("Walking");
	}

	protected override void Attack () {
		timer = 0f;

		// Check if the destination has been reached, then deal damage
		if (!agent.pathPending) {
			if (agent.remainingDistance <= agent.stoppingDistance) {
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
					transform.LookAt (target.transform);
					if (anim != null)
						anim.SetTrigger ("Attacking");
					if (target.species.type == ClashSpecies.SpeciesType.HERBIVORE)
						target.TakeDamage (damage + 10, this);
					else
						target.TakeDamage (damage, this);
				}
			}
     	}
 	}
}
