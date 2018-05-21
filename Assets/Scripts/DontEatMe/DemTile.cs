using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/**
    The DemTile class represents a single tile of the Don't Eat Me grid board.
*/
public class DemTile : MonoBehaviour
{
  	public int idX; // X-coord for DemTile
  	public int idY; // Y-coord for DemTile
    public Color currentColor;
    public Color rangeColor; // Color for plant range indicator pulse

    public bool available;

    private Vector3 center;

  	public GameObject resident; // Resident object (HerbivoreObject or PlantObject) placed on tile
    private GameObject nextPredator;
    private BuildMenu buildMenu;
    public  GameObject mainObject;
    public GameObject panelObject;
    private DemMain main;
    private DemTurnSystem turnSystem;

    private IEnumerator pulse; // Called as a coroutine to start/stop pulsing of tile colors
    private bool hasPulse; // Denotes if tile has currently active pulse
    private const float defaultPulseFrequency = 0.625f; // Default pulse frequency
    private const float defaultPulseFactor = 0.1f; // Default value for synchronizing pulse modulation with frequency
    public float DefaultPulseFrequency { get { return defaultPulseFrequency; } } // Public accessor

    // Values for defining a restore point for pulse
    private Color restoreColor1;
    private Color restoreColor2;
    private float restoreColorFrequency;

    private static int pulseTick; // Synchronizes each pulse
    private static float pulseFactor; // Synchronizes pulse modulation with frequency
    private static float masterPulseFrequency; // Master pulse frequency
    private static bool masterPulseEnabled; // True if master pulse clock enabled

    // Use this for initialization
    void Start ()
    {
        mainObject = GameObject.Find ("MainObject");
        panelObject = GameObject.Find("Canvas/Panel");
		//panelObject = GameObject.Find("Canvas/mainUI/Panel");
        buildMenu = mainObject.GetComponent<BuildMenu> ();
        main = mainObject.GetComponent<DemMain> ();
        turnSystem = mainObject.GetComponent<DemTurnSystem> ();
        // Parse X and Y coords from name
        // Name format = "X,Y", so X is stored @ name[0] and Y @ name[2]
        // The char value for '0' starts at 0x30, subtract this to parse numeric value
        // NOTE: this assumes that X and Y values remain within the range [0,9]
        idX = this.name[0] - 0x30; 
        idY = this.name[2] - 0x30;
        //Debug.Log("Cube at (" + idX + ", " + idY + ")");

        // Set resident to null
        resident = null;

        center = this.GetComponent<Renderer>().bounds.center;
        center.z = -1.5f;

        

        currentColor = Color.white;

        // Set initial pulse parameters
        ResetPulse();
        SetRestorePulse(Color.white, new Color(0.7f, 0.7f, 0.7f));
        hasPulse = false;
        pulseTick = 0;
        pulseFactor = defaultPulseFactor;
        masterPulseFrequency = defaultPulseFrequency;
        rangeColor = new Color(40f/255f, 170f/255f, 220f/255f);

        // Enable master pulse if needed
        if (!masterPulseEnabled)
        {
            StartCoroutine(EnableMasterPulse());
            masterPulseEnabled = true;
        }
    }

    /**
        Static method for DemTile pulse synchronization.
    */
    static IEnumerator EnableMasterPulse ()
    {
        float delayFactor = pulseFactor / 2.0f;
        while (true)
        {
            yield return new WaitForSeconds(delayFactor / masterPulseFrequency);
            pulseTick ++;
        }
    }

