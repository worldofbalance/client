using UnityEngine;
using System.Text;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

public class View : MonoBehaviour {

	private int window_id = Constants.VIEW_WIN;
	// Window Properties
	private float left;
	private float top;
	private float width;
	private float height;
	private bool isActive = true;
	// Other
	private Rect windowRect;
	public List<Card> cardList = new List<Card>();
	public List<List<Card>> predatorList = new List<List<Card>>();
	public List<List<Card>> preyList = new List<List<Card>>();
	private List<Card> mouseOverList = new List<Card>();
	private Card lastCardToDraw;
	private Texture2D selectTexture;
	private Font font;
	private bool isExpanded = false;
	private bool isReady = false;
	private int mode = Constants.MODE_SHOP;
	private ConvergeManager manager;

	//Factory method for creating new View component with specified mode and manager
	public static View NewView (GameObject gameObject, int mode, ConvergeManager manager) {
		View thisObj = gameObject.AddComponent<View> ().GetComponent<View> ();
		//calls Start() on the object and initializes it.
		thisObj.mode = mode;
		thisObj.manager = manager;
		return thisObj;
	}
	
	void Awake() {
		width = Screen.width;
		height = Screen.height;

		left = (Screen.width - width) / 2;
		top = (Screen.height - height) / 2;

		windowRect = new Rect(left, top, width, height);

		selectTexture = Resources.Load<Texture2D>("card_highlight_full");
		font = Resources.Load<Font>("Fonts/" + "Chalkboard");
	}
	
	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown("1")) {
			Collapse();
		}
		if (Input.GetKeyDown("2")) {
			Expand();
		}
		
		// "Collision" Detection for Cards
		if (mouseOverList.Count > 0) {
			lastCardToDraw = mouseOverList[0];
			
			if (Input.GetMouseButtonUp(0)) {
				if (lastCardToDraw != cardList[0]) {
					SetCard(lastCardToDraw);
				}
			}
			// Clear "Collisions"
			mouseOverList.Clear();
		} else {
			lastCardToDraw = null;
		}
	}

	void OnGUI() {
		if (cardList.Count > 0) {
			GUI.depth = 0;

			GUI.BeginGroup(windowRect, GUI.skin.box);
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.UpperCenter;
				style.font = font;
				style.fontSize = 18;

				bool isMovingA = DrawList(predatorList), isMovingB = DrawList(preyList);

				isReady = isMovingA && isMovingB;

				if (isExpanded) {
					float y = Mathf.Min(
						predatorList.Count > 0 ? predatorList[0][0].y : cardList[0].y,
	                    preyList.Count > 0 ? preyList[0][0].y : cardList[0].y
                    );

					style.normal.textColor = Color.red;
					GUI.Label(new Rect(cardList[0].x - 145, y - 40, 100, 30), "Predators", style);

					style.normal.textColor = Color.green;
					GUI.Label(new Rect(cardList[0].x + 205, y - 40, 100, 30), "Preys", style);
				}

				if (lastCardToDraw != null) {
					lastCardToDraw.Draw();
				}

				for (int i = cardList.Count - 1; i >= 0; i--) {
					cardList[i].Draw();

					if (cardList[i] == lastCardToDraw) {
						Functions.DrawBackground(lastCardToDraw.GetRect(), selectTexture, Color.yellow);
					}
				
					if (cardList[i].GetRect().Contains(Event.current.mousePosition)) {
						mouseOverList.Insert(0, cardList[i]);
					}
				}

				if (GUI.Button(new Rect(cardList[0].x + (cardList[0].width - 80) / 2, cardList[0].y + cardList[0].height + 10, 80, 30), "Close")) {
					Destroy(this);
				}
			GUI.EndGroup();
			
			if (lastCardToDraw != null && !cardList.Contains(lastCardToDraw)) {
				Functions.DrawBackground(lastCardToDraw.GetRect(), selectTexture, lastCardToDraw.color);
			}
		}
	}

	public bool DrawList(List<List<Card>> cList) {
		bool isMoving = true;
		int maxPerColumn = 0;
		
		foreach (List<Card> list in cList) {
			maxPerColumn = Mathf.Max(list.Count, maxPerColumn);
		}

		for (int i = 0; i < maxPerColumn; i++) {
			for (int j = cList.Count - 1; j >= 0; j--) {
				if (i >= cList[j].Count) continue;

				Card temp = cList[j][i];
				
				if (temp.isMoving) {
					isMoving = false;
				} else {
					if (isExpanded && isReady && temp.GetRect().Contains(Event.current.mousePosition)) {
						mouseOverList.Insert(0, temp);
					}
				}
				
				if (temp == lastCardToDraw) {
					continue;
				}
				
				temp.Draw();
			}
		}

		return isMoving;
	}
	
	public void SetCard(Card card) {
		if (cardList.Count > 0 && card != cardList[0]) {
			predatorList.Clear();
			preyList.Clear();
		}

		if (cardList.Contains(card)) {
			int index = cardList.IndexOf(card);
			cardList.RemoveRange(0, index);
		} else {
			if (cardList.Count > 5) {
				cardList.RemoveAt(cardList.Count - 2);
			}
			
			cardList.Insert(0, card);
		}

		card.x = (width - card.width) / 2;
		card.y = (height - card.height) / 2;

		for (int i = cardList.Count - 1; i >= 0; i--) {
			float y = (height - card.height) / 2 - i * card.height * 0.15f;
			
			if (cardList.Count > 1 && i == cardList.Count - 1) {
				y -= card.height * 0.15f;
			}

			cardList[i].Translate(cardList[i].x, y, 1.2f);
		}
        /*
        //2018 Spring semester WoB Ecosystem
        //Lion (86) > Bufflao, Bush Pig
        //Buffalo (7) > Grass and Herbs
        //Bush Pig (83) > Decayling Material, Tree Mouse
        //Tree Mouse (31) > Grass and Herbs, Decaying Materials, Cockroach
        //Cockroach (19) > Decaying Materials
        //Decaying Materials (89) 
        //Grass and Herbs (96 or 1005)
        */
        List<Card> tempList = new List<Card>();
		GameState gs = GameObject.Find ("Global Object").GetComponent<GameState> ();
        int [] foodweb = { 86, 7, 83, 31, 19, 89, 1005 };

		foreach (int species_id in card.species.predatorList.Keys) {
			SpeciesData species = SpeciesTable.speciesList[species_id];
			//for converge game, ignore predators not currently in ecosystem
			if (mode == Constants.MODE_CONVERGE_GAME && !gs.speciesList.ContainsKey(species_id)) {
				continue;
			}
            //for foodweb, ignore predators not currently in mini-game ecosystem
            if (mode == Constants.MODE_FOODWEB && !foodweb.Contains(species_id))
            {
                continue;
            }

            string name = manager.MatchSeriesLabel (species.name);
			Texture2D image = Resources.Load<Texture2D>(Constants.IMAGE_RESOURCES_PATH + species.name);
			Card temp = new Card(
				species.name, 
			    image, 
				species, 
				new Rect(card.x, card.y, 160, 200), 
				name == null ? Color.red : manager.seriesColors [name]
				);

			tempList.Add(temp);
		}

		tempList.Sort(SortByName);
		CreateLayout(tempList, predatorList, -1);

		tempList = new List<Card>();

		foreach (int species_id in card.species.preyList.Keys) {
			SpeciesData species = SpeciesTable.speciesList[species_id];
			//for converge game, ignore prey not currently in ecosystem
			if (mode == Constants.MODE_CONVERGE_GAME && !gs.speciesList.ContainsKey(species_id)) {
				continue;
			}
            //for foodweb, ignore predators not currently in mini-game ecosystem
            if (mode == Constants.MODE_FOODWEB && !foodweb.Contains(species_id))
            {
                continue;
            }

            string name = manager.MatchSeriesLabel (species.name);
			Texture2D image = Resources.Load<Texture2D>(Constants.IMAGE_RESOURCES_PATH + species.name);
			Card temp = new Card(
				species.name, 
				image, 
				species, 
				new Rect(card.x, card.y, 160, 200), 
				name == null ? Color.green : manager.seriesColors [name]
				);
			
			tempList.Add(temp);
		}

		tempList.Sort(SortByName);
		CreateLayout(tempList, preyList, 1);

		Expand();
	}

	public void CreateLayout(List<Card> from, List<List<Card>> to, int direction) {
		direction = Mathf.Clamp(direction, -1, 1);

		int i = 0, numItems = from.Count > 18 ? 6 : 3;

		while (i < from.Count) {
			List<Card> cList = new List<Card>();

			numItems = Mathf.Min(from.Count - i, numItems);
			for (int j = 0; j < numItems; j++) {
				cList.Add(from[i + j]);
			}

			i += numItems;
			numItems++;
			to.Add(cList);
		}
	}

	public void Expand() {
		StartCoroutine("ExpandSet", 0);
		StartCoroutine("ExpandSet", 1);
	}
	
	public IEnumerator ExpandSet(int type) {
		isExpanded = true;

		List<List<Card>> cList;
		int direction;

		if (type == 0) {
			cList = predatorList;
			direction = -1;
		} else {
			cList = preyList;
			direction = 1;
		}

		int numItems = 0;
		foreach (List<Card> list in cList) {
			numItems += list.Count;
		}
		numItems = numItems > 18 ? 6 : 3;

		for (int i = 0; i < cList.Count; i++) {
			List<Card> tempList = cList[i];

			for (int j = 0; j < tempList.Count; j++) {
				Card temp = tempList[j];
				
				float x = (cardList[0].x + direction * (temp.width + 12)) + direction * i * (temp.width * 0.7f);
				float y;

				if (tempList.Count > 1 || i > 0) {
					y = (cardList[0].y - ((numItems - 1) * temp.height * 0.3f) / 2) + j * (temp.height * 0.3f);
					
					if (j == 0 || j == numItems - 1) {
						x += direction * temp.width * 0.2f;
					}
				} else {
					y = cardList[0].y;
				}
				
				temp.Translate(x, y, 1.2f);

				yield return new WaitForSeconds(0.01f);
			}

			numItems++;
		}
	}

	public void Collapse() {
		isExpanded = false;
	
		StopCoroutine("ExpandSet");

		foreach (List<Card> cList in predatorList) {
			foreach (Card card in cList) {
				card.Translate(cardList[0].x, cardList[0].y, 1.4f);
			}
		}

		foreach (List<Card> cList in preyList) {
			foreach (Card card in cList) {
				card.Translate(cardList[0].x, cardList[0].y, 1.4f);
			}
		}
	}
	
	public int SortByName(Card x, Card y) {
		return x.name.CompareTo(y.name);
	}

	public void SetMode (int mode) {
		this.mode = mode;
	}
}