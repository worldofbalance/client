using UnityEngine;
using System.Collections;


// attach this as a component of the species prefabs for animals and plants

public class SpeciesBehavior : MonoBehaviour 
{
	// The types are Omnivore, Carnivore, Herbivore, and Plant.
	protected SpeciesFactory.SpeciesType speciesType;
	protected ArrayList preyList;

	protected bool alive;
	protected int health;
	protected const int maxHealth = 6;
	protected const int injured = 3;
	protected const int dead = 0;


	// the initial state
	void Start() {
		this.alive = true;
		this.health = maxHealth;
		preyList = new ArrayList();
	}
		
	public void setSpeciesType(SpeciesFactory.SpeciesType species){
		speciesType = species;
		// TODO add correct material based on species type ??
	}

	public SpeciesFactory.SpeciesType getSpeciesType() {
		return speciesType;
	}

	public void setPreyList(ArrayList prey){
		this.preyList = prey;
	}

	// for testing of code, uses a hard coded list,
	// sets preyList to empty list for plants
	public void setPreyList(SpeciesFactory.SpeciesType species)
	{
		SpeciesFactory factory = GameObject.Find("MasterController").GetComponent<SpeciesFactory>();
		preyList = factory.setAnimalPrey (species);
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

	// makes sure that all new health values are within the max and min range for health
	public void setHealth(int health){
		this.health = health;
	}

	public int getHealth() { 
		return this.health; 
	}

	public virtual void ReactToHit() {
		// set its alive state to false, so it can wander no more
		setAlive(false);
		// Start a coroutine Die to let the object react to being hit
		StartCoroutine(Die());
	}

	public virtual IEnumerator Die() {
		// The object reacts to being hit by falling over,
		this.transform.Rotate(-75, 0, 0);
		// and then laying dead for 1.5 seconds, while the function yields control,
		// so that the game keeps on playing.
		yield return new WaitForSeconds(1.0f);
		// After 1.5 seconds, the dead object is destroyed, so it leaves the game.
		Destroy(this.gameObject);
	}

}