    /**
        Defines and returns a coroutine for pulsing between two different colors at a particular frequency.
        An optional delay to start the coroutine may be specified in seconds.
        Note that by default, the pulse is synced with the master clock defined by EnableMasterPulse.
        Also note that defining a non-positive pulse factor will set the tile to a solid color specified by 'color1'.
    */
    IEnumerator Pulse
    (
        Color color1, 
        Color color2, 
        float frequency = defaultPulseFrequency, 
        float pulseFactor = defaultPulseFactor,
        bool syncWithMaster = true
    )
    {
        if (syncWithMaster)
        {
            for (int i = pulseTick; true; i = pulseTick)
            {
                this.GetComponent<Renderer>().material.color = Color.Lerp(color1, color2, Mathf.PingPong(pulseTick * pulseFactor, 1.0f));
                while (i == pulseTick)
                    yield return null;
            }
        }
        else
        {
            float delayFactor = 0.5f * pulseFactor;

            for (int i = 0; true; i++)
            {
                this.GetComponent<Renderer>().material.color = Color.Lerp(color1, color2, Mathf.PingPong(i * pulseFactor, 1.0f));
                yield return new WaitForSeconds(delayFactor / frequency);
            }
        }
    }


    /**
        Sets the pulse parameters.
        Changes will affect both active and inactive pulses.
    */
    public void SetPulse
    (
        Color color1, 
        Color color2, 
        float frequency = defaultPulseFrequency, 
        float pulseFactor = defaultPulseFactor,
        bool syncWithMaster = true
    )
    {
        if (hasPulse)
        {
            SignalPulse(false);
            pulse = Pulse(color1, color2, frequency, pulseFactor, syncWithMaster);
            SignalPulse(true);
        }
        else
            pulse = Pulse(color1, color2, frequency, pulseFactor, syncWithMaster);
    }

    /**
        Resets pulse to default values.
    */
    public void ResetPulse ()
    {
        pulse = Pulse(Color.white, new Color(0.7f, 0.7f, 0.7f));
    }

    /**
        Sets the pulse to its restore point.
    */
    void RestorePulse ()
    {
        if (hasPulse)
        {
            SignalPulse(false);
            pulse = GetRestorePulse();
            SignalPulse(true);
        }
        else
            pulse = GetRestorePulse();
    }

    /**
        Defines the restore pulse.
    */
    public void SetRestorePulse (Color color1, Color color2, float frequency = defaultPulseFrequency)
    {
        restoreColor1 = color1;
        restoreColor2 = color2;
        restoreColorFrequency = frequency;
    }

    /**
        Returns a pulse instance as a restore.
    */
    IEnumerator GetRestorePulse ()
    {
        return Pulse(restoreColor1, restoreColor2, restoreColorFrequency);
    }

    /**
        Sends a signal to activate (signal = true) or deactivate (signal = false) the Pulse coroutine.

        Ignores activation signals if a pulse coroutine is currently active, preventing pulse overlap.
        Note that the pulse parameters must be defined with SetPulse prior to calling, otherwise the current values
        will be used.
    */
    public void SignalPulse (bool signal)
    {
        if (signal)
        {
            if (!hasPulse)
            {
                StartCoroutine(this.pulse);
                hasPulse = true;
            }
        }

        else if (hasPulse)
        {
            StopCoroutine(this.pulse);
            hasPulse = false;
        }
    }

