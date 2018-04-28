using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

// RR does not work presently. It has been commented out. The comment statement denotes the lines changed
// RR comment out

public class MultiplayerGame {
  public String name;
  public int numRooms = 0;
  public MultiplayerGame(String name) {
    this.name = name;
  }
}
public class MultiplayerGames : MonoBehaviour {
  private System.Collections.Generic.Dictionary<String, MultiplayerGame> games = new System.Collections.Generic.Dictionary<String, MultiplayerGame>();
  private MultiplayerGame activeGame;

  // Window Properties
  private float width = 800;
  private float height = 600;
  private float heightOffset = 50;    // DH

  // Multiplayer Convergence Window Properties    DH
  private float widthMC = 550;
  private float heightMC;       // Set based upon left over space
  private float heightMCSpaceTop = 75;
  private float heightMCSpaceBot = 25;

  // DH  Other additions
  private bool enableHostEntry = false;
  private bool hostEntryError = false;
  private float widthConfig;
  private float heightConfig;
  private float topConfig;
  private float leftConfig;
  private int pixelPerChar = 15;
  private Texture2D bgTexture;
  // Parameters that host specifies
  private short numPlayers = 0;
  private string numPlayersS = "";
  private short numRounds = 0;
  private string numRoundsS = "";
  private short timeWindow = 0;
  private string timeWindowS = "";
  private short betAmount = 0;
  private string betAmountS = "";
  private short ecoCount = 0;
  private short ecoNumber = 0;
  private string ecoNumberS = "";
  private short allowSliders = 0;    // 1 = display *** above sliders
  private string allowSlidersS = "";
  private bool isInitial = true;
  private int host_config_id = Constants.CONVERGE_HOST_CONFIG;
  private Font font;
  private short host;
  private string tStr = "5";

  // Other
  private int window_id, window_idMC;
  private string message = "Single Player Game. Click start to play.";
  private Rect windowRect, windowRectMC, windowRectConfig;
  private GameObject mainObject;

  private bool enableRRButton = true;
  private bool enableCWButton = true;
  private bool enableMCButton = true;
  private bool enableSDButton = true;

  private bool quiting = false;
  private bool waiting = false;

  private int room_id = 0;
  private Room room;

    public Dictionary<string, MultiplayerGame> Games
    {
        get
        {
            return games;
        }

        set
        {
            games = value;
        }
    }
    private void setActiveGame(MultiplayerGame game) {
      activeGame = game;
    }

