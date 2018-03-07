using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnitType = ClashSpecies.SpeciesType;

public class ClashBattleController : MonoBehaviour
{

    private Dictionary<int, int> remaining = new Dictionary<int, int>();
    private ClashGameManager manager;
    private ClashSpecies selected;
    private Terrain terrain;
    private ToggleGroup toggleGroup;

    public HorizontalLayoutGroup unitList;
    public GameObject attackItemPrefab;
    public GameObject healthBar;
    public ClashHealthBar activeHealthBar;
    public AudioClip[] audioClip;
    public AudioSource audioSource;

    public List<ClashBattleUnit> enemiesList = new List<ClashBattleUnit>();
    public List<ClashBattleUnit> alliesList = new List<ClashBattleUnit>();

    public GameObject messageCanvas;
    public Text messageText;
    public Text selectedSpeciesText;

    public Text hpBuffValue;
    public Text dmgBuffValue;
    public Text spdBuffValue;

    public Text typeSpecies1, typeSpecies2, typeSpecies3, typeSpecies4, typeSpecies5;
    public Text nameSpecies1, nameSpecies2, nameSpecies3, nameSpecies4, nameSpecies5;
    public Text countSpecies1, countSpecies2, countSpecies3, countSpecies4, countSpecies5;

    public Text enemyTypeSpecies1, enemyTypeSpecies2, enemyTypeSpecies3, enemyTypeSpecies4, enemyTypeSpecies5;
    public Text enemyNameSpecies1, enemyNameSpecies2, enemyNameSpecies3, enemyNameSpecies4, enemyNameSpecies5;
    public Text enemyCountSpecies1, enemyCountSpecies2, enemyCountSpecies3, enemyCountSpecies4, enemyCountSpecies5;

    private Boolean finished = false;
    private bool isStarted = false;

    public Text timer;
    //    float mTime = 120f;

    public float timeLeft = 120f;

    //    public float moveSensitivityX = 1.0f;
    //    public float moveSensitivityY = 1.0f;
    //    public bool updateZoomSensitivity = true;
    //    public float zoomSpeed = 0.05f;
    //    public float minZoom = 1.0f;
    //    public float maxZoom = 20.0f;
    //    public bool invertMoveX = true;
    //    public bool invertMoveY = false;

    public bool enemyAIEnabled = false;

    public bool allyAIEnabled = false;

    //    public float minFOV = 10f;
    //    public float maxFOV = 79.9f;

    //    float minPanDistance = 2;

    //    bool isPanning;

    //    public float inertiaDuration = 1.0f;

    private Camera _camera;

    public float terrainCameraPadding = 40;

    private float minX, maxX, minZ, maxZ;
    private float horizontalExtent, verticalExtent;

    //    private float scrollVelocity = 0.0f;
    private float timeTouchPhaseEnded;
    private Vector3 oldTouchPos;

    public Dictionary<string, int> enemySpecies = new Dictionary<string, int> ();
    public Dictionary<string, int> allySpecies = new Dictionary<string, int> ();

    COSAbstractInputController cosInController;

    /// <summary>
    /// Touch controler fields end
    /// </summary>

    void Awake()
    {
        selectedSpeciesText = GameObject.Find("txtSelectedSpecies").GetComponent<Text>();
        manager = GameObject.Find("MainObject").GetComponent<ClashGameManager>();
        toggleGroup = unitList.gameObject.GetComponent<ToggleGroup>();
    } 

    int walkableAreaMask;

