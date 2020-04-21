using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public abstract class COSAbstractInputController:ScriptableObject
{
	private float tileSize = 0.5f;
    protected COSTouchState eTouchRes;
    protected static int walkableAreaMask;

    public COSTouchState TouchState
    {
        get { return eTouchRes; }
        set { eTouchRes = value; }
    }

    public COSAbstractInputController()
    {
        eTouchRes = COSTouchState.None;

    }

    public abstract RaycastHit InputUpdate(Camera _camera);

    public abstract void InputControllerAwake(Terrain terrain);

    public GameObject SpawnAlly(RaycastHit hit, 
		ClashSpecies selected, Dictionary<int, int> remaining, ToggleGroup toggleGroup, Vector3 position)
    {
            var allyResource = Resources.Load<GameObject>("Prefabs/ClashOfSpecies/Units/" + selected.name);
            var allyObject = Instantiate(allyResource, position, Quaternion.identity) as GameObject;

            allyObject.tag = "Ally";

            remaining[selected.id]--;
            var toggle = toggleGroup.ActiveToggles().FirstOrDefault();
            if (remaining[selected.id] == 0)
            {
                toggle.enabled = false;
                toggle.interactable = false;
            }
			allyObject.transform.position = position;
            return allyObject;
    }

}

public enum COSTouchState
{
    None,
    TerrainTapped,
    IsZooming,
    IsRotating,
    IsPanning
}

