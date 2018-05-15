using UnityEngine;
using System.Collections;


// attach this as a component of the species prefabs for animals and plants

public class SpeciesBehavior : MonoBehaviour 
{
	// the specific type of animal or plant
	protected string species;
	//diet is "Omnivore", "Carnivore", "Herbivore", or "Plant"
	protected string dietType;
	protected ArrayList preyList;
	protected bool alive;
	protected int health;
	protected const int maxHealth = 6;
	protected const int injured = 3;
	protected const int dead = 0;
	// for action on the game board
	protected AttackBehavior attackBehavior;


	// the initial state, each inheriting behavior type has its own Start() 
	// for additional plant or animal specific stuff, which would be called after the Awake() function
	void Awake() {
		this.alive = true;
		this.health = maxHealth;
		preyList = new ArrayList ();
		attackBehavior = new AttackBehavior ();
	}


	public virtual void ReactToHit() {
		// set its alive state to false, so it can wander no more
		setAlive(false);
		// Start a coroutine Die to let the object react to being hit
		StartCoroutine(Die());
	}


	public virtual IEnumerator Die() {
		// The object reacts to being hit by falling over,
		this.transform.Rotate(0, 0, -75);
		// and then laying dead for 1.5 seconds, while the function yields control,
		// so that the game keeps on playing.
		yield return new WaitForSeconds(2.0f);
		// After 1.5 seconds, the dead object is destroyed, so it leaves the game.
		Destroy(this.gameObject);
	}





	/*** getters and setters ****/

	public void setDietType(string dietType){
		this.dietType = dietType;
	}

	public string getDietType() {
		return dietType;
	}

	public void setSpecies(string species){
		this.species = species;
	}
		
	public string getSpecies() {
		return species;
	}

	public void setPreyList(ArrayList prey){
		this.preyList = prey;
	}
		
	public ArrayList getPreyList() {
		return preyList;
	}

	public void setAlive(bool alive) {
		this.alive = alive;
	}

	public bool getAlive(){
		return this.alive;
	}
		
	public void setHealth(int health){
		this.health = health;
	}

	public int getHealth() { 
		return this.health; 
	}

}