    void Start()
    {
        
        walkableAreaMask = (int)Math.Pow(2, NavMesh.GetAreaFromName("Walkable"));
        var terrainResource = Resources.Load("Prefabs/ClashOfSpecies/Terrains/" + manager.currentTarget.terrain);
        var terrainObject = Instantiate(terrainResource, Vector3.zero, Quaternion.identity) as GameObject;

        terrain = terrainObject.GetComponentInChildren<Terrain>();


//        Camera.main.GetComponent<ClashBattleCamera>().target = terrain;

        _camera = Camera.main;
        minX = terrainCameraPadding;
        maxX = Terrain.activeTerrain.terrainData.size.x - terrainCameraPadding;
        minZ = terrainCameraPadding;
        maxZ = Terrain.activeTerrain.terrainData.size.z - terrainCameraPadding;

        foreach (var pair in manager.currentTarget.layout)
        {
            var species = pair.Key;

            // Place navmesh agent.
            List<Vector2> positions = pair.Value;
            foreach (var pos in positions)
            {
//                Debug.DrawRay()
                var speciesPos = new Vector3(pos.x * terrain.terrainData.size.x, 0.0f, pos.y * terrain.terrainData.size.z);
                RaycastHit hitInfo;
                Physics.Raycast(new Vector3(speciesPos.x, 50, speciesPos.z), 50 * Vector3.down, out hitInfo);
//                Debug.DrawRay(new Vector3(speciesPos.x, 50, speciesPos.z), 50 * Vector3.down, Color.green, 10f);
                NavMeshHit placement;
                if (NavMesh.SamplePosition(hitInfo.point, out placement, 1000, walkableAreaMask))
                {
                    var speciesResource = Resources.Load<GameObject>("Prefabs/ClashOfSpecies/Units/" + species.name);
                    var speciesObject = Instantiate(speciesResource, placement.position, Quaternion.identity) as GameObject;
                    speciesObject.tag = "Enemy";

                    var unit = speciesObject.AddComponent<ClashBattleUnit>();

                    var key = unit.name.Split ('(') [0];

                    enemySpecies [key] = 5;

                    enemiesList.Add(unit);
                    unit.species = species;

                    var trigger = speciesObject.AddComponent<SphereCollider>();
                    trigger.radius = Constants.UnitColliderRadius;  

                    var bar = Instantiate(healthBar, unit.transform.position, Quaternion.identity) as GameObject;
                    bar.transform.SetParent(unit.transform);
                    bar.transform.localPosition = new Vector3(0.0f, 6.0f, 0.0f);
                    bar.SetActive(true);
                    bar.tag = Constants.TAG_HEALTH_BAR;

                    GetBuffs(unit, speciesObject.tag);
                    if (species.type == UnitType.PLANT)
                    {
                        GiveBuffs(unit, speciesObject.tag);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to place unit: " + species.name);
                }
            }

            List<string> enemyKeyList = new List<string>(this.enemySpecies.Keys);
            if (enemyKeyList.Count () == 1) {
                if (enemySpecies.ContainsKey (enemyKeyList [0])) {
                    if (enemyTypeSpecies1.text == "") {
                        enemyTypeSpecies1.text = species.type.ToString();
                    }
                    enemyNameSpecies1.text = enemyKeyList [0].ToString();
                    enemyCountSpecies1.text = enemySpecies[enemyKeyList[0]].ToString();
                }
            }
            if (enemyKeyList.Count () == 2) {
            
                if (enemySpecies.ContainsKey (enemyKeyList [1])) {
                    if (enemyTypeSpecies2.text == "") {
                        enemyTypeSpecies2.text = species.type.ToString();
                    }
                    enemyNameSpecies2.text = enemyKeyList [1].ToString();
                    enemyCountSpecies2.text = enemySpecies[enemyKeyList[1]].ToString();
                }
            }

            if (enemyKeyList.Count () == 3) {
            
                if (enemySpecies.ContainsKey (enemyKeyList [2])) {
                    if (enemyTypeSpecies3.text == "") {
                        enemyTypeSpecies3.text = species.type.ToString();
                    }
                    enemyNameSpecies3.text = enemyKeyList [2].ToString();
                    enemyCountSpecies3.text = enemySpecies[enemyKeyList[2]].ToString();
                }
            }

            if (enemyKeyList.Count () == 4) {
                if (enemySpecies.ContainsKey (enemyKeyList [3])) {
                    if (enemyTypeSpecies4.text == "") {
                        enemyTypeSpecies4.text = species.type.ToString();
                    }
                    enemyNameSpecies4.text = enemyKeyList [3].ToString();
                    enemyCountSpecies4.text = enemySpecies[enemyKeyList[3]].ToString();
                }
            }

            if (enemyKeyList.Count () == 5) {
                if (enemySpecies.ContainsKey (enemyKeyList [4])) {
                    if (enemyTypeSpecies5.text == "") {
                        enemyTypeSpecies5.text = species.type.ToString();
                    }
                    enemyNameSpecies5.text = enemyKeyList [4].ToString();
                    enemyCountSpecies5.text = enemySpecies[enemyKeyList[4]].ToString();
                }
            }			
        }

        // Populate user's selected unit panel.
        foreach (var species in manager.attackConfig.layout)
        {
            var currentSpecies = species;
            var item = Instantiate(attackItemPrefab) as GameObject;
            remaining.Add(currentSpecies.id, 5);
            
            var itemReference = item.GetComponent<ClashUnitListItem>();

            var texture = Resources.Load<Texture2D>("Images/" + species.name);
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

        // Send initiate battle request to the server
        List<int> speciesIds = new List<int>();
        foreach (var species in manager.attackConfig.layout)
        {
            speciesIds.Add(species.id);
        }
        var request = ClashInitiateBattleProtocol.Prepare(manager.currentTarget.owner.GetID(), speciesIds);
        NetworkManagerCOS.getInstance().Send(request, (res) =>
            {
                var response = res as ResponseClashInitiateBattle;
                Debug.Log("Received ResponseClashInitiateBattle from server. valid = " + response.valid);
            });

//        walkableArea = NavMesh.GetAreaFromName("Walkable");
//        waterArea = NavMesh.GetAreaFromName("Water");
//        notWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
        if (manager.isRunningOnMobile)
            cosInController = ScriptableObject.CreateInstance<COSMobileInputControler>();
        else
            cosInController = ScriptableObject.CreateInstance<COSDesktopInputController>();
        cosInController.InputControllerAwake(Terrain.activeTerrain);
    }

    private ClashBattleUnit allySelected;


    void Update()
    {
        RaycastHit hit = cosInController.InputUpdate(_camera);

        if (isStarted && !finished)
            UpdateTimer();

        if (cosInController.TouchState == COSTouchState.TerrainTapped)
        {
            if (selected != null && remaining[selected.id] > 0)
            {
                var allyObject = cosInController.SpawnAlly(hit, selected, remaining, toggleGroup);

                if (allyObject == null)
                {
//                selected = null;
                    return;
                }
                
                var unit = allyObject.AddComponent<ClashBattleUnit>();
                var key = unit.name.Split ('(') [0];
                if (!allySpecies.ContainsKey(key)) {
                    allySpecies [key] = 1;
                } else {
                    allySpecies [key] += 1;
                }

                alliesList.Add(unit);
                unit.species = selected;
                var trigger = allyObject.AddComponent<SphereCollider>();
                trigger.radius = Constants.UnitColliderRadius;
                var bar = Instantiate(healthBar, unit.transform.position, Quaternion.identity) as GameObject;
                bar.transform.SetParent(unit.transform);
                bar.transform.localPosition = new Vector3(0.0f, 8.0f, 0.0f);
                bar.SetActive(false);
                //                    bar.tag = Constants.TAG_HEALTH_BAR;

                //check if all allies are spawned
                if (alliesList.Count == 25)
                {
                    allyAIEnabled = true;
                    enemyAIEnabled = true;
                    isStarted = true;
                }

                GetBuffs(unit, allyObject.tag);
                if (unit.species.type == UnitType.PLANT)
                {
                    GiveBuffs(unit, allyObject.tag);
                    UpdateBuffPanel(unit.species, true);
                }
            }
            else if (selected == null)
            {
                if (hit.collider.CompareTag("Ally"))
                {

                    allySelected = hit.collider.gameObject.GetComponent<ClashBattleUnit>();
                    allySelected.target = null;
                    allySelected.TargetPoint = Vector3.zero;
                    allySelected.setSelected(true);
                }
                else if (allySelected != null && hit.collider.CompareTag("Enemy"))
                {
                    //                        targetedEnemy = hit.transform;
                    allySelected.target = hit.collider.gameObject.GetComponent<ClashBattleUnit>();
                    allySelected.setSelected(false);
                }
                else
                {
                    if (allySelected)
                    {
                        allySelected.TargetPoint = hit.point;
                        allySelected.setSelected(false);
                        allySelected = null;
                    }
                }
            }
            else if (remaining[selected.id] == 0)
            {
                selected = null;
            }
        }

        List<string> keyList = new List<string>(this.allySpecies.Keys);

        if (keyList.Count () > 0) {
            if (allySpecies.ContainsKey (keyList [0])) {
                if (typeSpecies1.text == "") {
                    typeSpecies1.text = selected.type.ToString();

                }
                nameSpecies1.text = keyList [0].ToString();
                countSpecies1.text = allySpecies[keyList[0]].ToString();
            }
        }


        if (keyList.Count () > 1) {
            if (allySpecies.ContainsKey (keyList [1])) {
                if (typeSpecies2.text == "") {
                    typeSpecies2.text = selected.type.ToString();

                }
                nameSpecies2.text = keyList [1].ToString();
                countSpecies2.text = allySpecies[keyList[1]].ToString();
            }

        }

        if (keyList.Count () > 2) {
            if (allySpecies.ContainsKey (keyList [2])) {
                if (typeSpecies3.text == "") {
                    typeSpecies3.text = selected.type.ToString();

                }
                nameSpecies3.text = keyList [2].ToString();
                countSpecies3.text = allySpecies[keyList[2]].ToString();
            }
        }


        if (keyList.Count () > 3) {
            if (allySpecies.ContainsKey (keyList [3])) {
                if (typeSpecies4.text == "") {
                    typeSpecies4.text = selected.type.ToString();

                }
                nameSpecies4.text = keyList [3].ToString();
                countSpecies4.text = allySpecies[keyList[3]].ToString();
            }
        }


        if (keyList.Count () > 4) {
            if (allySpecies.ContainsKey (keyList [4])) {
                if (typeSpecies5.text == "") {
                    typeSpecies5.text = selected.type.ToString();

                }
                nameSpecies5.text = keyList [4].ToString();
                countSpecies5.text = allySpecies[keyList[4]].ToString();
            }
        }



        List<string> enemyKeyList = new List<string>(this.enemySpecies.Keys);

        if (enemyKeyList.Count () > 0) {
            if (enemySpecies.ContainsKey (enemyKeyList [0])) {
                enemyNameSpecies1.text = enemyKeyList [0].ToString();
                enemyCountSpecies1.text = enemySpecies[enemyKeyList[0]].ToString();
            }
        }


        if (enemyKeyList.Count () > 1) {
            if (enemySpecies.ContainsKey (enemyKeyList [1])) {
                enemyNameSpecies2.text = enemyKeyList [1].ToString();
                enemyCountSpecies2.text = enemySpecies[enemyKeyList[0]].ToString();
            }
        }


        if (enemyKeyList.Count () > 2) {
            if (enemySpecies.ContainsKey (enemyKeyList [2])) {
                enemyNameSpecies3.text = enemyKeyList [2].ToString();
                enemyCountSpecies3.text = enemySpecies[enemyKeyList[2]].ToString();
            }
        }


        if (enemyKeyList.Count () > 3) {
            if (enemySpecies.ContainsKey (enemyKeyList [3])) {
                enemyNameSpecies4.text = enemyKeyList [3].ToString();
                enemyCountSpecies4.text = enemySpecies[enemyKeyList[3]].ToString();
            }
        }


        if (enemyKeyList.Count () > 4) {
            if (enemySpecies.ContainsKey (enemyKeyList [4])) {
                enemyNameSpecies5.text = enemyKeyList [4].ToString();
                enemyCountSpecies5.text = enemySpecies[enemyKeyList[4]].ToString();
            }
        }

    }

    void LateUpdate()
    {
        Vector3 pos = new Vector3(
                          Mathf.Clamp(_camera.transform.position.x, minX, maxX),
                          _camera.transform.position.y, 
                          Mathf.Clamp(_camera.transform.position.z, minZ, maxZ));
        _camera.transform.position = pos;
    }

    void UpdateTimer()
    {
        timeLeft -= Time.deltaTime;

        int intTime = (int)timeLeft;
        int seconds = (int)intTime % 60;
        int min = intTime / 60;

        if (timeLeft < 0 && !finished)
        {
            // if there are more enemies than me. I lose
           if (enemiesList.Count() > alliesList.Count()) {
               ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.LOSS);
           }
           // otherwise I win.
           else {
                ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.WIN);
           }

        }
        else
            timer.text = min + ":" + seconds;
    }

