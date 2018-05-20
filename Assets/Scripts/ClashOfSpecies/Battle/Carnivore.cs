using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Carnivore : ClashBattleUnit {

	//Awake, Start, and Update are identical to ClashBattleUnit (Omnivore)
	//Only FindTarget and Attack are overridden and changed from ClashBattleUnit
	//to accomodate the difference in priorities this type has

    void Awake (){
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponent<Animator> ();
        controller = GameObject.Find ("Battle Menu").GetComponent<ClashBattleController> ();
    }

    void Start (){
		//Set variables according to species data
		speciesName = species.name;
		currentHealth += species.hp;
		damage += species.attack;
		timeBetweenAttacks = 100f / species.attackSpeed;
		type = species.type.ToString ();
		if (agent != null) {
			agent.speed += species.moveSpeed / 20.0f;
			agent.stoppingDistance = stoppingDistance;
		}		

		// Sorts all species in the scene by invading and defending species then
		// sorts by species type in to their respective list (e.g. omnivore -> omnivoreList)
		SortSpecies();	
    }

    void Update (){
		if (controller.isStarted && !controller.finished) {
			//Find a target
			targetTimer += Time.deltaTime;
			if (targetTimer >= Random.Range(2.0f, 3.5f) && !isDead) {
				FindTarget ();
				if (target) {
					if (!target.isDead) {
						agent.SetDestination (target.transform.position);
						targetTimer = 0.0f;
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
    } 
	//End of Update

	// Sort by invading/defending -> sort by species type -> get closest target for each type -> set target
    protected override void FindTarget () {
		float minDistance = Mathf.Infinity;
		float dist = 0;
		
		SortSpecies ();

		//No prioritization for carnivore/omnivore type, so amalgamate them in to one list
		animalList.AddRange(carnivoreList);
		animalList.AddRange(omnivoreList);
		
		//Priority Targeting: favoritePrey > herbivores > omnivore/carnivore > obstacles
//		if (favoritePreyList.Count > 0) {
//			dist = FindClosestTarget (favoritePreyList); //sets tempTarget
//			if (dist <= 45.0f || (gameObject.tag == "Ally" && dist < 80.0f)) {
//				target = tempTarget;
//				anim.SetTrigger ("Walking");
//				return;
//			}
//		}
		if (herbivoreList.Count > 0) {
			dist = FindClosestTarget (herbivoreList); //sets tempTarget
			if (dist <= 45.0f || (gameObject.tag == "Ally" && dist < 80.0f)) {
				target = tempTarget;
				anim.SetTrigger ("Walking");
				return;
			}
		}
		if (animalList.Count > 0) {
			dist = FindClosestTarget (animalList); //sets tempTarget
			if (dist <= 45.0f || (gameObject.tag == "Ally" && dist < 80.0f)) {
				target = tempTarget;
				anim.SetTrigger ("Walking");
				return;
			}
		}
		if (obstacleList.Count > 0) {
			dist = FindClosestTarget (obstacleList);
			target = tempTarget;
			anim.SetTrigger ("Walking");
			return;
		}
	}
	//End of FindTarget

    protected override void Attack (){
		timer = 0f;
		// Check if the destination has been reached, then deal damage
		if (!agent.pathPending) {
			if (agent.remainingDistance <= agent.stoppingDistance) {
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
					transform.LookAt (target.transform);
					if (anim != null)
//					foreach (ClashBattleUnit prey in favoritePreyList){
//						if (target.speciesName == prey)
//							target.TakeDamage (damage + 100, this);
//					}
					if (target.species.type == ClashSpecies.SpeciesType.HERBIVORE)
						target.TakeDamage (damage + 50, this);
					else
						target.TakeDamage (damage, this);
				}
			}
			else {
                if (anim != null)
                    anim.SetTrigger ("Walking");
            }
     	}
    }
}