    void Awake() {
      mainObject = GameObject.Find("MainObject");
      window_id = Constants.GetUniqueID();
      window_idMC = Constants.GetUniqueID();

      Game.networkManager.Listen (NetworkCode.PAIR, OnPairResult);
      Game.networkManager.Listen (NetworkCode.QUIT_ROOM, OnQuitRoomResult);

      // DH other values 
      widthConfig = Screen.width * 0.80f;
      heightConfig = Screen.height * 0.80f;
      leftConfig = (Screen.width - widthConfig) / 2;
      topConfig = (Screen.height - heightConfig) / 2;
      bgTexture = Resources.Load<Texture2D> (Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
      font = Resources.Load<Font> ("Fonts/" + "Chalkboard");
      windowRectConfig = new Rect (leftConfig, topConfig, widthConfig, heightConfig);

      Application.runInBackground = false;     // Is this ok?
  }

  void OnDestroy () {
    Game.networkManager.Ignore (NetworkCode.PAIR, OnPairResult);
    Game.networkManager.Ignore (NetworkCode.QUIT_ROOM, OnQuitRoomResult);
  }

  // Use this for initialization
  void Start() {
    Game.StartEnterTransition ();
    // ReadConvergeEcosystemsFileCount();
	Game.networkManager.Send(ConvergeEcosystemsProtocol.Prepare(),ProcessConvergeEcosystems);

    windowRect = new Rect ((Screen.width - width) / 2, Screen.height - height - heightOffset, width, height);

    heightMC = Screen.height - height - heightOffset - heightMCSpaceTop - heightMCSpaceBot;
    windowRectMC = new Rect ((Screen.width - widthMC) / 2, heightMCSpaceTop, widthMC, heightMC);

    StartCoroutine(RequestGetRooms(1f));
    Games.Add("Multiplayer Convergence", new MultiplayerGame("Multiplayer Convergence"));
    // Games.Add("Running Rhino", new MultiplayerGame("Running Rhino"));     // RR comment out
    Games.Add("Sea Divided", new MultiplayerGame("Sea Divided"));
    Games.Add("Cards of Wild", new MultiplayerGame("Cards of Wild"));
    setActiveGame(games["Multiplayer Convergence"]);
  }

  public void ProcessConvergeEcosystems (NetworkResponse response)
  {
    Debug.Log("Inside ProcessConvergeEcosystems");
	ResponseConvergeEcosystems args = response as ResponseConvergeEcosystems;
	ecoCount = (short) args.ecosystemList.Count;
  }
		
  void OnGUI() {

    /*
    if (enableHostEntry) {    
      windowRectConfig = GUI.Window (host_config_id, windowRectConfig, MakeWindowHost, "Host Configuration Entry", GUIStyle.none);
    } else {
      Color newColor = new Color(1,1,1,1.0f);
      GUI.color = newColor;
      windowRect = GUILayout.Window(window_id, windowRect, MakeWindow, "Game Rooms");
      windowRectMC = GUILayout.Window(window_idMC, windowRectMC, MakeWindowMC, "Multiplayer Convergence Game Rooms");
    }
    */

    Color newColor = new Color(1,1,1,1.0f);
    GUI.color = newColor;
    windowRect = GUILayout.Window(window_id, windowRect, MakeWindow, "Game Rooms");
    // windowRectMC = GUILayout.Window(window_idMC, windowRectMC, MakeWindowMC, "Multiplayer Convergence Game Rooms");
  }
  void drawTabHeaders() {
    // create the colors for the buttons.
    const float DarkGray = 0.4f;
    const float LightGray = 0.9f;
    Color highlightCol = new Color(LightGray, LightGray, LightGray);
    Color bgCol = new Color(DarkGray, DarkGray, DarkGray);

    // create a style for the buttons
    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
    buttonStyle.padding.bottom = 8;
    buttonStyle.padding.top = 8;
    buttonStyle.margin.left = 0;
    buttonStyle.margin.right = 0;
    buttonStyle.margin.top = 0;
    buttonStyle.margin.bottom = 0;
    GUI.backgroundColor = bgCol;

    updateGameCounts();

    // actually draw the headers
    GUILayout.BeginHorizontal();
    foreach (System.Collections.Generic.KeyValuePair<String, MultiplayerGame> entry in this.Games) {
      GUI.backgroundColor = activeGame == entry.Value ? highlightCol : bgCol;
      String buttonText = entry.Key;
      if (games[entry.Key].numRooms != 0) {
        buttonText = buttonText + " (" + games[entry.Key].numRooms.ToString() + ")";
      }
      if (GUILayout.Button(buttonText, buttonStyle)) {
        setActiveGame(entry.Value);
      }
    }
    GUILayout.EndHorizontal();
  }

  // updates the number of rooms for every MultiplayerGame so that we can display it to the user.
  private void updateGameCounts() {
    System.Collections.Generic.Dictionary<String, int> counts = new System.Collections.Generic.Dictionary<String, int>();
    foreach (System.Collections.Generic.KeyValuePair<String, MultiplayerGame> entry in this.Games) {
      entry.Value.numRooms = 0;
    }
    // loop over every game
    foreach (var item in RoomManager.getInstance().getRooms()) {
      // increase the count of that specific game
      String key = Room.getGameName(item.Value.game_id);
      if (counts.ContainsKey(key)) {
        counts[key]++;
      } else {
        counts[key] = 1;
      }
    }
    foreach (var key in counts.Keys) {
      // Debug.Log("updating roomcount for " + key + " to " + counts[key]);
      this.games[key].numRooms = counts[key];
    }
  }
  void MakeWindow(int id) {
    Color newColor = new Color(1,1,1,1.0f);
    GUI.color = newColor;

    // draw the headers of the tabs.
    drawTabHeaders();
    GUILayout.Space(10);

    // draw the headers themselves
    GUIStyle style = new GUIStyle();
    style.alignment = TextAnchor.MiddleCenter;
    style.normal.textColor = Color.white;
    drawLabels();

    // draw the game rooms
    if (activeGame.name != "Multiplayer Convergence") {
      drawGameRows();
    } else {
      drawGameRowsConvergence();
    }
    GUILayout.Space(30);
    drawFooter();
    GUI.BringWindowToFront(window_id);
    GUI.DragWindow();
  }
  private void drawLabels() {
    if (activeGame.name == "Multiplayer Convergence") {
      // labels for Convergence.
      GUILayout.BeginHorizontal();
      GUILayout.Label(new GUIContent("  "));
      GUILayout.Label(new GUIContent("   Total "));
      GUILayout.Label(new GUIContent("  Number "));
      GUILayout.Label(new GUIContent(" Number "));
      GUILayout.Label(new GUIContent(" Secs/ "));
      GUILayout.Label(new GUIContent("  Bet"));
      GUILayout.Label(new GUIContent(" Eco"));
      GUILayout.Label(new GUIContent("        "));
      GUILayout.Label(new GUIContent("        "));
      GUILayout.Label(new GUIContent(""), GUILayout.Width(40));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label(new GUIContent(" #"));
      GUILayout.Label(new GUIContent(" Players"));
      GUILayout.Label(new GUIContent("Joined "));
      GUILayout.Label(new GUIContent("  Rounds"));
      GUILayout.Label(new GUIContent(" Round "));
      GUILayout.Label(new GUIContent(" Amt"));
      GUILayout.Label(new GUIContent("  # "));
      GUILayout.Label(new GUIContent("Hints?"));
      GUILayout.Label(new GUIContent("Host"));
      GUILayout.Label(new GUIContent(""), GUILayout.Width(40));
      GUILayout.EndHorizontal();
    } else {
      // Labels for other games.
      GUILayout.BeginHorizontal();
      GUILayout.Label(new GUIContent("#"));
      GUILayout.Label(new GUIContent("         Game"));
      GUILayout.Label(new GUIContent("         Status"));
      GUILayout.Label(new GUIContent("    Host"));
      GUILayout.Label(new GUIContent(""), GUILayout.Width(100));
      GUILayout.EndHorizontal();
    }
  }
  private void drawGameRowsConvergence() {
    foreach(var item in RoomManager.getInstance().getRooms()) {
      if (item.Value.game_id == Constants.MINIGAME_MULTI_CONVERGENCE) {
        string nR = "   " + item.Value.numRounds.ToString();
        nR = nR.Substring (nR.Length - 3);
        string sR = item.Value.secPerRound.ToString();
        if (item.Value.secPerRound < 100) {
          sR = "  " + sR;
        }
        string bA = item.Value.betAmt.ToString();
        if (item.Value.betAmt < 100) {
          bA = "  " + bA;
        }
        GUILayout.BeginHorizontal();
          GUILayout.Label(new GUIContent("" + item.Key));
          GUILayout.Label(new GUIContent("   " + item.Value.totalPlayers));
          GUILayout.Label(new GUIContent("           " + item.Value.players.Count));
          GUILayout.Label(new GUIContent("           " + nR));
          GUILayout.Label(new GUIContent("        " + sR));
          GUILayout.Label(new GUIContent("      " + bA));
          GUILayout.Label(new GUIContent("    " + item.Value.ecoNum));
          GUILayout.Label(new GUIContent("     " + (item.Value.helps == 1 ? "Y" : "N") + "   "));
          GUILayout.Label(new GUIContent("  " + item.Value.host));

          if (item.Value.containsPlayer(GameState.account.account_id)) {
            if(GUILayout.Button(new GUIContent("Quit"), GUILayout.Width(40))) {
              Game.networkManager.Send (QuitRoomProtocol.Prepare ());
            }
          } else {
            GUI.enabled = !this.waiting;
            if(GUILayout.Button(new GUIContent("Join"), GUILayout.Width(40))) {
              Game.networkManager.Send (PairProtocol.Prepare (item.Value.game_id, item.Value.id));
            }
            GUI.enabled = true;
          }
        GUILayout.EndHorizontal();
      }
    }

    if (enableHostEntry) {
      // Block for user entry
      // GUIStyle style2 = new GUIStyle();
      // style2.normal.textColor = Color.red;

      GUILayout.BeginHorizontal();
      GUILayout.Label(new GUIContent("           "));
      numPlayersS = GUILayout.TextField(numPlayersS, 1,  GUILayout.Width(20));
      GUILayout.Label(new GUIContent("         " + 0));
      GUILayout.Label(new GUIContent("           "));
      numRoundsS = GUILayout.TextField(numRoundsS, 2,  GUILayout.Width(30));
      GUILayout.Label(new GUIContent("       "));
      timeWindowS = GUILayout.TextField(timeWindowS, 3,  GUILayout.Width(35));
      GUILayout.Label(new GUIContent("   "));
      betAmountS = GUILayout.TextField(betAmountS, 3, GUILayout.Width(35));
      GUILayout.Label(new GUIContent("  "));
      ecoNumberS = GUILayout.TextField(ecoNumberS, 2,  GUILayout.Width(25));
      GUILayout.Label(new GUIContent("   "));
      allowSlidersS = GUILayout.TextField(allowSlidersS, 1,  GUILayout.Width(20));
      GUILayout.Label(new GUIContent("    " + GameState.player.GetName()));
      if(GUILayout.Button(new GUIContent("Enter"), GUILayout.Width(50))) {
        SubmitHostConfig();
      }
      GUILayout.EndHorizontal();

      // Ranges of valid entries displayed next 
      GUILayout.BeginHorizontal();
      GUILayout.Label(new GUIContent("          "));
      GUILayout.Label(new GUIContent(" 2-5 "));
      GUILayout.Label(new GUIContent("             "));
      GUILayout.Label(new GUIContent("       "));
      GUILayout.Label(new GUIContent(" 5-50 "));
      GUILayout.Label(new GUIContent("     "));
      GUILayout.Label(new GUIContent(" 30-180"));
      GUILayout.Label(new GUIContent(" "));
      GUILayout.Label(new GUIContent("10-200 "));
      GUILayout.Label(new GUIContent(" "));
      GUILayout.Label(new GUIContent("0-" + (ecoCount-1) + " "));
      GUILayout.Label(new GUIContent(" "));
      GUILayout.Label(new GUIContent(" Y/N "));
      GUILayout.Label(new GUIContent("             "));
      GUILayout.Label(new GUIContent(""), GUILayout.Width(50));
      GUILayout.EndHorizontal();


      if (hostEntryError) {
        GUIStyle styleLocal = new GUIStyle();
        styleLocal.normal.textColor = Color.red;
        GUI.Label (new Rect (10, windowRectMC.height - 70, 500, 30), 
          "The entry with a '?' is out of range. Please fix. Then press 'Enter'", styleLocal);
      } else {
        GUI.Label (new Rect (10, windowRectMC.height - 70, 500, 30), 
          "Update values as desired. Range of valid entry given below entry box. Press 'Enter'");
      }
    }
  }
  private void drawGameRows() {
    foreach (var item in RoomManager.getInstance().getRooms()) {
      // only draw the games for the currently selected tab.
      if (activeGame.name == Room.getGameName(item.Value.game_id)) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("" + item.Key));
        GUILayout.Label(new GUIContent(Room.getGameName(item.Value.game_id)));
        GUILayout.Label(new GUIContent(item.Value.status()));
        GUILayout.Label(new GUIContent(item.Value.host));
        if (item.Value.containsPlayer(GameState.account.account_id)) {
          if(GUILayout.Button(new GUIContent("Quit"), GUILayout.Width(100))) {
            Game.networkManager.Send (QuitRoomProtocol.Prepare ());
          }
        } else {
          GUI.enabled = !this.waiting;
          if(GUILayout.Button(new GUIContent("Join"), GUILayout.Width(100))) {
            Game.networkManager.Send (PairProtocol.Prepare (item.Value.game_id, item.Value.id));
          }
          GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
      }
    }
  }

