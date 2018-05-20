using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; //for mouseover
public class NavBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public RectTransform container;
	public bool isOpen;


	// Use this for initialization
	void Start () {
		container = transform.Find ("container").GetComponent<RectTransform> ();
		isOpen = false;

	}

	// Update is called once per frame
	void Update () {
		//if (menuScript.menuOpen == true) {
		//	menuScript.CloseAllMenus ();
			Vector3 scale = container.localScale;
			//mathf(to, from, time)
			//if isOpen = true, set x to 1, else 0
			scale.y = Mathf.Lerp (scale.y, isOpen ? 1 : 0, Time.deltaTime * 40);
			container.localScale = scale;
		//}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isOpen = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isOpen = false;
	}
}
