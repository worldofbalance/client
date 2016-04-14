using UnityEngine;
using System.Collections;
using System;

public abstract class DemTween :Component{
  
  protected GameObject tweenObject;
  protected Vector3 oldPosition;
  protected Vector3 newPosition;
  protected int tweenTime;
  protected float initialTime;
  protected float deltaTime;
  protected float progress;
  protected Transform objectTransform;
  protected Vector3 journeyLength;
  protected bool complete;
  public bool running;
  protected DemTurnSystem turnSystem;
  protected  GameObject mainObject;
  protected DemMain main;
  protected DemTweenManager tweenManager;


  public DemTween(GameObject _tweenObject , int _tweenTime){

    mainObject = GameObject.Find ("MainObject");
    main = mainObject.GetComponent<DemMain> ();
    tweenManager = mainObject.GetComponent<DemTweenManager> ();
    turnSystem = mainObject.GetComponent<DemTurnSystem> ();
    running = false;
    complete = false;
    progress = 0;
    tweenTime = _tweenTime;
    this.tweenObject = _tweenObject;
    objectTransform = this.tweenObject.GetComponent<Transform> ();



  }

  public void Start(){
    
    initialTime = Time.time;
    running = true;

  }

  public abstract void Update(int id);





}