  private void drawFooter() {
   
    GUI.enabled = enableRRButton;
    if (activeGame.name == "Running Rhino") {
      if (GUI.Button(new Rect(10, windowRect.height - 40, 140, 30), "Host Running Rhino")) {
        Game.networkManager.Send (PairProtocol.Prepare (Constants.MINIGAME_RUNNING_RHINO, -1));
		Game.networkManager.Send(ChatProtocol.Prepare (GameState.player.GetName(), "I just hosted Running Rhino. Do you want to join?"));
      }
    }

    GUI.enabled = enableCWButton;
    if (activeGame.name == "Cards of Wild") {
      if (GUI.Button(new Rect(10, windowRect.height - 40, 140, 30), "Host Cards of Wild")) {
        Game.networkManager.Send (PairProtocol.Prepare (Constants.MINIGAME_CARDS_OF_WILD, -1));
		Game.networkManager.Send(ChatProtocol.Prepare (GameState.player.GetName(), "I just hosted Cards of Wild. Do you want to join?"));
      }      
    }
    
    GUI.enabled = enableSDButton;
    if (activeGame.name == "Sea Divided") {
      if (GUI.Button(new Rect(10, windowRect.height - 40, 140, 30), "Host Sea Divided")) {
        Game.networkManager.Send (PairProtocol.Prepare (Constants.MINIGAME_SEA_DIVIDED, -1));
		Game.networkManager.Send(ChatProtocol.Prepare (GameState.player.GetName(), "I just hosted Sea Divided. Do you want to join?"));
      }
    }

    GUI.enabled = enableMCButton;
    
    if (activeGame.name == "Multiplayer Convergence") {
        // DH change
      if (GUI.Button(new Rect(10, windowRect.height - 40, 140, 30), "Host Convergence")) {
        if (!enableHostEntry) {
          // isInitial = true;
          enableHostEntry = true;
          hostEntryError = false;

          // set default values 
          numPlayersS = "2";
          numRoundsS = "10";
          timeWindowS = "60";
          betAmountS = "20";
          ecoNumberS = "0";
          allowSlidersS = "Y";
        }
      }
    }
    if (GUI.Button(new Rect(windowRect.width - 110, windowRect.height - 40, 100, 30), "Quit")) {
      GameObject.Find("Local Object").GetComponent<WorldMouse>().popOversEnabled = true;
      GameObject.Find("MenuScript").GetComponent<MenuScript>().menuOpen = false;
      GameObject.Find("MenuScript").GetComponent<MenuScript>().enableDropdown();
      Quit();
    }
    GUI.enabled = true;
  }


