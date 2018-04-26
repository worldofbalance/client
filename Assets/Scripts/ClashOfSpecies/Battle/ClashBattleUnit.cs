using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ClashBattleUnit : MonoBehaviour
{

    public UnityEngine.AI.NavMeshAgent agent;
    private ClashBattleController controller;
    private Animator anim;

    public ClashBattleUnit target;
    Vector3 targetPoint = Vector3.zero;

    public Vector3 TargetPoint {
        get { return targetPoint; }
        set { targetPoint = value; }
    }

    public ClashSpecies species;
    public int currentHealth = 0;
    public int damage = 0;
    public float timeBetweenAttacks = 1.0f;
    // The time in seconds between each attack.
    float timer;
    // Timer for counting up to the next attack.
    float timeSinceSpawn = 0.0f;

    void Awake ()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
        anim = GetComponent<Animator> ();
        controller = GameObject.Find ("Battle Menu").GetComponent<ClashBattleController> ();
    }

    void Start ()
    {
        // Set current health depending on the species data.
        currentHealth += species.hp;
        timeBetweenAttacks = 100f / species.attackSpeed;
        damage += species.attack;
        if (agent != null) {
            agent.speed += species.moveSpeed / 20.0f;
        }
    }

    void Update ()
    {
        timer += Time.deltaTime;
        timeSinceSpawn += Time.deltaTime;
        if (!target && targetPoint == Vector3.zero) {
            Idle ();
        } else if (targetPoint != Vector3.zero) {
            anim.SetTrigger ("Walking");
            if (agent && agent.isActiveAndEnabled)
                agent.destination = targetPoint;
        } else if ((target.currentHealth > 0) && (timer >= timeBetweenAttacks) && (currentHealth >= 0.0f)) {
			SpeciesAttack ();
        } else if (target.currentHealth <= 0) {
            target = null;		
        }

    }

	void SpeciesAttack() {
	
		if (target.name == "African Clawless Otter(Clone)" && (agent.name == "Nile Crocodile(Clone)")) {
			Attack ();
		}else if (target.name == "Nile Crocodile(Clone)" && (agent.name == "Nile Crocodile(Clone)")) {
			Attack ();
		}else if (target.name == "Southern Ground Hornbill(Clone)" && (agent.name == "Leopard(Clone)" || agent.name == "Lion(Clone)")) {
			Attack ();
		}else if (target.name == "Leopard(Clone)" && (agent.name == "Nile Crocodile(Clone)" || agent.name == "Lion(Clone)")) {
			Attack ();
		}else if (target.name == "Lion(Clone)" && (agent.name == "Lion(Clone)")) {
			Attack ();
		}else if (target.name == "Black Mamba(Clone)" && (agent.name == "Southern Ground Hornbill(Clone)")) {
			Attack ();
		}else if (target.name == "Cape Dwarf Gecko(Clone)" && (agent.name == "Black Mamba(Clone)" || agent.name == "Southern Ground Hornbill(Clone)" || agent.name == "Leopard(Clone)" || agent.name == "Greater Bushbaby(Clone)")) {
			Attack ();
		}else if (target.name == "Catfish(Clone)" && (agent.name == "African Clawless Otter(Clone)" || agent.name == "African Grey Hornbill(Clone)" || agent.name == "Black Mamba(Clone)" || agent.name == "Catfish(Clone)" || agent.name == "Leopard(Clone)" || agent.name == "Nile Crocodile(Clone)")) {
			Attack ();
		}else if (target.name == "Centipede(Clone)" && (agent.name == "Centipede(Clone)")) {
			Attack ();
		}else if (target.name == "Cockroach(Clone)" && (agent.name == "Centipede(Clone)")) {
			Attack ();
		}else if (target.name == "African Grey Hornbill(Clone)" && (agent.name == "African Clawless Otter(Clone)")) {
			Attack ();
		}else if (target.name == "Ants(Clone)" && (agent.name == "Cape Dwarf Gecko(Clone)" || agent.name == "Cape Teal(Clone)")) {
			Attack ();
		}else if (target.name == "Black and White Columbus Monkey(Clone)" && (agent.name == "Black Mamba(Clone)" || agent.name == "Leopard(Clone)")) {
			Attack ();
		}else if (target.name == "Bush Pig(Clone)" && (agent.name == "Lion(Clone)")) {
			Attack ();
		}else if (target.name == "Cape Teal(Clone)" && (agent.name == "African Grey Hornbill(Clone)" || agent.name == "Black Mamba(Clone)"|| agent.name == "Bush Pig(Clone)" || agent.name == "Greater Bushbaby(Clone)")) {
			Attack ();
		}else if (target.name == "Crickets(Clone)" && (agent.name == "African Grey Hornbill(Clone)" || agent.name == "Southern Ground Hornbill(Clone)" || agent.name == "Greater Bushbaby(Clone)" || agent.name == "Crickets(Clone)" || agent.name == "Centipede(Clone)" || agent.name == "Cape Teal(Clone)" || agent.name == "Cape Dwarf Gecko(Clone)")) {
			Attack ();
		}else if (target.name == "Decaying Material(Clone)" && (agent.name == "Ants(Clone)" || agent.name == "Black and White Columbus Monkey(Clone)" || agent.name == "Bush Pig(Clone)" || agent.name == "Catfish(Clone)" || agent.name == "Cockroach(Clone)" || agent.name == "Crickets(Clone)" || agent.name == "Flies(Clone)" || agent.name == "Harvester Termite(Clone)")) {
			Attack ();
		}else if (target.name == "Flies(Clone)" && (agent.name == "African Grey Hornbill(Clone)" || agent.name == "Cape Dwarf Gecko(Clone)" || agent.name == "Southern Ground Hornbill(Clone)")) {
			Attack ();
		}else if (target.name == "Greater Bushbaby(Clone)" && (agent.name == "Leopard(Clone)" || agent.name == "Nile Crocodile(Clone)")) {
			Attack ();
		}else if (target.name == "African Grey Hornbill(Clone)" && (agent.name == "African Clawless Otter(Clone)")) {
			Attack ();
		}else if (target.name == "Bohor Reedbuck(Clone)" && (agent.name == "Lion(Clone)" || agent.name == "Nile Crocodile(Clone)")) {
			Attack ();
		}else if (target.name == "Buffalo(Clone)" && (agent.name == "Lion(Clone)")) {
			Attack ();
		}else if (target.name == "Bush Hyrax(Clone)" && (agent.name == "Black Mamba(Clone)" || agent.name == "Leopard(Clone)")) {
			Attack ();
		}else if (target.name == "Crested Porcupine(Clone)" && (agent.name == "Black Mamba(Clone)" || agent.name == "Leopard(Clone)")) {
			Attack ();
		}else if (target.name == "Harvester Termite(Clone)" && (agent.name == "Ants(Clone)")) {
			Attack ();
		}else if (target.name == "Herbivorous True Bugs(Clone)" && (agent.name == "African Grey Hornbill(Clone)" || agent.name == "Cape Dwarf Gecko(Clone)")) {
			Attack ();
		}else if (target.name == "Kirk's Dik-dik(Clone)" && (agent.name == "Leopard(Clone)" || agent.name == "Lion(Clone)")) {
			Attack ();
		}else if (target.name == "Oribi(Clone)" && (agent.name == "Leopard(Clone)" || agent.name == "Lion(Clone)")) {
			Attack ();
		}else if (target.name == "Red-Faced Crombec(Clone)" && (agent.name == "Black Mamba(Clone)")) {
			Attack ();
		}else if (target.name == "Smith's Red Hock Hare(Clone)" && (agent.name == "Greater Bushbaby(Clone)" || agent.name == "Kirk's Dik-dik(Clone)" || agent.name == "Leopard(Clone)")) {
			Attack ();
		}else if (target.name == "Acacia(Clone)" && (agent.name == "Crickets(Clone)" || agent.name == "Greater Bushbaby(Clone)" || agent.name == "African Grey Hornbill(Clone)" || agent.name == "Black and White Columbus Monkey(Clone)" || agent.name == "Bush Pig(Clone)" || agent.name == "Cape Teal(Clone)"
			|| agent.name == "Crested Porcupine(Clone)" || agent.name == "Flies(Clone)" || agent.name == "Kirk's Dik-dik(Clone)" || agent.name == "Oribi(Clone)" || agent.name == "Red-Faced Crombec(Clone)" || agent.name == "Bohor Reedbuck(Clone)" || agent.name == "Ants(Clone)" || agent.name == "Bush Hyrax(Clone)" || agent.name == "Smith's Red Hock Hare(Clone)")) {
			Attack ();
		}else if (target.name == "Baobab(Clone)" && (agent.name == "Crickets(Clone)" || agent.name == "Greater Bushbaby(Clone)" || agent.name == "African Grey Hornbill(Clone)" || agent.name == "Black and White Columbus Monkey(Clone)" || agent.name == "Bush Pig(Clone)" || agent.name == "Cape Teal(Clone)"
			|| agent.name == "Crested Porcupine(Clone)" || agent.name == "Flies(Clone)" || agent.name == "Kirk's Dik-dik(Clone)" || agent.name == "Oribi(Clone)" || agent.name == "Red-Faced Crombec(Clone)" || agent.name == "Bohor Reedbuck(Clone)" || agent.name == "Ants(Clone)" || agent.name == "Bush Hyrax(Clone)" || agent.name == "Smith's Red Hock Hare(Clone)")) {
			Attack ();
		}else if (target.name == "Big Tree(Clone)" && (agent.name == "Crickets(Clone)" || agent.name == "Greater Bushbaby(Clone)" || agent.name == "African Grey Hornbill(Clone)" || agent.name == "Black and White Columbus Monkey(Clone)" || agent.name == "Bush Pig(Clone)" || agent.name == "Cape Teal(Clone)"
			|| agent.name == "Crested Porcupine(Clone)" || agent.name == "Flies(Clone)" || agent.name == "Kirk's Dik-dik(Clone)" || agent.name == "Oribi(Clone)" || agent.name == "Red-Faced Crombec(Clone)" || agent.name == "Bohor Reedbuck(Clone)" || agent.name == "Ants(Clone)" || agent.name == "Bush Hyrax(Clone)" || agent.name == "Smith's Red Hock Hare(Clone)")) {
			Attack ();
		}else if (target.name == "Fruits and Nectar(Clone)" && (agent.name == "Greater Bushbaby(Clone)" || agent.name == "African Grey Hornbill(Clone)" || agent.name == "Black and White Columbus Monkey(Clone)" || agent.name == "Bush Pig(Clone)" || agent.name == "Cape Teal(Clone)"
			|| agent.name == "Crested Porcupine(Clone)" || agent.name == "Flies(Clone)" || agent.name == "Kirk's Dik-dik(Clone)" || agent.name == "Oribi(Clone)" || agent.name == "Red-Faced Crombec(Clone)")) {
			Attack ();
		}else if (target.name == "Grains and Seeds(Clone)" && (agent.name == "Greater Bushbaby(Clone)" || agent.name == "African Grey Hornbill(Clone)" || agent.name == "Bush Pig(Clone)" || agent.name == "Cape Teal(Clone)"
			|| agent.name == "Crested Porcupine(Clone)" || agent.name == "Kirk's Dik-dik(Clone)" || agent.name == "Oribi(Clone)" || agent.name == "Red-Faced Crombec(Clone)" || agent.name == "Bohor Reedbuck(Clone)")) {
			Attack ();
		}else if (target.name == "Grass and Herbs(Clone)" && (agent.name == "Crickets(Clone)" || agent.name == "African Grey Hornbill(Clone)"
			|| agent.name == "Kirk's Dik-dik(Clone)" || agent.name == "Oribi(Clone)" || agent.name == "Bohor Reedbuck(Clone)" || agent.name == "Smith's Red Hock Hare(Clone)")) {
			Attack ();
		}else if (target.name == "Plant Juices(Clone)" && (agent.name == "Crickets(Clone)" || agent.name == "Greater Bushbaby(Clone)")) {
			Attack ();
		}else if (target.name == "Trees and Shrubs(Clone)" && (agent.name == "Crickets(Clone)" || agent.name == "Black and White Columbus Monkey(Clone)" || agent.name == "Cape Teal(Clone)"
			|| agent.name == "Kirk's Dik-dik(Clone)" || agent.name == "Oribi(Clone)" || agent.name == "Bohor Reedbuck(Clone)" || agent.name == "Ants(Clone)" || agent.name == "Bush Hyrax(Clone)" || agent.name == "Smith's Red Hock Hare(Clone)")) {
			Attack ();
		}
	}

    void Idle ()
    {
        //Added by Omar triggers eating animation
        if (anim != null) {
            anim.SetTrigger ("Eating");
        }
    }

    void Attack ()
    {
        timer = 0f;
        if (agent && agent.isActiveAndEnabled) {
            agent.destination = target.transform.position;

//			Debug.Log (tag + " " + species.name +
//			           " distance to " +
//			           target.tag + " " + target.species.name +
//			           " is " + agent.remainingDistance);
            if (agent.remainingDistance <= agent.stoppingDistance) {
                //Added by Omar triggers Attacking animation
//				Debug.Log(species.name + " attacking " + target.species.name);
                agent.gameObject.transform.LookAt (target.transform);
                if (anim != null) {
                    anim.SetTrigger ("Attacking");
                }
                target.TakeDamage (damage, this);
            } else {
                if (anim != null) {
                    anim.SetTrigger ("Walking");
                }
            }
        }
    }

    void Die ()
    {
        //Disable all functions here
        if (anim != null) {
            anim.SetTrigger ("Dead");
        }
		if (this.gameObject.tag == "Ally") {
			controller.allySpecies[species.name] -= 1;
			//controller.ActiveSpecies ();
		}
		if (this.gameObject.tag == "Enemy") {
			controller.enemySpecies[species.name] -= 1;
		}


        target = null;
        if (agent != null)
            agent.enabled = false;
        if (species.type == ClashSpecies.SpeciesType.PLANT) {
            RemoveBuffs (species, this.gameObject.tag);
            if (this.gameObject.tag == "Ally")
                controller.UpdateBuffPanel (species, false);
            this.gameObject.GetComponentInChildren<Renderer> ().enabled = false;
        }
    }

    void TakeDamage (int damage, ClashBattleUnit source = null)
    {
        if (timeSinceSpawn < 1.0)
            return; // Be invincible for the first second after being spawned
//		Debug.Log (tag + " " + species.name + " taking " + damage + " damage from " + source.tag + " " + source.species.name);

        currentHealth = Mathf.Max (0, currentHealth - damage);
        if (currentHealth == 0) {
            Die ();
        }
    }

    void RemoveBuffs (ClashSpecies cs, string tag)
    {
        var team = GameObject.FindGameObjectsWithTag (tag);
		
        foreach (var teammate in team) {
            //teammate != this.gameObject so it doesn't get a buff from itself
            if (teammate != this.gameObject) {
                var teammateAttribute = teammate.GetComponent<ClashBattleUnit> ();

                switch (cs.name) {
                case "Big Tree":	//hp buff
                    teammateAttribute.currentHealth -= 100;
                    //teammateAttribute.TakeDamage (100);
                    break;
                case "Baobab":	//damage buff
                    teammateAttribute.damage -= 8;
                    break;
                case "Trees and Shrubs":	//attack speed buff
                    if (teammateAttribute.agent != null)
                        teammateAttribute.agent.speed -= 5.0f;
                    break;
                default:
                    break;
                }
            }
        }
    }

    public void setSelected (bool isSelected)
    {
        foreach (Transform child in transform) {
            if (child.CompareTag (Constants.TAG_HEALTH_BAR))
                child.gameObject.SetActive (isSelected);
        }
    }
}
