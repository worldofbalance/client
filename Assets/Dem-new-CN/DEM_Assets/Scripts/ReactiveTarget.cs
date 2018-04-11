using UnityEngine;
using System.Collections;

// added as a component to the enemy or player objects so it can react to being hit by dying

public class ReactiveTarget : MonoBehaviour {

	public void ReactToHit() {
		// If this object has a SpeciesBehavior component, get that object, 
		// and set its alive state to false, so it can wander no more
		SpeciesBehavior behavior = GetComponent<SpeciesBehavior>();
		if (behavior != null) {
			behavior.setAlive(false);
		}
		// Start a coroutine Die to let the object react to being hit
		StartCoroutine(Die());
	}

	private IEnumerator Die() {
		// The object reacts to being hit by falling over,
		this.transform.Rotate(-75, 0, 0);
		// and then laying dead for 1.5 seconds, while the function yields control,
		// so that the game keeps on playing.
		yield return new WaitForSeconds(1.5f);
		// After 1.5 seconds, the dead object is destroyed, so it leaves the game.
		Destroy(this.gameObject);
	}
}