    void FixedUpdate()
    {

        if (finished)
            return;

        int totalEnemyHealth = 0;

        foreach (var enemy in enemiesList)
        {
            totalEnemyHealth += enemy.currentHealth;
            //Debug.Log(totalEnemyHealth);
            if (enemy.currentHealth > 0 && !enemy.target && alliesList.Count > 0)
            {
                //Debug.Log ("Finding Enemy Target", gameObject);
                var target = alliesList.Where(u =>
                    {
                        if (u.currentHealth <= 0)
                            return false;
                        switch (enemy.species.type)
                        {
                            case UnitType.CARNIVORE:
                                return (u.species.type == UnitType.CARNIVORE) || (u.species.type == UnitType.HERBIVORE) ||
                                (u.species.type == UnitType.OMNIVORE);
                            case UnitType.HERBIVORE:
                                return (u.species.type == UnitType.PLANT);
                            case UnitType.OMNIVORE:
                                return (u.species.type == UnitType.HERBIVORE) || (u.species.type == UnitType.PLANT) ||
                                (u.species.type == UnitType.CARNIVORE) ||
                                (u.species.type == UnitType.OMNIVORE);
                            case UnitType.PLANT:
                                return false;
                            default:
                                return false;
                        }
                        return false;
                    }).OrderBy(u =>
                    {
                        return (enemy.transform.position - u.transform.position).sqrMagnitude;
                    }).FirstOrDefault();
                if (enemyAIEnabled)
                    enemy.target = target;
            }
        }

        if (Time.timeSinceLevelLoad > 5.0f && totalEnemyHealth == 0 && enemiesList.Count() > 0 && !finished)
        {
            // ALLIES HAVE WON!
            ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.WIN);
        }

