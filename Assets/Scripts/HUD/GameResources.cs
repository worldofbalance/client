using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class GameResources : MonoBehaviour {
    
    public GUISkin skin;
    
    private int credits;
    private int coins;
    private Boolean showTopListDialog = false;
    
    void Awake() {
        skin = Resources.Load("Skins/DefaultSkin") as GUISkin; 
    }
    
    // Update is called once per frame
    void Update () {
        try {
            //			credits = GameState.world.credits;
            credits = GameState.player.credits;
        } catch (NullReferenceException e) {
        }
    }
        
    public void SetCredits(int credits) {
        this.credits = credits;
    }
    
    public void SetCoins(int coins) {
        this.coins = coins;
    }
}
