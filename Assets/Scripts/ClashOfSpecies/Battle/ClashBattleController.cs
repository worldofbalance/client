using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnitType = ClashSpecies.SpeciesType;

public class ClashBattleController : MonoBehaviour
{
    public HorizontalLayoutGroup unitList;
    public GameObject attackItemPrefab;
    public GameObject healthBar;
    public ClashHealthBar activeHealthBar;
    public AudioClip[] audioClip;
    public AudioSource audioSource;

	// Used for keeping count of the type, names, and count of each team's species
	// In the context of an attack, 'allies' are the invaders, and 'enemies' are defenders
    public List<ClashBattleUnit> enemiesList = new List<ClashBattleUnit>();
    public List<ClashBattleUnit> alliesList = new List<ClashBattleUnit>();
    public Dictionary<string, int> enemySpecies = new Dictionary<string, int> ();
    public Dictionary<string, int> allySpecies = new Dictionary<string, int> ();
	public List<Text> allyTypeSpecies; // References to the UI elements in the hierachy
	public List<Text> allyNameSpecies;
	public List<Text> allyCountSpecies;
	public List<Text> enemyTypeSpecies;
	public List<Text> enemyNameSpecies;
	public List<Text> enemyCountSpecies;

	//Text to display win or loss
    public GameObject messageCanvas;
    public Text messageText;
    public Text selectedSpeciesText;

    public Text timer;
    public float timeLeft = 120f;
    public bool finished = false;
    public bool isStarted = false;

	//tile building
	//cube

	public float tileSize = 5.0f;
	public Transform tileTrans;
	public MeshRenderer tileRend;
	//tile material
	public Material canPlace;
	public Material cantPlace;

    private Dictionary<int, int> remaining = new Dictionary<int, int>();
    private ClashGameManager manager;
    private ClashSpecies selected;
    private Terrain terrain;
    private ToggleGroup toggleGroup;
	private int walkableAreaMask;

	//For mobile/touchscreen
    private float timeTouchPhaseEnded;
    private Vector3 oldTouchPos;
	private COSAbstractInputController cosInController;

    void Awake() {
        selectedSpeciesText = GameObject.Find("txtSelectedSpecies").GetComponent<Text>();
        manager = GameObject.Find("MainObject").GetComponent<ClashGameManager>();
        toggleGroup = unitList.gameObject.GetComponent<ToggleGroup>();
    } 