  /*
  void MakeWindowMC(int id) {
    Color newColor = new Color(1,1,1,1.0f);
    GUI.color = newColor;

    GUILayout.Space(10);

    GUIStyle style = new GUIStyle();
    style.alignment = TextAnchor.MiddleCenter;
    style.normal.textColor = Color.white;

    GUILayout.BeginHorizontal();
    GUILayout.Label(new GUIContent("  "));
    GUILayout.Label(new GUIContent("   Total "));
    GUILayout.Label(new GUIContent("  Number "));
    GUILayout.Label(new GUIContent(" Number "));
    GUILayout.Label(new GUIContent(" Secs/ "));
    GUILayout.Label(new GUIContent("  Bet"));
    GUILayout.Label(new GUIContent(" Eco"));
    GUILayout.Label(new GUIContent("        "));
    GUILayout.Label(new GUIContent("        "));
    GUILayout.Label(new GUIContent(""), GUILayout.Width(40));
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Label(new GUIContent(" #"));
    GUILayout.Label(new GUIContent(" Players"));
    GUILayout.Label(new GUIContent("Joined "));
    GUILayout.Label(new GUIContent("  Rounds"));
    GUILayout.Label(new GUIContent(" Round "));
    GUILayout.Label(new GUIContent(" Amt"));
    GUILayout.Label(new GUIContent("  # "));
    GUILayout.Label(new GUIContent("Helps?"));
    GUILayout.Label(new GUIContent("Host"));
    GUILayout.Label(new GUIContent(""), GUILayout.Width(40));
    GUILayout.EndHorizontal();

    foreach(var item in RoomManager.getInstance().getRooms()) {
      if (item.Value.game_id == Constants.MINIGAME_MULTI_CONVERGENCE) {
        string nR = "   " + item.Value.numRounds.ToString();
        nR = nR.Substring (nR.Length - 3);
        string sR = item.Value.secPerRound.ToString();
        if (item.Value.secPerRound < 100) {
          sR = "  " + sR;
        }
        string bA = item.Value.betAmt.ToString();
        if (item.Value.betAmt < 100) {
          bA = "  " + bA;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("" + item.Key));
        GUILayout.Label(new GUIContent("   " + item.Value.totalPlayers));
        GUILayout.Label(new GUIContent("           " + item.Value.players.Count));
        GUILayout.Label(new GUIContent("           " + nR));
        GUILayout.Label(new GUIContent("        " + sR));
        GUILayout.Label(new GUIContent("      " + bA));
        GUILayout.Label(new GUIContent("    " + item.Value.ecoNum));
        GUILayout.Label(new GUIContent("     " + (item.Value.helps == 1 ? "Y" : "N") + "   "));
        GUILayout.Label(new GUIContent("  " + item.Value.host));

        if (item.Value.containsPlayer(GameState.account.account_id)) {
          if(GUILayout.Button(new GUIContent("Quit"), GUILayout.Width(40))) {
            Game.networkManager.Send (QuitRoomProtocol.Prepare ());
          }
        } else {
          GUI.enabled = !this.waiting;
          if(GUILayout.Button(new GUIContent("Join"), GUILayout.Width(40))) {
            Game.networkManager.Send (PairProtocol.Prepare (item.Value.game_id, item.Value.id));
          }
          GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
      }
    }

    if (enableHostEntry) {
      // Block for user entry
      // GUIStyle style2 = new GUIStyle();
      // style2.normal.textColor = Color.red;

      GUILayout.BeginHorizontal();
      GUILayout.Label(new GUIContent("           "));
      numPlayersS = GUILayout.TextField(numPlayersS, 1,  GUILayout.Width(20));
      GUILayout.Label(new GUIContent("         " + 0));
      GUILayout.Label(new GUIContent("           "));
      numRoundsS = GUILayout.TextField(numRoundsS, 2,  GUILayout.Width(30));
      GUILayout.Label(new GUIContent("       "));
      timeWindowS = GUILayout.TextField(timeWindowS, 3,  GUILayout.Width(35));
      GUILayout.Label(new GUIContent("   "));
      betAmountS = GUILayout.TextField(betAmountS, 3, GUILayout.Width(35));
      GUILayout.Label(new GUIContent("  "));
      ecoNumberS = GUILayout.TextField(ecoNumberS, 2,  GUILayout.Width(25));
      GUILayout.Label(new GUIContent("   "));
      allowSlidersS = GUILayout.TextField(allowSlidersS, 1,  GUILayout.Width(20));
      GUILayout.Label(new GUIContent("    " + GameState.player.GetName()));
      if(GUILayout.Button(new GUIContent("Enter"), GUILayout.Width(50))) {
        SubmitHostConfig();
      }
      GUILayout.EndHorizontal();

      // Ranges of valid entries displayed next 
      GUILayout.BeginHorizontal();
      GUILayout.Label(new GUIContent("          "));
      GUILayout.Label(new GUIContent(" 2-5 "));
      GUILayout.Label(new GUIContent("             "));
      GUILayout.Label(new GUIContent("       "));
      GUILayout.Label(new GUIContent(" 5-50 "));
      GUILayout.Label(new GUIContent("     "));
      GUILayout.Label(new GUIContent(" 30-180"));
      GUILayout.Label(new GUIContent(" "));
      GUILayout.Label(new GUIContent("10-200 "));
      GUILayout.Label(new GUIContent(" "));
      GUILayout.Label(new GUIContent("0-" + (ecoCount-1) + " "));
      GUILayout.Label(new GUIContent(" "));
      GUILayout.Label(new GUIContent(" Y/N "));
      GUILayout.Label(new GUIContent("             "));
      GUILayout.Label(new GUIContent(""), GUILayout.Width(50));
      GUILayout.EndHorizontal();


      if (hostEntryError) {
        GUIStyle styleLocal = new GUIStyle();
        styleLocal.normal.textColor = Color.red;
        GUI.Label (new Rect (10, windowRectMC.height - 70, 500, 30), 
          "The entry with a '?' is out of range. Please fix. Then press 'Enter'", styleLocal);
      } else {
        GUI.Label (new Rect (10, windowRectMC.height - 70, 500, 30), 
          "Update values as desired. Range of valid entry given below entry box. Press 'Enter'");
      }
    }

    // GUILayout.Space(30);

    // DH change
    GUI.enabled = enableMCButton;
    if (GUI.Button(new Rect(10, windowRectMC.height - 40, 160, 30), "Play Multi-Convergence")) {
      if (!enableHostEntry) {
        // isInitial = true;
        enableHostEntry = true;
        hostEntryError = false;

        // set default values 
        numPlayersS = "2";
        numRoundsS = "10";
        timeWindowS = "60";
        betAmountS = "20";
        ecoNumberS = "0";
        allowSlidersS = "Y";
      }
    }

    GUI.enabled = true;
   

    GUI.BringWindowToFront(window_id);
    GUI.DragWindow();
  }		
  */



