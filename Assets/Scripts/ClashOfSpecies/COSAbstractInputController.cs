using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public abstract class COSAbstractInputController:ScriptableObject
{
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
                                ClashSpecies selected, Dictionary<int, int> remaining, ToggleGroup toggleGroup)
    {
        NavMeshHit placement;

        if (NavMesh.SamplePosition(hit.point, out placement, 100, walkableAreaMask) && hit.collider.CompareTag("Terrain"))
        {
            Debug.Log("mask is" + placement.mask);
            var allyResource = Resources.Load<GameObject>("Prefabs/ClashOfSpecies/Units/" + selected.name);
            var allyObject = Instantiate(allyResource, placement.position, Quaternion.identity) as GameObject;
            allyObject.tag = "Ally";

            remaining[selected.id]--;
            var toggle = toggleGroup.ActiveToggles().FirstOrDefault();
            toggle.transform.parent.GetComponent<ClashUnitListItem>().amountLabel.text = remaining[selected.id].ToString();
            if (remaining[selected.id] == 0)
            {
                toggle.enabled = false;
                toggle.interactable = false;
                //                selected = null;
            }
            return allyObject;
        }
        return null;
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

