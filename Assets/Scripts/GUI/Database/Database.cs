using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Database : MonoBehaviour
{

	private int window_id = Constants.DATABASE_WIN;
	// Window Properties
	private float left;
	private float top;
	private float width = 640;
	private float height = Screen.height;
	private bool isActive = false;
	// Other
	public string title { get; set; }

	private Dictionary<string, Rect> rect = new Dictionary<string, Rect> ();
	private Rect windowRect;
	private Vector2 scrollPosition = Vector2.zero;
	private float startX = 0;
	private float hideX;
	private float winDT = 0;
	private Dictionary<int, SpeciesData> speciesTable = SpeciesTable.speciesList;
	private Texture2D cardHighlightTexture;
	private Texture2D cardHighlightFullTexture;
	private Texture2D bgTexture;
	private Texture2D iconTexture;
	private Dictionary<string, Card> cardList = new Dictionary<string, Card> ();

	private List<Page> viewList { get; set; }

	private List<Page> ecoList = new List<Page> ();
	private List<Page> shopList = new List<Page> ();
	private List<string> excludeList = new List<string> ();
	private bool styleExists;
	private GUIStyle style;

	public string selected { get; set; }

	private List<string> mouseOverList = new List<string> ();
	private int mode = Constants.MODE_SHOP;
	private bool toggle;
	private string[] btnStrings = new string[]{"Ecosystem", "Shop"};
	private Font font;
	private string showDetails = "";
	public ConvergeManager manager { get; set; }
	private int priorMode = Constants.ID_NOT_SET;
	private bool activatedBySpeciesClick = false;
	private bool openedDetails = true;
	private bool closedDetails = false;

	//Factory method for creating new Database component with specified mode
	public static Database NewDatabase (GameObject gameObject, int mode, ConvergeManager manager)
	{
		Database thisObj = gameObject.AddComponent<Database> ().GetComponent<Database> ();
		//calls Start() on the object and initializes it.
		thisObj.mode = mode;
		thisObj.manager = manager;
		return thisObj;
	}

	void Awake ()
	{
		title = "Species";

		left = -(windowRect.width - 45);
		top = 0;

		windowRect = new Rect (left, top, width, height);
		rect ["subWin"] = new Rect (0, 0, windowRect.width - 25, windowRect.height);
		rect ["btn"] = new Rect (windowRect.width - 20, (windowRect.height - 100) / 2, 15, 100);
		rect ["content"] = new Rect (10, 90, rect ["subWin"].width - 20, rect ["subWin"].height - 100);
		rect ["scroll"] = new Rect (0, 0, rect ["content"].width, rect ["content"].height);

		hideX = startX = -rect ["subWin"].width;

		bgTexture = Resources.Load<Texture2D> (Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
		iconTexture = Resources.Load<Texture2D> ("card_bg_w");
		cardHighlightTexture = Resources.Load<Texture2D> ("card_highlight");
		cardHighlightFullTexture = Resources.Load<Texture2D> ("card_highlight_full");
		font = Resources.Load<Font> ("Fonts/" + "Chalkboard");
	}

	// Use this for initialization
	void Start ()
	{
		Refresh ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isActive) {
			windowRect.x = Mathf.Lerp (startX, 0, winDT += Time.deltaTime * 2f);
		} else {
			windowRect.x = Mathf.Lerp (startX, hideX, winDT += Time.deltaTime * 2f);
		}

		if (openedDetails && GameObject.Find ("Global Object").GetComponent<View> () == null) {
			closedDetails = true;
		}
	}

	void OnGUI ()
	{
		if (!styleExists) {
			style = new GUIStyle (GUI.skin.label);
			style.alignment = TextAnchor.UpperCenter;
			style.font = font;
			style.richText = true;

			styleExists = true;
		}

		windowRect = GUI.Window (window_id, windowRect, MakeWindow, "", GUIStyle.none);

		if (windowRect.x <= hideX) {
			GUIStyle style = new GUIStyle (GUI.skin.box);
			style.alignment = TextAnchor.MiddleCenter;
			style.font = font;
			
			float textWidth = style.CalcSize (new GUIContent (title)).x;
			GUI.Label (new Rect (windowRect.x + windowRect.width, (windowRect.height - 30) / 2, textWidth + 10, 30), title, style);
		}

		// "Collision" Detection for Cards
		if (mouseOverList.Count > 0) {
			// Clear "Collisions"
			mouseOverList.Clear ();
		}
	}
	
	public void MakeWindow (int id)
	{
		GUI.BringWindowToFront (id);
		GUI.BeginGroup (rect ["subWin"], GUI.skin.box);
		Functions.DrawBackground (new Rect (0, 0, rect ["subWin"].width, rect ["subWin"].height), bgTexture);

		style.alignment = TextAnchor.UpperCenter;
		GUI.Label (new Rect (0, 10, rect ["subWin"].width, 30), "<size=20>" + title + "</size>", style);

		if (mode != Constants.MODE_CONVERGE_GAME) {
			mode = GUI.SelectionGrid (new Rect (rect ["content"].x + 20, rect ["content"].y - 40, 200, 30), 
		                          mode, 
		                          btnStrings, 
		                          btnStrings.Length);
		}

		switch (mode) {
		case 0:  //Constants.MODE_ECOSYSTEM:
		case 2:  //Constants.MODE_CONVERGE_GAME:
			Switch (mode);
			viewList = ecoList;
			break;
		case 1:  //Constants.MODE_SHOP:
			viewList = shopList;
			break;
		}

		GUI.BeginGroup (rect ["content"], GUI.skin.box);
		float trophicRectHeight = 300;

		GUIStyle[] scrollStyle = new GUIStyle[]{GUIStyle.none, GUIStyle.none};
		if (rect ["scroll"].Contains (Event.current.mousePosition)) {
			scrollStyle [0] = GUI.skin.horizontalScrollbar;
			scrollStyle [1] = GUI.skin.verticalScrollbar;
		}
		scrollPosition = GUI.BeginScrollView (
			rect ["scroll"], 
		    scrollPosition, 
			new Rect (0, 0, rect ["scroll"].width - 32, viewList.Count * trophicRectHeight + 16), 
			scrollStyle [0], 
			scrollStyle [1]
		);
		for (int i = 0; i < viewList.Count; i++) {
			Page page = viewList [i];

			Rect trophicRect = new Rect (16, i * trophicRectHeight, rect ["scroll"].width - 32, trophicRectHeight);

			scrollStyle = new GUIStyle[]{GUIStyle.none, GUIStyle.none};
			if (trophicRect.Contains (Event.current.mousePosition)) {
				scrollStyle [0] = GUI.skin.horizontalScrollbar;
				scrollStyle [1] = GUI.skin.verticalScrollbar;
			}

			GUI.BeginGroup (trophicRect);
			style.alignment = TextAnchor.UpperLeft;
			GUI.Label (new Rect (0, 5, 200, 30), page.title, style);

			page.scrollPos = GUI.BeginScrollView (
				new Rect (0, 30, trophicRect.width, trophicRectHeight - 30),
				page.scrollPos, 
				new Rect (0, 0, page.contents.Count * (160 + 12) - 12, trophicRectHeight - 30 - 32), 
				scrollStyle [0], 
				scrollStyle [1]
			);
			for (int j = 0; j < page.contents.Count; j++) {
				Card card = cardList [page.contents [j]];
				card.x = j * (card.width + 12);
				card.y = 12;

				DrawCard (card);

				GUI.BeginGroup (new Rect (card.x, card.y + card.height + 5, card.width, 40));
				Rect btnRect = new Rect ((card.width - 70) / 2, 0, 70, 30);

				if (mode == Constants.MODE_SHOP) {
					btnRect = new Rect (8, 0, 70, 30);
				}

				if (GUI.Button (btnRect, "Details") || showDetails.Equals (card.name)) {
					if (!FindObjectOfType<View> ()) {
						View view = View.NewView (gameObject, mode, manager);

						view.SetCard (new Card (card.name, card.image, card.species, new Rect (0, 0, 160, 200), card.color));
						showDetails = "";
						openedDetails = true;
					}
				}

				if (mode == Constants.MODE_SHOP) {
					if (GUI.Button (new Rect (83, 0, 70, 30), "Buy")) {
						GameObject.Find ("Global Object").GetComponent<GameState> ().CreateSpecies (-1, card.species.biomass, card.name, card.species);
					}
				}
				GUI.EndGroup ();
			}
			GUI.EndScrollView ();
			GUI.EndGroup ();
		}
		GUI.EndScrollView ();
		GUI.EndGroup ();

		GUI.BeginGroup (new Rect (rect ["content"].width - 230, 50, 220, 30), GUI.skin.box);
		GUI.color = Color.green;
		GUI.DrawTexture (new Rect (30, 7, 16, 16), iconTexture);
		GUI.Label (new Rect (60, 4, 100, 20), "Prey");
		GUI.color = Color.red;
		GUI.DrawTexture (new Rect (120, 7, 16, 16), iconTexture);
		GUI.Label (new Rect (150, 4, 100, 20), "Predator");
		GUI.color = Color.white;
		GUI.EndGroup ();
		GUI.EndGroup ();

		if (GUI.Button (rect ["btn"], "") || (activatedBySpeciesClick && closedDetails)) {
			SetActive (!isActive, null);
			activatedBySpeciesClick = false;
			closedDetails = false;
			openedDetails = false;
		}
	}

	public void DrawCard (Card card)
	{
		Color color = Color.clear;
		if (selected != null) {
			if (card.name == selected) {
				color = Color.blue;  //.yellow;
			} else if (card.species.preyList.ContainsValue (selected)) {
				color = Color.red;
			} else if (card.species.predatorList.ContainsValue (selected)) {
				color = Color.green;
			}
		}

		if (color == Color.clear || card.name == selected) {
			card.color = Color.black;
		} else {
			card.color = color;
		}

		//jtc - override with info from manager
		Debug.Log(manager);
		string name = manager.MatchSeriesLabel (card.name);
		if (name != null) {
			color = manager.seriesColors [name];
			card.color = color;		
		}
		card.Draw ();

		// Selection
		if (color != Color.clear) {
			Functions.DrawBackground (card.GetRect (), cardHighlightFullTexture, color);
			
			if (card.name == selected && card.species.preyList.ContainsValue (selected)) { // Prey = Self
				Functions.DrawBackground (new Rect (card.x + 5, card.y + 5, card.width - 10, card.height - 10), 
				                          cardHighlightFullTexture, 
				                          Color.green
				);
			}
		}

		if (card.GetRect ().Contains (Event.current.mousePosition)) {
			mouseOverList.Add (card.name);

			Functions.DrawBackground (card.GetRect (), cardHighlightTexture, Color.yellow);
		}

		if (GUI.Button (card.GetRect (), "", GUIStyle.none)) {
			if (card.name == selected) {
				selected = null;
			} else {
				selected = card.name;
			}
		}
	}

	public void SetActive (bool active, string details)
	{
		isActive = active;
		if (details == null) {
			showDetails = "";
		} else {
			showDetails = details;
			activatedBySpeciesClick = true;
		}

		startX = windowRect.x;
		winDT = 0;

		if (active) {
			GUI.BringWindowToFront (window_id);
		}

	}

	public void Switch (int view)
	{
		switch (view) {
		case 0:  //Constants.MODE_ECOSYSTEM:
		case 2:  //Constants.MODE_CONVERGE_GAME:
			List<string> contents = new List<string> ();
			GameState gs = GameObject.Find ("Global Object").GetComponent<GameState> ();
				
			foreach (Species species in gs.speciesList.Values) {
				contents.Add (species.name);
			}
				
			ecoList.Clear ();
			contents.Sort (SortByTrophicLevels);
			Prepare (contents, ecoList);
			break;
		case 1:  //Constants.MODE_SHOP:
			break;
		}
	}
	
	public void Refresh ()
	{
		foreach (SpeciesData species in speciesTable.Values) {
			Texture2D image = Resources.Load (Constants.IMAGE_RESOURCES_PATH + species.name) as Texture2D;

			Card card = new Card (species.name, image, species, new Rect (0, 0, 160, 200));
			cardList [species.name] = card;
		}
		
		shopList.Clear ();
		List<string> tempList = new List<string> (cardList.Keys);
		tempList.Sort (SortByTrophicLevels);
		Prepare (tempList, shopList);
	}

	public void Prepare (List<string> from, List<Page> into)
	{
		if (from.Count == 0) {
			return;
		}

		int index = 0;
		for (float trophic = 3.5f; trophic >= 1f; trophic -= 0.5f) {
			Card card = cardList [from [index]];

			if (card.species.trophic_level < trophic) {
				continue;
			}

			List<string> contents = new List<string> ();

			do {
				contents.Add (card.name);
				index++;

				if (index < from.Count) {
					card = cardList [from [index]];
				} else {
					break;
				}
			} while (card.species.trophic_level >= trophic);

			contents.Sort ();
			into.Add (new Page ("Trophic Level " + trophic.ToString ("0.0"), contents));

			if (index >= from.Count) {
				break;
			}
		}
	}
	
	public int SortByTrophicLevels (string x, string y)
	{
		return cardList [y].species.trophic_level.CompareTo (cardList [x].species.trophic_level);
	}

	public void SetExcludeSpecies (string name, bool exclude)
	{
		if (exclude) {
			excludeList.Add (name);
		} else {
			excludeList.Remove (name);
		}
	}

	public List<string> GetSpecies ()
	{
		return new List<string> (cardList.Keys);
	}
	
	public void SetMode (int mode)
	{
		this.mode = mode;
	}
}