	/*
  void MakeWindowHost(int id) {
    Functions.DrawBackground(new Rect(0, 0, widthConfig, heightConfig), bgTexture);
    string hdr1 = "Multiplayer Convergence";
    int hdr1P = hdr1.Length * pixelPerChar;
    string hdr2 = "You are the host of this game";
    int hdr2P = hdr2.Length * pixelPerChar;
    string hdr3 = "Please enter the information below and click 'Submit'";
    int hdr3P = hdr3.Length * pixelPerChar;

    GUIStyle style = new GUIStyle(GUI.skin.label);
    style.alignment = TextAnchor.UpperCenter;
    style.font = font;
    style.fontSize = 16;

    GUI.Label(new Rect((windowRectConfig.width - hdr1P) / 2, windowRectConfig.height * 0.02f, hdr1P, 45), hdr1, style);
    GUI.Label(new Rect((windowRectConfig.width - hdr2P) / 2, windowRectConfig.height * 0.07f, hdr2P, 45), hdr2, style);
    GUI.Label(new Rect((windowRectConfig.width - hdr3P) / 2, windowRectConfig.height * 0.12f, hdr3P, 45), hdr3, style);

    GUI.BeginGroup(new Rect(10, windowRectConfig.height * 0.18f, windowRectConfig.width * 0.80f, windowRectConfig.height * 0.30f));
    {
      style.alignment = TextAnchor.UpperLeft;
      style.fontSize = 14;
      GUI.Label(new Rect(0, 0, windowRectConfig.width * 0.85f, 30), "Enter Number of Players (2 to 5)", style);
      GUI.SetNextControlName("number_of_players");
      numPlayersS = GUI.TextField(new Rect(0, 25, windowRectConfig.width * 0.80f, 22), numPlayersS, 22);
    }
    GUI.EndGroup();

    GUI.BeginGroup(new Rect(10, windowRectConfig.height * 0.30f, windowRectConfig.width * 0.80f, windowRectConfig.height * 0.30f));
    {
      style.alignment = TextAnchor.UpperLeft;
      style.fontSize = 14;
      GUI.Label(new Rect(0, 0, windowRectConfig.width * 0.85f, 30), "Enter Number of Rounds (5 to 50)", style);
      GUI.SetNextControlName("number_of_rounds");
      numRoundsS = GUI.TextField(new Rect(0, 25, windowRectConfig.width * 0.80f, 22), numRoundsS, 22);
    }
    GUI.EndGroup();

    GUI.BeginGroup(new Rect(10, windowRectConfig.height * 0.42f, windowRectConfig.width * 0.80f, windowRectConfig.height * 0.30f));
    {
      style.alignment = TextAnchor.UpperLeft;
      style.fontSize = 14;
      GUI.Label(new Rect(0, 0, windowRectConfig.width * 0.85f, 30), "Enter Round Play Time in seconds (30 to 180)", style);
      GUI.SetNextControlName("bet_time");
      timeWindowS = GUI.TextField(new Rect(0, 25, windowRectConfig.width * 0.80f, 22), timeWindowS, 22);
    }
    GUI.EndGroup();

    GUI.BeginGroup(new Rect(10, windowRectConfig.height * 0.54f, windowRectConfig.width * 0.80f, windowRectConfig.height * 0.30f));
    {
      style.alignment = TextAnchor.UpperLeft;
      style.fontSize = 14;
      GUI.Label(new Rect(0, 0, windowRectConfig.width * 0.85f, 30), "Enter Bet Amount (20 to 200)", style);
      GUI.SetNextControlName("bet_amount");
      betAmountS = GUI.TextField(new Rect(0, 25, windowRectConfig.width * 0.80f, 22), betAmountS, 22);
    }
    GUI.EndGroup();

    GUI.BeginGroup(new Rect(10, windowRectConfig.height * 0.66f, windowRectConfig.width * 0.80f, windowRectConfig.height * 0.30f));
    {
      style.alignment = TextAnchor.UpperLeft;
      style.fontSize = 14;
      GUI.Label(new Rect(0, 0, windowRectConfig.width * 0.85f, 30), "Enter EcoSystem Number (0 to " + (ecoCount-1) + ")", style);
      GUI.SetNextControlName("ecosystem_number");
      ecoNumberS = GUI.TextField(new Rect(0, 25, windowRectConfig.width * 0.80f, 22), ecoNumberS, 22);
    }
    GUI.EndGroup();

    GUI.BeginGroup(new Rect(10, windowRectConfig.height * 0.78f, windowRectConfig.width * 0.80f, windowRectConfig.height * 0.30f));
    {
      style.alignment = TextAnchor.UpperLeft;
      style.fontSize = 14;
      GUI.Label(new Rect(0, 0, windowRectConfig.width * 0.85f, 30), "Enter 'Y' to see Slider Help *'s", style);
      GUI.SetNextControlName("allow_slider");
      allowSlidersS = GUI.TextField(new Rect(0, 25, windowRectConfig.width * 0.80f, 22), allowSlidersS, 22);
    }
    GUI.EndGroup();

    if (isInitial) {  // && GUI.GetNameOfFocusedControl() == "") {
      GUI.FocusControl("number_of_players");
      isInitial = false;
    }

    float butY = Math.Max (windowRectConfig.height * 0.90f, windowRectConfig.height * 0.76f + 25 + 22 + 10);
    if (GUI.Button(new Rect((windowRectConfig.width - 100) / 2,  butY, 100, 25), "Submit")) {
      SubmitHostConfig();
    }
  }
	*/