    void Start ()
	{
		//Creating the terrain
		walkableAreaMask = (int)Math.Pow (2, UnityEngine.AI.NavMesh.GetAreaFromName ("Walkable"));
		var terrainResource = Resources.Load ("Prefabs/ClashOfSpecies/Terrains/" + manager.currentTarget.terrain);
		var terrainObject = Instantiate (terrainResource, Vector3.zero, Quaternion.identity) as GameObject;
		terrain = terrainObject.GetComponentInChildren<Terrain> ();

		foreach (var pair in manager.currentTarget.layout) {
			var species = pair.Key;

			// Place navmesh agent.
			List<Vector2> positions = pair.Value;
			foreach (var pos in positions) {
				//Debug.DrawRay()
				var speciesPos = new Vector3 (pos.x * terrain.terrainData.size.x, 0.0f, pos.y * terrain.terrainData.size.z);
				RaycastHit hitInfo;
				Physics.Raycast (new Vector3 (speciesPos.x, 50, speciesPos.z), 50 * Vector3.down, out hitInfo);
				//Debug.DrawRay(new Vector3(speciesPos.x, 50, speciesPos.z), 50 * Vector3.down, Color.green, 10f);
				UnityEngine.AI.NavMeshHit placement;

				//Place objects/species down here AQ
				if (UnityEngine.AI.NavMesh.SamplePosition (hitInfo.point, out placement, 1000, walkableAreaMask)) {
					var speciesResource = Resources.Load<GameObject> ("Prefabs/ClashOfSpecies/Units/" + species.name);
					var speciesObject = Instantiate (speciesResource, placement.position, Quaternion.identity) as GameObject;
					var trigger = speciesObject.AddComponent<SphereCollider> ();
					trigger.radius = Constants.UnitColliderRadius;
					speciesObject.tag = "Enemy";

//					if (species.type == ClashSpecies.SpeciesType.OMNIVORE)
						speciesObject.AddComponent<ClashBattleUnit> ();
//					else if (species.type == ClashSpecies.SpeciesType.CARNIVORE)
//						speciesObject.AddComponent<Carnivore> ();
//					else if (species.type == ClashSpecies.SpeciesType.HERBIVORE)
//						speciesObject.AddComponent<Herbivore> ();
//					else
//						speciesObject.AddComponent<Obstacle> ();
					var unit = speciesObject.GetComponent<ClashBattleUnit> ();

                    // Key that is used for the dictionary which keeps track of unit type/name/count
					var key = unit.name.Split ('(') [0];
					enemySpecies [key] = 5;
					enemiesList.Add (unit);
					unit.species = species;

					// Creating and attaching the health bars above units
                    var bar = Instantiate(healthBar, unit.transform.position, Quaternion.identity) as GameObject;
                    bar.transform.SetParent(unit.transform);
                    bar.transform.localPosition = new Vector3(0.0f, 6.0f, 0.0f);
                    bar.SetActive(true);
                    bar.tag = Constants.TAG_HEALTH_BAR;
                }
                else {
                    Debug.LogWarning("Failed to place unit: " + species.name);
                }
            }

			// Keeps track of type/name/count of enemies (defenders) when attacking
            List<string> enemyKeyList = new List<string>(this.enemySpecies.Keys);
			if (enemyKeyList.Count() > 0 && enemyKeyList.Count() < 6) {
				int i = enemyKeyList.Count() - 1;
                if (enemySpecies.ContainsKey (enemyKeyList [i]) ) {
					if (enemyTypeSpecies[i].text == "") 
						enemyTypeSpecies[i].text = species.type.ToString();
					enemyNameSpecies[i].text = enemyKeyList[i].ToString();
					enemyCountSpecies[i].text = enemySpecies[enemyKeyList[i]].ToString();
                }
			}
        }

        // Populate user's selected unit panel.
        foreach (var species in manager.attackConfig.layout) {
            var currentSpecies = species;
            var item = Instantiate(attackItemPrefab) as GameObject;
            remaining.Add(currentSpecies.id, 5);
            
            var itemReference = item.GetComponent<ClashUnitListItem>();

            var texture = Resources.Load<Texture2D>("Images/" + species.name);
            itemReference.toggle.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            // Changes the color of the image of the selected unit in the selection menu
			itemReference.toggle.onValueChanged.AddListener((val) =>
                {
                    if (val){
                        selected = currentSpecies;
                        itemReference.toggle.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    }
                    else{
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

        // Send initial battle request to the server
        List<int> speciesIds = new List<int>();
        foreach (var species in manager.attackConfig.layout) {
            speciesIds.Add(species.id);
        }
        var request = ClashInitiateBattleProtocol.Prepare(manager.currentTarget.owner.GetID(), speciesIds);
        NetworkManagerCOS.getInstance().Send(request, (res) =>
            {
                var response = res as ResponseClashInitiateBattle;
                Debug.Log("Received ResponseClashInitiateBattle from server. valid = " + response.valid);
            });

        if (manager.isRunningOnMobile)
            cosInController = ScriptableObject.CreateInstance<COSMobileInputControler>();
        else
            cosInController = ScriptableObject.CreateInstance<COSDesktopInputController>();
        cosInController.InputControllerAwake(Terrain.activeTerrain);
    } 
	//End of Start

    void Update (){
		RaycastHit hit = cosInController.InputUpdate (Camera.main);

		//from here will run script for tile building
		Vector3 spawnPosition = tilePlacer (); //this updates position of tile prefab 

		if (isStarted && !finished)
			UpdateTimer ();

		if (cosInController.TouchState == COSTouchState.TerrainTapped && checkBuildSpace (spawnPosition)) {
			if (selected != null && remaining [selected.id] > 0) {
				//pass the spawn position, which is determined by the tilePlacer()
				var allyObject = cosInController.SpawnAlly (hit, selected, remaining, toggleGroup, spawnPosition);
				if (allyObject == null)
					return;

				var trigger = allyObject.AddComponent<SphereCollider> ();
				trigger.radius = Constants.UnitColliderRadius;
				var unit = allyObject.AddComponent<ClashBattleUnit> ();
				alliesList.Add (unit);
				unit.species = selected;
                
				// Creating keys for spawned allies
				var key = unit.name.Split ('(') [0];
				if (!allySpecies.ContainsKey (key))
					allySpecies [key] = 1;
				else
					allySpecies [key] += 1;

				var bar = Instantiate (healthBar, unit.transform.position, Quaternion.identity) as GameObject;
				bar.transform.SetParent (unit.transform);
				bar.transform.localPosition = new Vector3 (0.0f, 8.0f, 0.0f);
				bar.SetActive (false);
				//bar.tag = Constants.TAG_HEALTH_BAR;

				//check if all allies (attackers) are spawned
				if (alliesList.Count == 25)
					isStarted = true;
			} 
			else if (selected == null) {/*Do nothing*/}
            else if (remaining[selected.id] == 0)
                selected = null;
        }

		// Keeps track of type/name/count of allies when attacking
		List<string> keyList = new List<string> (this.allySpecies.Keys);
		for (int i = 0; i < keyList.Count(); i++) {
			if (allySpecies.ContainsKey (keyList [i])) {
				if (allyTypeSpecies[i].text == "") 
					allyTypeSpecies[i].text = selected.type.ToString();
				allyNameSpecies[i].text = keyList[i].ToString();
				allyCountSpecies[i].text = allySpecies[keyList[i]].ToString();
            }
		}

		// Keeps track of type/name/count of enemies when attacking
        List<string> enemyKeyList = new List<string>(this.enemySpecies.Keys);
		for (int i = 0; i < enemyKeyList.Count(); i++){
			if (enemySpecies.ContainsKey (enemyKeyList [i])) {
				enemyNameSpecies[i].text = enemyKeyList[i].ToString();
				enemyCountSpecies[i].text = enemySpecies[enemyKeyList[i]].ToString();
            }
		}
    }
	//End of Update

    void UpdateTimer() {
        timeLeft -= Time.deltaTime;

        int intTime = (int)timeLeft;
        int seconds = (int)intTime % 60;
        int min = intTime / 60;

        if (timeLeft < 0 && !finished) {
            // If there are more defenders left than attackers, defender wins
           if (enemiesList.Count() > alliesList.Count())
               ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.LOSS);
           else // otherwise attacker wins 
                ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.WIN);
        }
        else
            timer.text = min + ":" + seconds;
    }

    void FixedUpdate (){
		if (finished)
			return;

		int totalEnemyHealth = 0;
		foreach (var enemy in enemiesList) {
			totalEnemyHealth += enemy.currentHealth;
		}

		if (Time.timeSinceLevelLoad > 5.0f && totalEnemyHealth == 0 && enemiesList.Count () > 0 && !finished) 
			ReportBattleOutcome (ClashEndBattleProtocol.BattleResult.WIN); //Attackers have won

        int totalAllyHealth = 0;
		foreach (var ally in alliesList) {
			totalAllyHealth += ally.currentHealth;
		}

		//Enemies (defenders) have won
        if (Time.timeSinceLevelLoad > 5.0f && totalAllyHealth <= 0 && alliesList.Count() == 25 && !finished)
            ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.LOSS); 
    }

    public void Surrender () {
        ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.LOSS);
    }