        int totalAllyHealth = 0;
        foreach (var ally in alliesList)
        {
            totalAllyHealth += ally.currentHealth;
            if (ally.currentHealth > 0 && !ally.target && enemiesList.Count() > 0)
            {
                //Debug.Log ("Finding Ally Target", gameObject);
                var target = enemiesList.Where(u =>
                    {
                        if (u.currentHealth <= 0)
                            return false;

                        switch (ally.species.type)
                        {
                            case UnitType.CARNIVORE:
                                return (u.species.type == UnitType.CARNIVORE) || (u.species.type == UnitType.HERBIVORE) ||
                                (u.species.type == UnitType.OMNIVORE);
                            case UnitType.HERBIVORE:
                                return (u.species.type == UnitType.PLANT);
                            case UnitType.OMNIVORE:
                                return (u.species.type == UnitType.HERBIVORE) || (u.species.type == UnitType.PLANT) ||
                                (u.species.type == UnitType.CARNIVORE) ||
                                (u.species.type == UnitType.OMNIVORE);
                            case UnitType.PLANT:
                                return false;
                            default:
                                return false;
                        }
                        return false;
                    }).OrderBy(u =>
                    {
                        return (ally.transform.position - u.transform.position).sqrMagnitude;
                    }).FirstOrDefault();
                if (allyAIEnabled)
                    ally.target = target;
            }
        }

