using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ClashBattleUnit : MonoBehaviour
{
    public NavMeshAgent agent;
	public ClashSpecies species;
    private ClashBattleController controller;
    private Animator anim;

    public ClashBattleUnit target;
    Vector3 targetPoint = Vector3.zero;

    public Vector3 TargetPoint {
        get { return targetPoint; }
        set { targetPoint = value; }
    }

    public int currentHealth = 0;
    public int damage = 0;
	// The time in seconds between each attack.
    public float timeBetweenAttacks = 1.0f;
    float timer;

    void Awake (){
        agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
        anim = GetComponent<Animator> ();
        controller = GameObject.Find ("Battle Menu").GetComponent<ClashBattleController> ();
    }

    void Start (){
        // Set current health depending on the species data.
        currentHealth += species.hp;
        timeBetweenAttacks = 100f / species.attackSpeed;
        damage += species.attack;
        if (agent != null)
            agent.speed += species.moveSpeed / 20.0f;
    }

    void Update (){
        timer += Time.deltaTime;
        if (!target && targetPoint == Vector3.zero) {
            Idle ();
        } else if (targetPoint != Vector3.zero) {
            anim.SetTrigger ("Walking");
            if (agent && agent.isActiveAndEnabled)
                agent.destination = targetPoint;
        } else if ((target.currentHealth > 0) && (timer >= timeBetweenAttacks) && (currentHealth >= 0.0f)) {
			Attack();
        } else if (target.currentHealth <= 0) {
            target = null;		
        }
    }

    void Idle (){
        //Triggers eating animation
        if (anim != null)
            anim.SetTrigger ("Eating");
    }

    void Attack (){
        timer = 0f;
        if (agent && agent.isActiveAndEnabled) {
            agent.destination = target.transform.position;

//			Debug.Log (tag + " " + species.name +
//			           " distance to " +
//			           target.tag + " " + target.species.name +
//			           " is " + agent.remainingDistance);
            if (agent.remainingDistance <= agent.stoppingDistance) {
                //Triggers Attacking animation
//				Debug.Log(species.name + " attacking " + target.species.name);
                agent.gameObject.transform.LookAt (target.transform);
                if (anim != null)
                    anim.SetTrigger ("Attacking");
                target.TakeDamage (damage, this);
            } else {
                if (anim != null)
                    anim.SetTrigger ("Walking");
            }
        }
    }

    void Die (){
        //Disable all functions here
        if (anim != null)
            anim.SetTrigger ("Dead");
		if (this.gameObject.tag == "Ally")
			controller.allySpecies[species.name] -= 1;
			//controller.ActiveSpecies ();
		if (this.gameObject.tag == "Enemy")
			controller.enemySpecies[species.name] -= 1;

        target = null;
        if (agent != null)
            agent.enabled = false;
        if (species.type == ClashSpecies.SpeciesType.PLANT)
            this.gameObject.GetComponentInChildren<Renderer> ().enabled = false;
    }

    void TakeDamage (int damage, ClashBattleUnit source = null){
		//Debug.Log (tag + " " + species.name + " taking " + damage + " damage from " + source.tag + " " + source.species.name);

        currentHealth = Mathf.Max (0, currentHealth - damage);
        if (currentHealth == 0)
            Die ();
    }

//    public void setSelected (bool isSelected){
//        foreach (Transform child in transform) {
//            if (child.CompareTag (Constants.TAG_HEALTH_BAR))
//                child.gameObject.SetActive (isSelected);
//        }
//    }
}
