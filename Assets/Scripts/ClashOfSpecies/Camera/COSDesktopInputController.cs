using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class COSDesktopInputController : COSAbstractInputController
{

    private SphereCollider reticle = null;

    private Terrain surface = null;

    private Bounds bounds;


    public Terrain target
    {
        set
        {
            surface = value;
            reticle = new GameObject("Reticle", typeof(SphereCollider), typeof(ClashReticle)).GetComponent<SphereCollider>();
            reticle.GetComponent<SphereCollider>().isTrigger = true;
            reticle.GetComponent<SphereCollider>().radius = 1.0f;

            // Set the reticle position to the center of the terrain.
            var reticlePos = value.transform.position + (value.terrainData.size * 0.5f);
            reticlePos.y = value.SampleHeight(reticle.transform.position);
            reticle.transform.position = reticlePos;
            reticle.transform.rotation = Quaternion.identity;

            // Set transform position to the initial offset. Assign the offset vector and make camera look at reticle.
            Camera.main.transform.position = reticle.transform.position + ((Vector3.back + Vector3.up).normalized * zoomLevel);
//            offset = Camera.main.transform.position - reticle.transform.position;
            Camera.main.transform.LookAt(reticle.transform);


            bounds = new Bounds(surface.terrainData.size * 0.5f, surface.terrainData.size);
        }
    }

    public bool dragging = false;
    public Vector3 lastMouse;
    public float yawSpeed = 1.0f;
    public float pitchSpeed = 1.0f;
    public float moveSpeed = 2.0f;
    public float zoomLevel = 100.0f;

    public float minFOV = 10f;
    public float maxFOV = 79.9f;
    //private float minX, maxX, minZ, maxZ, minY = 35.0f, maxY = 80.0f;
    public float terrainCameraPadding = 40;

    //    Camera _camera;

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(bounds.center, bounds.size);
    }

    public void setDraging(bool isDragging)
    {
        dragging = isDragging;
        lastMouse = Input.mousePosition;
    }

    //    COSTouchState // eTouchRes;
    //
    //    public COSTouchState TouchState {
    //        get { return // eTouchRes; }
    //        set { // eTouchRes = value; }
    //    }

    public COSDesktopInputController()
    {
        eTouchRes = COSTouchState.None;

    }

    public override void InputControllerAwake(Terrain surface)
    {
        target = surface;
        walkableAreaMask = (int)Math.Pow(2, UnityEngine.AI.NavMesh.GetAreaFromName("Walkable"));
		/*
        minX = terrainCameraPadding;
        maxX = Terrain.activeTerrain.terrainData.size.x - terrainCameraPadding;
        minZ = terrainCameraPadding;
        maxZ = Terrain.activeTerrain.terrainData.size.z - terrainCameraPadding;
        */
    }

    public override RaycastHit InputUpdate(Camera _camera)
    {
        RaycastHit hit = new RaycastHit();
        if (!reticle)
            return hit;

        if (Input.GetMouseButtonDown(1))
        {
            dragging = true;
            eTouchRes = COSTouchState.IsPanning;
            lastMouse = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            dragging = false;
            eTouchRes = COSTouchState.None;
        }
        else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            eTouchRes = COSTouchState.TerrainTapped;
            Debug.Log("clicked using mouse");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
                return hit;
        }
        else
        {
            eTouchRes = COSTouchState.None;
        }

        var offset = reticle.transform.position - _camera.transform.position;
        var tempX = Vector3.Cross(Vector3.up, offset).normalized;
        var tempZ = Vector3.Cross(tempX, Vector3.up).normalized;

        Debug.DrawRay(reticle.transform.position, tempX, Color.red);
        Debug.DrawRay(reticle.transform.position, tempZ, Color.blue);


		//CAMERA STUFF COMMENTING OUT FOR NOW ALFRED ** 4/11
        /*if (dragging)
        {
            var delta = Input.mousePosition - lastMouse;
            _camera.transform.RotateAround(reticle.transform.position, Vector3.up, yawSpeed * delta.x);
            _camera.transform.RotateAround(reticle.transform.position, tempX, -pitchSpeed * delta.y);
//            if (transform.position.y >= minY &&_camera.transform.position.y <= maxY)
//               _camera.transform.RotateAround(reticle.transform.position, tempX, -pitchSpeed * delta.y);
//            else
//               _camera.transform.position = new Vector3(transform.position.x,
//                    Mathf.Clamp(_camera.fieldOfView, minY, maxY),
//                   _camera.transform.position.z);

            // Counter-rotate to offset overpitch.
            if (_camera.transform.rotation.eulerAngles.x > 80.0f)
            {
                if (_camera.transform.position.y > 35.0f)
                {
                    _camera.transform.RotateAround(reticle.transform.position, tempX, -1.0f * (_camera.transform.rotation.eulerAngles.x - 85.0f));
                }
                else
                {
                    _camera.transform.RotateAround(reticle.transform.position, tempX, 360.0f - _camera.transform.rotation.eulerAngles.x);
                }
            }
            _camera.transform.LookAt(reticle.transform, Vector3.up);
            lastMouse = Input.mousePosition;
        }
        else
        {
            dragging = false;
        }

        // Calculate the attempted translation.
        var attempt = (tempX * Input.GetAxis("Horizontal")) + (tempZ * Input.GetAxis("Vertical")) * moveSpeed;

        // Attempt to move the reticle, ensuring it remains on the board.
        var height = surface.SampleHeight(reticle.transform.position + attempt);
        var valid = bounds.Contains(reticle.transform.position + attempt);

        attempt.y = height - reticle.transform.position.y;

        if (!valid)
            return hit;

        // If succesful, translate both the camera_camera.transform and the reticle by the same amount.
        reticle.transform.Translate(attempt);
        _camera.transform.Translate(attempt, Space.World);
		
        CheckZoom();
        */
        return hit;
    }



    /*public void CheckZoom()
    {
        

        // Handle zoom.
        var zoomAxis = (Camera.main.transform.position - reticle.transform.position).normalized;
        if (Input.GetKey(KeyCode.Q))
        {
//            zoomLevel = Mathf.Max(20.0f, zoomLevel - 2.0f);
            Camera.main.fieldOfView -= 1.0f;
        }

        if (Input.GetKey(KeyCode.Z))
        {
//            zoomLevel = Mathf.Min(50.0f, zoomLevel + 2.0f);
            Camera.main.fieldOfView += 1.0f;
        }
        // Clamp the field of view to make sure it's between minFOV and maxFOV.
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minFOV, maxFOV);



//        transform.position = reticle.transform.position + (zoomAxis * zoomLevel);
//        transform.LookAt(reticle.transform);
    }*/
}
