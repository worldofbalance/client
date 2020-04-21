using UnityEngine;

using System.Collections;

public class EcosystemCamera : MonoBehaviour {

    private Vector3 mouseDownPos;
    private Vector3 oldCameraPos;

    //Camera Zoom related variables
    private float cameraZoomed = 15f;
    private float cameraNormal = 60f;

    private float cameraSmoothing = 5f;
    private bool isZoomed = false;
    private float cameraEnter = 90f;
    private float cameraEnterSmoothing = 2.5f;

    public int isEntering { get; set; }
    private bool isDragging = false;
    private bool isPanning = false;
    private bool isZooming = false;

    private Vector3 cameraStartPos;
    private Vector3 cameraCenterPos;
    private float cameraOffset;
    private Vector3 cameraNextPos;

    private Vector3 nLowerBound;
    private Vector3 nUpperBound;
    private Vector3 zLowerBound;
    private Vector3 zUpperBound;
	private Database database;

    void Awake() {
        TriggerEntering();
    }

    // Use this for initialization
    void Start() {		
        cameraStartPos = transform.position;
        Setup();
    }

    // Update is called once per frame
    void Update() {
        if (PerformCameraEntrance()) {
            return;
        }
			
		if (database.getActive()) {
			return;
		}

        switch (InputExtended.GetMouseNumClick(0)) {
            case 1: // Single Click
                // mouse drag
                mouseDownPos = Input.mousePosition;
                oldCameraPos = transform.position;
                break;
            case 2: // Double Click
                isZoomed = !isZoomed;

                if (isZoomed) {
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit)) {
                        float xPos = hit.point.x;
                        xPos = Mathf.Clamp(xPos, zLowerBound.x, zUpperBound.x);

                        float zPos = hit.point.z + cameraOffset;
                        zPos = Mathf.Clamp(zPos, zLowerBound.z, zUpperBound.z);

                        cameraNextPos = new Vector3(xPos, transform.position.y, zPos);
                    }
                }