    public void ConfirmResult() {
        Game.LoadScene("ClashMain");
    }

    public void ReportBattleOutcome(ClashEndBattleProtocol.BattleResult outcome) {
        if (outcome == ClashEndBattleProtocol.BattleResult.WIN) {
            PlaySound (0);
            messageCanvas.SetActive(true);
            messageText.text = "You Won!\n\nKeep on fighting!";
        }
        else {
            PlaySound (1);
            messageCanvas.SetActive(true);
            messageText.text = "You Lost!\n\nTry again next time!";
        }

        finished = true;
        var request = ClashEndBattleProtocol.Prepare(outcome);
        NetworkManagerCOS.getInstance().Send(request, (res) =>
            {
                var response = res as ResponseClashEndBattle;
                int creditsEarned = response.credits - manager.currentPlayer.credits;
                if (creditsEarned >= 0)
                    messageText.text += "\n\nCredits Earned: " + creditsEarned;
                else
                    messageText.text += "\n\nCredits Lost: " + (-creditsEarned);
                messageText.text += "\nTotal Credits: " + response.credits;
                manager.currentPlayer.credits = response.credits;
                Debug.Log("Received ResponseClashEndBattle from server. credits earned: " + creditsEarned);
                finished = true;
            });
    }

    public void PlaySound (int clip) {
        audioSource.clip = audioClip [clip];
		audioSource.volume = .50f;
        audioSource.Play ();
    }

    public bool isSpawningSpecies() {
        return selected == null;
    }

	private Vector3 tilePlacer () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		Vector3 newPos = new Vector3 (0, 0, 0);
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			if (hit.collider.tag == "Terrain") {
				newPos = showTile (hit.point);
				if (checkBuildSpace (newPos)) {
					tileRend.material = canPlace;
					if (Input.GetMouseButtonDown (0)) {
						return newPos;
					}
				}
				//Cant build on this part of terrain
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

	private Vector3 showTile(Vector3 mousePosition) {
		Vector3 position = mousePosition;
		//if collider is within bounds of map then show cube, if no object is already in that space canPlace material
		//else cantPlace material
		//Y is height

		float x = position.x;
		float z = position.z;
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

	//Attack Build
	private bool checkBuildSpace(Vector3 checkPos)
	{
		//attacker gets 5 tile padding, each tile is 5x5
		//Terrain origin is at 0x0x0
		if(checkPos.x <= 10 || checkPos.z <= 10 || checkPos.x >= 215 || checkPos.z >= 215)
			return true;
		else
			return false;
	}
}
