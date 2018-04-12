using UnityEngine;
using System.Collections;

// Make the RayShooter script a component of the camera object (the camera not the player),
// by draging the script from Project Assets screen onto the camera in the Hierarchy screen.
// This illustrates the concept of ray casting.

public class RayShooter : MonoBehaviour {
	private Camera _camera;

	void Start() {
		// get access to the camera and its components
		_camera = GetComponent<Camera>();
		// locks the location of the mouse cursor in the center of the screen,
		// and makes it invisible so that it does not distract.
		// WARNING: Remember to use Escape Key (ESC) to restore the mouse cursor 
		// so you can leave the game.
		Debug.Log("WARNING: Remember to use Escape Key to restore the mouse cursor.");
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// this lets the user know where the ray shooter is currently pointing,
	// by laying the text "*" on the screen using a GUI game interface lable
	void OnGUI() {
		int size = 12;
		float posX = _camera.pixelWidth/2 - size/4;
		float posY = _camera.pixelHeight/2 - size/2;
		GUI.Label(new Rect(posX, posY, size, size), "*");
	}

	// If the mouse was clicked, shoot a ray in the direction that the camera is looking,
	// and at the center of the camera field of view.
	void Update() {
		
		// if the mouse has been clicked this returns true
		if (Input.GetMouseButtonDown(0)) {
			// find the center of the camera field of view X and Y
			Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0);
			// create the ray (cast it out) with that vector location as its point of origin
			Ray ray = _camera.ScreenPointToRay(point);
			RaycastHit hit;
			// did the cast out ray hit anything
			if (Physics.Raycast(ray, out hit)) {
				// get the object that the ray hit
				GameObject hitObject = hit.transform.gameObject;
				// get the reactive target component from the object that was hit
				// the target returned is null if the object hit does not contain that type of component
				SpeciesBehavior target = hitObject.GetComponent<SpeciesBehavior>();
				if (target != null) {
					// if true, tell the object that was hit to react to being hit
					target.ReactToHit();
				} else {
					// if false, show where the object was hit, by running a coroutine called SphereIndicator
					StartCoroutine(SphereIndicator(hit.point));
				} 
			}
		}
	}

	// This is called if a non-SpeciesBehavior object was hit by the mouse ray.
	// This shows where an object was hit, by creating a sphere at that point for one second.
	private IEnumerator SphereIndicator(Vector3 pos) {
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = pos;

		// Pause this function for one second, so that the user can see the sphere 
		// indicating where the ray hit an object.  
		// Meanwhile yield control to the rest of the game playing, and
		// then come back to this function to continue at the next line of code.
		yield return new WaitForSeconds(1);

		// destroy the temporary sphere object
		Destroy(sphere);
	}
}