  private void SubmitHostConfig() {
    Debug.Log ("SubmitHostConfig called");

    numPlayersS = numPlayersS.Trim();
    numRoundsS = numRoundsS.Trim();
    timeWindowS = timeWindowS.Trim();
    betAmountS = betAmountS.Trim();
    ecoNumberS = ecoNumberS.Trim();

    short tempConvert;
    short neg1 = -1;
    numPlayers = Int16.TryParse (numPlayersS, out tempConvert) ? tempConvert : neg1;
    numRounds = Int16.TryParse (numRoundsS, out tempConvert) ? tempConvert : neg1;
    timeWindow = Int16.TryParse (timeWindowS, out tempConvert) ? tempConvert : neg1;
    betAmount = Int16.TryParse (betAmountS, out tempConvert) ? tempConvert : neg1;
    ecoNumber = Int16.TryParse (ecoNumberS, out tempConvert) ? tempConvert : neg1;
    allowSliders = (short) (((allowSlidersS.Length > 0) && (allowSlidersS.ToUpper().Substring(0, 1)) == "Y") ? 1 : 0);
    if (allowSliders == 1) {
      allowSlidersS = "Y";
    } else {
      allowSlidersS = "N";
    }

    hostEntryError = true;
    if ((numPlayers < 2) || (numPlayers > 5)) {
      numPlayersS = "?" + numPlayersS;
    } else if ((numRounds < 5) || (numRounds > 50)) {
      numRoundsS = "?" + numRoundsS;
    } else if ((timeWindow < 30) || (timeWindow > 180)) {
      timeWindowS = "?" + timeWindowS;
    } else if ((betAmount < 10) || (betAmount > 200)) {
      betAmountS = "?" + betAmountS;
    } else if ((ecoNumber < 0) || (ecoNumber >= ecoCount)) {
      ecoNumberS = "?" + ecoNumberS;
    } else {
      enableHostEntry = false;
      hostEntryError = false;
      Game.networkManager.Send (MCSetupProtocol.Prepare (Constants.MINIGAME_MULTI_CONVERGENCE, numPlayers, numRounds,
        timeWindow, betAmount, ecoNumber, allowSliders));
	  Game.networkManager.Send(ChatProtocol.Prepare (GameState.player.GetName(), 
				"I just hosted Multiplayer Convergence. Do you want to join?"));
    }
  }

