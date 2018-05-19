using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour{
	public bool dragging = false;
	public bool collision = false;
	Vector3 position;

	Ray myRay;
	RaycastHit hit;
	Species3DFactory Factory;
	public GameObject objectToInstantiate;

	void Update(){

	}

	public void drop () {
		if (PlayerStats.Money <= 0) {
			gameObject.transform.position = position;
			return;
		}
		myRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (myRay, out hit)) {
			if (Input.GetMouseButtonUp (0)) {
				Instantiate (objectToInstantiate, hit.point, Quaternion.identity);
				if (objectToInstantiate.tag != "Plant") {
					/*string species = "Elephant";
					ArrayList prey = new ArrayList ();
					SpeciesBehavior behavior;
					objectToInstantiate.AddComponent<EnemyBehavior> ();
					behavior = objectToInstantiate.GetComponent<EnemyBehavior> ();
					objectToInstantiate.AddComponent<EnemyAINavigation> ();
					objectToInstantiate.tag = "Enemy";
					prey.Add ("TreeOfLife");
					prey = Factory.setAnimalPrey("Omnivore");
					behavior.setDietType("Omnivore");
					behavior.setPreyList (prey);*/
				}

				gameObject.transform.position = position;
				PlayerStats.Money--;
			}
		}
		return;
	}
		
	public void beginDrag(){
		position = gameObject.transform.position;
		dragging = true;
	}

	public void drag(){
		transform.position = Input.mousePosition;
	}

}
