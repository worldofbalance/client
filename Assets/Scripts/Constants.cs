using UnityEngine;

using System.Collections.Generic;

public class Constants {
	
	// Constants
	public static readonly string CLIENT_VERSION = "1.00";

	public static readonly string REMOTE_HOST = "localhost";
	//public static readonly string REMOTE_HOST = "thecity.sfsu.edu";

	public static readonly int REMOTE_PORT = 9255;//20041;

	public static readonly float HEARTBEAT_RATE = 1f;
	
	// Other
	public static readonly string IMAGE_RESOURCES_PATH = "Images/";
	public static readonly string PREFAB_RESOURCES_PATH = "Prefabs/";
	public static readonly string TEXTURE_RESOURCES_PATH = "Textures/";
	public static readonly string THEME_PATH = "Themes/";

	public static string ACTIVE_THEME = "Default";
	
	public static readonly Font FONT_01 = Resources.Load<Font>("Fonts/" + "Chalkboard");
	public static readonly Texture2D BG_TEXTURE_01 = Resources.Load<Texture2D>(Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");

	// Converge game
	public static readonly int ID_NOT_SET = -1;
	public static readonly int MODE_ECOSYSTEM = 0;
	public static readonly int MODE_SHOP = 1;
	public static readonly int MODE_CONVERGE_GAME = 2;

	//author: Lobby Team
	public static readonly float BATTLE_REQUEST_RATE = 1f;
	
	// Battle Team Constants
	public static readonly short ACTION_ATTACK = 1;
	public static readonly short ACTION_DEFEND = 2;
	public static readonly short ACTION_EXTERMINATE = 3;
	public static readonly short ACTION_PROTECT = 4;
	public static readonly short ACTION_DISASTER = 5;
	public static readonly short DISASTER_BLIZZARD = 1;
	public static readonly short DISASTER_TORNADO = 2;
	public static readonly short DISASTER_FIRE = 3;
	public static readonly short DISASTER_RAIN = 4; 
	
	public static int OPPONENT_ID = -1;

	// GUI Window IDs
	public static readonly int LOGIN_WIN = 1;
	public static readonly int REGISTER_WIN = 2;
	public static readonly int GRAPH_WIN = 3;
	public static readonly int CHAT_WIN = 4;
	public static readonly int PARAM_WIN = 5;
	public static readonly int DATABASE_WIN = 6;
	public static readonly int MENU_WIN = 7;
	public static readonly int PLIST_WIN = 8;
	public static readonly int SHOP_WIN = 9;
	public static readonly int ZONE_WIN = 10;
	public static readonly int VIEW_WIN = 11;
	public static readonly int CONVERGE_WIN = 12;
	public static readonly int CONVERGE_POPUP_WIN = 13;

	public static readonly float ECO_HEX_SCALE = 3;

	public static int unique_id = 1000;
	
	public static int GetUniqueID() {
		return unique_id++;
	}

	public static string SESSION_ID = "";
	
	public static int MONTH_DURATION = 180;
	
	public static Dictionary<int, SpeciesData> shopList = new Dictionary<int, SpeciesData>();

	// Mini games
	public static readonly int MINIGAME_RUNNING_RHINO    = 1;
	public static readonly int MINIGAME_CARDS_OF_WILD    = 2;
	public static readonly int MINIGAME_DONT_EAT_ME      = 3;
	public static readonly int MINIGAME_CLASH_OF_SPECIES = 4;
	public static readonly int MINIGAME_SEA_DIVIDED      = 5;
}
