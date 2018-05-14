using UnityEngine;
// using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConvergeGame : MonoBehaviour
{
	private int player_id;
	// Window Properties
	private int window_id = Constants.CONVERGE_WIN;
	private float left;
	private float top;
	private float width = Screen.width;
	private float height = Screen.height;
	private float widthGraph;
	private float heightGraph;
	private int bufferBorder = 10;
	private float leftGraph = 10;
	private float topGraph = 75;
	private Rect windowRect;
	// Logic
	private bool isActive = true;
	private bool isInitial = true;
	private bool isProcessing = true;
	private bool blinkOn = true;
	//public Texture background;
	private Texture2D bgTexture;
	private Font font;
	//ecosystems
	private int ecosystem_id;
	private int ecosystem_idx;
	private int new_ecosystem_id;
	private int new_ecosystem_idx;
	private int temp_ecosystem_idx;
	private List<ConvergeEcosystem> ecosystemList = null;
	private string[] ecosysDescripts;
	//attempts
	private int referenceAttemptIdx;
	private ConvergeAttempt currAttempt = null;
	private ConvergeAttempt currAttemptTarget = null;
	private List<ConvergeAttempt> attemptList = null;
	private int attemptCount = 0;
	private String config_field_default;
	//graphs, visualization
	private GraphsCompare graphs = null;
	private ConvergeManager manager = null;
	private BarGraph barGraph = null;
	private Database foodWeb = null;
	//popup messaging
	private bool showPopup = false;
	private string popupMessage = "";
	private Rect popupRect;
	private Vector2 popupScrollPosn  = Vector2.zero;
	//hints
	private bool allowHintsConfigured = false;
	private bool allowHintsMaster;
	private int hintCount = 0;
	private Dictionary <int, string> hintDict = new Dictionary <int, string>();
	private List<int> priorHintIdList = new List<int>();
	//reset attempt slider
	private bool isResetSliderInitialized = false;
	private int resetSliderValue = 0;
	private int maxResetSliderValue = 0;
	CSVObject graph1CSV, graph2CSV;
	private bool ecosRcvd = false;
	private int init_idx;
	String inputEcoStr = "";
	private List<int> keyList;
	private int foodWebWidth, foodWebHeight, foodWebWidthP, foodWebHeightP, foodWebDPI;
	private int FWWWidth, FWWHeight;
	private int imageByteCount, segCount, segCounter;
	private string speciesStr, configStr;
	private byte[] imageContents;
	private bool foodWebImageExists;
	private Texture2D fwTexture = null;
	private Rect foodWebRect, foodWebButtonRect;
	private GUI.WindowFunction winFunction;

	void Awake ()
	{
		DontDestroyOnLoad (gameObject.GetComponent ("Login"));
		player_id = GameState.player.GetID ();

		left = (Screen.width - width) / 2;
		top = (Screen.height - height) / 2;
		
		windowRect = new Rect (left, top, width, height);
		widthGraph = windowRect.width - (bufferBorder * 2);
		heightGraph = windowRect.height / 2;
		popupRect = new Rect ((Screen.width / 2) - 250, (Screen.height / 2) - 125, 500, 200);
		bgTexture = Resources.Load<Texture2D> (Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
		font = Resources.Load<Font> ("Fonts/" + "Chalkboard");
		SetIsProcessing (true);
		foodWebImageExists = false;
	}
	
	// Use this for initialization
	void Start ()
	{
		foodWebWidth = (int) ((Screen.width / Screen.dpi) * WorldController.FOOD_WEB_FRACTION);
		foodWebHeight = (int) ((Screen.height / Screen.dpi) * WorldController.FOOD_WEB_FRACTION);
		foodWebDPI = (int)Screen.dpi; 
		foodWebWidthP = foodWebWidth * foodWebDPI;
		foodWebHeightP = foodWebHeight * foodWebDPI;
		FWWWidth = foodWebWidthP + 140;
		FWWHeight = foodWebHeightP + 120;
		foodWebButtonRect = new Rect (windowRect.width - 220 - bufferBorder, 0, 100, 30);
		Debug.Log ("ConvergeGame: foodWebWidth, foodWebHeight = " + foodWebWidth + " " + foodWebHeight);
		Debug.Log ("ConvergeGame: Screen.dpi, foodWebDPI = " + Screen.dpi + " " + foodWebDPI);

		// int sw = Screen.width;
		// int sh = Screen.height;
		// float sDpi = Screen.dpi;
		// Debug.Log ("sw/sh/sDpi: " + sw + " " + sh + " " + sDpi);
		Game.StartEnterTransition ();
		//to generate converge-ecosystem.txt, remove comments and let protocol run;
		//server will generate txt from sql table
		Game.networkManager.Send(ConvergeEcosystemsProtocol.Prepare(),ProcessConvergeEcosystems);

		/* This code move to ProcessConvergeEcosystems
		
		//get list of ecosystems
		// ReadConvergeEcosystemsFile ();
		//set default ecosystem values
		new_ecosystem_id = Constants.ID_NOT_SET;
		ecosystem_idx = 0;
		ecosystem_id = GetEcosystemId (ecosystem_idx);
		//get player's most recent prior attempts 
		GetPriorAttempts ();
		//create array of ecosystem descriptions
		if (ecosystemList != null) {
			ecosysDescripts = ConvergeEcosystem.GetDescriptions (ecosystemList);
		}
		GetHints ();
		*/

	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void OnGUI ()
	{		
		// Background
		//GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), background, ScaleMode.ScaleAndCrop);
		
		// Client Version Label
		GUI.Label (new Rect (Screen.width - 75, Screen.height - 30, 65, 20), "v" + Constants.CLIENT_VERSION + " Beta");

		// Converge Game Interface
		if (isActive && ecosRcvd) {
			windowRect = GUI.Window (window_id, windowRect, MakeWindow, "Converge", GUIStyle.none);
			
			//if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return) {
			//	Submit();
			//}
		}
		GUI.skin.label.fontSize = 12;  //for legend series labels

		if (showPopup && ecosRcvd) {
			GUI.Window (Constants.CONVERGE_POPUP_WIN, popupRect, ShowPopup, "Error", GUIStyle.none);
		}			

		if (foodWebImageExists) {
			foodWebRect = GUI.Window (Constants.FOOD_WEB_VIEW, foodWebRect, MakeFoodWebWindow, "food Web view", GUIStyle.none);
		}
	}

	void MakeFoodWebWindow(int id) {
		Functions.DrawBackground(new Rect(0, 0, FWWWidth, FWWHeight), bgTexture);
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = 18;

		GUI.DrawTexture(new Rect(70, 60, foodWebWidthP, foodWebHeightP), fwTexture, ScaleMode.ScaleToFit);

		GUI.Label(new Rect((FWWWidth - 150)/2, 40, 150, 30), "Food Web Display", style);

		if (GUI.Button (new Rect (FWWWidth/2 - 50, FWWHeight - 60, 100, 30), "Close")) {
			foodWebImageExists = false;
		}			
		GUI.DragWindow (new Rect(0, 0, Screen.width, Screen.height));
	}

	public void ProcessFWImage(NetworkResponse response) {
		ResponseSpeciesAction args = response as ResponseSpeciesAction;
		Debug.Log ("ConvergeGame: ProcessFWImage, byteCount = " + args.byteCount);
		imageByteCount = args.byteCount;
		if (imageByteCount > 0) {
			short action = 9;
			imageContents = new byte[imageByteCount];
			segCounter = 0;
			segCount = ((imageByteCount - 1) / WorldController.FOOD_WEB_BLOCK_SIZE) + 1;
			for (int i = 0; i < segCount; i++) {
				Game.networkManager.Send (SpeciesActionProtocol.Prepare 
					(action, configStr, speciesStr, i * WorldController.FOOD_WEB_BLOCK_SIZE), ProcessFWImage2);
			}
		}
	}

	public void ProcessFWImage2(NetworkResponse response) {
		ResponseSpeciesAction args = response as ResponseSpeciesAction;
		Debug.Log ("ConvergeGame: ProcessFWImage2, startByte, byteCount = " 
			+ args.startByte + " " + args.byteCount);
		for (int i = 0; i < args.byteCount; i++) {
			imageContents [i + args.startByte] = args.fileContents [i];
		}
		segCounter++;
		if (segCount == segCounter) {
			string fileName = "foodweb_" + speciesStr.Replace (" ", "-") + ".png";
			using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create))) {
				writer.Write(imageContents);
			}
			DrawTexture (fileName);
		}
	}


	void DrawTexture(String filePath) {
		byte[] fileData;
		if (File.Exists(filePath)) {
			fileData = File.ReadAllBytes(filePath);
			fwTexture = new Texture2D(2, 2);
			fwTexture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
			foodWebImageExists = true;
		}
	}

	
	void MakeWindow (int id)
	{
		Functions.DrawBackground (new Rect (0, 0, width, height), bgTexture);
		
		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.font = font;
		style.fontSize = 16;

		GUI.Label (new Rect ((windowRect.width - 100) / 2, 0, 100, 30), "Convergence Game", style);
		if (GUI.Button (new Rect (windowRect.width - 100 - bufferBorder, 0, 100, 30), "Return to Lobby")) {
			Destroy (this);
			Destroy (foodWeb);
			// GameState gs = GameObject.Find ("Global Object").GetComponent<GameState> ();
			// Species[] s = gs.GetComponents<Species>();
			// foreach (Species sp in s) Destroy (sp); //destroy the "species" objects
			Game.SwitchScene("World");
		}

		GUI.BeginGroup (new Rect (bufferBorder, 0, windowRect.width, 100));
		style.alignment = TextAnchor.LowerLeft;
		style.fontSize = 14;
		GUI.Label (new Rect (0, 0, 300, 30), "Enter Ecosystem Number: (Valid range: 1 to " 
			+ ecosystemList.Count + ")", style);
		GUI.SetNextControlName ("ecosystem_idx_field");
		int new_idx = ecosystem_idx;
		// new_idx = GUI.SelectionGrid (new Rect (0, 35, windowRect.width - 20, 30), ecosystem_idx, 
        //                 ecosysDescripts, ecosysDescripts.Length);

		inputEcoStr = GUI.TextField(new Rect (0, 35, 35, 25), inputEcoStr, 3);
		GUI.Label (new Rect (50, 30, 360, 30), "To change Ecosystem, enter new value, press 'OK'", style);
		if (GUI.Button (new Rect (410, 35, 35, 25), "OK")) {
			if (Int32.TryParse (inputEcoStr, out new_idx)) {
				new_idx--;
				if ((new_idx < 0) || (new_idx > ecosystemList.Count - 1)) {
					new_idx = ecosystem_idx;
				}
				inputEcoStr = "" + (new_idx + 1);
			} else {
				new_idx = ecosystem_idx;
				inputEcoStr = "" + (new_idx + 1);
			}
		}
			
		GUI.EndGroup ();
		if (!isProcessing && new_idx != ecosystem_idx) {
			//Debug.Log ("Updating selected ecosystem.");
			SetIsProcessing (true);
			new_ecosystem_idx = new_idx;
			new_ecosystem_id = GetEcosystemId (new_ecosystem_idx);

			if (ecosystemList [new_ecosystem_idx].csv_default_string.Length == 0) {
				SubmitTargetInit ();
			} else {
				GetPriorAttempts ();
			}
		}
		if (graphs != null) {
			graphs.DrawGraphs ();
		}

		GUIStyle styleEdit = new GUIStyle (GUI.skin.textArea);
		styleEdit.wordWrap = true;

		//data entry fields
		DrawParameterFields (style);

		if (isInitial && GUI.GetNameOfFocusedControl () == "") {
			GUI.FocusControl ("ecosystem_idx_field");
			isInitial = false;
		}

		string buttonTitle = isProcessing ? "Processing" : "Submit";
		if (!(isProcessing && !blinkOn)) {
			if (GUI.Button (new Rect (bufferBorder, height - 30 - bufferBorder, 100, 30), buttonTitle) &&
				!isProcessing) {
				//make sure new config is distinct from prior attempts and initial value
				currAttempt.UpdateConfig ();  //update config based on user data entry changes
				ConvergeAttempt prior = attemptList.Find (entry => entry.config == currAttempt.config);
				if (prior == null && currAttempt.ParamsUpdated ()) {
					Submit ();
					SetIsProcessing (true);
				} else if (!showPopup) {
					int prior_idx = attemptList.FindIndex (entry => entry.config == currAttempt.config);
					if (prior_idx == Constants.ID_NOT_SET) {
						popupMessage = "This attempt matches the initial ecosystem. Each attempt must be unique. ";
						popupMessage +=  "Please change at least one slider and press 'Submit' again.";
					} else {
						popupMessage = "This attempt matches prior attempt #" 
							+ (prior_idx + 1) + ". Each attempt must be unique. Please change at least one slider and press 'Submit' again.";
					}
					//Debug.Log (popupMessage);
					showPopup = true;
				}
			}
		}

		if (GUI.Button (new Rect (bufferBorder + 110, height - 30 - bufferBorder, 110, 30), "Progress Report")) {
			GenerateBarGraph ();
		}

		int screenOffset = 0;
		if (currAttempt != null && currAttempt.allow_hints) {
			screenOffset += bufferBorder + 110;
			if (GUI.Button (new Rect (bufferBorder * 2 + 110 * 2, height - 30 - bufferBorder, 110, 30), "Get Hint")) {
				//only give new hint if one hasn't been provided during this session.
				if (currAttempt.hint_id == Constants.ID_NOT_SET) {
					int hintId = GetRandomHint (true);
					if (hintId == Constants.ID_NOT_SET) {
						popupMessage = "Sorry, no new hints are available.";
					} else {
						popupMessage = hintDict[hintId];
						currAttempt.hint_id = hintId;
					}
				} else {
					if (hintDict.ContainsKey (currAttempt.hint_id)) {
						popupMessage = hintDict[currAttempt.hint_id] + "\n[Only one hint is provided per attempt.]";
					} else {
						Debug.LogError ("Invalid hint for current attempt, ID = " + currAttempt.hint_id);
						popupMessage = "Error, hint not available.";
					}
				}
				if (priorHintIdList.Count > 0) {
					popupMessage += "\n\nPrior Hints:";
				}
				for (int i = 0; i < priorHintIdList.Count; i++) {
					popupMessage += "\n\n" + hintDict[priorHintIdList[i]];
				}
				//Debug.Log (popupMessage);
				showPopup = true;
			}
		}

		DrawResetButtons (screenOffset, style);

		if (GUI.Button (foodWebButtonRect, "Food Web") && !foodWebImageExists) {
			foodWebRect = new Rect ((Screen.width - FWWWidth) / 2, (Screen.height - FWWHeight) / 2, FWWWidth, FWWHeight);
			string fileName = "foodweb_" + speciesStr.Replace (" ", "-") + ".png";
			if (File.Exists (fileName)) {
				DrawTexture (fileName);
				return;
			}
			configStr = " --figsize " + foodWebWidth + " " + foodWebHeight + " "
				+ " --dpi " + foodWebDPI;
			short action = 8;
			Debug.Log ("ConvergeGame: :config:species: = :" + configStr + ":" + speciesStr + ":");
			Game.networkManager.Send (SpeciesActionProtocol.Prepare 
				(action, configStr, speciesStr), ProcessFWImage);
		}	
			
		Event e = Event.current;
		if((e.type == EventType.MouseDown) && !foodWebRect.Contains(e.mousePosition) && !foodWebButtonRect.Contains(e.mousePosition))
		{
			Debug.Log ("mouse click outside foodweb");
			foodWebImageExists = false;
		}

	}

	private void DrawParameterFields (GUIStyle style)
	{
		style.alignment = TextAnchor.UpperRight;
		style.font = font;
		style.fontSize = 14;
		Color savedColor = GUI.color;
		Color savedBkgdColor = GUI.backgroundColor;

		if (currAttempt != null && currAttempt.seriesParams.Count > 0) {
			int paramCnt = currAttempt.seriesParams.Count;
			int row = 0;
			int col = 0;
			float entryHeight = height - heightGraph - 30 * 3 - bufferBorder * 2;
			GUI.BeginGroup (new Rect (bufferBorder, topGraph + heightGraph + bufferBorder, width, entryHeight));
			//use seriesNodes to force order
			foreach (int nodeId in manager.seriesNodes) {
				//look for all possible parameter types for each node
				foreach (char paramId in new char[]{'K', 'R', 'X'}) {
					string strId = paramId.ToString ();
					ConvergeParam param;
					string nodeIdParamId = ConvergeParam.NodeIdParamId (nodeId, strId);
					if (currAttempt.seriesParams.ContainsKey (nodeIdParamId)) {
						param = currAttempt.seriesParams [nodeIdParamId];
					} else {
						continue;
					}
				
					float min = 0f;
					float max = 1f;
					switch (paramId) {
					case 'K':
						min = 1000f;
						max = 15000f;
						break;
					case 'R':
						min = 0f;
						max = 3f;
						break;
					default:
						break;
					}
				
					Rect labelRect, TFRect;
					//draw name, paramId
					labelRect = new Rect (col * (350 + bufferBorder), row * 35, 200, 30);
					TFRect = new Rect (col * (350 + bufferBorder) + 215, row * 35, 30, 25);
					if (labelRect.Contains (Event.current.mousePosition) || TFRect.Contains(Event.current.mousePosition)) {
						manager.mouseOverLabels.Add (param.name);
						manager.selected = param.name;
						manager.lastSeriesToDraw = param.name;
					}
					GUI.color = (param.name.Equals (manager.selected)) ? 
						manager.seriesColors [param.name] : Color.white;
					String pStr = param.name;
					int pSIdx = pStr.IndexOf ("[");
					if (pSIdx != -1) {
						pStr = pStr.Substring (0, pSIdx);
					}
					// GUI.Label (labelRect, param.name + " - " + param.paramId, style);
					GUI.Label (labelRect, pStr, style);

					int pVInInt = (int) (99.5 * (param.value - min) / (max - min));
					String pVInStr = "" + pVInInt;
					if (pVInInt == 0) {
						pVInStr = "";
					} 
					String pVOutStr = GUI.TextField(TFRect, pVInStr, 2);
					if (!pVInStr.Equals (pVOutStr)) {
						int pVOutInt;
						if (!Int32.TryParse(pVOutStr, out pVOutInt)) {
							pVOutInt = 0;
						}
						param.value = min + (pVOutInt + 0.5f) * (max - min) / 99.5f;
					}
						
					//if player clicks on species, set as selected and activate foodWeb
					if (GUI.Button (labelRect, "", GUIStyle.none)) {
						foodWeb.selected = SpeciesTable.GetSpeciesName (param.name);
						foodWeb.SetActive (true, foodWeb.selected);
					}
				
					//draw slider with underlying colored bar showing original value
					Rect sliderRect = new Rect (labelRect.x + 250 + bufferBorder, labelRect.y + 5, 100, 20);
					if (sliderRect.Contains (Event.current.mousePosition) || TFRect.Contains(Event.current.mousePosition)) {
						manager.mouseOverLabels.Add (param.name);
						manager.selected = param.name;
						manager.lastSeriesToDraw = param.name;
					}
					GUI.color = manager.seriesColors [param.name];
					float origValWidth = 
						ConvergeParam.NormParam (param.origVal, min, max, sliderRect.width);
					//float origValWidth = (param.origVal / (max - min)) * sliderRect.width;
					Color slTexture= new Color (0.85f, 0.85f, 0.85f);
					GUI.DrawTexture (new Rect (sliderRect.x, sliderRect.y, origValWidth, 10), //sliderRect.height),
					                 Functions.CreateTexture2D (slTexture));
//					GUI.color = savedColor;
					
					//draw slider for parameter value manipulation
					GUI.backgroundColor = manager.seriesColors [param.name];
					param.value = GUI.HorizontalSlider (
					sliderRect, 
					param.value, 
					min, 
					max
					);

					//show normalized value for parameter
					if (param.name.Equals (manager.selected)) {
						string valLabel = String.Format (
							"{0}", 
							ConvergeParam.NormParam (param.value, min, max));
						if (param.value != param.origVal) {
							valLabel = valLabel + String.Format (
								" [{0}]", 
								ConvergeParam.NormParam (param.origVal, min, max));
						}
						style.alignment = TextAnchor.UpperLeft;
						float xPosn = 
							sliderRect.x + 
							(param.value / (max - min)) * 
								sliderRect.width +
								bufferBorder;
						Rect valRect = new Rect(xPosn, labelRect.y, 70, labelRect.height - 5);

						GUI.Box (valRect, valLabel);
						style.alignment = TextAnchor.UpperRight;
					}

					if ((row + 1) * 35 + 30 > entryHeight) {
						col++;
						row = 0;
					} else {
						row++;
					}

					GUI.color = savedColor;
					GUI.backgroundColor = savedBkgdColor;

				}
			}
			GUI.EndGroup ();
		}
		style.alignment = TextAnchor.UpperLeft;
		style.font = font;
		style.fontSize = 16;
	}

	void DrawResetButtons (int screenOffset, GUIStyle style)
	{
		GUI.Label (new Rect (bufferBorder + 260 + screenOffset, height - 30 - bufferBorder, 110, 30), "Reset to:", style);
		Rect initial = new Rect (bufferBorder * 2 + 330 + screenOffset, height - 30 - bufferBorder, 50, 30);
		if (GUI.Button (initial, "Initial") && !isProcessing) {
			ResetCurrAttempt (Constants.ID_NOT_SET);
		}
		//use slider to accommodate more reset attempt buttons that fit on the screen
		int widthPer = 45;
		int sliderWidth = 100;
		if (!isResetSliderInitialized) {
			InitializeResetSlider (width - (initial.x + initial.width + sliderWidth + bufferBorder), widthPer);
			isResetSliderInitialized = !isProcessing;
		}
		int maxVal = attemptList.Count - maxResetSliderValue + resetSliderValue;
		for (int i = resetSliderValue; i < maxVal; i++) {
			int slideNo = i - resetSliderValue;
			if (GUI.Button (
				new Rect (bufferBorder * 2 + 390 + (slideNo * widthPer) + screenOffset, height - 30 - bufferBorder, 
			          widthPer - 5, 
			          30
			          ), 
			    String.Format ("#{0}", i + 1)
				)
			    && !isProcessing) {
				ResetCurrAttempt (i);
			}
		}

		if (maxResetSliderValue > 0) {
			Rect sliderRect = new Rect (width - sliderWidth - bufferBorder, initial.y, sliderWidth, 30); 
			resetSliderValue = Mathf.RoundToInt (GUI.HorizontalSlider (
				sliderRect, 
				resetSliderValue, 
				0, 
				maxResetSliderValue
			));
		}

	}

	void InitializeResetSlider (float availWidth, int widthPer)
	{
		int perPage = Mathf.FloorToInt (availWidth / widthPer);
		// Adjust init slider val
		maxResetSliderValue = Mathf.Max (0, attemptList.Count - perPage);
		resetSliderValue = maxResetSliderValue;
	}

	void ShowPopup (int windowID)
	{
		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.font = font;
		style.fontSize = 16;
		
		Functions.DrawBackground (new Rect (0, 0, popupRect.width, popupRect.height), bgTexture);
		GUI.BringWindowToFront (windowID);
		Rect outerRect = new Rect (
			bufferBorder, 
			bufferBorder, 
			popupRect.width - bufferBorder, 
			popupRect.height - 30 - bufferBorder * 2
			);
		float msgHeight = Mathf.Max (1, popupMessage.Length / 150) * outerRect.height;
		popupScrollPosn = GUI.BeginScrollView (
			outerRect,
			popupScrollPosn, 
			new Rect (0, 0, outerRect.width - 32, msgHeight),
			null, 
		    GUI.skin.verticalScrollbar
			);
		GUI.Label (
			new Rect (0, 0, outerRect.width - 32, msgHeight), 
			popupMessage, 
			style
			);
		GUI.EndScrollView ();

		if (GUI.Button (new Rect ((popupRect.width - 80) / 2, popupRect.height - 30 - bufferBorder, 80, 30), "OK")) {
			showPopup = false;
		}
	}

	/* This is the Multiplayer Convergence implementation. Doesn't work for single player Convergence 
	public void SubmitTarget ()
	{
		int attempt_id = 0;
		currAttemptTarget = new ConvergeAttempt (
			player_id,
			ecosystem_id, 
			attempt_id,
			allowHintsMaster,
			Constants.ID_NOT_SET,
			ecosystemList [ecosystem_idx].config_target,
			ecosystemList [ecosystem_idx].csv_target_string
		);

		Game.networkManager.Send (
			ConvergeNewAttemptProtocol.Prepare (
				player_id, 
				ecosystem_id + 1000,    // Offset by 1000 to seperate from player's turn This is only for DB     
				currAttemptTarget.attempt_id,
				currAttemptTarget.allow_hints,
				currAttemptTarget.hint_id,
				ecosystemList [ecosystem_idx].timesteps,
				currAttemptTarget.config),
			ProcessConvergeNewAttemptTarget
		);
		Debug.Log ("Submit RequestConvergeNewAttemptTarget");
	}
		
	public void ProcessConvergeNewAttemptTarget (NetworkResponse response)
	{
		Debug.Log ("ProcessConvergeNewAttemptTarget");
		ConvergeAttempt attempt;
		ResponseConvergeNewAttempt args = response as ResponseConvergeNewAttempt;
		attempt = args.attempt;

		//if the submission resulted in a valid attempt, add to attempt list and reinitialize 
		//currAttempt for next attempt.  Otherwise, keep current attempt
		if (attempt != null) {
			currAttemptTarget.attempt_id = attempt.attempt_id;
			currAttemptTarget.SetCSV (attempt.csv_string);
			graph2CSV = currAttemptTarget.csv_object;
			graphs.UpdateGraph2Data (graph2CSV);	
		} else {
			Debug.LogError ("Submission of new attempt failed to produce results for target.");
		}
	}
	*/
		
	public void Submit ()
	{
		Game.networkManager.Send (
			ConvergeNewAttemptProtocol.Prepare (
			player_id, 
			ecosystem_id, 
			currAttempt.attempt_id,
			currAttempt.allow_hints,
			currAttempt.hint_id,
			ecosystemList [ecosystem_idx].timesteps,
			currAttempt.config),
			ProcessConvergeNewAttempt
		);
		//Debug.Log ("Submit RequestConvergeNewAttempt");
	}

	private void GetPriorAttempts ()
	{
		//get attempts from server based on specified ecosystem
		attemptList = new List<ConvergeAttempt> ();
		attemptCount = 0;

		Game.networkManager.Send (
			ConvergePriorAttemptCountProtocol.Prepare (player_id, new_ecosystem_id),
			ProcessConvergePriorAttemptCount
		);
		//Debug.Log ("Send RequestConvergePriorAttemptCount, new_ecosystem_id = " + new_ecosystem_id);

	}
	
	public void ProcessConvergeEcosystems (NetworkResponse response)
	{
		ecosystemList = new List<ConvergeEcosystem> ();
		Debug.Log("Inside ProcessConvergeEcosystems");
		ResponseConvergeEcosystems args = response as ResponseConvergeEcosystems;
		// ecosystemList = args.ecosystemList;
		int size = args.ecosystemList.Count;
		Debug.Log("DB Ecosystem list size: " + size);
		for (int i = 0; i < size; i++) {
			// Debug.Log ("Eco #" + i + " id: " + args.ecosystemList[i].ecosystem_id);
			// Debug.Log ("Description: " + args.ecosystemList [i].description);
			// Debug.Log ("Timesteps: " + args.ecosystemList [i].timesteps);
			// Debug.Log ("Config_default: " + args.ecosystemList [i].config_default);
			// Debug.Log ("Config_target: " + args.ecosystemList [i].config_target);
			args.ecosystemList [i].description = "Eco #" + args.ecosystemList [i].ecosystem_id;  // Description is too long for buttons 
			args.ecosystemList [i].csv_default_string = "";
			args.ecosystemList [i].csv_target_string = "";
			ecosystemList.Add (args.ecosystemList [i]);
		}

		//set default ecosystem values
		new_ecosystem_id = Constants.ID_NOT_SET;
		ecosystem_idx = 0;
		SubmitTargetInit ();

		/* Moved after ProcessConvergeNewAttemptTargetInitT
		ecosystem_id = GetEcosystemId (ecosystem_idx);
		//get player's most recent prior attempts 
		GetPriorAttempts ();
		//create array of ecosystem descriptions
		if (ecosystemList != null) {
			ecosysDescripts = ConvergeEcosystem.GetDescriptions (ecosystemList);
		}
		GetHints ();
		ecosRcvd = true;
		*/
	}
		
	public void SubmitTargetInit ()
	{
		init_idx = ecosRcvd ? new_ecosystem_idx : ecosystem_idx;
		int attempt_id = 0;
		currAttemptTarget = new ConvergeAttempt (
			player_id,
			ecosystemList [init_idx].ecosystem_id,
			attempt_id,
			allowHintsMaster,
			Constants.ID_NOT_SET,
			ecosystemList [init_idx].config_default,
			""
			//ecosystemList [ecosystem_idx].csv_target_string
		);

		Game.networkManager.Send (
			ConvergeNewAttemptProtocol.Prepare (
				player_id, 
				ecosystem_id + 1000,    // Offset by 1000 to seperate from player's turn This is only for DB     
				currAttemptTarget.attempt_id,
				currAttemptTarget.allow_hints,
				currAttemptTarget.hint_id,
				ecosystemList [init_idx].timesteps,
				currAttemptTarget.config),
			ProcessConvergeNewAttemptTargetInitD
		);
		Debug.Log ("Submit RequestConvergeNewAttemptTargetInitD");
	}

	public void ProcessConvergeNewAttemptTargetInitD (NetworkResponse response)
	{
		Debug.Log ("ProcessConvergeNewAttemptTargetInitD");
		ConvergeAttempt attempt;
		ResponseConvergeNewAttempt args = response as ResponseConvergeNewAttempt;
		attempt = args.attempt;

		//if the submission resulted in a valid attempt, add to attempt list and reinitialize 
		//currAttempt for next attempt.  Otherwise, keep current attempt
		if (attempt != null) {
			currAttemptTarget.attempt_id = attempt.attempt_id;
			currAttemptTarget.SetCSV (attempt.csv_string);
			ecosystemList [init_idx].csv_default_string = attempt.csv_string;
		} else {
			Debug.LogError ("Submission of new attempt failed to produce results for target init default.");
		}
			
		int attempt_id = 1;
		currAttemptTarget = new ConvergeAttempt (
			player_id,
			ecosystemList [init_idx].ecosystem_id,
			attempt_id,
			allowHintsMaster,
			Constants.ID_NOT_SET,
			ecosystemList [init_idx].config_target,
			""
			//ecosystemList [ecosystem_idx].csv_target_string
		);

		Game.networkManager.Send (
			ConvergeNewAttemptProtocol.Prepare (
				player_id, 
				ecosystem_id + 1000,    // Offset by 1000 to seperate from player's turn This is only for DB     
				currAttemptTarget.attempt_id,
				currAttemptTarget.allow_hints,
				currAttemptTarget.hint_id,
				ecosystemList [init_idx].timesteps,
				currAttemptTarget.config),
			ProcessConvergeNewAttemptTargetInitT
		);
		Debug.Log ("Submit RequestConvergeNewAttemptTargetInitT");
	}
		
	public void ProcessConvergeNewAttemptTargetInitT (NetworkResponse response)
	{
		Debug.Log ("ProcessConvergeNewAttemptTargetInitT");
		ConvergeAttempt attempt;
		ResponseConvergeNewAttempt args = response as ResponseConvergeNewAttempt;
		attempt = args.attempt;

		//if the submission resulted in a valid attempt, add to attempt list and reinitialize 
		//currAttempt for next attempt.  Otherwise, keep current attempt
		if (attempt != null) {
			currAttemptTarget.attempt_id = attempt.attempt_id;
			currAttemptTarget.SetCSV (attempt.csv_string);
			ecosystemList [init_idx].csv_target_string = attempt.csv_string;
		} else {
			Debug.LogError ("Submission of new attempt failed to produce results for target init default.");
		}

		if (ecosRcvd) {
			GetPriorAttempts ();
		} else {
			ecosystem_id = GetEcosystemId (ecosystem_idx);
			//get player's most recent prior attempts 
			GetPriorAttempts ();
			//create array of ecosystem descriptions
			if (ecosystemList != null) {
				ecosysDescripts = ConvergeEcosystem.GetDescriptions (ecosystemList);
			}
			GetHints ();
			inputEcoStr = "" + (ecosystem_idx + 1);
			ecosRcvd = true;
		}
	}
		
	public void ProcessConvergeNewAttemptScore (NetworkResponse response)
	{
		ResponseConvergeNewAttemptScore args = response as ResponseConvergeNewAttemptScore;
		int status = args.status;
		//Debug.Log ("Processed ReponseConvergeNewAttemptScore, status = " + status);
	}
	
	public void ProcessConvergeNewAttempt (NetworkResponse response)
	{
		ConvergeAttempt attempt;
		ResponseConvergeNewAttempt args = response as ResponseConvergeNewAttempt;
		attempt = args.attempt;

		//if the submission resulted in a valid attempt, add to attempt list and reinitialize 
		//currAttempt for next attempt.  Otherwise, keep current attempt
		if (attempt != null && attempt.attempt_id != Constants.ID_NOT_SET) {
			currAttempt.attempt_id = attempt.attempt_id;
			currAttempt.SetCSV (attempt.csv_string);
			attemptList.Add (currAttempt);
			attemptCount = attemptList.Count;

			//calculate score and send back to server.
			CSVObject target = ecosystemList[ecosystem_idx].csv_target_object;
			int score = currAttempt.csv_object.CalculateScore (target);
			Game.networkManager.Send (
				ConvergeNewAttemptScoreProtocol.Prepare (
				player_id, 
				ecosystem_id, 
				attempt.attempt_id, 
				score
				),
				ProcessConvergeNewAttemptScore
				);

			//update pertinent variables with new data
			if (currAttempt.hint_id != Constants.ID_NOT_SET) {
				priorHintIdList.Add (currAttempt.hint_id);
			}
			//need to recalc reset slider config due to additional attempt
			isResetSliderInitialized = false;

			if (barGraph != null) {
				barGraph.InputToCSVObject (currAttempt.csv_string, manager);
			}

			currAttempt = new ConvergeAttempt (
				player_id, 
			    ecosystem_id, 
			    attempt.attempt_id + 1,
			    allowHintsMaster,
			    Constants.ID_NOT_SET,
			    attempt.config,
			    null
			    //manager
			);

			FinalizeAttemptUpdate (attemptCount - 1, false);
		} else {
			Debug.LogError ("Submission of new attempt failed to produce results.");
			SetIsProcessing (false);
		}
	}

	public void ProcessConvergePriorAttemptCount (NetworkResponse response)
	{
		ResponseConvergePriorAttemptCount args = response as ResponseConvergePriorAttemptCount;
		new_ecosystem_id = args.ecosystem_id;
		attemptCount = args.count;
		priorHintIdList = new List<int>();
		isResetSliderInitialized = false;

		temp_ecosystem_idx = ecosystemList.FindIndex (entry => entry.ecosystem_id == new_ecosystem_id);

		if ((temp_ecosystem_idx >= 0) && (ecosystemList [temp_ecosystem_idx].csv_target_string.Length == 0)) {
			int attempt_id = 2;
			currAttemptTarget = new ConvergeAttempt (
				player_id,
				ecosystemList [temp_ecosystem_idx].ecosystem_id,
				attempt_id,
				allowHintsMaster,
				Constants.ID_NOT_SET,
				ecosystemList [temp_ecosystem_idx].config_target,
				""
				//ecosystemList [ecosystem_idx].csv_target_string
			);

			Game.networkManager.Send (
				ConvergeNewAttemptProtocol.Prepare (
					player_id, 
					ecosystem_id + 1000,    // Offset by 1000 to seperate from player's turn This is only for DB     
					currAttemptTarget.attempt_id,
					currAttemptTarget.allow_hints,
					currAttemptTarget.hint_id,
					ecosystemList [temp_ecosystem_idx].timesteps,
					currAttemptTarget.config),
				ProcessConvergeNewAttemptTarget
			);
			Debug.Log ("Submit RequestConvergeNewAttemptTarget");
		} else {
			ProcessConvergePriorAttemptCount2 ();
		}
			
		/*  Moved to ProcessConvergePriorAttemptCount2()
		// once count of attempts has been received, send requests for individual attempt's data
		for (int attemptOffset = 0; attemptOffset < attemptCount; attemptOffset++) {
			Game.networkManager.Send (
				ConvergePriorAttemptProtocol.Prepare (player_id, new_ecosystem_id, attemptOffset),
				ProcessConvergePriorAttempt
			);
			//Debug.Log ("Send RequestConvergePriorAttempt");
		}

		UpdateEcosystemIds ();

		//if no prior attempts found create new curAttempt based on default config
		//of curr ecosystem
		if (attemptCount == 0) {
			FinalizeLoadPriorAttempts ();
		}
		*/
	}
		

	public void ProcessConvergeNewAttemptTarget (NetworkResponse response)
	{
		Debug.Log ("ProcessConvergeNewAttemptTarget");
		ConvergeAttempt attempt;
		ResponseConvergeNewAttempt args = response as ResponseConvergeNewAttempt;
		attempt = args.attempt;

		//if the submission resulted in a valid attempt, add to attempt list and reinitialize 
		//currAttempt for next attempt.  Otherwise, keep current attempt
		if (attempt != null) {
			currAttemptTarget.attempt_id = attempt.attempt_id;
			currAttemptTarget.SetCSV (attempt.csv_string);
			ecosystemList [temp_ecosystem_idx].csv_target_string = attempt.csv_string;
			ProcessConvergePriorAttemptCount2 ();
		} else {
			Debug.LogError ("Submission of new attempt failed to produce results for target init default.");
		}
	}

	public void ProcessConvergePriorAttemptCount2 () {
		//once count of attempts has been received, send requests for individual attempt's data
		for (int attemptOffset = 0; attemptOffset < attemptCount; attemptOffset++) {
			Game.networkManager.Send (
				ConvergePriorAttemptProtocol.Prepare (player_id, new_ecosystem_id, attemptOffset),
				ProcessConvergePriorAttempt
			);
			//Debug.Log ("Send RequestConvergePriorAttempt");
		}

		UpdateEcosystemIds ();

		//if no prior attempts found create new curAttempt based on default config
		//of curr ecosystem
		if (attemptCount == 0) {
			FinalizeLoadPriorAttempts ();
		}
	}


	public void ProcessConvergePriorAttempt (NetworkResponse response)
	{

		ResponseConvergePriorAttempt args = response as ResponseConvergePriorAttempt;
		ConvergeAttempt attempt = args.attempt;

		//add new attempt to list if response includes an attempt
		if (attempt.attempt_id == Constants.ID_NOT_SET) {
			Debug.LogError ("attempt_id not valid in ProcessConvergePriorAttempt");
		} else  {    
			attemptList.Add (attempt);
			if (attempt.hint_id != Constants.ID_NOT_SET) {
				priorHintIdList.Add (attempt.hint_id);
			}
		}

		//Once all attempts have been processed, finalize.
		if (attemptList.Count == attemptCount) {
			FinalizeLoadPriorAttempts ();
		}

	}

	private void UpdateEcosystemIds ()
	{
		//If ecosystem_id is not set, use default (ecosystem 0)
		if (new_ecosystem_id == Constants.ID_NOT_SET) {
			new_ecosystem_idx = 0;
			new_ecosystem_id = GetEcosystemId (new_ecosystem_idx);
		} else {
			//otherwise, set new index based on selected ecosystem id
			new_ecosystem_idx = ecosystemList.FindIndex (entry => entry.ecosystem_id == new_ecosystem_id);
		}

		//update current ecosystem info 
		ecosystem_id = new_ecosystem_id;
		ecosystem_idx = new_ecosystem_idx;
	}

	//after all prior attempts have been retrieved from server, finalize
	private void FinalizeLoadPriorAttempts ()
	{
		//if this is the first load, whether or not to allow hints for this player
		//has not been established; determine randomly if no prior, o/w use prior
		//attempt.
		if (!allowHintsConfigured) {
			if (attemptCount == 0) {
				allowHintsMaster = Functions.RandomBoolean ();
			} else {
				allowHintsMaster = attemptList [attemptCount - 1].allow_hints;
			}
			allowHintsConfigured = true;
		}

		//if count is 0, base next attempt info on ecosystem default
		if (attemptCount == 0) {
			int attempt_id = 0;
			currAttempt = new ConvergeAttempt (
				player_id,
				ecosystem_id, 
			    attempt_id,
			    allowHintsMaster,
			    Constants.ID_NOT_SET,
			    ecosystemList [ecosystem_idx].config_default,
			    ecosystemList [ecosystem_idx].csv_default_string
			    //manager
			);
			//otherwise, base next attempt info on immediate prior attempt
		} else {
			int attempt_id = attemptList [attemptCount - 1].attempt_id + 1;
			currAttempt = new ConvergeAttempt (
				player_id, 
			    ecosystem_id, 
			    attempt_id,
			    allowHintsMaster,
			    Constants.ID_NOT_SET,
			    attemptList [attemptCount - 1].config,
			    attemptList [attemptCount - 1].csv_string
			    //manager
			);
		}


		FinalizeAttemptUpdate (attemptCount - 1, true);
	}

	//reset currattempt based on requested attempt (or default)
	private void ResetCurrAttempt (int attemptIdx)
	{
		//if count is 0, base next attempt info on ecosystem default
		int attempt_id = currAttempt.attempt_id;
		int hint_id = currAttempt.hint_id;  //if player received a hint, retain history.
//		if (attemptCount > 0) {
//			attempt_id = attemptList [attemptCount - 1].attempt_id + 1;
//		} else {
//			attempt_id = 0;
//		}
		if (attemptIdx == Constants.ID_NOT_SET) {
			currAttempt = new ConvergeAttempt (
				player_id, 
		        ecosystem_id, 
		        attempt_id,
		        allowHintsMaster,
		        hint_id,
		        ecosystemList [ecosystem_idx].config_default,
			    ecosystemList [ecosystem_idx].csv_default_string
			    //manager
			);
		} else {
			currAttempt = new ConvergeAttempt (
				player_id, 
		        ecosystem_id, 
		        attempt_id,
		        allowHintsMaster,
			    hint_id,
			    attemptList [attemptIdx].config,
			    attemptList [attemptIdx].csv_string
			    //manager
			);
		}

		FinalizeAttemptUpdate (attemptIdx, false);
	}

	//update variables and graph configuration as necessary with updated attempt info
	private void FinalizeAttemptUpdate (int attemptIdx, bool newEcosystem)
	{
		referenceAttemptIdx = attemptIdx;

		if (newEcosystem) {
			manager = new ConvergeManager ();
		}

		//refresh or replace graphs as appropriate
		GenerateGraphs (newEcosystem);

		//extract data entry field data 
		//note: has dependency on manager info generated in graphs, 
		//so has to appear following GenerateGraphs
		currAttempt.ParseConfig (manager);

		SetIsProcessing (false);
	}
	
	public void SetActive (bool active)
	{
		this.isActive = active;
	}

	private void ReadConvergeEcosystemsFile ()
	{
		ResponseConvergeEcosystems response = new ResponseConvergeEcosystems ();
		
		string filename = "converge-ecosystems";
		ecosystemList = new List<ConvergeEcosystem> ();
		
		
		if (!File.Exists (filename)) {
			Debug.LogError (filename + " not found.");
		} else {
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
				using (BinaryReader br = new BinaryReader(fs, Encoding.UTF8)) {
					int size = br.ReadInt16 ();
					int responseId = br.ReadInt16 ();
					int ecosystemCnt = br.ReadInt16 ();

					for (int i = 0; i < ecosystemCnt; i++) {
						int ecosystem_id = br.ReadInt32 ();
						Debug.Log("**** ecosystem_id: " + ecosystem_id);

						ConvergeEcosystem ecosystem = new ConvergeEcosystem (ecosystem_id);
						int fldSize = br.ReadInt16 ();
						ecosystem.description = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));
						ecosystem.description = "Eco #" + ecosystem_id;  // Description is too long for buttons 
						ecosystem.timesteps = br.ReadInt32 ();
						fldSize = br.ReadInt16 ();
						ecosystem.config_default = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));
						fldSize = br.ReadInt16 ();
						ecosystem.config_target = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));
						fldSize = br.ReadInt16 ();
						ecosystem.csv_default_string = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));
						fldSize = br.ReadInt16 ();
						ecosystem.csv_target_string = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));
						
						ecosystemList.Add (ecosystem);
					}
					
					//set initial ecosystem id
					if (ecosystemList.Count == 0) {
						Debug.LogError ("No converge ecosystems found.");
					}

					response.ecosystemList = ecosystemList;
					
				}
			}
		}
	}

	private void GenerateGraphs (bool newEcosystem)
	{
		string title = (
			referenceAttemptIdx == Constants.ID_NOT_SET ? 
			"Initial config" : 
			String.Format ("Attempt #{0}", referenceAttemptIdx + 1)
			);  //hide 0 start from user

		graph1CSV = (referenceAttemptIdx == Constants.ID_NOT_SET) ? 
			ecosystemList [ecosystem_idx].csv_default_object : 
				attemptList [referenceAttemptIdx].csv_object;
		
		if (newEcosystem) {			
			//destroy prior bargraph if it exists
			//Note: barGraph is regenerated when requested
			if (barGraph != null) {
				Destroy (barGraph);
				barGraph = null;
			}

			GenerateFoodWeb ();
			
			graphs = new GraphsCompare (
				graph1CSV, 
				ecosystemList [ecosystem_idx].csv_target_object, 
				// graph1CSV,
				leftGraph, 
				topGraph, 
				widthGraph,
				heightGraph,
				title,
				"Target Graph",
				ecosystemList [ecosystem_idx].timesteps,
				foodWeb,
				manager
			);

			// SubmitTarget ();

		} else {
			graphs.UpdateGraph1Data (graph1CSV, title);
		}
		
	}

	private int GetEcosystemId (int idx)
	{
		return ecosystemList [idx].ecosystem_id;

	}

	private void GenerateBarGraph ()
	{
		if (barGraph == null) {
			barGraph = gameObject.AddComponent<BarGraph> ().GetComponent<BarGraph> ();
			barGraph.SetLegendActive (true);

			//first object must be target, then default 
			barGraph.InputToCSVObject (ecosystemList [ecosystem_idx].csv_target_string, manager);
			barGraph.InputToCSVObject (ecosystemList [ecosystem_idx].csv_default_string, manager);

			//followed by all of the player's prior attempts
			for (int i = 0; i < attemptCount; i++) {
				barGraph.InputToCSVObject (attemptList [i].csv_string, manager);
			}
		}
		barGraph.SetActive (true);

	}

	private void GenerateFoodWeb ()
	{
		//reset gamestate for new ecosystem
		GameState gs = GameObject.Find ("Global Object").GetComponent<GameState> ();
		gs.speciesList = new Dictionary<int, Species> ();
		keyList = new List<int> ();

		//loop through species in ecosystem and add to gamestate species list
		List <string> ecosystemSpecies = new List<string> (currAttempt.csv_object.csvList.Keys);
		foreach (string name in ecosystemSpecies) {
            //find name in species table
			SpeciesData species = SpeciesTable.GetSpecies (name);
			if (species == null) {
				Debug.LogError ("Failed to create Species '" + name + "'");
				continue;
			}

			gs.CreateSpecies (Constants.ID_NOT_SET, species.biomass, species.name, species);
			keyList.Add (species.species_id);
		}
			
		//generate foodWeb if not present
		if (foodWeb == null) {
			foodWeb = Database.NewDatabase (
				GameObject.Find ("Global Object"), 
		        Constants.MODE_CONVERGE_GAME,
				manager
			);
		} else {
			foodWeb.manager = manager;
		}
			
		keyList.Sort();
		speciesStr = "" + keyList [0];
		for (int i = 1; i < keyList.Count; i++) {
			speciesStr += " " + keyList [i];
		}

		Debug.Log ("speciesStr = :" + speciesStr + ":");
	}

	private void SetIsProcessing (bool isProc)
	{
		this.isProcessing = isProc;
		blinkOn = true;
		if (isProc) {
			StartCoroutine ("BlinkProcessing");
		} else {
			StopCoroutine ("BlinkProcessing");
		}
	}

	//function to blink the text
	private IEnumerator BlinkProcessing ()
	{
		while (true) {
			//set the Text's text to blank
			blinkOn = true;
			//display blank text for 0.5 seconds
			yield return new WaitForSeconds (.5f);
			//display “I AM FLASHING TEXT” for the next 0.5 seconds
			blinkOn = false;
			yield return new WaitForSeconds (.5f);
		}
	}
	
	public void ProcessLogout (NetworkResponse response)
	{
		ResponseLogout args = response as ResponseLogout;
		//act on logout regardless of response
		Application.Quit ();
		Game.SwitchScene ("Login");
	}

	void OnDestroy ()
	{
	}
	
	public void GetHints ()
	{
		hintCount = 0;
		
		Game.networkManager.Send (
			ConvergeHintCountProtocol.Prepare (),
			ProcessConvergeHintCount
			);
		//Debug.Log ("Send RequestConvergeHintCount");
	} 
	
	public void ProcessConvergeHintCount (NetworkResponse response)
	{
		ResponseConvergeHintCount args = response as ResponseConvergeHintCount;
		hintCount = args.count;
		
		//once count of hints has been received, send requests for individual hint's data
		for (int hintOffset = 0; hintOffset < hintCount; hintOffset++) {
			Game.networkManager.Send (
				ConvergeHintProtocol.Prepare (hintOffset),
				ProcessConvergeHint
				);
			//Debug.Log ("Send RequestConvergeHint");
		}
	}

	
	public void ProcessConvergeHint (NetworkResponse response)
	{
		
		ResponseConvergeHint args = response as ResponseConvergeHint;
		ConvergeHint hint = args.hint;
		
		if (hint == null) {
			Debug.LogError ("Returned hint not valid in ProcessConvergeHint");
		} else {
			hintDict.Add (hint.hintId, hint.text);
			//Debug.Log ("adding hint: " + hint.text);
		}
	}
	
	public int GetRandomHint (bool excludePrior) {
		int hintIdx;
		int isPrior;
		List <int> available = new List<int> ();
		
		//create set of available ids
		foreach (int id in hintDict.Keys) {
			int found = priorHintIdList.Find (entry => entry.Equals (id));
			if (!excludePrior || priorHintIdList.IndexOf (id) == Constants.ID_NOT_SET) {
				available.Add (id);
			}
		}
		
		//return -1 if no available hints found
		if (available.Count == 0) {
			return Constants.ID_NOT_SET;
		}
		//select hint randomly
		hintIdx = UnityEngine.Random.Range (0, available.Count - 1);
		
		return available [hintIdx];
	}

}