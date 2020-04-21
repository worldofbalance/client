using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class WorldMouse : MonoBehaviour
{

    public GameObject defenderCursor;
    public GameObject roamingCursor;
    public GameObject attackerCursor;
    public Zone currentTile { get; set; }

    public Zone currentSelectedTile { get; set; }

    public TileInfoGUI tileInfoGUI;
    private string[] terrain = new string[] { "Desert", "Jungle", "Grasslands", "Arctic" };

    GameObject mapTileSelected;
    public GameObject tileUi;
	public GameObject tilePC;
    public GameObject purchaseCursor;
    Button purchaseButton;
    GameObject tilePurchaseSuccess;

    GameObject firstPlayUi;

    private int prevPlayerID;

    private MapCamera mapCamera;
	private Graph graph;

    // Use this for initialization
    void Start()
    {
        tileInfoGUI = gameObject.AddComponent<TileInfoGUI>();
        mapCamera = GameObject.Find("MapCamera").GetComponent<MapCamera>();
        tileUi = GameObject.Find("Canvas/Tilepurchasedialog") as GameObject;
		tilePC = GameObject.Find("Canvas/PurchaseSuccess") as GameObject;
		if (GameObject.Find("Cube").GetComponent<Graph>() == null) {
			GameObject.Find("Cube").AddComponent<Graph>();
		}
		graph = GameObject.Find("Cube").GetComponent<Graph>();

        purchaseCursor = GameObject.Find("PurchaseCursor") as GameObject;
        purchaseCursor.SetActive(false);
        
        if (tileUi != null) {
          tileUi.SetActive(false);
          purchaseButton = tileUi.transform.GetChild(6).GetComponent<Button>();
          purchaseButton.interactable = false;
        }

		if (tilePC != null) {
			tilePC.SetActive(false);
		} 
			
        if (tilePurchaseSuccess != null) {
            tilePurchaseSuccess = GameObject.Find("Canvas/PurchaseSuccess") as GameObject;
            tilePurchaseSuccess.SetActive(false);
        }

        firstPlayUi = GameObject.Find("Canvas/FirstWelcome") as GameObject;
        if (firstPlayUi != null) {
            firstPlayUi.SetActive(false);
        }
        if (tileUi != null) {
            tileUi.SetActive(false);
        }
        try {
            Game.networkManager.Send(TilePriceProtocol.Prepare(1), processWelcome);
        } catch (Exception ex) {
            // Debug.LogException(ex);
        }
    }
    void Awake()
    {
    }
    public bool popOversEnabled = true;
    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.UpArrow) && !graph.isActive) {
			mapCamera.ZoomIn ();
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) && !graph.isActive) {
			mapCamera.ZoomOut ();
		}    

        if (!popOversEnabled) {
          return;
        }

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && (!EventSystem.current.IsPointerOverGameObject()))
        {
            if (hit.transform.gameObject.tag == "Zone")
            {
                currentTile = hit.transform.gameObject.GetComponent<Zone>();

                //  Debug.Log("zone Id: " + currentTile.zone_id);

                if (currentTile.player_id != prevPlayerID)
                {
                    if (prevPlayerID > 0)
                    {
                        GameObject.Find("Map").GetComponent<Map>().SelectTerritory(prevPlayerID, false);
                    }

                    GameObject.Find("Map").GetComponent<Map>().SelectTerritory(currentTile.player_id, true);
                    prevPlayerID = currentTile.player_id;
                }

                string owner_name = "";

                if (currentTile.player_id > 0)
                {
                    // Debug.Log("Tile Owner Id: " + currentTile.player_id);
                    owner_name = GameObject.Find("Map").GetComponent<Map>().playerList[currentTile.player_id].name;

                    Color playerColor = GameObject.Find("Map").GetComponent<Map>().playerList[currentTile.player_id].color; ;
                    tileInfoGUI.bgColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0.8f);
                }
                else {
                    tileInfoGUI.DefaultColor();
                }

                tileInfoGUI.SetInformation(currentTile.terrain_type, currentTile.v_capacity, owner_name);
                tileInfoGUI.position = Camera.main.WorldToScreenPoint(currentTile.transform.position);

                tileInfoGUI.SetActive(true);

                roamingCursor.SetActive(true);
                roamingCursor.transform.position = currentTile.transform.position + new Vector3(0, 0.1f, 0);
                //				roamingCursor.renderer.material.color = new Color32(0, 181, 248, 255);
            }
        }
        else {
            tileInfoGUI.SetActive(false);
            roamingCursor.SetActive(false);
            currentTile = null;
        }

        //used for tile purchasing 
        switch (InputExtended.GetMouseNumClick(0))
        {
            case 1: // Single Click
                    //modifies which tile you are currently selecting 
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        //Check to make sure they are not currently using shop (Raycasting issues) 
                        if (!Shop.gInshop)
                        {
                            // fixed a null reference here.
                            if (tileUi != null && currentTile != null && tileUi.activeSelf && currentTile.player_id == 0) {
                                //Debug.Log(tileUi.name);


                                currentSelectedTile = currentTile;
                                //  purchaseCursor.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 1);
                                //locking the cursor on the tile 
                                purchaseCursor.transform.position = currentTile.transform.position + new Vector3(0, 0.1f, 0);
                                purchaseCursor.SetActive(true);

                                // Get and compare the price of the tile with the user's credits
                                Game.networkManager.Send(
                                    TilePriceProtocol.Prepare(currentSelectedTile.zone_id), processPrice);

                                tileUi.transform.GetChild(2).GetComponent<Text>().text = terrain[currentSelectedTile.terrain_type - 1];
                                tileUi.transform.GetChild(3).GetComponent<Text>().text = currentSelectedTile.v_capacity.ToString();
                            }

                            //Launch the ecosystem if they are clicking on owned tiles 
                            if (currentTile.player_id == GameState.player.GetID())
                            {
                                // Disabled for now. Leave to develop more later
                                mapCamera.Move(currentTile.transform.position);

                                // displays ecosystem graph
                                //graph.ToggleGraph();

                                Debug.Log("clicked on own tile");
                            }
                        }
                    }
                }
                break;

            case 2: // Double Click
                    // This case is being used for purchasing tiles, and accessing your ecosystem 
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    //Check to make sure they are not currently using shop (Raycasting issues) 
                    if (!Shop.gInshop)
                    {
                        //Launch the ecosystem if they are clicking on owned tiles 
                        if (currentTile.player_id == GameState.player.GetID())
                        {
                            // Disabled for now. Leave to develop more later
                            mapCamera.Move(currentTile.transform.position);

                        }
                        else if (currentTile.player_id == 0 && !tileUi.activeSelf) // check to see if tile can be purchased 
                                                                                   // **NOTE** if the tile's owner is "NULL" in the db, player_id = 0
                        {
                            currentSelectedTile = currentTile;
                            //purchase cursor
                            purchaseCursor.transform.position = currentTile.transform.position + new Vector3(0, 0.1f, 0);
                            purchaseCursor.SetActive(true);

                            // Get and compare the price of the tile with the user's credits
                            Game.networkManager.Send(
                                TilePriceProtocol.Prepare(currentSelectedTile.zone_id), processPrice);

                            tileUi.transform.GetChild(2).GetComponent<Text>().text = terrain[currentSelectedTile.terrain_type - 1];
                            tileUi.transform.GetChild(3).GetComponent<Text>().text = currentSelectedTile.v_capacity.ToString();
                            // set the price using Request/REsponse 
                            // tileUi.transform.GetChild(4).GetComponent<Text>().text = "XXXX";
                            tileUi.SetActive(true);


                        }
                        //View other player's ecosystem (Future )
                        else
                        {
                            //content 
                        }
                    }
                }
                Debug.Log("double click event success");
                //Exit the case
                break;
        }
    }

    public void processPrice(NetworkResponse response)
    {
        TilePrice args = response as TilePrice;
        if (args.status == 0)
        {
            // set price to free
            if (args.price == 0)
                tileUi.transform.GetChild(4).GetComponent<Text>().text = "Free";
            else
                //sets the price to calculated
                tileUi.transform.GetChild(4).GetComponent<Text>().text = args.price.ToString();


            //can purchase tile 
            if (args.canBuy)
            {
                purchaseButton.interactable = true;
                tileUi.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 200);
                tileUi.transform.GetChild(5).gameObject.SetActive(false);


            }
            else // cant purchase Tile 
            {
                purchaseButton.interactable = false;
                tileUi.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 210);
                tileUi.transform.GetChild(5).gameObject.SetActive(true);

            }
        }
        else
        {
            Debug.Log("Failed to send response");
        }

    }
    public void processWelcome(NetworkResponse response)
    {
        TilePrice args = response as TilePrice;
        Debug.Log("Welome Screen if you dont own tiles!");
        if (args.status == 0)
        {
            // set price to free
            if (args.price == 0)
                firstPlayUi.SetActive(true);

        }
    }
}
