using UnityEngine;

using System.Collections.Generic;
namespace CW{
public class Constants {
	
	public static readonly Font FONT_01 = Resources.Load<Font>("Fonts/" + "Chalkboard");
	public static readonly Texture2D BG_TEXTURE_01 = Resources.Load<Texture2D>(Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");


	// Constants
	public static readonly string CLIENT_VERSION = "1.00";
    //public static readonly string REMOTE_HOST =  "localhost";//"thecity.sfsu.edu"; // "localhost"; //"localhost";  "smurf.sfsu.edu";
    // public static readonly string REMOTE_HOST =  "thecity.sfsu.edu";
	// public static readonly string REMOTE_HOST =  "smurf.sfsu.edu";
	public static readonly string REMOTE_HOST = "seadivided.ddns.net"; // AWS Spr 2019
	// public static readonly string REMOTE_HOST = "54.153.66.118";   // AWS from Ben, 2-2017
	public static readonly int REMOTE_PORT = 9260;  	// for thecity
	//public static readonly int REMOTE_PORT = 20038;     // for localhost
	public static readonly float HEARTBEAT_RATE = 1f;
	public static readonly float ANIMATE_RATE = 20.0f;
	// Other
	public static readonly string IMAGE_RESOURCES_PATH = "Images/";
	public static readonly string PREFAB_RESOURCES_PATH = "Prefabs/";
	public static readonly string TEXTURE_RESOURCES_PATH = "Textures/";
	public static readonly string THEME_PATH = "Themes/";

	public static string ACTIVE_THEME = "Default";
	

	// Converge game
	public static readonly int ID_NOT_SET = -1;
	public static readonly int MODE_ECOSYSTEM = 0;
	public static readonly int MODE_SHOP = 1;
	public static readonly int MODE_CONVERGE_GAME = 2;

	//author: Lobby Team
	public static readonly float BATTLE_REQUEST_RATE = 1f;
	
	// Battle Team Constants // update: not used by cards of the wild 
	public static readonly short ACTION_ATTACK = 1;
	public static readonly short ACTION_DEFEND = 2;
	public static readonly short ACTION_EXTERMINATE = 3;
	public static readonly short ACTION_PROTECT = 4;
	public static readonly short ACTION_DISASTER = 5;
	public static readonly short DISASTER_BLIZZARD = 1;
	public static readonly short DISASTER_TORNADO = 2;
	public static readonly short DISASTER_FIRE = 3;
	public static readonly short DISASTER_RAIN = 4; 

	// Cards of the Wild
	//public static readonly bool SINGLE_PLAYER = true;
	public static readonly bool SINGLE_PLAYER = false;
	public static readonly int PLAYER2ID = 108;
	public static readonly float UPDATE_RATE = 1.0f;
	// Response status codes
	public static readonly short STATUS_SUCCESS = 0;
	public static readonly short STATUS_FAILURE = 1;
	// used for MatchStatus and MatchAction 
	public static readonly short STATUS_NO_MATCH = 2;


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
}
}