                isZooming = isPanning = true;
                break;
        }

        if (Input.GetMouseButton(0)) {
            isDragging = Vector3.Distance(mouseDownPos, Input.mousePosition) > 0.1f;
        } else {
            isDragging = false;
        }

        if (isZoomed) {
            if (isDragging) {
                float xPos = oldCameraPos.x + (mouseDownPos.x - Input.mousePosition.x) * 0.2f;
                xPos = Mathf.Clamp(xPos, zLowerBound.x - 20, zUpperBound.x + 20);

                float zPos = oldCameraPos.z + (mouseDownPos.y - Input.mousePosition.y) * 0.2f;
                zPos = Mathf.Clamp(zPos, zLowerBound.z - 20, zUpperBound.z + 20);

                transform.position = new Vector3(xPos, transform.position.y, zPos);
                isPanning = false;
            }

            if (isPanning && Vector3.Distance(transform.position, cameraNextPos) > 0.1f) {
                transform.position = Vector3.Lerp(transform.position, cameraNextPos, Time.deltaTime * cameraSmoothing);
            } else {
                isPanning = false;
            }

            if (isZooming && !Mathf.Approximately(GetComponent<Camera>().fieldOfView, cameraZoomed)) {
                GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, cameraZoomed, Time.deltaTime * cameraSmoothing);
            } else {
                isZooming = false;
            }

            if (!isDragging) {
                transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(transform.position.x, zLowerBound.x, zUpperBound.x), transform.position.y, Mathf.Clamp(transform.position.z, zLowerBound.z, zUpperBound.z)), Time.deltaTime * cameraSmoothing);
            }
        } else {
            if (isDragging) {
                float xPos = oldCameraPos.x + (mouseDownPos.x - Input.mousePosition.x) * 0.4f;
                // xPos = Mathf.Clamp(xPos, cameraStartPos.x - 30, cameraStartPos.x + 30);
                xPos = Mathf.Clamp(xPos, nLowerBound.x - 30, nUpperBound.x + 30);

                float zPos = oldCameraPos.z + (mouseDownPos.y - Input.mousePosition.y) * 0.4f;
                // zPos = Mathf.Clamp(zPos, cameraStartPos.z - 30, cameraStartPos.z + 30);
                zPos = Mathf.Clamp(zPos, nLowerBound.z - 30, nUpperBound.z + 30);

                transform.position = new Vector3(xPos, transform.position.y, zPos);
                isPanning = false;
            }

            if (isPanning && Vector3.Distance(transform.position, cameraStartPos) > 0.1f) {
                transform.position = Vector3.Lerp(transform.position, cameraStartPos, Time.deltaTime * cameraSmoothing);
            } else {
                isPanning = false;
            }

            if (isZooming && !Mathf.Approximately(GetComponent<Camera>().fieldOfView, cameraNormal)) {
                GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, cameraNormal, Time.deltaTime * cameraSmoothing);
            } else {
                isZooming = false;
            }

           // if (!isDragging && Vector3.Distance(transform.position, cameraStartPos) > 0.1f) {
           //     transform.position = Vector3.Lerp(transform.position, new Vector3(cameraStartPos.x, transform.position.y, cameraStartPos.z), Time.deltaTime * cameraSmoothing);
           // }

            if (!isDragging) {
                transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(transform.position.x, nLowerBound.x, nUpperBound.x), transform.position.y, Mathf.Clamp(transform.position.z, nLowerBound.z, nUpperBound.z)), Time.deltaTime * cameraSmoothing);
            }
        }
    }

    public void Setup() {
		database = GameObject.Find("Local Object").GetComponent<EcosystemController>().getDatabase();
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)), out hit)) {
            cameraOffset = transform.position.z - hit.point.z;
        }

        EcosystemController ecoController = GameObject.Find("Local Object").GetComponent<EcosystemController>();

        nLowerBound = ecoController.lowerBound;
        nLowerBound = new Vector3(Mathf.Min(0, nLowerBound.x + 120), 0, Mathf.Min(-75, nLowerBound.z - 30));

        nUpperBound = ecoController.upperBound;
        nUpperBound = new Vector3(Mathf.Max(0, nUpperBound.x - 120), 0, Mathf.Max(-75, nUpperBound.z + cameraOffset - 60));

        zLowerBound = ecoController.lowerBound;
        zLowerBound = new Vector3(Mathf.Min(-80, zLowerBound.x), 0, Mathf.Min(-145, zLowerBound.z - 75));

        zUpperBound = ecoController.upperBound;
        zUpperBound = new Vector3(Mathf.Max(80, zUpperBound.x), 0, Mathf.Max(25, zUpperBound.z + cameraOffset + 10));
    }

    private bool PerformCameraEntrance() {
        if (Input.GetKeyDown("5")) {
            TriggerEntering();
        }
        if (Input.GetKeyDown("6")) {
            TriggerLeaving();
        }
        if (isEntering != 0) {
            transform.position = Vector3.Lerp(transform.position, cameraStartPos, Time.deltaTime * cameraSmoothing);
        }

        if (isEntering > 0 && Mathf.Abs(GetComponent<Camera>().fieldOfView - cameraNormal) > 1f) {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, cameraNormal, Time.deltaTime * cameraEnterSmoothing);
        } else if (isEntering < 0 && Mathf.Abs(GetComponent<Camera>().fieldOfView - cameraEnter) > 1f) {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, cameraEnter, Time.deltaTime * cameraEnterSmoothing);
        } else {
            isEntering = 0;
        }

        return isEntering != 0;
    }

    public float GetEnteringProgress() {
        return Mathf.Abs(GetComponent<Camera>().fieldOfView - cameraEnter);
    }

    public void TriggerEntering() {
        isDragging = isPanning = isZooming = false;

        isEntering = 1;
        GetComponent<Camera>().fieldOfView = cameraEnter;
    }

    public void TriggerLeaving() {
        isDragging = isPanning = isZooming = false;

        isEntering = -1;
        GetComponent<Camera>().fieldOfView = cameraNormal;
    }
}
