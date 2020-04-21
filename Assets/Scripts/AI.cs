using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AI : MonoBehaviour {
	
	private bool isMoving;
	public Vector3 destination;
	public float speed = 0.5f;
	private NavMeshAgent agent;
	public GameObject alphaLeader {get; set;}
	public Vector3 territoryPos {get; set;}
	
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		StartCoroutine(ChooseDestination(0.0f));
	}
	
	// Update is called once per frame
	void Update () {
		if (isMoving) {
			if (GetComponent<Animation>() != null) {
				if (transform.GetComponent<Animation>()["walk"]) {
					transform.GetComponent<Animation>().Play("walk");
				}
			}

			if (agent.remainingDistance < 3) {
				isMoving = false;
				StartCoroutine(ChooseDestination(1f));
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		string name = other.gameObject.name;

		if (name.StartsWith("Tile")) {
			int species_id = int.Parse(gameObject.name.Split('_')[1]);
			int tile_id = int.Parse(name.Split('_')[1]);
			
//			Debug.Log(gameObject.name + " enters Tile #" + tile_id);

			// Entered "Tile" (Visual Purposes Only)
			other.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
		}
	}
	
	void OnTriggerExit(Collider other) {
		// Exited "Tile" (Visual Purposes Only)
		other.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.3f);
	}

	public IEnumerator ChooseDestination(float time) {
		return ChooseDestination(time, Vector3.one);
	}

    public IEnumerator ChooseDestination(float time, Vector3 faceDirection) {
		yield return new WaitForSeconds(time);

		isMoving = true;
		
		faceDirection.Scale(new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)));

		if (gameObject == alphaLeader) {
			faceDirection *= Random.Range(30, 200);
			destination = transform.position + faceDirection;
		}  else {
			faceDirection *= Random.Range(30, 100);
			destination = alphaLeader.transform.position + faceDirection;
		}
		
		NavMeshHit hit;
		
		if (NavMesh.SamplePosition(destination, out hit, 100, 1)) {
			destination = hit.position;
			agent.SetDestination(destination);

			//CreateDestinationSphere(destination);
		}
	}
	
	public void CreateDestinationSphere(Vector3 destination) {
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.GetComponent<Collider>().enabled = false;
		sphere.transform.localScale = new Vector3(5f, 5f, 5f);
		sphere.transform.position = destination;
	}
}

