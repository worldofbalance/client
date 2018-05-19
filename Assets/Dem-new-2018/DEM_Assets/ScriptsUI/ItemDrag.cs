using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour{
	public bool dragging = false;
	public bool collision = false;
	Vector3 position;

	Ray myRay;
	RaycastHit hit;
	Species3DFactory factory;
	public GameObject objectToInstantiate;

	void Awake() {
		factory = GameObject.FindGameObjectWithTag ("GameController").GetComponent<Species3DFactory> ();
	}

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
				//GameObject speciesToMake = Instantiate (objectToInstantiate, hit.point, Quaternion.identity) as GameObject;
				//string speciesType = speciesToMake.ToString ();
				string speciesType = objectToInstantiate.ToString();

				if (objectToInstantiate.tag != "Plant") {
					
					ArrayList prey = new ArrayList ();
					string diet = factory.getSpeciesDietType(speciesType);
					prey = factory.setAnimalPrey(diet);
					GameObject clone = Instantiate (objectToInstantiate, hit.point, Quaternion.identity) as GameObject;
					SpeciesBehavior behavior = clone.AddComponent<PlayerAnimalBehavior> ();
					behavior.setDietType (diet);
					behavior.setPreyList (prey);

				} else {
					Instantiate (objectToInstantiate, hit.point, Quaternion.identity);
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