  /*
  public void ProcessConvergeHostConfig (NetworkResponse response)
  {
    ResponseConvergeHostConfig args = response as ResponseConvergeHostConfig;
    Debug.Log ("In MultiplayerGames, responseconvergehostconfg");
    short status = args.status;
    Debug.Log ("status: " + status);
    Game.SwitchScene ("MultiConverge");
  }
  */


  public void setMessage(string message) {
    this.message = message;
  }

    public void Quit() {
        if (!this.enableRRButton || !this.enableCWButton || !this.enableSDButton || !this.enableMCButton) {
            Game.networkManager.Send (QuitRoomProtocol.Prepare ());
            quiting = true;
        } else {
            Destroy (this);
        }
    }

    public void OnQuitRoomResult (NetworkResponse response) {
        // DH change
        this.enableMCButton = true;
        this.enableRRButton = true;
        this.enableCWButton = true;
        this.enableSDButton = true;
        this.waiting = false;

        RoomManager.getInstance().removePlayer(GameState.account.account_id);
        if (quiting) {
            Destroy (this);
        }
    }

    public void OnPairResult (NetworkResponse response) {
        ResponsePair args = response as ResponsePair;
        int userID = GameState.account.account_id;

        if (args.status == 0) {
            Debug.Log ("All players are ready to play [room id=" + args.id + "]");
            this.room_id = args.id;

            this.enableRRButton = true;
            this.enableCWButton = true;
            this.enableMCButton = true;
            this.enableSDButton = true;

            var room = RoomManager.getInstance ().getRoom (args.id);
            if (!room.containsPlayer (userID)) {
                room.addPlayer (userID);
            }
				
            // switch scene
            if (args.gameID == Constants.MINIGAME_RUNNING_RHINO) {
                //RR.RRConnectionManager cManager = RR.RRConnectionManager.getInstance ();
                //cManager.Send (RR_RequestRaceInit ());
                //Game.SwitchScene ("RRReadyScene");
            } else if (args.gameID == Constants.MINIGAME_CARDS_OF_WILD) {
                CW.GameManager.matchID = args.id;
                gameObject.AddComponent <CWGame>();
                Quit();
                CWGame.networkManager.Send (CW.MatchInitProtocol.Prepare 
                    (GameState.player.GetID(), args.id), 
                    ProcessMatchInit);
            } else if (args.gameID == Constants.MINIGAME_SEA_DIVIDED) {
                if (SD.Constants.PLAYER_NUMBER != 1) {
                    SD.SDMain.networkManager.Send (
                        SD.SDPlayInitProtocol.Prepare (SD.Constants.USER_ID, this.room_id), 
                        SDProcessPlayInit
                    );
                    /* Added by Rupal. 2017-31-3. Not relying on the server's response to determine this.
                    Need to set this after the Protocol request is sent and before the scene is switched. */
                    if (room.host == GameState.player.GetName()) {
                        SD.Constants.PLAYER_NUMBER = 1;
                    } else {
                        SD.Constants.PLAYER_NUMBER = 2;
                    }
                } else {
                    if (room.host == GameState.player.GetName()) {
                        SD.Constants.PLAYER_NUMBER = 1;
                    } else {
                        SD.Constants.PLAYER_NUMBER = 2;
                    }
                    Game.SwitchScene("SDReadyScene");
                }
            } else if (args.gameID == Constants.MINIGAME_MULTI_CONVERGENCE) {
        		// DH change
        		MultiConvergeGame.matchID = args.id;   // game id
        		host = 0;  // Default - not the host

                string playerName = GameState.player.GetName ();
                if (playerName == room.host) {
                    host = 1;  // this is the host
                }
                Game.networkManager.Send (MCMatchInitProtocol.Prepare 
                    (GameState.player.GetID (), args.id, host, playerName), 
                    MCProcessMatchInit);
                Debug.Log("MultiplayerGames: MC notice sent to server(game id, player id): " + args.id + " " + playerName);
                Debug.Log ("Player Name: " + playerName);
                Debug.Log ("userID: " + userID);
        		Debug.Log ("This player host value is: " + host);
      		}
    } else {
      Debug.Log("New room allocated [room id=" + args.id + "]");
      room = RoomManager.getInstance().addRoom(args.id, args.gameID);
      room.host = GameState.player.GetName();
      room.addPlayer(userID);

      this.enableRRButton = false;
      this.enableCWButton = false;
      this.enableMCButton = false;
      this.enableSDButton = false;
      this.waiting = true;
    }
  }