    /**
        Activates on mouse enter.
    */
    void OnMouseEnter ()
    {
        // Set tile pulse
        // If building...
        if (buildMenu.currentAnimalFactory != null)
        {
            if (available)
            {
                // As build origin, paint a solid color (no pulse)
                SetPulse(Color.cyan, Color.cyan, 0, 0);

                // Pulse the range tiles for a plant
                if (buildMenu.currentAnimalFactory.GetType() == 0)
                {
                    // Grab range offset data for the plant
                    int[][] range = SpeciesConstants.Range(buildMenu.currentAnimalFactory.GetName());
                    foreach (int[] coord in range)
                    {
                        // Skip tiles that are out of range
                        if (idX + coord[0] < 0 || idX + coord[0] > 8 || idY + coord[1] < 0 || idY + coord[1] > 4)
                            continue;

                        // If tile is free, set pulse
                        DemTile tile =  main.boardController.Tiles[idX + coord[0], idY + coord[1]].GetComponent<DemTile>();
                        if (!tile.resident)
                            tile.SetPulse(rangeColor, Color.white);
                    }
                }

            }

            // If not available, pulse red
	        else
            {
                SetPulse(Color.red, Color.Lerp(Color.red, Color.white, 0.25f), 0.25f, 0.05f, false);
                SignalPulse(true);
            }
        // If not building...
        }
        else
        {
            // Sublte white-to-gray for empty cursor
            SetPulse(Color.white, new Color(0.7f, 0.7f, 0.7f));
            SignalPulse(true);

            // If tile occupied, output some stats
            if (resident)
            {
                Debug.Log(resident.name + " is here");
                BuildInfo info = resident.GetComponent<BuildInfo>();

                // Determine Health or Hunger text
                string healthLevel = "";
                if (SpeciesConstants.SpeciesType(info.name) == 1)
                    healthLevel = "Health: " + (info as PreyInfo).GetHealth().ToString();
                else if (SpeciesConstants.SpeciesType(info.name) == 2)
                    healthLevel = "Hunger: " + (info as PredatorInfo).GetHunger().ToString();

                // Creates tooltip when mouse is over a resident on the board and keeps tooltip in the view of the screen
                if (resident.transform.position.x < 2)
                    panelObject.transform.position = new Vector3(Input.mousePosition.x + 150, Input.mousePosition.y);
                else
                    panelObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

                panelObject.SetActive(true);
                panelObject.transform.GetChild(1).GetComponent<Text>().text = resident.name;
                panelObject.transform.GetChild(2).GetComponent<Text>().text = healthLevel;
                panelObject.transform.GetChild (3).GetChild (0).GetComponent<Text> ().text = "Biomass : " +
                    SpeciesConstants.Biomass (resident.name).ToString();

                panelObject.transform.GetChild(1).gameObject.SetActive(true);
                panelObject.transform.GetChild(2).gameObject.SetActive(true);
            }
        }

    }

    /**
        Activates on mouse exit.
    */
    void OnMouseExit ()
    {
        // Reset highlight color
        if (buildMenu.currentAnimalFactory != null && available)
        {
            // ... for current tile
            RestorePulse();

            // ... for plant range tiles (if building)
            if (buildMenu.currentAnimalFactory.GetType() == 0)
            {
                int[][] range = SpeciesConstants.Range(buildMenu.currentAnimalFactory.GetName());
                foreach (int[] coord in range)
                {
                    // Skip tiles that are out of range
                    if (idX + coord[0] > 8 || idX + coord[0] < 0 || idY + coord[1] > 4 || idY + coord[1] < 0)
                        continue;

                    DemTile tile =  main.boardController.Tiles[idX + coord[0], idY + coord[1]].GetComponent<DemTile>();
                    // Tile must be free
                    if (!tile.resident)
                        // Set the pulse for each tile
                        tile.RestorePulse();
                }
            }
        }
        else
        {
            SignalPulse(false);
            this.GetComponent<Renderer>().material.color = currentColor;
        }
        
        // Sets tooltip as inactive
        panelObject.SetActive(false);
        panelObject.transform.GetChild(1).gameObject.SetActive(false);
        panelObject.transform.GetChild(2).gameObject.SetActive(false);

    }

    /**
        Activates on mouse click.
    */
    void OnMouseDown ()
    {
        // Get center coords of tile, set z offset for resident placement
        

        // DEBUG
        Debug.Log("Tile (" + idX + ", " + idY + ") clicked, center @ (" + center.x + ", " + center.y + ", " + center.z + ")");

        
            // If a creature is flagged for building...
      if (buildMenu.currentAnimalFactory != null) {
                // Set the resident as the DemMain's current selection if clicked within the tile; center resident on tile

      // If tile is empty...
          if (available) {

                DemAudioManager.audioUiLoad.Play ();
                //resident = DemMain.currentSelection;
                
                //resident.transform.position = center;
                AddAnimal(main.currentSelection);
                
                if (buildMenu.currentAnimalFactory.isPlant ()) {
                    int currentBiomass = buildMenu.getPlantBiomass ();
                    int newBiomass = SpeciesConstants.Biomass (main.currentSelection.name);
                    buildMenu.UpdatePlantBiomass (currentBiomass - newBiomass);
                    buildMenu.AddTier2Biomass ((int)(newBiomass * 0.5));

                } else { //Else it is prey
                    int currentBiomass = buildMenu.GetTier2Biomass();
                    buildMenu.SubtractTier2Biomass (SpeciesConstants.Biomass (main.currentSelection.name));
                }

                

    


                // Set BuildMenu.currentlyBuilding to null after successful placement
                buildMenu.currentAnimalFactory = null;
                main.currentSelection = null;
                main.boardController.ClearAvailableTiles();
                turnSystem.PredatorTurn();

                // DEBUG 
                if (resident)
                    Debug.Log("Placed " + resident.name + " @ " + resident.GetComponent<Transform>().position);
           }else {
              DemAudioManager.audioFail2.Play ();
           }

        }
        // If tile is inhabited...
        
    }


