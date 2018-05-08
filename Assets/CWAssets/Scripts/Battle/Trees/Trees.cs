using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
namespace CW{
public class Trees : MonoBehaviour {

    private int hp, maxHP, dmgTimer = 0;
    private BattlePlayer player;
    private TreesHandler handler;
    private bool removeAfterDelay;
    private float delayTimer = 0, DELAY_CONSTANT = 3;
    private bool surrendering = false;

    Texture2D tree1Texture;
    Texture2D tree2Texture;
    Texture2D tree3Texture;
        //initializing all canvases and buttons
    GameObject optionsButton;
    GameObject surrenderButton;
    GameObject yesButton;
    GameObject noButton;
    GameObject optionCanvas;
    GameObject confirmCanvas;
    GameObject closeButton;
    GameObject helpButton;
    GameObject helpCloseButton;
    GameObject helpCanvas;
    // Use this for initialization
    void Start () {
        
        
    }

    public void init(BattlePlayer player){
        tree1Texture = (Texture2D) Resources.Load ("Images/Battle/tree1", typeof(Texture2D));
        tree2Texture = (Texture2D) Resources.Load ("Images/Battle/tree2", typeof(Texture2D));
        tree3Texture = (Texture2D) Resources.Load ("Images/Battle/tree3", typeof(Texture2D));
        this.player = player;
        maxHP =hp= 30; 

        if (player.player1) { //Your name is pink
            //transform.Find ("NameText").GetComponent<TextMesh> ().text = this.player.playerName;
                transform.Find ("NameText").GetComponent<Renderer> ().material.SetColor("_Color", Color.red);
        } else { //Enemy name is redateria
            //transform.Find ("NameText").GetComponent<TextMesh> ().text = this.player.playerName;
            transform.Find("NameText").GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
        //Set dmg text
        transform.Find("DamageText").GetComponent<TextMesh>().text = "";
        //Set alpha level for fading
        //transform.Find ("DamageText").GetComponent<TextMesh> ().color.a = 0;
        handler = new LivingTreeClick(this, player);
        transform.position = new Vector3(player.TreePos.x, player.TreePos.y, player.TreePos.z);

        /*GameObject optionsButton = GameObject.Find ("/OptionsButtonCanvas/OptionsButton");
        GameObject surrenderButton = GameObject.Find ("/OptionsCanvas/OptionsPanel/QuitButtons/SurrenderButton");
        optionsButton.GetComponent<Button> ().onClick.AddListener (() => {toggleSurrender();});
        surrenderButton.GetComponent<Button>().onClick.AddListener (() => {surrenderConfirm();});*/

        optionsButton = GameObject.Find ("/OptionsButtonCanvas/OptionsButton");
        surrenderButton = GameObject.Find ("/OptionsCanvas/OptionsPanel/QuitButtons/SurrenderButton");
        yesButton = GameObject.Find("/ConfirmCanvas/ConfirmPanel/YesButton");
        noButton = GameObject.Find("/ConfirmCanvas/ConfirmPanel/NoButton");
        optionCanvas = GameObject.Find ("OptionsCanvas");
        confirmCanvas = GameObject.Find ("ConfirmCanvas");
        closeButton = GameObject.Find("OptionsCanvas/OptionsPanel/CloseOptions");
        helpCanvas = GameObject.Find("HelpCanvas");
        helpButton = GameObject.Find ("/OptionsCanvas/OptionsPanel/QuitButtons/HelpButton");
        helpCloseButton = GameObject.Find("HelpCanvas/BG/Button");

        //adding listeners for buttons
        optionsButton.GetComponent<Button> ().onClick.AddListener (() => {toggleSurrender();});
        closeButton.GetComponent<Button> ().onClick.AddListener (() => {toggleSurrender();});
        surrenderButton.GetComponent<Button>().onClick.AddListener (() => {surrenderConfirm();});
        yesButton.GetComponent<Button>().onClick.AddListener(() => {surrenderOn();});
        noButton.GetComponent<Button>().onClick.AddListener(() => {surrenderConfirm();});
        helpButton.GetComponent<Button> ().onClick.AddListener (() => {toggleHelp();});
        helpCloseButton.GetComponent<Button> ().onClick.AddListener (() => {toggleHelp();});


        Debug.Log("adding listeners");
    }

    //use this version of init to initialize player 2, so that we dont add redundant listeners on certain buttons(ie. surrender) -Jeremy
    public void init2(BattlePlayer player)
    {
        this.player = player;
        maxHP = hp = 30;

        if (player.player1)
        { //Your name is pink
            //transform.Find ("NameText").GetComponent<TextMesh> ().text = this.player.playerName;
            transform.Find("NameText").GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        else
        { //Enemy name is redateria
            //transform.Find ("NameText").GetComponent<TextMesh> ().text = this.player.playerName;
            transform.Find("NameText").GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
        //Set dmg text
        transform.Find("DamageText").GetComponent<TextMesh>().text = "";
        //Set alpha level for fading
        //transform.Find ("DamageText").GetComponent<TextMesh> ().color.a = 0;
        handler = new LivingTreeClick(this, player);
        transform.position = new Vector3(player.TreePos.x, player.TreePos.y, player.TreePos.z);

        /*GameObject optionsButton = GameObject.Find ("/OptionsButtonCanvas/OptionsButton");
        GameObject surrenderButton = GameObject.Find ("/OptionsCanvas/OptionsPanel/QuitButtons/SurrenderButton");
        optionsButton.GetComponent<Button> ().onClick.AddListener (() => {toggleSurrender();});
        surrenderButton.GetComponent<Button>().onClick.AddListener (() => {surrenderConfirm();});*/

        optionsButton = GameObject.Find("/OptionsButtonCanvas/OptionsButton");
        surrenderButton = GameObject.Find("/OptionsCanvas/OptionsPanel/QuitButtons/SurrenderButton");
        yesButton = GameObject.Find("/ConfirmCanvas/ConfirmPanel/YesButton");
        noButton = GameObject.Find("/ConfirmCanvas/ConfirmPanel/NoButton");
        optionCanvas = GameObject.Find("OptionsCanvas");
        confirmCanvas = GameObject.Find("ConfirmCanvas");
        closeButton = GameObject.Find("OptionsCanvas/OptionsPanel/CloseOptions");
        helpCanvas = GameObject.Find("HelpCanvas");
        helpButton = GameObject.Find("/OptionsCanvas/OptionsPanel/QuitButtons/HelpButton");
        helpCloseButton = GameObject.Find("HelpCanvas/BG/Button");

        //adding listeners for buttons
        /*
        optionsButton.GetComponent<Button>().onClick.AddListener(() => { toggleSurrender(); });
        closeButton.GetComponent<Button>().onClick.AddListener(() => { toggleSurrender(); });
        surrenderButton.GetComponent<Button>().onClick.AddListener(() => { surrenderConfirm(); });
        yesButton.GetComponent<Button>().onClick.AddListener(() => { surrenderOn(); });
        noButton.GetComponent<Button>().onClick.AddListener(() => { surrenderConfirm(); });
        */
        helpButton.GetComponent<Button>().onClick.AddListener(() => { toggleHelp(); });
        helpCloseButton.GetComponent<Button>().onClick.AddListener(() => { toggleHelp(); });


        Debug.Log("adding listeners");
    }



        //OnMouseOver does not accept mouse clicks if mouse is not moving when the input from user
        //was received. I don't think it's necessary for the tree to get larger when the player clicks the tree
        //  void OnMouseDown () 
        //  {
        //      if(handler != null)
        //          handler.clicked ();
        //
        //  }
        void OnMouseOver ()
    {
            if (!EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log ("MouseOver Clicked the Tree");
            if (Input.GetMouseButtonDown (0)) {
                if(handler != null )
                    handler.clicked ();
            }
            this.transform.localScale = new Vector3 (25.5f, 1, 30);
        }
    }
    
    void OnMouseExit()
    {
        this.transform.localScale = new Vector3 (20.5f, 1, 25);

    }

    /*public void attack(Trees target){
        
        GameManager.curPlayer.clickedCard.calculateDirection (target.transform.position, true);
        target.receiveAttack(GameManager.curPlayer.clickedCard.getDamage());    
    }*/

    public void receiveAttack(int dmg){
        dmgTimer = 120;
        transform.Find("DamageText").GetComponent<TextMesh>().text = "-"+dmg;
        hp -= dmg;
        Debug.Log ("Was dealt " + dmg + " damage and is now at " + hp + " hp");
        
        if(hp <= 0){
            Debug.Log("End Game");  
            
            if (this.player.playerID != 0) { //if playerID = 0, the other player lost
                this.player.isWon = false;
            }
                removeAfterDelay = true;
        }
        
    }

    
    // Update is called once per frame
    void Update () {

        if(removeAfterDelay){
            delayTimer += Time.deltaTime;

            if(delayTimer > DELAY_CONSTANT){

                handler = new EndGame(this, player);
                handler.affect();
                delayTimer = 0;
                removeAfterDelay = false;
            }
        }
        //Display health and change texture accordingly
        transform.Find("HealthText").GetComponent<TextMesh>().text = hp.ToString();
        if(hp <= (maxHP)/4) { //Under 1/4 hp
            GetComponent<Renderer>().material.mainTexture = tree3Texture;
            transform.Find("HealthText").GetComponent<TextMesh>().color = Color.red;
        } else if (hp <= (3*maxHP)/4) {//Under 3/4 hp
            GetComponent<Renderer>().material.mainTexture = tree2Texture;
            transform.Find("HealthText").GetComponent<TextMesh>().color = Color.yellow;
        } else if (hp > (3*maxHP)/4) { //Over 3/4 hp
            GetComponent<Renderer>().material.mainTexture = tree1Texture;
            transform.Find ("HealthText").GetComponent<TextMesh> ().color = Color.green;
        }
        if (this.player.isActive) 
        {
            transform.Find ("NameText").GetComponent<TextMesh> ().text = ">>"+this.player.playerName+"<<";
            transform.Find ("ActionText").GetComponent<TextMesh>().text = "Playing...";
            transform.Find ("ActionText").GetComponent<Renderer> ().material.SetColor("_Color", Color.yellow);
        } 
        else { //Enemy name is red
            transform.Find ("NameText").GetComponent<TextMesh> ().text = this.player.playerName;
            transform.Find ("ActionText").GetComponent<TextMesh>().text = "Waiting...";
            transform.Find ("ActionText").GetComponent<Renderer> ().material.SetColor("_Color", Color.grey);
        }
        if (dmgTimer > 0) {
            dmgTimer--;
        } 
        else 
        {
            transform.Find("DamageText").GetComponent<TextMesh>().text = "";
        }
    }
    /*
    void OnGUI(){
        
        //End game
        /*
        if(GUI.Button(new Rect(Screen.width-(Screen.width/12.8f)/100 *150, (Screen.height/2.0f), 
                               (Screen.width/12.8f)/100 *150, (Screen.width/12.8f)/100 *40), 
                                "Surrender")){
            toggleSurrender();
        }
        *//*
        if (surrendering) { //should only show when surrendering  is true
            GUI.skin.box.fontStyle = FontStyle.Bold;
            GUI.skin.box.fontSize = 30;
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "Do you want to surrender?");
            GUILayout.BeginArea(new Rect((Screen.width/2.0f)-100, (Screen.height/2.0f), 400, 250));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Yes", GUILayout.Width(100),GUILayout.Height (100)))
            {
                toggleSurrender();//get rid of buttons
                Debug.Log("End Game");  
                this.player.isWon=false;
//              handler = new EndGame(this, player);
//              handler.affect();

                // Call quitmatch protocol -- notify oponent that player is quitting
                // return player to lobby
                GameManager.protocols.sendQuitMatch(player.playerID);
            }
            if (GUILayout.Button("No", GUILayout.Width(100),GUILayout.Height (100)))
            {
                toggleSurrender();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
    */
    //Toggles surrender gui true or false

    /*
    void toggleSurrender(){
        if (surrendering) {
            surrendering=false;
        } else if (!surrendering){
            surrendering=true;
        }
    }
    */
    void toggleSurrender()
    {

        //GameObject optionCanvas = GameObject.Find ("OptionsCanvas");
        if (optionCanvas.GetComponent<CanvasGroup>().alpha == 0 && player.player1)
        {
            Debug.Log("options openned");
            //optionsButton.GetComponent<CanvasGroup>().interactable = false;
            optionCanvas.GetComponent<CanvasGroup>().alpha = 1;
            optionCanvas.GetComponent<CanvasGroup>().interactable = true;
            optionCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        }
        else if (optionCanvas.GetComponent<CanvasGroup>().alpha == 1 && player.player1)
        {
            Debug.Log("options closed");
            optionCanvas.GetComponent<CanvasGroup>().alpha = 0;
            optionCanvas.GetComponent<CanvasGroup>().interactable = false;
            optionCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            confirmCanvas.GetComponent<CanvasGroup>().alpha = 0;
            confirmCanvas.GetComponent<CanvasGroup>().interactable = false;
            confirmCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    void toggleHelp()
    {
        if (helpCanvas.GetComponent<CanvasGroup>().alpha == 0 && player.player1)
        {
            //close options canvas
            Debug.Log("options closed");
            optionCanvas.GetComponent<CanvasGroup>().alpha = 0;
            optionCanvas.GetComponent<CanvasGroup>().interactable = false;
            optionCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            confirmCanvas.GetComponent<CanvasGroup>().alpha = 0;
            confirmCanvas.GetComponent<CanvasGroup>().interactable = false;
            confirmCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            //show help
            Debug.Log("Help Overlay opened");
            helpCanvas.GetComponent<CanvasGroup>().alpha = 1;
            helpCanvas.GetComponent<CanvasGroup>().interactable = true;
            helpCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (helpCanvas.GetComponent<CanvasGroup>().alpha == 1 && player.player1)
        {
            helpCanvas.GetComponent<CanvasGroup>().alpha = 0;
            helpCanvas.GetComponent<CanvasGroup>().interactable = false;
            helpCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    void surrenderConfirm()
    {
        Debug.Log("confirm dialog");
        //GameObject confirmCanvas = GameObject.Find ("ConfirmCanvas");
        //GameObject optionCanvas = GameObject.Find ("OptionsCanvas");
        if (confirmCanvas.GetComponent<CanvasGroup>().alpha == 0 && player.player1)
        {
            //disable options panel
            //show/enable confirm dialog
            //optionCanvas.GetComponent<CanvasGroup>().alpha = 0;
            optionCanvas.GetComponent<CanvasGroup>().interactable = false;
            optionCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            confirmCanvas.GetComponent<CanvasGroup>().alpha = 1;
            confirmCanvas.GetComponent<CanvasGroup>().interactable = true;
            confirmCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (confirmCanvas.GetComponent<CanvasGroup>().alpha == 1 && player.player1)
        {
            //optionsButton.GetComponent<CanvasGroup>().interactable = true;
            optionCanvas.GetComponent<CanvasGroup>().alpha = 0;
            confirmCanvas.GetComponent<CanvasGroup>().alpha = 0;
            confirmCanvas.GetComponent<CanvasGroup>().interactable = false;
            confirmCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    void surrenderOn()
    {
        //toggleSurrender();//get rid of buttons
        surrenderConfirm();//hide the opened canvases
        Debug.Log("End Game");  
        this.player.isWon=false;
        //handler = new EndGame(this, player);
        //handler.affect();

        // Call quitmatch protocol -- notify oponent that player is quitting
        // return player to lobby
        GameManager.protocols.sendQuitMatch(player.playerID);
        removeAfterDelay = true;
    }
    
}
}
