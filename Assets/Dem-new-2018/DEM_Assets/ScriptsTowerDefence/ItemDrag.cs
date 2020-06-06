using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IDragHandler, IEndDragHandler {

	public void OnDrag(PointerEventData eventData){
		transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData){
		transform.localPosition = Vector3.zero;
	}



	Ray myRay;
	RaycastHit hit;
	public GameObject objectToInstantiate;
	// Use this for initialization
	void Start () {
		Debug.Log ("Place Plant");
	}
	
	// Update is called once per frame
	public void CreatePlant () {
		myRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (myRay, out hit)) {
			if (Input.GetMouseButtonUp (0)) {
				Instantiate (objectToInstantiate, hit.point, Quaternion.identity);
			}
		}
		return;
	}
}
