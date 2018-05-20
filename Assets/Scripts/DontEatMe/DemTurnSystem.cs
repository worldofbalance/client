
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemTurnSystem : MonoBehaviour {
  
  private  DemBoard board;
  private Dictionary<int, GameObject> activePredators;// = new Dictionary<int, GameObject>();
  private  Queue<DemTween> tweenList = new Queue<DemTween>();
  private  DemTile nextTile;
  private  DemTile currentTile;
  public  bool turnLock = false;
  //private  GameObject predator;
  private DemMain main;
  private GameObject mainObject;
  private DemTweenManager tweenManager;
  DemTile tile;
  private int lives;
  private BuildMenu buildMenu;
  private int credits;
  private int turnNumber;


  void Awake()
  {
    
    mainObject = GameObject.Find ("MainObject");
    board = GameObject.Find ("GameBoard").GetComponent<DemBoard>();
    main = mainObject.GetComponent<DemMain> ();
    tweenManager = mainObject.GetComponent<DemTweenManager> ();
    buildMenu = mainObject.GetComponent<BuildMenu> ();
    lives = 3;
    credits = 0;
    turnNumber = 0;


  }

  void Start()
  {
    buildMenu.UpdateLives (lives);
  }
	

  public  void PredatorTurn()
  {
    turnNumber++;
    turnLock = true;
    buildMenu.ToggleButtonLocks ();
    activePredators = board.GetPredators ();
    Debug.Log ("Total predators :" + activePredators.Count);
    foreach(KeyValuePair<int, GameObject> predator in activePredators)
    {

      BuildInfo animal = predator.Value.GetComponent<BuildInfo> ();
      int x = animal.GetTile ().GetIdX ();
      int y = animal.GetTile ().GetIdY ();
	    
      currentTile = board.Tiles [x, y].GetComponent<DemTile> ();

      if (x + 1 == 9) {
        //Do the bounds checking first instead of last
        //currentTile.RemoveAnimal (); //remove predator
          
        //Debug.Log ("you lost, or whatever, bla bla bla...");
        Debug.Log("arrived at final tile");
        tweenList.Enqueue (new DemPredatorExitTween (predator.Value,  500));
        //continue;

      } else {
        

        nextTile = board.Tiles [x + 1, y].GetComponent<DemTile> ();


        animal.SetNextTile (nextTile);
        tweenList.Enqueue (new DemTileTransitionTween (predator.Value, nextTile.GetCenter (), 500));
      }

    }

    if(turnNumber % 2 == 1){
      GenerateNewPredators ();
    }

    ProcessTweens ();

	// turn count 1++
	buildMenu.statistic.setTurnCount (1);

  }

  public  bool IsTurnLocked(){
    
    return !turnLock;

  }


  public  void PredatorFinishedMove (GameObject finishedPredator)
  {
    
    BuildInfo predator = finishedPredator.GetComponent<BuildInfo> ();
    Boolean markForDeletion = false;

    if( predator.GetNextTile().resident  != null){
      
      BuildInfo nextAnimal = predator.GetNextTile ().resident.GetComponent<BuildInfo> ();

      if(nextAnimal.isPrey() || nextAnimal.isPlant()){
        predator.GetNextTile ().RemoveAnimal();
      }

      if(nextAnimal.isPrey()){
        buildMenu.AddTier2Biomass (SpeciesConstants.Biomass (nextAnimal.name));
        DemAudioManager.audioSelection.Play ();
        credits++;
        buildMenu.UpdateCredits (credits);
        markForDeletion = true;
      }else if (nextAnimal.isPlant()){
        buildMenu.AddPlantBiomass (SpeciesConstants.Biomass (nextAnimal.name));
        buildMenu.SubtractTier2Biomass ((int)(SpeciesConstants.Biomass (nextAnimal.name) * 0.5));
      }

    }



    predator.AdvanceTile ();

    if(markForDeletion){
      buildMenu.SubtractTier3Biomass (SpeciesConstants.Biomass (predator.name));
      DemTile tempTile = predator.GetTile ();
      activePredators.Remove(finishedPredator.GetInstanceID());
      tempTile.RemoveAnimal ();


    }

      
    ProcessTweens ();


  }


  public void PredatorExit(GameObject finishedPredator){

    BuildInfo predator = finishedPredator.GetComponent<BuildInfo> ();
    predator.GetTile ().RemoveAnimal ();
    lives--;
    buildMenu.UpdateLives (lives);

    if (lives == 0) {
      activePredators.Remove(finishedPredator.GetInstanceID());
      tweenList.Clear();
      GameOver ();
    
    } else {
      buildMenu.SubtractTier3Biomass (SpeciesConstants.Biomass (predator.name));
      activePredators.Remove(finishedPredator.GetInstanceID());
      ProcessTweens ();

    }

  }

  public  void GenerateNewPredators()
  {

    //For Testing
    int random = UnityEngine.Random.Range (0, 5);


    int randomPredator = UnityEngine.Random.Range (0, main.predators.Length);

    GameObject newPredator = main.predators [randomPredator].Create ();
    newPredator.GetComponent<BuildInfo> ().speciesType = 2;

    //activePredators.Add (newPredator.GetInstanceID() , newPredator);

    board.AddNewPredator(0, random, newPredator );
    //board.AddNewPredator(8, random, newPredator );

    tweenList.Enqueue(new DemPredatorEnterTween (newPredator, 700));
    buildMenu.AddTier3Biomass (SpeciesConstants.Biomass (newPredator.GetComponent<BuildInfo> ().name));
    //tweenList.Clear ();

    //turnLock = false;

  }

  public void ProcessTweens()
  {
    
    if (tweenList.Count > 0) {
      tweenManager.AddTween (tweenList.Dequeue ());
    } else {
      //GenerateNewPredators ();
      tweenList.Clear();
      turnLock = false;
      buildMenu.ToggleButtonLocks ();
      buildMenu.UpdateMenuLocks ();
    }

  }

  public int GetLives(){
    return lives;
  }



  public void GameOver(){
    //turnLock = true;
    Debug.Log ("game Over");

    //let's award players 30 credits for playing - Jeremy
    Game.networkManager.Send(UpdateCreditsProtocol.Prepare((short)0, 30), ProcessUpdateCredits);
    Debug.Log("old credits: " + GameState.player.credits);
    Debug.Log("player awarded 30 credits.");

    buildMenu.EndGame ();
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
