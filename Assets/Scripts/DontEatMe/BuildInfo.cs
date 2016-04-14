using UnityEngine;
using System.Collections;

public class BuildInfo : MonoBehaviour {

	// the menu button sprite
	public Texture previewImage;

  public GameObject parent;

	// Species ID
	// NOTE: this is made public for the time being to allow assignment of species ID to GameObject instances
	public int speciesId;

	// the cost to build the item
	public int price;

    //public bool isPlant;
	public short speciesType; //for classify if it is plant =0, prey =1, or preditor =2

  public DemTile tile = null;

  public DemTile nextTile = null;



	// Current Health
	//[SerializeField]
	//int speciesId = 0;

  public int GetSpeciesId() {
		return speciesId;
	}

	// Use this for initialization
	void Start () {
    //parent = GetComponent<Transform> ();

	}
	
	// Update is called once per frame
  public DemTile GetTile () {
    return tile;
	}

  public void SetTile(DemTile newTile){
    tile = newTile;
  }

  public void SetNextTile(DemTile newNextTile){
    nextTile = newNextTile;
  }

  public void AdvanceTile()
  {

    if (tile) {
    
      DemTile oldTile = tile;
      nextTile.AddAnimal (parent);
      oldTile.SetResident (null);
      tile = nextTile;
      nextTile = null;
    
    } else {
      nextTile.AddAnimal (parent);
      tile = nextTile;
      nextTile = null;  
    }
      

  }

	public bool isPlant(){
		return speciesType == 0;
	}

	public bool isPrey(){
		return speciesType == 1;
	}

	public bool isPredator(){

		return speciesType == 2;

	}

  public void SetParent(GameObject _parent){
    parent = _parent;
  }

  public DemTile GetNextTile()
  {

    return nextTile;

  }

}
