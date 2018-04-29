using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace CW
{
    public class BattlePlayer : MonoBehaviour
    {
        
        //CLASS VARIABLES
        public ArrayList deck;
        public ArrayList hand, cardsInPlay, GraveYard, treeID;
        public Vector3 DeckPos, handPos, FieldPos, TreePos;
        public AbstractCard clickedCard, targetCard;
        public bool player1, isReady, isActive, hasDeck, isGameOver = false, isWon = true;
        public int cardID;
        public int count = 0, currentMana, maxMana;
        private int showTurn = -1;
        private ProtocolManager protocols;
        private GameObject manaObj, gameOver, manaBarr, manaFaded;
        private DeckData deckData;
        public int playerID;
        public int matchID;
        public string playerName;
        public bool handCentered = false;
        public bool playerFrozen=false;
        public ProtocolManager getProtocolManager ()
        {
            return protocols;
        }

        float showWeatherEffect=CW.Constants.ANIMATE_RATE;

        public Transform weathereffect;


        //Initializes the player variables
        public void init (bool player1)
        {
            // get protocolManager from GameManager
            protocols = GameManager.protocols;
            
            deck = new ArrayList ();
            hand = new ArrayList ();
            GraveYard = new ArrayList ();
            cardsInPlay = new ArrayList ();
            treeID = new ArrayList ();
            
            //Set's player's coordinates of interest for p1 and p2
            setPlayerNum (player1);
            
            //Creates the player's tree and mana displayer
            createTree ();
            createMana ();
        }
        
        public GameObject instantiateCard ()
        {
            return (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/Card"));
        }
        
        //Sets the starting mana for each player as well
        public void setPlayerNum (bool isPlayer1)
        {
        
            //Boolean so client can know which is the player and the Ghost Player
            //Ghost Player = player over the net
            this.player1 = isPlayer1;
        
            //PLAYER 1 COORDINATES OF INTEREST
            if (player1) {
                DeckPos = new Vector3 (900, 10, -150);//orig(825, 10, -400)
                handPos = new Vector3 (550, 10, -375);//orig(150, 10, -375)
                FieldPos = new Vector3 (-450, 10, -150);
                TreePos = new Vector3 (0, 10, -375);//org(-800,10, -300)
                
                
                //Player 1 takes first turn every time
                currentMana = 1;
                maxMana = 1;
                isActive = true;
            
                //PLAYER 2 COORDINATES OF INTEREST
            } else {

                Debug.Log ("Set PLayer Num Working");
                handPos = new Vector3 (-460, 10, 375);
                FieldPos = new Vector3 (-450, 10, 150);
                DeckPos = new Vector3 (-825, 10, 400);//orig(-825, 10, 400)
                TreePos = new Vector3 (0, 10, 375);//org(800, 10, 300)
            
                //Mana and sets p2's inactive to false
                currentMana = 1;
                maxMana = 1;
                isActive = false;

            }
        }
        bool isRaining=false,isFiring=false,isFreezing=false; // only used for animation effects
        public void applyWeather(int card_id, bool currentPlayer){

            AbstractCard card = null;
            AudioSource audioSource = null;
            //by Pedro
            if (currentPlayer) {
                card = ((GameObject)hand [0]).GetComponent<AbstractCard> ();
                audioSource = card.GetComponent<AudioSource> ();
            }

            switch (card_id) {
                
                //fire
            case 89:

                if(cardsInPlay.Count>0)
                {
                    isFiring=true;
                    //by Pedro
                    if (currentPlayer && audioSource!=null) {
                        audioSource.loop = false;
                        audioSource.clip = Resources.Load ("Sounds/fire") as AudioClip;
                        //audioSource.PlayDelayed (1);
                        audioSource.Play ();
                        
                        //stop rain
                        weathereffect = GameObject.Find("WeatherEffect").transform.Find("rain");
                        weathereffect.gameObject.SetActive(false);

                        showWeatherEffect = CW.Constants.ANIMATE_RATE;
                    }
                }    
                
                for(int i = 0; i < cardsInPlay.Count; i++){
                    //by Pedro
                    card = ((GameObject)cardsInPlay [i]).GetComponent<AbstractCard> ();
                    card.Remove();
                }
                break;
                
                //freeze
            case 90:
                if(currentPlayer && audioSource!=null)
                {
                    audioSource.loop = false;
                    playerFrozen=true;// used to show frozen text
                    isFreezing=true;
                    //by Pedro
                    audioSource.clip = Resources.Load ("Sounds/freeze") as AudioClip;
                    //audioSource.PlayDelayed (1);
                    audioSource.Play ();
                    audioSource.loop = true;
                    
                    //stop rain
                    weathereffect = GameObject.Find("WeatherEffect").transform.Find("rain");
                    weathereffect.gameObject.SetActive(false);

                    showWeatherEffect=CW.Constants.ANIMATE_RATE;
                }

                for(int i = 0; i < cardsInPlay.Count; i++){
                    //by Pedro
                    card = ((GameObject)cardsInPlay [i]).GetComponent<AbstractCard> ();
                    card.freeze();
                }
                break;
                
                //rain
            case 91:
                if (currentPlayer && audioSource!=null) {
                    isRaining = true;
                    //by Pedro
                    audioSource.loop = false;
                    audioSource.clip = Resources.Load ("Sounds/rain") as AudioClip;
                    //audioSource.PlayDelayed (1);
                    audioSource.Play ();
                    audioSource.loop = true;

                    //start rain
                    weathereffect = GameObject.Find("WeatherEffect").transform.Find("rain");
                    weathereffect.gameObject.SetActive(true);

                    showWeatherEffect = CW.Constants.ANIMATE_RATE;
                    givePlayerFoodCard (2);
                }
                else
                    dealDummyCard(2);
                break;
            }
        }

        /*public void applyWeather(int card_id, bool currentPlayer){
            
            switch (card_id) {
                
                //fire
            case 89:
                    if(cardsInPlay.Count>0)
                    {
                        isFiring=true;
                        showWeatherEffect=CW.Constants.ANIMATE_RATE;
                    }
                
                for(int i = 0; i < cardsInPlay.Count; i++){
                    AbstractCard card = ((GameObject)cardsInPlay [i]).GetComponent<AbstractCard> ();
                    card.Remove();
                }
                break;
                
                //freeze
            case 90:
                if(cardsInPlay.Count>0)
                {
                    playerFrozen=true;// used to show frozen text
                    isFreezing=true;
                    showWeatherEffect=CW.Constants.ANIMATE_RATE;
                }

                for(int i = 0; i < cardsInPlay.Count; i++){
                    AbstractCard card = ((GameObject)cardsInPlay [i]).GetComponent<AbstractCard> ();
                    card.freeze();
                }
                break;
                
                //rain
            case 91:
                isRaining=true;
                showWeatherEffect=CW.Constants.ANIMATE_RATE;
                if(currentPlayer)
                    givePlayerFoodCard(2);
                else
                    dealDummyCard(2);
                break;
            }
        }*/
         

        public void givePlayerFoodCard(int num)
        {
            
            for (int i = 0; i < num; i++)
            {
                GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Battle/Card"));
        
                //Card back for deck
                //GameObject cardBacks = (GameObject) Instantiate(Resources.Load("Prefabs/Battle/card_old"));
        
                //Card front for hand
                //obj.AddComponent("AbstractCard");
                AbstractCard script = obj.GetComponent<AbstractCard>();

                //public void init(BattlePlayer player, int cardID, int diet, int level, int attack,
                //int health,string species_name, string type, string description
                script.init(this, 92, "f", 1, 0, 
                    0, "Trees and Shrubs", "Plant", "Special");
                script.handler = new InHand (script, this);
                //if(hand.Count<7)
                    hand.Add(obj);
            }
            
            positionNewCard ();
        }
        //Instantiates the Tree with Tree script
        public void createTree ()
        {
            GameObject obj = (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/Tree"));
            obj.AddComponent <Trees>();
            Trees script = obj.GetComponent<Trees> ();
            script.init (this);
            treeID.Add (obj);
        }
        
        
        //Creates a visual for the text that displays how much mana a player has
        private void createMana ()
        {
        
            //Instantiates the Graphic for the Mana and sets it's position
            //manaObj = (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/Mana"));
            manaFaded = (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/ManaFaded", typeof(GameObject)));
            manaBarr = (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/ManaBarr", typeof(GameObject)));
            if (player1) {
                //manaObj.transform.position = new Vector3 (TreePos.x - 800, TreePos.y, TreePos.z);//org(tree +200, y, z)
                //manaBarr.transform.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3 (-940,-520,0);
                //manaFaded.transform.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3 (-940,-520,0);
            } else {
                //manaObj.transform.position = new Vector3 (TreePos.x + 800, TreePos.y, TreePos.z);//org(tree -200, y, z)
                //manaBarr.transform.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3 (860,440,0);
                //manaFaded.transform.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3 (860,440,0);
                manaFaded.transform.Find("Image").GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
                manaFaded.transform.Find("Image").GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                manaFaded.transform.Find("Image").GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-20, -115, 0);
                
                manaBarr.transform.Find("Image").GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
                manaBarr.transform.Find("Image").GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                manaBarr.transform.Find("Image").GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-20, -115, 0);

                manaBarr.transform.Find("Image").GetComponent<RectTransform>().localRotation = new Quaternion(0,1,0,0);
                manaFaded.transform.Find("Image").GetComponent<RectTransform>().localRotation = new Quaternion(0,1,0,0);
            }
            /*
            if (player1) {
                manaBarr.transform.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3 (-940,-520,0);
                manaFaded.transform.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3 (-940,-720,0);
            } else {
                manaBarr.transform.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3 (860,440,0);
                manaFaded.transform.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3 (860,440,0);
                manaBarr.transform.Find("Image").GetComponent<RectTransform>().localRotation = new Quaternion(0,1,0,0);
                manaFaded.transform.Find("Image").GetComponent<RectTransform>().localRotation = new Quaternion(0,1,0,0);
            }*/
        }
        
        public void applyFoodBuff(AbstractCard target, int deltaAttack, int deltaHealth){
            
            target.applyFood (target, deltaAttack, deltaHealth);
        }
        
        //Instantiate's the GameOver button 
        public void createGameover ()
        {
            int gold = 100; //100 gold if won
            isGameOver = true;
            Debug.Log ("Battleplayer game_over");
            
            gameOver = (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/GameOver"));
            if (!isWon) {
                Debug.Log("lost the game");
                gold = 25;//25 gold if lost
                Texture2D loseTexture = (Texture2D)Resources.Load ("Prefabs/Battle/lose", typeof(Texture2D));
                gameOver.GetComponent<Renderer>().material.mainTexture = loseTexture;
            } else {
                Debug.Log("won the game");
                Texture2D winTexture = (Texture2D)Resources.Load ("Prefabs/Battle/win", typeof(Texture2D));
                gameOver.GetComponent<Renderer>().material.mainTexture = winTexture;
            }
            //gameOver.transform.Find ("GameOverText").GetComponent<TextMesh> ().text = "You've been awarded " + gold + " gold";
            //gameOver.transform.position = new Vector3 (0, 30, 0);
            // return player to lobby
            int wonGame = isWon ? 1 : 0;
            if (playerID == 0) {
                playerID = GameManager.player1.playerID;
            }
            GameManager.protocols.sendMatchOver (playerID, wonGame);
        }
        
        //Method Instantiates the cards with AbstractCard scripts from the deckData
        //passed in from server and adds the new card to the deck arraylist
        public void createDeck ()
        {       
            //For loop for every card in deck passed in from server
            for (int i = 0; i < deckData.getSize(); i++) {
                
                //GameObject instantiated for Card
                GameObject obj = (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/Card"));
                
                //Card back for deck
                //GameObject cardBacks = (GameObject) Instantiate(Resources.Load("Prefabs/Battle/card_old"));
                
                //Card front for hand
                //obj.AddComponent("AbstractCard");
                AbstractCard script = obj.GetComponent<AbstractCard> ();
                
                //CardData contains all the variables for an indivdual card
                CardData tempCard = deckData.popCard ();
                //public void init(BattlePlayer player, int cardID, int diet, int level, int attack,
                //int health,string species_name, string type, string description
                script.init (this, tempCard.cardID, tempCard.dietType, tempCard.level, tempCard.attack, 
                             tempCard.health, tempCard.speciesName, "Large Animal", tempCard.description);
                
                //Add the card to the deck arraylist
                deck.Add (obj);
                
            }
            
            //Makes the deck 
            GameObject DeckTop = (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/CardBack"));
            DeckTop.transform.position = new Vector3 (DeckPos.x + 2000, DeckPos.y, DeckPos.z);
            
            
        }
        
        
        
        //Deals cards to player's hand a
        public void dealCard (int numToDeal)
        {
            
            //Deal as many cards as required
            for (int i = 0; i < numToDeal; i++) {
                
                //Max amount of cards reached
                if (hand.Count >= 7)
                    return;
                
                //Card taken from deck, and is given the logic from
                GameObject p1card = (GameObject)deck [0];           
                AbstractCard script = p1card.GetComponent<AbstractCard> ();
                script.handler = new InHand (script, this);
                
                //Position the newly dealt card
                p1card.transform.position = new Vector3 ((handPos.x + 280) - 165 * hand.Count, handPos.y, handPos.z);
                
                //Remove from  deck and add to hand
                deck.Remove (p1card);
                hand.Add (p1card);
                // Send position = hand.Count to Server
                
                // Hack PlayerID = 0 means its player2
                if (playerID != 0) {
                    //protocols.sendDealCard(playerID, hand.Count);
                    
                }
            }
            positionNewCard();

        }
        void positionNewCard()
        {
            //For every object in hand arraylist
            for (int i = 0; i < hand.Count; i++) {
                
                //Reposition and arrange each card in hand
                GameObject setCard = (GameObject)hand [i];
                setCard.transform.position = new Vector3 ((handPos.x + 280) - 165 * i, handPos.y, handPos.z);
                
            }
        }
        public void dealDummyCard (int numToDeal)
        {
            // TODO
            //Deal as many cards as required
            for (int i = 0; i < numToDeal; i++) {
                
                //Max amount of cards reached
                if (hand.Count >= 7)
                    return;
                
                //Card taken from deck, and is given the logic from
                GameObject p2card = (GameObject)Instantiate (Resources.Load ("Prefabs/Battle/CardBack"));
                p2card.transform.position = new Vector3 ((handPos.x + 280) - 165 * hand.Count, handPos.y, handPos.z);
                //Position the newly dealt card
                //p2card.transform.position = new Vector3((handPos.x + 280) - 185 * hand.Count, 10, handPos.z);
                
                //Remove from  deck and add to hand
                //deck.Remove (p2card);
                hand.Add (p2card);
                // Send position = hand.Count to Server
                
                // Hack
                //if(playerID != 0){
                //  protocols.sendDealCard(playerID, hand.Count);
                
                //}
            }
            
            //For every object in hand arraylist
            for (int i = 0; i < hand.Count; i++) {
                
                //Reposition and arrange each card in hand
                GameObject setCard = (GameObject)hand [i];
                if(setCard != null)
                    setCard.transform.position = new Vector3 ((handPos.x + 280) - 165 * i, handPos.y, handPos.z);
                
            }
        }
        
        public void attackWith (int attackerIndex, int attackedIndex)
        {
            GameObject attackerObj = (GameObject)GameManager.player2.cardsInPlay [attackerIndex];
            AbstractCard attackerCard = attackerObj.GetComponent<AbstractCard> ();
            bool damageBack = false;
            
            GameObject attackedObj = (GameObject)GameManager.player1.cardsInPlay [attackedIndex];
            AbstractCard attackedCard = attackedObj.GetComponent<AbstractCard> ();
            if (attackedCard.diet != AbstractCard.DIET.HERBIVORE) {
                damageBack = true;
                attackerCard.receiveAttack (attackedCard.dmg);
            }
            attackerCard.attack (attackerCard, attackedCard, damageBack);
        }
        
        public void attackTree (int attackerIndex)
        {
            GameObject attackerObj = (GameObject)GameManager.player2.cardsInPlay [attackerIndex];
            AbstractCard attackerCard = attackerObj.GetComponent<AbstractCard> ();
            
            GameObject attackedObj = (GameObject)GameManager.player1.treeID [0];
            Trees tree = attackedObj.GetComponent<Trees> ();
            
            attackerCard.attackTree (tree);
        }
        
        //Called by GameManager when a card is removed from play
        //to keep all the cards neatly arranged preventing
        //cards from stacking on top of each other
        public void reposition ()
        {
            
            //Position each card in play correctly 
            for (int i = 0; i < hand.Count; i++) {
                
                //Retrieves the card from the array of cards in play
                GameObject obj = (GameObject)hand [i];
                if(obj != null)
                    obj.transform.position = new Vector3 ((handPos.x + 280) - 165 * i, handPos.y, handPos.z);
            }
            
            //Position each card in play correctly 
            for (int i = 0; i < cardsInPlay.Count; i++) {
                
                //Retrieves the card from the array of cards in play
                GameObject obj = (GameObject)cardsInPlay [i];
                if(obj != null){
                    obj.transform.position = new Vector3 (FieldPos.x + 185 * i, FieldPos.y, FieldPos.z);
                    
                    AbstractCard card = obj.GetComponent<AbstractCard> ();
                    card.fieldIndex = i;
                }
            }
        }
        
        // Added to update player 2 mana only 
        public void addMana ()
        {
            //Increment the max mana
            if (maxMana < 9)
                maxMana++;
            
            //Restore full mana
            currentMana = maxMana;
        }
        
        
        // Deal new card for layer
        public void startTurn ()
        {
            showTurn = 120;
            if (hand.Count != 5) {
                dealCard (1);
                
                //TODO:  Does the player really lose if he gets to 1 card?
                if (deck.Count == 0) {
                    Debug.Log ("Player loses");
                    createGameover ();
                }
            }
            
            //Increment the max mana
            if (maxMana < 9)
                maxMana++;
            
            //Restore full mana
            currentMana = maxMana;
            checkHandGlow();
        }
        
        
        // resets Cards 
        public void endTurn ()
        {
            //DebugConsole.Log("end turn in battle player");
            showTurn = 120;
            playerFrozen=false;
            GameManager.player2.playerFrozen=false;
            //Debug.Log ("endTurn called");
            for (int i = 0; i < cardsInPlay.Count; i++) {
                //Gets the AbstractCard component
                AbstractCard cardInPlay = ((GameObject)cardsInPlay [i]).GetComponent<AbstractCard> ();
                cardInPlay.endTurn ();
            }
        }
        
        //Resets the players active cards in field  
        public void resetCards ()
        {
            for (int i = 0; i < cardsInPlay.Count; i++) {
                //Gets the AbstractCard component
                AbstractCard cardInPlay = ((GameObject)cardsInPlay [i]).GetComponent<AbstractCard> ();
                cardInPlay.endTurn ();
            }
        }
        
        private float manaAnimate = 1.0f;
        private float manaCount;
        // Update is called once per frame
        void Update ()
        {
            showTurn--;
            Texture2D manaTexture = (Texture2D)Resources.Load ("Images/Battle/mana" + (int)manaAnimate, typeof(Texture2D));
            //Constantly sets the text for mana
            //manaObj.transform.Find ("ManaText").GetComponent<TextMesh> ().text = currentMana + " / " + maxMana;
            //manaObj.transform.GetComponent<MeshRenderer> ().material.mainTexture = manaTexture;
            manaAnimate += 0.05f;
            if (manaAnimate > 4.9) {
                manaAnimate = 1.0f;
            }
            if (currentMana == 0)
            {
                manaCount = 0.0f;
            }
            else {
                manaCount = (float)currentMana / 9f;
            }
            manaBarr.transform.Find("Image").GetComponent<Image>().fillAmount = manaCount;
            manaFaded.transform.Find ("Image").GetComponent<Image> ().fillAmount = (float)maxMana / 9f;
        }
        
        
        //When this method's called, Server has just sent the deck information
        //needed to instantiate cards, so we instantiate cards
        public void setDeck (DeckData deckData)
        {
            this.deckData = deckData; 
            
            //Takes the deck data and instatiates card GameObjects with AbstractCard scripts
            createDeck ();
        }
        
        void OnGUI()
        {
            if (showTurn > 0 && isActive)
            { //Shows active player
                GUI.skin.box.fontStyle = FontStyle.Bold;
                GUI.skin.box.fontSize = 20;
                GUI.Box(new Rect((Screen.width / 2.0f) - 100, (Screen.height / 2.0f) - 50, 250, 50), playerName + "'s Turn");
            }
            if (isGameOver)
            {
                if (GUI.Button(new Rect((Screen.width / 2.0f) - ((Screen.width / 12.8f) / 100 * 150) / 2.0f, //left
                            ((Screen.height * 2.0f) / 3.0f), //height
                            (Screen.width / 12.8f) / 100 * 150, 
                            (Screen.width / 12.8f) / 100 * 40), "Back to Lobby"))
                {
                    //GameManager.protocols.sendReturnToLobby();
                    Game.SwitchScene("World");
                }
            }
            if (showWeatherEffect <= 0)
            {
                isRaining = false;
                isFiring=false;
                isFreezing=false;
            }
            else if(showWeatherEffect>0 && (isRaining || isFiring || isFreezing))
            {
                
                Texture effectObj=null,effectTextObj=null;
                if(isRaining)
                {
                    System.Threading.Thread.Sleep(10);
                    effectObj = (Texture)Instantiate (Resources.Load ("Images/Battle/raindrop"));   
                    effectTextObj = (Texture)Instantiate (Resources.Load ("Images/Battle/raintext"));
                }
                else if(isFiring)
                {
                    System.Threading.Thread.Sleep(40);
                    effectObj = (Texture)Instantiate (Resources.Load ("Images/Battle/firedrop"));   
                    effectTextObj = (Texture)Instantiate (Resources.Load ("Images/Battle/firetext"));
                }
                else if(isFreezing)
                {
                    System.Threading.Thread.Sleep(30);
                    effectObj = (Texture)Instantiate (Resources.Load ("Images/Battle/freezedrop"));   
                    effectTextObj = (Texture)Instantiate (Resources.Load ("Images/Battle/freezetext"));
                }
                if(effectObj!=null)
                {

                    for(int i=0;i<5;i++)
                    {
                        float start=Random.Range (10.0f, Screen.width-effectObj.width);
                        float end=Random.Range (10.0f, Screen.height-effectObj.height);
                        float wid=effectObj.width;
                        float hei=effectObj.height;
                        var rect=new Rect(start,end,wid,hei);
                        GUI.DrawTexture(rect, effectObj);

                    } 
                    float start1=Screen.width/2-effectTextObj.width/2;
                    float end1=Screen.height/2-effectTextObj.height/2;
                    GUI.DrawTexture(new Rect(start1,end1,effectTextObj.width,effectTextObj.height), effectTextObj);
                    showWeatherEffect-=0.5f;
                }

            }
        }

        public void checkHandGlow()
        {
            if (hand.Count > 0)
            {
                GameObject currentCard;
                for (int i = 0; i < hand.Count; i++)
                {
                    currentCard = ((GameObject)hand[i]);
                    if(currentCard.GetComponent<AbstractCard>().manaCost <= currentMana)
                    {
                        ((Behaviour)currentCard.GetComponent("Halo")).enabled = true;
                    } else
                    {
                        ((Behaviour)currentCard.GetComponent("Halo")).enabled = false;
                    }
                }
            }
        }

        public void cardsInPlayGlow()
        {
            if (cardsInPlay.Count > 0)
            {
                GameObject currentCard;
                for (int i = 0; i < cardsInPlay.Count; i++)
                {
                    currentCard = ((GameObject)cardsInPlay[i]);
                    ((Behaviour)currentCard.GetComponent("Halo")).enabled = true;
                }
            }
        }
    }
}