  public bool hasPlant(){
    
    if (this.resident) {
		return this.resident.GetComponent<BuildInfo> ().isPlant();
    } else {
      return false;
    }

  }

  public GameObject GetResident(){
    return resident;
  }

  public bool ResidentIsPredator(){
    
    if (resident) {
      return resident.GetComponent<BuildInfo> ().isPredator ();
    } else if(nextPredator){
      return true;
    }
    else
    {
      return false;
    }

  }

  public void SetResident(GameObject newResident){
    resident = newResident;
  }


  public void AddAnimal(GameObject animal){

    this.resident = animal;

    this.resident.GetComponent<BuildInfo> ().tile = this;

    this.resident.transform.position = this.center;

		//add statistic to tree place down, and prey place down
		if (this.resident.GetComponent<BuildInfo> ().isPlant ()) {
			buildMenu.statistic.setTreeDown (1);
		} else if (this.resident.GetComponent<BuildInfo> ().isPrey ()) {
			buildMenu.statistic.setPreyDown (1);
		}


  }


  public void AddNewPredator(GameObject animal){

    this.nextPredator = animal;

    this.nextPredator.GetComponent<BuildInfo> ().tile = this;

    Vector3 newPosition = new Vector3();
    newPosition.x = this.center.x+2;
    newPosition.y = this.center.y;
    newPosition.z = this.center.z;

    this.nextPredator.transform.position = newPosition;

  }

  public void UpdateNewPredator(){
    resident = nextPredator;
    nextPredator = null;
  }

  public void RemoveAnimal(){

		//add statistic to tree destroy, and prey eaten
		if (this.resident.GetComponent<BuildInfo> ().isPlant ()) {
			buildMenu.statistic.setTreeDestroy (1);
            //player gains 1 credits for each plant eaten
            Game.networkManager.Send(UpdateCreditsProtocol.Prepare((short)0, 1), ProcessUpdateCredits);
            Debug.Log("old credits: " + GameState.player.credits);
            Debug.Log("player awarded 1 credits.");
        } else if (this.resident.GetComponent<BuildInfo> ().isPrey ()) {
			buildMenu.statistic.setPreyEaten (1);
            //player gains 15 credits for each prey eaten
            Game.networkManager.Send(UpdateCreditsProtocol.Prepare((short)0, 15), ProcessUpdateCredits);
            Debug.Log("old credits: " + GameState.player.credits);
            Debug.Log("player awarded 15 credits.");
        }
    
    Destroy (resident);
    this.resident = null;
  
  }

  public int GetIdX(){
    return idX;
  }

  public int GetIdY(){
    return idY;
  }

  public Vector3 GetCenter(){
    return center;
  }

    public void ProcessUpdateCredits(NetworkResponse response)
    {
        ResponseUpdateCredits args = response as ResponseUpdateCredits;
        Debug.Log("ResponseUpdateCredits: action= " + args.action);

        if (args.status == 0)
        {
            GameState.player.credits = args.newCredits;
            Debug.Log("new credits: " + args.newCredits);
        }
        else
            Debug.Log("failed to update credits");
    }

}
