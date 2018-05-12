using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace CW
{
	public class AbstractCard : MonoBehaviour
	{
		public int cardID, fieldIndex;
        public int maxHP, hp, dmg, naturalDmg, manaCost, level, dmgTimer = 0, frozenTurns = 0;
		private Font font;
		private BattlePlayer player;
		public string name, type = " ", description = " ", dietChar= " ";
		public DIET diet;
        private bool canAttackNow, inMotion, moveBack;
		private Vector3 oriPosition;
		private Vector3 newPosition;
		private bool zoomed = false;
        private bool clicked = false, removeAfterDelay;
        public bool frozen = false;
        private bool receivedmg = false;
		//VELOCITY
		private Vector3 targetPosition, startPosition;
		private float velocity, terminalVelocity, angle, distance;
		private float delayTimer, DELAY_CONSTANT = 3.5f;
        //Enum for Animal Type
        private ParticleSystem ground;
        private ParticleSystem dead;
		public enum DIET
		{
			OMNIVORE,
			CARNIVORE,
			HERBIVORE,
			WEATHER,
			FOOD
		}
        //byPedro
        private AudioSource audioSource;
		public AbstractCardHandler handler;
        private float raised = 50f;
        public bool isInHand = true;
        public bool isInPlay = false;
        public Vector3 temp;

        //mouse hover
        private bool _mouseOver = false;

        //shaking
        private bool shake = true;

        //Summon effects
        public bool effect = false;
        public float  z = 0.1f;

        //Die Effects
        public bool deffect = false;


        //Prey Predator Button
        Button pbtn;

        //x button
        Button xbtn;

        //Initialization for a card and sets it's position to (1000, 1000, 1000)
        public void init(BattlePlayer player, int cardID, string diet, int level, int attack, int health, string species_name, string type, string description)
        {
            this.player = player;
            this.cardID = cardID;
            this.manaCost = level;
            this.transform.position = new Vector3(player.DeckPos.x, player.DeckPos.y, player.DeckPos.z);
            canAttackNow = true;
            velocity = 0;
            terminalVelocity = 6000;
            distance = 0;
            delayTimer = 0;
            name = species_name;
            this.diet = getDietType(diet);
            this.dietChar = diet;
            this.level = level;
            maxHP = hp = health;
            type = this.type;
            naturalDmg = dmg = attack;
            //this.type = type; //hide temporarily
            this.description = description; //hide temporarily

            Texture2D cardTexture;
            Texture2D speciesTexture;
            //o-omnivore, c-carnivore, h-herbivore, f-food, w-weather
            if (!this.dietChar.Equals("w"))
            {
                cardTexture = (Texture2D)Resources.Load("Images/Battle/cardfront_" + this.dietChar, typeof(Texture2D));
                speciesTexture = (Texture2D)Resources.Load("Images/" + this.name, typeof(Texture2D));
            }
            else // for weather, its blue similar to food card
            {
                cardTexture = (Texture2D)Resources.Load("Images/Battle/cardfront_f", typeof(Texture2D));
                if (this.name.Equals("Acacia"))
                {
                    speciesTexture = (Texture2D)Resources.Load("Images/Battle/fire", typeof(Texture2D));
                    this.name="Fire";
                }
                else if(this.name.Equals("Big Tree"))
                {
                    speciesTexture = (Texture2D)Resources.Load("Images/Battle/freez", typeof(Texture2D));
                    this.name="Freez";
                }
                else
                {
                    speciesTexture = (Texture2D)Resources.Load("Images/Battle/rain", typeof(Texture2D));
                    this.name="Rain";
                }
                    
            }
            //Changing cardfront texture
            GetComponent<Renderer>().material.mainTexture = cardTexture;
			transform.Find ("CardArt").GetComponent<MeshRenderer> ().material.mainTexture = speciesTexture;

            Transform child = transform.Find("Canvas/Pop/SImage/Image");
            Image i = child.GetComponent<Image>();
            Sprite s = Sprite.Create(speciesTexture, new Rect(0, 0, speciesTexture.width, speciesTexture.height), Vector2.zero);
            i.sprite = s;

            Texture2D TypeTexture;
            if (diet == "o")
            {
                TypeTexture = (Texture2D)Resources.Load("Images/Battle/omnivore", typeof(Texture2D));
            } else if (diet == "c")
            {
                TypeTexture = (Texture2D)Resources.Load("Images/Battle/carnivore", typeof(Texture2D));
            }
            else if (diet == "h")
            {
                TypeTexture = (Texture2D)Resources.Load("Images/Battle/herbivore", typeof(Texture2D));
            }
            else if (diet == "f")
            {
                TypeTexture = (Texture2D)Resources.Load("Images/Battle/plant", typeof(Texture2D));
            } else
            {
                TypeTexture = (Texture2D)Resources.Load("Images/Battle/plant", typeof(Texture2D));
            }

            Transform child1 = transform.Find("Canvas/Pop/SImageType");
            
            Image j = child1.GetComponent<Image>();
            Sprite r = Sprite.Create(TypeTexture, new Rect(0, 0, TypeTexture.width, TypeTexture.height), Vector2.zero);
            j.sprite = r;
            

            //Changing card text 
            //		Color gold = new Color (209f, 234f, 50f, 255f);
            transform.Find ("NameText").GetComponent<TextMesh> ().text = TextWrap (this.name, 16);
			//transform.Find ("TypeText").GetComponent<TextMesh> ().text = this.type;
			//transform.Find ("TypeText").GetComponent<MeshRenderer> ().material.color = Color.white;
			//transform.Find ("DescriptionText").GetComponent<TextMesh> ().text = TextWrap (this.description, 26);
			//transform.Find ("DescriptionText").GetComponent<MeshRenderer> ().material.color = Color.white;
			transform.Find ("LevelText").GetComponent<TextMesh> ().text = "" + this.level;
			transform.Find ("LevelText").GetComponent<MeshRenderer> ().material.color = Color.white;
			transform.Find ("DoneText").GetComponent<MeshRenderer> ().material.color = Color.red;
			transform.Find ("DamageText").GetComponent<TextMesh> ().text = "";
			transform.Find ("DamageText").GetComponent<MeshRenderer> ().material.color = Color.red;
            ground = transform.Find("Basic_aura/Ground_pulse").GetComponent<ParticleSystem>();
            ground.Stop();
            //dead = transform.Find("Basic_pop/DeathPop").GetComponent<ParticleSystem>();
            //dead.Stop();


            //Initializes off screen
            transform.position = new Vector3 (9999, 9999, 9999);

			//rotate facedown if player 2
			if (!player.player1 && !Constants.SINGLE_PLAYER) {
				transform.rotation = new Quaternion (180, 0, 0, 0); 
			}

            //by Pedro
            audioSource = gameObject.AddComponent<AudioSource> ();

            //PreyPreadatorButton
            pbtn = transform.Find("Canvas/Pop/pbutton").GetComponent<Button>();
            pbtn.onClick.AddListener(pppout);

            //closebutton
            xbtn = transform.Find("Canvas/Pop/xbutton").GetComponent<Button>();
            xbtn.onClick.AddListener(closebutton);

        }

        //Returns the enum for the animal's diet. Herbivore, Omnivore, Carnivore
        DIET getDietType (string diet)
		{
			if (diet == "o") {
                //added
                type = "Omnivore";
				return DIET.OMNIVORE;	
			} else if (diet == "c") {
                type = "Carnivore";
                return DIET.CARNIVORE;	
			} else if (diet == "h") {
                type = "Herbivore";
                return DIET.HERBIVORE;
			} else if (diet == "f") {
                type = "food";
                return DIET.FOOD;
			} else 
                type = "weather";
				return DIET.WEATHER;
			//else diet == 2
		}
	
        //OnMouseDown also checks for touch events
        void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //DebugConsole.Log("on mouse down");
                /*if (inMotion)
                {

                    
                    return;
                }    
                //keeping original position for Zoom
                if (!zoomed)
                {
                    oriPosition = this.transform.position;
                    zoomed = true;  

                }
            
                newPosition = oriPosition;*/
            
                //this.transform.localScale = new Vector3 (21, 2, 29); //About 1.4x size
                //this.transform.position = new Vector3(newPosition.x, 50, newPosition.z);

            
                //if left-button clicked, set centered boolean true and move handpos
                if (Input.GetMouseButtonDown(0) && !player.handCentered && player == player.player1 && isInHand)
                {
                    //DebugConsole.Log("1st");
                    //newPosition.z = oriPosition.z + 200;
                    //this.transform.position = new Vector3(newPosition.x, 10.5f, newPosition.z);
                    clicked = true;
                    player.handCentered = true;
                    player.handPos = new Vector3(50, 400, -125);
                    player.reposition();
                    
                }//check if it is centered then do handler action
                else if (Input.GetMouseButtonDown(0) && player.handCentered && player == player.player1 && isInHand)
                {
                    //DebugConsole.Log("2nd");
                    player.handCentered = false;
                    player.handPos = new Vector3(550, 10, -375);
                    player.reposition();
                    if (handler != null)
                    {
                        handler.clicked();
                    }
                    else
                    {
                        //DebugConsole.Log("handler is null");
                        _mouseOver = false;
                    }
                }

                else if (!isInHand)
                {
                    //DebugConsole.Log("last");
                    if (handler != null)
                    {
                        handler.clicked();
                    }
                }

                
                


                //if right-click is held down
                /*if (Input.GetMouseButton(1))
                { 
                    /*if (player.player1) { //player 1
                        newPosition.z = oriPosition.z + 200; //Move up from bottom of screen
                    } else if (!player.player1) { //player 2
                        newPosition.z = oriPosition.z - 200; //Move down from top of screen
                    }
                    this.transform.position = newPosition;*//*
                    this.transform.localScale = new Vector3(45, 10, 63); //3x size
                    this.transform.position = new Vector3(newPosition.x, 50, newPosition.z + 200);
                }

                //if right-click is up
                //echan
                if (Input.GetMouseButtonUp(1))
                { 
                    /*if (player.player1) { //player 1
                        newPosition.z = oriPosition.z + 200; //Move up from bottom of screen
                    } else if (!player.player1) { //player 2
                        newPosition.z = oriPosition.z - 200; //Move down from top of screen
                    }
                    this.transform.position = newPosition;*//*
                    this.transform.localScale = new Vector3(15, 1, 21); //3x size
                    this.transform.position = newPosition;
                }*/
            
            } else
            {
                _mouseOver = false;
            }
        }

		void OnMouseExit ()
		{
			//Normal scaling
			this.transform.localScale = new Vector3 (15, 1, 21);
		
			//Moves back to normal position if not clicked
			/*if (!clicked && !inMotion) {
				this.transform.position = oriPosition;
			}*/
			zoomed = false;
			clicked = false;
            //if the mouse leave the card, set the _mouseOver false
            //_mouseOver = false;
		}

        //when mouse hover on card
        void OnMouseOver()
        {
            //AND when mouse right click the card, it would set the _mouseOver true
            if (Input.GetMouseButtonDown(1))
            {
                _mouseOver = !_mouseOver;

            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _mouseOver = false;

            }
            

        }

        
        void OnGUI()
        {
            //if _mouseOver true, then display the description panel
            if (_mouseOver)
            {
                transform.Find("opening").GetComponent<TextMesh>().text = "Opening";
                if (type != "weather")
                {
                    transform.Find("Canvas/Pop").gameObject.SetActive(true);
                }
                //transform.Find("Pop/Image").GetComponent<Image>().sprite = sImage;
                //transform.Find("Canvas/Pop/Sname").GetComponent<TextMesh>().text = TextWrap(this.name, 16);
                //transform.Find("Canvas/Pop/Stype").GetComponent<TextMesh>().text = this.type;
                //transform.Find("Canvas/Pop/Sdescription").GetComponent<TextMesh>().text = TextWrap(this.description, 16);
                Transform child = transform.Find("Canvas/Pop/SImageName/Sname");
                Text t = child.GetComponent<Text>();
                t.text = this.name;

                child = transform.Find("Canvas/Pop/SImageDescription/Sdescription");
                t = child.GetComponent<Text>();
                t.text = "What is " + this.name + " ? "  + this.description;
               
            }
            //if _mouseOver false, then turn off the description
            else
            {
                transform.Find("opening").GetComponent<TextMesh>().text = "---";
                transform.Find("Canvas/Pop").gameObject.SetActive(false);
               

            }
        }

   

        public int getDamage ()
		{
			return this.dmg;	
		}

		public int getManaCost ()
		{
			return manaCost;	
		}

		public void setCanAttack (bool canAttackNow)
		{
            this.canAttackNow = canAttackNow;	
		}
        public void freeze(){
            
            frozen = true;

        }

        public void unfreeze(){
            
            frozen = false;

        }
		public bool canAttack ()
		{
			return canAttackNow && !frozen;	
		}
	
		public void attack(AbstractCard clicked, AbstractCard target, bool damageback)
		{
            
			calculateDirection (target.transform.position, true);

			//NetworkManager.Send (CardAttackProtocol.Prepare (GameManager.matchID, attack, fieldPosition), ProcessSummonCard);		
			target.receiveAttack (dmg);
            //by Pedro
            audioSource.clip = Resources.Load ("Sounds/attack") as AudioClip;
            //audioSource.PlayDelayed (1);
            audioSource.Play ();
			if (damageback) {

				clicked.receiveAttack (target.dmg);
			}
			canAttackNow = false;		
		}

		public void applyFood(AbstractCard target, int deltaAttack, int deltaHealth){
			target.dmg += deltaAttack;
			target.maxHP += deltaHealth;
			target.hp += deltaHealth;
		}


	
		public void attackTree (Trees tree)
		{
			tree.receiveAttack (dmg);
            //by Pedro
            audioSource.clip = Resources.Load ("Sounds/attack") as AudioClip;
            //audioSource.PlayDelayed (1);
            audioSource.Play ();
			setCanAttack (false);
			player.clickedCard = null;
			calculateDirection (tree.transform.position, true);

		
		}
	
		//Set the card so it can attack again
		public void endTurn ()
		{
            /*P1 freezes, P2 animals frozenTurns = 1
             * P1 ends turn and animals unfrozen, P2 animals check frozenTurns, still frozen
             * P2 animals frozenTurns--, frozenTurns = 0
             * P2 animals still frozen, end turn, frozen = false
             */
            
            canAttackNow = true;
            frozen=false;
            
		}
        public void Remove(){
            removeAfterDelay = true;
        }
		public void receiveAttack (int dmg)
		{
			dmgTimer = 120;
			transform.Find ("DamageText").GetComponent<TextMesh> ().text = "-" + dmg;
			hp -= dmg;
            //Debug.Log ("Was dealt " + dmg + " damage and is now at " + hp + " hp");
            
            //transform.position = transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 50;
            //transform.position = new Vector3(transform.position.x + 50 * 1.3f * Time.deltaTime * 3, transform.position.y, transform.position.z);
            //transform.position = new Vector3(transform.position.x - 100 * 1.3f * Time.deltaTime * 5, transform.position.y, transform.position.z);
            //transform.position = new Vector3(transform.position.x + 150 * 1.3f * Time.deltaTime * 3, transform.position.y, transform.position.z);
            //transform.position = new Vector3(transform.position.x - 200 * 1.3f * Time.deltaTime * 7, transform.position.y, transform.position.z);


            receivedmg = true;
            temp = transform.position;

            if (hp <= 0) {
				Debug.Log ("DEAD");
                //handler = new RemoveFromPlay (this, player);
                //handler.affect ();

                //transform.Find("CardArt").GetComponent<Renderer>().material.SetColor("_Color", new Color32(0, 0, 0, 255));
                Renderer rend = transform.Find("CardArt").GetComponent<Renderer>();
                rend.material.mainTexture = (Texture2D)Resources.Load("Images/Battle/Death1", typeof(Texture2D));

                //deffect = true;

                //when card dead
                audioSource.clip = Resources.Load("Sounds/card_destroyed") as AudioClip;
                //audioSource.PlayDelayed (1);
                audioSource.Play();
                removeAfterDelay = true;
			}
		
		}
	
		public void calculateDirection (Vector3 targetPos, bool moveBack)
		{
			this.moveBack = moveBack;
			this.startPosition = transform.position;
			inMotion = true;

			this.targetPosition = targetPos;
			float deltaX = targetPos.x - transform.position.x;
			float deltaZ = targetPos.z - transform.position.z;
			velocity = 5;
			terminalVelocity = 5500;
		

		
			angle = Mathf.Atan2 (deltaZ, deltaX);
		
	
			distance = Mathf.Sqrt (deltaX * deltaX + deltaZ * deltaZ);

		
		}
	
		bool moving ()
		{
		
			//Moves as long as it is supposed to
			if (distance > 20) {
				velocity *= 1.3f;
				if (velocity > terminalVelocity)
					velocity = terminalVelocity;
			
				float deltaX = Mathf.Cos (angle) * velocity * Time.deltaTime * 3;
				float deltaZ = Mathf.Sin (angle) * velocity * Time.deltaTime * 3;
			
			
				distance -= Mathf.Sqrt (deltaX * deltaX + deltaZ * deltaZ);
				transform.position = new Vector3 (transform.position.x + deltaX, transform.position.y, transform.position.z + deltaZ);
			
				return true;
			} else if (inMotion) {
			
				inMotion = false;
				transform.position = targetPosition;
				if (moveBack) {
					calculateDirection (startPosition, false);
					terminalVelocity = 2500;
					velocity = 50;

                }
			}
		
			return false;
		}
	
		void Update ()
		{
            if (removeAfterDelay) {
                cardremove();
			}

            
            if (receivedmg)
            {
                cardshake();
            }

            if(effect)
            {
                //when card receive attack
                audioSource.clip = Resources.Load("Sounds/card_placed") as AudioClip;
                //audioSource.PlayDelayed (1);
                audioSource.Play();
                cardsummon();
            }

            //if(deffect)
           // {
            //    carddead();
           // }


			//Change text on card
			transform.Find ("AttackText").GetComponent<TextMesh> ().text = dmg.ToString ();
			transform.Find ("HealthText").GetComponent<TextMesh> ().text = hp.ToString ();
			if (hp < maxHP) {
				transform.Find ("HealthText").GetComponent<MeshRenderer> ().material.color = Color.red;
			} else if (hp > maxHP) {
				transform.Find ("HealthText").GetComponent<MeshRenderer> ().material.color = Color.green;
			} else if (hp == maxHP) {
				transform.Find ("HealthText").GetComponent<MeshRenderer> ().material.color = Color.white;
			}
			if (dmg < naturalDmg) {
				transform.Find ("AttackText").GetComponent<MeshRenderer> ().material.color = Color.red;
			} else if (dmg > naturalDmg) {
				transform.Find ("AttackText").GetComponent<MeshRenderer> ().material.color = Color.green;
			} else if (dmg == naturalDmg) {
				transform.Find ("AttackText").GetComponent<MeshRenderer> ().material.color = Color.white;
			}
            if (canAttack()) {
                transform.Find ("DoneText").GetComponent<TextMesh> ().text = "";
            } 
            else if (!canAttackNow) {
                ((Behaviour)GetComponent("Halo")).enabled = false;
                transform.Find ("DoneText").GetComponent<TextMesh> ().text = "Done";
            }
            if(player.currentMana < getManaCost ())
            {
                ((Behaviour)GetComponent("Halo")).enabled = false;
            }
            if (frozen) {
                ((Behaviour)GetComponent("Halo")).enabled = false;
                transform.Find ("DoneText").GetComponent<TextMesh> ().text = "Frozen";
            }
			//If damaged
			if (dmgTimer > 0) {
				dmgTimer--;
			} else {
				transform.Find ("DamageText").GetComponent<TextMesh> ().text = "";
			}
			//Moving
			moving ();

            /*if(Input.GetMouseButtonDown(0) && player.handCentered)
                {
                    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                    RaycastHit hit;
                    //player.reposition();
                    if(Physics.Raycast(ray, out hit))
                    {

                        Debug.Log(hit.transform.gameObject.name);
                        if(hit.transform.gameObject.name != "Card(Clone)" && hit.transform.gameObject.name != "Cardback(Clone)")
                        {
                            Debug.Log("Plane Clicked");
                            player.handCentered = false;
                            player.handPos = new Vector3(550, 10, -375);
                            player.reposition();

                        }
                    }
                }*/
		}


        public void cardshake()
        {
            //Debug.Log(transform.position.x);
            float d = 10f;

            if (shake)
            {
                //Debug.Log("R : " + transform.position.x);
                transform.position = new Vector3(transform.position.x + d, transform.position.y, transform.position.z);
                shake = false;

            }
            else
            {

                //Debug.Log("L : " + transform.position.x);
                transform.position = new Vector3(transform.position.x - d, transform.position.y, transform.position.z);
                shake = true;
            }

            d = d * 10f;

            delayTimer += Time.deltaTime;
            if (delayTimer > 1f)
            {
                receivedmg = false;
                transform.position = temp;
            }
        }

        public void cardsummon()
        {
            delayTimer += Time.deltaTime;

            if (!ground.isPlaying)
            {
                ground.Play();
            }

            if (delayTimer > 0.5f)
            {
                effect = false;
                ground.Stop();
            }
        }

        public void carddead()
        {
            delayTimer += Time.deltaTime;

            if (!dead.isPlaying)
            {
                dead.Play();
            }

            if (delayTimer > 0.5f)
            {
                deffect = false;
                dead.Stop();
            }
        }

        public void cardremove()
        {
            delayTimer += Time.deltaTime;
            if (delayTimer > DELAY_CONSTANT)
            {
                handler = new RemoveFromPlay(this, player);
                handler.affect();
                removeAfterDelay = false;
                delayTimer = 0;
            }
            //Debug.Log (Time.deltaTime);
        }

        void pppout()
        {

            Database foodWeb = null;
            ConvergeManager manager = new ConvergeManager();
            

            if (foodWeb == null)
            {
                foodWeb = Database.NewDatabase(
                    GameObject.Find("Global Object"),
                    Constants.MODE_FOODWEB,
                    manager
                );
            }
            else
            {
                foodWeb.manager = manager;
            }

            foodWeb.SetActive(true, this.name);

        }

        //close description window
        void closebutton()
        {
            //xbutton
            _mouseOver = false;
        }


        //For wrapping long text
        public static string TextWrap (string originaltext, int chars_in_line)
		{
			string output = "";
			string[] words = originaltext.Split (' ');
			int line = 0;
			int numWords = 0;
			foreach (string word in words) {
				if ((line + word.Length + 1) <= chars_in_line) {
					output += " " + word;
					line += word.Length + 1;
				} else { 
					output += " \n" + word;
					line = word.Length;
				}
				if (++numWords == 20) {
					output += "...";
					break;
				}
			}

			return output;
		}
	}
}
