namespace RR {
	public class Constants {
		
		// Constants
		public static readonly string CLIENT_VERSION = "1.00";
<<<<<<< 3730ac0044b067015ae8e372dd471e4b8d247bdb
		//public static readonly string REMOTE_HOST = "localhost";

=======
		// public static readonly string REMOTE_HOST = "localhost";
>>>>>>> Started refactoring code for converge
		public static readonly int REMOTE_PORT = 9256;//20039;
		
		// Request (1xx) + Response (2xx)
		public static readonly short CMSG_AUTH = 101;
		public static readonly short SMSG_AUTH = 201;
    public static readonly short CMSG_HEARTBEAT = 102;
    public static readonly short SMSG_HEARTBEAT = 202;
	
		public static readonly short CMSG_GAMEOVER = 103;
		public static readonly short SMSG_GAMEOVER = 203;
	
		public static readonly short CMSG_OPPONENTDATA = 104;
		public static readonly short SMSG_OPPONENTDATA = 204;
		public static readonly short CMSG_LOOKING_FOR_OPPONENT = 105;
		public static readonly short SMSG_LOOKING_FOR_OPPONENT = 205;
		public static readonly short CMSG_IN_GAME_HEARTBEAT = 106;
		public static readonly short SMSG_IN_GAME_HEARTBEAT = 206;
		public static readonly short CMSG_GAME_STATE = 107;
		public static readonly short SMSG_GAME_STATE = 207;
		public static readonly short CMSG_RACE_INIT = 109;
		public static readonly short SMSG_RACE_INIT = 209;
		public static readonly short CMSG_KEYBOARD = 108;
		public static readonly short SMSG_KEYBOARD = 208;
	
		public static readonly short CMSG_RRPOSITION = 110;
		public static readonly short SMSG_RRPOSITION = 210;	
	
	//	public static readonly short CMSG_RRSPECIES = 111;
	//	public static readonly short SMSG_RRSPECIES = 211;
	//>>>>>>> Dong
	
		public static readonly short CMSG_RRSPECIES= 111;
		public static readonly short SMSG_RRSPECIES = 211;
	
		public static readonly short CMSG_RRENDGAME = 112;
		public static readonly short SMSG_RRENDGAME = 212;
		public static readonly short CMSG_RRSTARTGAME = 113;
		public static readonly short SMSG_RRSTARTGAME = 213;
		public static readonly short CMSG_RRBOOST = 114;
		public static readonly short SMSG_RRBOOST = 214;

		public static readonly short CMSG_RRGETMAP = 115;
		public static readonly short SMSG_RRGETMAP = 215;
	//>>>>>>> start
		
		// Other
		public static readonly string IMAGE_RESOURCES_PATH = "Images/";
		public static readonly string PREFAB_RESOURCES_PATH = "Prefabs/";
		public static readonly string TEXTURE_RESOURCES_PATH = "Textures/";

		// Player Object names
		public static readonly string PLAYER1_NAME = "Player_sprite(Clone)";
		public static readonly string PLAYER2_NAME = "Player_sprite_2(Clone)";

		// GUI Window IDs
		public enum GUI_ID {
			Login
		};
	
		public static int USER_ID = -1;
	}
}
