using UnityEngine;
using System.Collections;

/**
    The DemTile class represents a single tile of the Don't Eat Me grid board.
*/
public class DemTile : MonoBehaviour
{
  	public int idX; // X-coord for DemTile
  	public int idY; // Y-coord for DemTile
    public Color currentColor;

    public bool available;

    private Vector3 center;

  	public GameObject resident; // Resident object (HerbivoreObject or PlantObject) placed on tile
    private GameObject nextPredator;
    private BuildMenu buildMenu;
    public  GameObject mainObject;
    private DemMain main;
    private DemTurnSystem turnSystem;
    

    // Use this for initialization
    void Start ()
    {
        mainObject = GameObject.Find ("MainObject");
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

        

        this.currentColor = Color.white;
    }

    /**
        Activates on mouse enter.
    */
    void OnMouseEnter ()
    {
        
        // Set highlight color
        // TODO: change highlight color based on a tile's legality
        if (buildMenu.currentAnimalFactory!= null) {
         if (available)
	            this.GetComponent<Renderer>().material.color = Color.cyan;
	        else
	            this.GetComponent<Renderer>().material.color = Color.red;
        }
        else {
        	this.GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    /**
        Activates on mouse exit.
    */
    void OnMouseExit ()
    {
        // Reset highlight color
      this.GetComponent<Renderer>().material.color = currentColor;
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
                // Subtract the appropriate resources for the build
                //BuildMenu.currentResources -= BuildMenu.currentlyBuilding.price;

                // Testing animation transition from IDLE to animated once placed on board
                // NOTE: only supported species (i.e. those currently with animations) should call 'SetTrigger';
                // unsupported species will cause unpredictable behavior
                // TODO: implement animations for all species
                /*
                if (
                    BuildMenu.currentlyBuilding.name == "BushHyrax" ||
                    BuildMenu.currentlyBuilding.name == "TreeMouse")
                    resident.GetComponent<Animator>().SetTrigger("initialized");
                    */

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


}
