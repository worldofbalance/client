using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ClashBattleUnit : MonoBehaviour
{
	//ClashBattleUnit is OMNIVORE behavior
	//Carnivore.cs, Herbivore.cs, and Obstacle.cs are subclasses of ClashBattleUnit

	//The ClashSpecies class holds species id, name, description, cost, hp, attack,
	//attack speed, move speed, and type - these values are from the database and
	//are held in a list in the ClashGameManager which is attached to the Main Menu
	//GameObject in the hierachy
	public ClashSpecies species;

	public string speciesName;
    public int currentHealth;
    public int damage = 0;
	//type not used, just for display in inspector. the type is stored in the species variable
	public string type;
    protected float timeBetweenAttacks = 1.0f;
	protected float stoppingDistance = 5.5f;

	[HideInInspector]
	public bool isDead = false;
	protected float timer;
	protected float targetTimer;
	protected ClashBattleController controller;
	protected Animator anim;

	protected NavMeshAgent agent;
	protected ClashBattleUnit target = null;
	protected ClashBattleUnit tempTarget = null;
//	protected List<ClashBattleUnit> favoritePreyList = new List<ClashBattleUnit>();
	protected List<ClashBattleUnit> omnivoreList = new List<ClashBattleUnit>();
	protected List<ClashBattleUnit> carnivoreList = new List<ClashBattleUnit>();
	protected List<ClashBattleUnit> herbivoreList = new List<ClashBattleUnit>();
	protected List<ClashBattleUnit> animalList = new List<ClashBattleUnit>(); 
	protected List<ClashBattleUnit> obstacleList = new List<ClashBattleUnit>();

	//Getting components
    void Awake ()
	{
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponent<Animator> ();
        controller = GameObject.Find ("Battle Menu").GetComponent<ClashBattleController> ();
    }

	//Intializing variables
    void Start ()
	{
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
    }

    void Update ()
	{
		if (controller.isStarted && !controller.finished) {
			//Find a target
			targetTimer += Time.deltaTime;
			//targeting is staggered so there is less performance issues
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

	// Sort by invaders/defenders -> sort by species type -> get closest target for each type -> set target
	protected virtual void FindTarget ()
	{
		float minDistance = Mathf.Infinity;
		float dist = 0;
		
		SortSpecies ();

		//Herbivore has no prioritization for any particular type, so amalgamate them in to one list
		animalList.AddRange(carnivoreList);
		animalList.AddRange(omnivoreList);
		animalList.AddRange(herbivoreList);
		
		//Priority Targeting: favoritePrey > animals > obstacles
		//Food web targeting not implemented
//		if (favoritePreyList.Count > 0) {
//			dist = FindClosestTarget (favoritePreyList); //sets tempTarget
//			if (dist <= 45.0f || (gameObject.tag == "Ally" && dist < 80.0f)) {
//				target = tempTarget;
//				anim.SetTrigger ("Walking");
//				return;
//			}
//		}
		if (animalList.Count > 0) {
			dist = FindClosestTarget (animalList); //sets tempTarget
			//  for defenders     for attackers
			if (dist <= 45.0f || (gameObject.tag == "Ally" && dist < 80.0f)) {
				target = tempTarget;
				anim.SetTrigger ("Walking");
				return;
			}
		}
		if (obstacleList.Count > 0) {
			dist = FindClosestTarget (obstacleList); //sets tempTarget
			target = tempTarget;
			anim.SetTrigger ("Walking");
			return;
		}
	}
	//End of FindTarget

	protected void SortSpecies () {
		ClashBattleUnit sortTarget = null;
		GameObject[] enemySpeciesArray;

		// Sorts all species in the scene by invading and defending species
		if (gameObject.tag == "Ally")
			enemySpeciesArray = GameObject.FindGameObjectsWithTag ("Enemy"); //"Enemy" tag is defenders
		else
			enemySpeciesArray = GameObject.FindGameObjectsWithTag ("Ally"); //"Ally" tag is attackers

//		favoritePreyList.Clear ();
		animalList.Clear();
		omnivoreList.Clear();
		carnivoreList.Clear();
		herbivoreList.Clear();
		obstacleList.Clear ();

		//Sorts by species type in to their respective list (e.g. speciestype == omnivore -> omnivoreList)
		foreach (GameObject enemySpecies in enemySpeciesArray) {
			sortTarget = enemySpecies.GetComponent<ClashBattleUnit> ();
			if (!sortTarget.isDead) {
//				foreach (ClashBattleUnit prey in favoritePreyList){
//					if (sortTarget.speciesName == prey)
//						favoritePreyList.Add (sortTarget);
//				}
				if (sortTarget.species.type == ClashSpecies.SpeciesType.OMNIVORE)
					omnivoreList.Add (sortTarget);
				if (sortTarget.species.type == ClashSpecies.SpeciesType.CARNIVORE)
					carnivoreList.Add (sortTarget);
				if (sortTarget.species.type == ClashSpecies.SpeciesType.HERBIVORE)
					herbivoreList.Add (sortTarget);
				if (sortTarget.species.type == ClashSpecies.SpeciesType.PLANT)
					obstacleList.Add (sortTarget);
			}
		}
	}
	
	//An alternative to using NavMeshAgent.remainingDistance, which does not work properly
	protected float CalculatePathDistance(NavMeshPath path) {
	    float distance = .0f;
	    for (var i = 0; i < path.corners.Length - 1; i++) {
	        distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
	    }
	    return distance;
	}

	protected float FindClosestTarget (List<ClashBattleUnit> targetList) {
		// Finds the closest target within the list, using remaining navigable distance
		NavMeshPath path = new NavMeshPath ();
		float pathDistance = 0;
		float closestDistance = Mathf.Infinity;
		
		foreach (ClashBattleUnit searchTarget in targetList) {
			//obstacles/plants use NavMeshObstacle component, so can't use path calculation
			if (searchTarget.species.type == ClashSpecies.SpeciesType.PLANT) {
				pathDistance = Vector3.Distance(transform.position, searchTarget.transform.position);
			} else {
				agent.CalculatePath (searchTarget.transform.position, path);
				pathDistance = CalculatePathDistance (path);
			}
			if (pathDistance < closestDistance) {
				closestDistance = pathDistance;
				tempTarget = searchTarget;
			}
		}

		return closestDistance;
	}
	
//    void Idle (){
//        //Triggers eating animation
//        if (anim != null)
//            anim.SetTrigger ("Eating");
//    }

    protected virtual void Attack (){
		timer = 0f;
		// Check if the destination has been reached, then deal damage
		if (!agent.pathPending) {
			if (agent.remainingDistance <= agent.stoppingDistance) {
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
					transform.LookAt (target.transform);
					if (anim != null)
						anim.SetTrigger ("Attacking");
					//Deal additional damage to favorite prey from food web
//					foreach (ClashBattleUnit prey in favoritePreyList){
//						if (target.speciesName == prey)
//							target.TakeDamage (damage + 100, this);
//					}
//					else
					target.TakeDamage (damage, this);
				}
			}
			else {
                if (anim != null)
                    anim.SetTrigger ("Walking");
            }
     	}
    }

    public void TakeDamage (int damageTaken, ClashBattleUnit source = null){
		//Debug.Log (tag + " " + species.name + " taking " + damage + " damage from " + source.tag + " " + source.species.name);
		if (isDead)
			return;

		currentHealth -= damageTaken;
        if (currentHealth <= 0)
            Die ();
    }

    protected void Die (){
		isDead = true;
        //Disable all functions here
        if (anim != null)
            anim.SetTrigger ("Dead");
		if (agent)
			agent.enabled = false;
		if (this.gameObject.tag == "Ally")
			controller.allySpecies[species.name] -= 1;
		if (this.gameObject.tag == "Enemy" && species.type != ClashSpecies.SpeciesType.PLANT)
			controller.enemySpecies[species.name] -= 1;

        target = null;
        if (species.type == ClashSpecies.SpeciesType.PLANT)
            this.gameObject.GetComponentInChildren<Renderer> ().enabled = false;
    }

//    public void setSelected (bool isSelected){
//        foreach (Transform child in transform) {
//            if (child.CompareTag (Constants.TAG_HEALTH_BAR))
//                child.gameObject.SetActive (isSelected);
//        }
//    }
}
