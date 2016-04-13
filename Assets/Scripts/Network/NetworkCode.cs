public class NetworkCode {
	
	// Request + Response
	public static readonly short CLIENT = 100;
	public static readonly short HEARTBEAT = 101;
	public static readonly short ACTIVITY = 102;
	public static readonly short LOGIN = 103;
	public static readonly short LOGOUT = 104;
	public static readonly short REGISTER = 105;
	public static readonly short ERROR_LOG = 106;
	public static readonly short MESSAGE = 107;
	
	public static readonly short PLAYERS = 108;
	public static readonly short SPECIES_LIST = 109;
	public static readonly short WORLD = 110;
	public static readonly short ZONE_LIST = 111;
	public static readonly short ZONE = 112;
	public static readonly short ZONE_UPDATE = 113;
	public static readonly short ECOSYSTEM = 114;
	public static readonly short PREDICTION = 115;
	
	public static readonly short SHOP = 116;
	public static readonly short SHOP_ACTION = 117;
	public static readonly short PARAMS = 118;
	public static readonly short CHANGE_PARAMETERS = 119;
	public static readonly short GET_FUNCTIONAL_PARAMETERS = 120;
	public static readonly short CHANGE_FUNCTIONAL_PARAMETERS = 121;
	public static readonly short STATISTICS = 122;
	public static readonly short HIGH_SCORE = 123;
	public static readonly short CHART = 124;
	public static readonly short SPECIES_ACTION = 125;
	public static readonly short BADGE_LIST = 126;
	
	public static readonly short BATTLE_REQ = 127;
	public static readonly short BATTLE_PREP = 128;
	public static readonly short SEASON_CHANGE = 129;
	public static readonly short BATTLE_CON = 130;
	public static readonly short BATTLE_ACTION = 131;
	public static readonly short BATTLE_TURN = 132;
	public static readonly short BATTLE_START = 133;
	
	public static readonly short UPDATE_RESOURCES = 134;
	public static readonly short SPECIES_KILL = 135;
	public static readonly short UPDATE_TIME = 136;
	public static readonly short SPECIES_CREATE = 137;
	public static readonly short OBJECTIVE_ACTION = 138;
	public static readonly short UPDATE_ENV_SCORE = 139;
	public static readonly short UPDATE_LEVEL = 140;
	public static readonly short BADGE_UPDATE = 141;
	public static readonly short UPDATE_SEASON = 142;
	public static readonly short UPDATE_CURRENT_EVENT = 143;
	public static readonly short BATTLE_END = 144;

	public static readonly short PLAYER_SELECT = 145;

	public static readonly short CONVERGE_ECOSYSTEMS = 146;
	public static readonly short CONVERGE_NEW_ATTEMPT = 147;
	public static readonly short CONVERGE_PRIOR_ATTEMPT = 148;
	public static readonly short CONVERGE_PRIOR_ATTEMPT_COUNT = 149;
	public static readonly short CONVERGE_HINT = 150;
	public static readonly short CONVERGE_HINT_COUNT = 151;
	public static readonly short CONVERGE_NEW_ATTEMPT_SCORE = 152;

	// DH change
	// Multiplayer Convergence
	public static readonly short MC_MATCH_INIT = 180;
	public static readonly short MC_GET_TIME = 181;
	public static readonly short MC_BET_UPDATE = 182;
	public static readonly short MC_GET_NAMES = 183;
	public static readonly short MC_GET_OTHER_SCORE = 184;


	public static readonly short TOPLIST = 153;

	public static readonly short WAITFORGAME = 154;
	public static readonly short NOWAITFORGAME = 155;
	public static readonly short WAITLIST = 156;
	public static readonly short WAITSTATUS = 157;
	public static readonly short STARTGAME = 158;

	public static readonly short PAIR = 159;
	public static readonly short QUIT_ROOM = 160;
	public static readonly short GET_ROOMS = 161;
	public static readonly short BACK_TO_LOBBY = 192;
	public static readonly short PLAY_GAME = 193;
	public static readonly short END_GAME = 194;

	//Clash of Species
	public static readonly short CLASH_ENTRY = 162;
	public static readonly short CLASH_SPECIES_LIST = 163;
	public static readonly short CLASH_DEFENSE_SETUP = 164;
	public static readonly short CLASH_PLAYER_LIST = 165;
	public static readonly short CLASH_PLAYER_VIEW = 166;
	public static readonly short CLASH_INITIATE_BATTLE = 167;
	public static readonly short CLASH_END_BATTLE = 168;

	// Cards of the wild 
	public static readonly short MATCH_INIT= 201;
	public static readonly short MATCH_STATUS = 202;
	public static readonly short GET_DECK = 203;
	public static readonly short SUMMON_CARD = 204;
	public static readonly short CARD_ATTACK = 205;
	public static readonly short QUIT_MATCH = 206;
	public static readonly short MATCH_OVER = 207;
	public static readonly short END_TURN = 208;
	public static readonly short DEAL_CARD = 209;
	public static readonly short TREE_ATTACK = 210;
	public static readonly short MATCH_ACTION = 211;
	public static readonly short MATCH_START = 212;
	public static readonly short RETURN_LOBBY = 213;
	public static readonly short APPLY_FOOD = 214;
	public static readonly short APPLY_WEATHER = 215;
}