  public IEnumerator RequestGetRooms(float time) {
    yield return new WaitForSeconds(time);
    
    Game.networkManager.Send (GetRoomsProtocol.Prepare ());
    
    StartCoroutine(RequestGetRooms(1f));
  }

  public void ProcessMatchInit(NetworkResponse response) {
    CW.ResponseMatchInit args = response as CW.ResponseMatchInit;
    
    if (args.status == 0) {
      Debug.Log("MatchID set to: " + args.matchID);
      Game.SwitchScene ("CWBattle");
    }
  }

  // DH change
  public void MCProcessMatchInit(NetworkResponse response) {
    ResponseMCMatchInit args = response as ResponseMCMatchInit;

    if (args.status == 0) {
      Debug.Log("MC MatchID set to: " + args.matchID + " Player id is: " + GameState.player.GetID ());
      Debug.Log("numRounds, timeWindow, betAmount, ecoNumber, allowSlliders");
      Debug.Log (numRounds + " " + timeWindow + " " + betAmount + " " + ecoNumber + " " + allowSliders);
	  Game.SwitchScene ("MultiConverge");
	  /* Handled by RoomManager
      if (host == 1) {
        Game.networkManager.Send (
          ConvergeHostConfigProtocol.Prepare (numRounds, timeWindow, betAmount, ecoNumber, allowSliders), 
          ProcessConvergeHostConfig);
      } else {
        MCWaitForParams ();
      }
      */

    }
  }
    
  /*
  public void MCWaitForParams() {
    Game.networkManager.Send (
      ConvergeNonHostConfigProtocol.Prepare (), ProcessConvergeNonHostConfig);
  }

  public void ProcessConvergeNonHostConfig (NetworkResponse response)
  {
    ResponseConvergeNonHostConfig args = response as ResponseConvergeNonHostConfig;
    numRounds = args.numRounds;
    Debug.Log ("In responseconvergenonhostconfg - received values. NumRounds = " + numRounds);
    if (numRounds > 0) {  // Host has loaded the values 
      Game.SwitchScene ("MultiConverge");
    } else {
      DateTime tStamp, tNow;
      TimeSpan tDiff;
      tStamp = DateTime.UtcNow;
      do {
        tNow = DateTime.UtcNow;
        tDiff = tNow.Subtract(tStamp);
      } while (tDiff.TotalSeconds < 0.2);
      MCWaitForParams ();
    }
  }
  */

  public RR.RequestRaceInit RR_RequestRaceInit ()
  {
    RR.RequestRaceInit request = new RR.RequestRaceInit ();
    request.Send (RR.Constants.USER_ID, this.room_id);
    return request;
  }
    
  /*
  private void ReadConvergeEcosystemsFileCount ()
  {
    // string filename = "converge-ecosystems-Ben";  // Problem with file
		string filename = "converge-ecosystems";   // remove txt extension. It has binary chars. Problems on mac
		int ecosystemCnt;

		if (!File.Exists (filename)) {
			Debug.LogError (filename + " not found.");
		} else {
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
				using (BinaryReader br = new BinaryReader(fs, Encoding.UTF8)) {
					int size = br.ReadInt16 ();
					int responseId = br.ReadInt16 ();
					ecosystemCnt = br.ReadInt16 ();
					ecoCount = (short) ecosystemCnt;
				}
			}
		}			    
  }
  */

  public void SDProcessPlayInit(NetworkResponse response) {
      SD.ResponseSDPlayInit args = response as SD.ResponseSDPlayInit;
      /* Modified by Rupal 2017-31-3. We are now assigning the player number based on the host player info. */
      //SD.Constants.PLAYER_NUMBER = args.playerNumber;
      Debug.Log ("The SD player number is " + SD.Constants.PLAYER_NUMBER);
	    Game.SwitchScene("SDReadyScene");
		/* Removed by DH 2016-11-12 by suggestion from Rupal. See email. 
      if (SD.Constants.PLAYER_NUMBER == 2) {
          Game.SwitchScene("SDReadyScene");
      }
      */
  }
    
}
