using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchInputSetup : MonoBehaviour
{
    private COSAbstractInputController cosInController;

    public bool isRunningOnMobile;
    public Text angle, angleValue;
    public Text pinch, pinchValue;

    public Slider slidePinch;
    public Slider slideRotate;

    ClashGameManager manager;
    public EventSystem eventSystem;

    // Use this for initialization
    void Start()
    {
	
        if (isRunningOnMobile)
            cosInController = ScriptableObject.CreateInstance<COSMobileInputControler>();
        else
            cosInController = ScriptableObject.CreateInstance<COSDesktopInputController>();
        cosInController.InputControllerAwake(Terrain.activeTerrain);

        slidePinch.value = DetectTouchMovement.pinchRatio;
        slideRotate.value = DetectTouchMovement.pinchTurnRatio;

        manager = GameObject.Find("MainObject").GetComponent<ClashGameManager>();
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
			ScreenCapture.CaptureScreenshot("/Users/dusan_cvetkovic/Documents/SFSUdocs/Semester2/CSC831MultGameD/Screenshot.png");
            Debug.Log("screenshot taken!!");
        }

        if (!EventSystem.current.IsPointerOverGameObject())
            cosInController.InputUpdate(Camera.main);
        
        angle.text = "angle " + DetectTouchMovement.turnAngleDeltaAccumulated;
        pinch.text = "pinch " + DetectTouchMovement.pinchDistanceDeltaAccumulated;

        angleValue.text = slideRotate.value + "";
        pinchValue.text = slidePinch.value + "";

        DetectTouchMovement.pinchRatio = slidePinch.value;
        DetectTouchMovement.pinchTurnRatio = slideRotate.value;

        manager.rotateSpeed = DetectTouchMovement.pinchTurnRatio;
        manager.zoomSpeed = DetectTouchMovement.pinchRatio;
    }

    public void GoToClashMain()
    {
        Game.LoadScene("ClashMain");
    }
}
