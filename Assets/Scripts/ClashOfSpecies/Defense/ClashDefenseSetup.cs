using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class ClashDefenseSetup : MonoBehaviour
{

    private Dictionary<int, int> remaining = new Dictionary<int, int>();
    private ClashGameManager manager;
    private ClashSpecies selected;
    private Terrain terrain;
    private ToggleGroup toggleGroup;

    public HorizontalLayoutGroup unitList;
    public GameObject defenseItemPrefab;

    public GameObject errorCanvas;
    public Text errorMessage;

	//tile building
	//cube
	public Transform tileTrans;
	public MeshRenderer tileRend;
	//material
	public Material canPlace;
	public Material cantPlace;

    COSAbstractInputController cosInController;


    void Awake()
    {
        try
        {
            manager = GameObject.Find("MainObject").GetComponent<ClashGameManager>();    
        }
        catch (Exception ex)
        {
            manager = GetComponent<ClashGameManager>();
        }

        toggleGroup = unitList.GetComponent<ToggleGroup>();
    }
    
    // Use this for initialization
    void Start()
    {
        try
        {
            var terrainObject = Resources.Load<GameObject>("Prefabs/ClashOfSpecies/Terrains/" + manager.pendingDefenseConfig.terrain);
            terrain = (Instantiate(terrainObject, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Terrain>();
        }
        catch (Exception ex)
        {
            terrain = Terrain.activeTerrain;
        }
        terrain.transform.position = Vector3.zero;
        terrain.transform.localScale = Vector3.one;


//        Camera.main.GetComponent<ClashBattleCamera>().target = terrain;

        foreach (var species in manager.pendingDefenseConfig.layout.Keys)
        {
            var currentSpecies = species;
            var item = Instantiate(defenseItemPrefab) as GameObject;
            remaining.Add(currentSpecies.id, 5);

            var itemReference = item.GetComponent<ClashUnitListItem>();

            var texture = Resources.Load<Texture2D>("Images/" + currentSpecies.name);
            itemReference.toggle.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            itemReference.toggle.onValueChanged.AddListener((val) =>
                {
                    if (val)
                    {
                        selected = currentSpecies;
                        itemReference.toggle.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    }
                    else
                    {
                        selected = null;
                        itemReference.toggle.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                });

            itemReference.toggle.group = toggleGroup;
            item.transform.SetParent(unitList.transform);
            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0.0f);
            item.transform.localScale = Vector3.one;
            itemReference.amountLabel.text = remaining[currentSpecies.id].ToString();
        }

        if (manager.isRunningOnMobile)
            cosInController = ScriptableObject.CreateInstance<COSMobileInputControler>();
        else
            cosInController = ScriptableObject.CreateInstance<COSDesktopInputController>();
        cosInController.InputControllerAwake(Terrain.activeTerrain);
    }


	private Vector3 tilePlacer(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		Vector3 newPos = new Vector3(0,0,0);
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			if (hit.collider.tag == "Terrain") {
				
				newPos = showTile (hit.point);
				if (checkBuildSpace (newPos)) {
					tileRend.material = canPlace;

					if (Input.GetMouseButtonDown (0)) {
						return newPos;
						/*CREATE OBJECT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
						//Transform go = Instantiate(chosenGO.transform, tileTrans.position, Quaternion.identity);
						//go.gameObject.name = "X : "+(int)Mathf.Floor(tileTrans.position.x)+" Z : "+(int)Mathf.Floor(tileTrans.position.z);
						//write to map
						//map [(int)Mathf.Floor(tileTrans.position.x)] [(int)Mathf.Floor(tileTrans.position.z)] = objectSelection;
					}
				}
				//Cant place on this part of terrain
				else
					tileRend.material = cantPlace;

			} 
			else if (hit.collider.tag != "Terrain") {
				//same as above fo different x y positions
				tileRend.material = cantPlace;
				showTile (hit.point);
			}
		} 
		else {
			tileRend.enabled = false;
		}
		return newPos;
	}
	//Defense Build
	private bool checkBuildSpace(Vector3 checkPos)
	{
		//attacker gets 5 tile padding, each tile is 10x10
		//Terrain origin is at 0x0x0
		if(checkPos.x <= 10 || checkPos.z <= 10 || checkPos.x >= 215 || checkPos.z >= 215){
			return false;
		}
		else
			return true;
	}
	private Vector3 showTile(Vector3 mousePosition)
	{
		Vector3 position = mousePosition;
		//if collider is within bounds of map then show cube, if no object is already in that space canPlace material
		//else cantPlace material
		//Y is height

		float x = position.x;
		float z = position.z;
		float tileSize = 5.0f;
		//x = Mathf.Floor (x);
		//z = Mathf.Floor (z);
		x /= tileSize;
		z /= tileSize;
		x = Mathf.Floor (x);
		z = Mathf.Floor (z);
		x *= tileSize;
		z *= tileSize;
		x += tileSize/2.0f;
		z += tileSize/2.0f;

		position.x = x;
		position.z = z;
		position.y = 0.0f;

		tileTrans.position = position;

		tileRend.enabled = true;
		return position;
	}

    void Update()
    {
        RaycastHit hit = cosInController.InputUpdate(Camera.main);
		//from here will run script for tile building
		Vector3 spawnPosition = tilePlacer(); //this updates position of tile prefab 
        if (selected == null)
            return;

		if (cosInController.TouchState == COSTouchState.TerrainTapped && checkBuildSpace(spawnPosition))
        {
			var allyObject = cosInController.SpawnAlly(hit, selected, remaining, toggleGroup, spawnPosition);

            if (allyObject != null)
            {

                Vector2 normPos = new Vector2(allyObject.transform.position.x - terrain.transform.position.x,
                                      allyObject.transform.position.z - terrain.transform.position.z);
                normPos.x = normPos.x / terrain.terrainData.size.x;
                normPos.y = normPos.y / terrain.terrainData.size.z;

                manager.pendingDefenseConfig.layout[selected].Add(normPos);

                if (remaining[selected.id] == 0)
                    selected = null;
            }
        }

    }



    public void ReturnToShop()
    {
        Game.LoadScene("ClashDefenseShop");
    }

    public void ConfirmDefense()
    {
        if (GameObject.FindGameObjectsWithTag("Ally").Count() != 25)
        {
            errorCanvas.SetActive(true);
            errorMessage.text = "Place all your units down before confirming";
            return;
        }
        
        var pending = manager.pendingDefenseConfig;
        var mappedLayout = pending.layout.ToDictionary((el) => el.Key.id, (el) => el.Value);

        var request = ClashDefenseSetupProtocol.Prepare(pending.terrain, mappedLayout);

        NetworkManagerCOS.getInstance().Send(request, (res) =>
            {
                var response = res as ResponseClashDefenseSetup;
                if (response.valid)
                {
                    manager.defenseConfig = manager.pendingDefenseConfig;
                    manager.pendingDefenseConfig = null;
                    Game.LoadScene("ClashMain");
                }
                else
                {
                    errorCanvas.SetActive(true);
                    errorMessage.text = "Error in saving data to the DB";
                }
            });
    }

    public void ConfirmError()
    {
        errorMessage.text = ""; 
        errorCanvas.SetActive(false);
    }
}