        if (Time.timeSinceLevelLoad > 5.0f && totalAllyHealth <= 0 && alliesList.Count() == 25 && !finished)
        {//            || Time.timeSinceLevelLoad > 75.0f)
            // ENEMIES HAVE WON!
            ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.LOSS);
        }
    }

    public void Surrender () {
        ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.LOSS);
    }

    public void GetBuffs(ClashBattleUnit attributes, string tag)
    {
        var team = GameObject.FindGameObjectsWithTag(tag);
        
        foreach (var teammate in team)
        {
            var teammateAttribute = teammate.GetComponent<ClashBattleUnit>();
            
            //found a plant
            //teammate != attributes.gameObject so it doesn't get a buff from itself
            if (teammateAttribute.species.type == UnitType.PLANT && teammate != attributes.gameObject
                && teammateAttribute.currentHealth > 0)
            {
                switch (teammateAttribute.species.name)
                {
                    case "Big Tree":	//hp buff
                        attributes.currentHealth += 100;
                        break;
                    case "Baobab":	//damage buff
                        attributes.damage += 8;
                        break;
                    case "Trees and Shrubs":	//movement speed buff
                        if (attributes.agent != null)
                            attributes.agent.speed += 5.0f;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void GiveBuffs(ClashBattleUnit attributes, string tag)
    {
        var team = GameObject.FindGameObjectsWithTag(tag);

        foreach (var teammate in team)
        {
            var teammateAttribute = teammate.GetComponent<ClashBattleUnit>();
            //teammate != attributes.gameObject so it doesn't get a buff from itself
            if (teammate != attributes.gameObject && teammateAttribute.currentHealth > 0)
            {
                switch (attributes.species.name)
                {
                    case "Big Tree":	//hp buff
                        teammateAttribute.currentHealth += 100;
                        break;
                    case "Baobab":	//damage buff
                        teammateAttribute.damage += 8;
                        break;
                    case "Trees and Shrubs":	//movement speed buff
                        if (teammateAttribute.agent != null)
                            teammateAttribute.agent.speed += 5.0f;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //status: true if plant spawn; false if it died
    public void UpdateBuffPanel(ClashSpecies unit, bool status)
    {
        int val = 0;
        switch (unit.name)
        {
            case "Big Tree":	//hp buff
                if (Int32.TryParse(hpBuffValue.text, out val))
                {
                    val = (status) ? val + 100 : val - 100;
                }
                hpBuffValue.text = val.ToString();
                break;
            case "Baobab":	//damage buff
                if (Int32.TryParse(dmgBuffValue.text, out val))
                {
                    val = (status) ? val + 8 : val - 8;
                }
                dmgBuffValue.text = val.ToString();
                break;
            case "Trees and Shrubs":	//movement speed buff
                if (Int32.TryParse(spdBuffValue.text, out val))
                {
                    val = (status) ? val + 5 : val - 5;
                }
                spdBuffValue.text = val.ToString();
                break;
            default:
                spdBuffValue.text = "0";
                hpBuffValue.text = "0";
                dmgBuffValue.text = "0";
                break;
        }
    }

    public void ConfirmResult()
    {
        Game.LoadScene("ClashMain");
    }

    public void ReportBattleOutcome(ClashEndBattleProtocol.BattleResult outcome)
    {
        if (outcome == ClashEndBattleProtocol.BattleResult.WIN)
        {
            PlaySound (0);
            messageCanvas.SetActive(true);
            messageText.text = "You Won!\n\nKeep on fighting!";
        }
        else
        {
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
                {
                    messageText.text += "\n\nCredits Earned: " + creditsEarned;
                }
                else
                {
                    messageText.text += "\n\nCredits Lost: " + (-creditsEarned);
                }
                messageText.text += "\nTotal Credits: " + response.credits;
                manager.currentPlayer.credits = response.credits;
                Debug.Log("Received ResponseClashEndBattle from server. credits earned: " + creditsEarned);
                finished = true;
            });
    }

    public void PlaySound (int clip) {
    
        audioSource.clip = audioClip [clip];
        audioSource.Play ();
    }

    public bool isSpawningSpecies()
    {
        return selected == null;
    }


}
