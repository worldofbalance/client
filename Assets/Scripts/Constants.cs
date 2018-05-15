using UnityEngine;

using System.Collections.Generic;
 
public class Constants
{
    public static readonly float UnitColliderRadius = 5.0f;
    //    public static readonly int REMOTE_PORT_COS = 9257;
    public static readonly int REMOTE_PORT_COS = 9254;
    public static readonly string TAG_HEALTH_BAR = "HealthBar";
    public static string SESSION_ID_COS = "";

    // Constants
    public static readonly string CLIENT_VERSION = "1.00";

    public static readonly string REMOTE_HOST = "localhost";
    // public static readonly string REMOTE_HOST = "thecity.sfsu.edu";
	//public static readonly string REMOTE_HOST = "smurf.sfsu.edu";
	//public static readonly string REMOTE_HOST = "worldofbalance.westus.cloudapp.azure.com";
	// public static readonly string REMOTE_HOST = "54.153.66.118";   // AWS from Ben, 2-2017
    // IP address may be 130.212.3.51

    public static readonly int REMOTE_PORT = 9255;    // 9255  wob_wob account
    public static readonly float HEARTBEAT_RATE = 1f;
	
    // Other
    public static readonly string IMAGE_RESOURCES_PATH = "Images/";
    public static readonly string PREFAB_RESOURCES_PATH = "Prefabs/";
    public static readonly string TEXTURE_RESOURCES_PATH = "Textures/";
    public static readonly string THEME_PATH = "Themes/";

    public static string ACTIVE_THEME = "Default";

	//void Awake(){
	//	Font FONT_01 = Resources.Load<Font>("Fonts/" + "Chalkboard");
	//	Texture2D BG_TEXTURE_01 = Resources.Load<Texture2D>(Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
	//}
	// moved loads to Awake() fuction to remove compile/build error 
	// Unity does not permit loads from out of function or static initializers
	public static readonly Font FONT_01 = Resources.Load<Font>("Fonts/" + "Chalkboard");
    public static readonly Texture2D BG_TEXTURE_01 = Resources.Load<Texture2D>(Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
	//public static readonly Font FONT_01;
	//public static readonly Texture2D BG_TEXTURE_01;

	// Converge game - foodweb (Database.cs) constants
    public static readonly int ID_NOT_SET = -1;
    public const int MODE_ECOSYSTEM = 0;
	public const int MODE_SHOP = 1;
	public const int MODE_CONVERGE_GAME = 2;
	public const int MODE_OWNED = 3;
    public const int MODE_FOODWEB = 4;

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
    public static readonly int CONVERGE_HOST_CONFIG = 14;
    public static readonly int CONVERGE_NONHOST_CONFIG = 15;
	public static readonly int CONVERGE_POPUP_WIN2 = 16;
	public static readonly int CONVERGE_SHOW_WINNERS = 17;
	public static readonly int SHOP_PURCHASE_POPUP = 18;
	public static readonly int SHOP_PURCHASE_ERROR = 19;
	public static readonly int SHOP_PURCHASE_COMPLETE = 20;
	public static readonly int GRAPH_WIN2 = 21;
	public static readonly int CONFIRM_LOGOUT = 22;
	public static readonly int FOOD_WEB_VIEW = 23;

    public static readonly float ECO_HEX_SCALE = 3;

    public static int unique_id = 1000;

    public static int GetUniqueID()
    {
        return unique_id++;
    }

    public static string SESSION_ID = "";
	
    public static int MONTH_DURATION = 180;
	
    public static Dictionary<int, SpeciesData> shopList = new Dictionary<int, SpeciesData>();

    // Mini games
    public static readonly int MINIGAME_RUNNING_RHINO = 1;
    public static readonly int MINIGAME_CARDS_OF_WILD = 2;
    public static readonly int MINIGAME_DONT_EAT_ME = 3;
    public static readonly int MINIGAME_CLASH_OF_SPECIES = 4;
    public static readonly int MINIGAME_SEA_DIVIDED = 5;
    public static readonly int MINIGAME_MULTI_CONVERGENCE = 6;
}
