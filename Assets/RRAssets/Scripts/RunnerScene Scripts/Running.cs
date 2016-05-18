using UnityEngine;
using System.Collections;

namespace RR
{
	public class Running : MonoBehaviour
	{
		public GameObject mainObject;
		public GameObject player1;
		public GameObject player2;

		public string species1 { get; set; }

		public static float time;
		public static bool completed;
		private bool flag = true;
		private bool speedUpFlag = true;
		private bool speedDownFlag = true;
		private bool speedReturnFlag = true;
		private RRConnectionManager cManager;
		private RRMessageQueue messageQueue;
		private short gameState;
		private bool called = false;
		//public static bool player1touchitem1 = false;
		//public static string hitItem = "";
	
		private const float SPEED_INCREASE = 50f;
		private const float SPEED_DECREASE = -100f;
		public const float BASE_SPEED = 100f;
		public const float HIGH_SPEED = 250f;
		public const float MAX_SPEED = 300f;
		public const float MIN_SPEED = 50f;

		void OnDestroy() 
		{
			RRMessageQueue.getInstance ().RemoveCallback (Constants.SMSG_RRENDGAME);
		}
	
		// Change player1's speed. boost can be positive or negative.
		// speed won'r increase if it's already the MAX_SPEED, won't decrease if already MIN_SPEED
		void ChangeSpeed (float boost)
		{
			PlayerController[] pcontroller = player1.GetComponents<PlayerController> ();
			float originalSpeed = pcontroller [0].speed;

			if (boost > 0) {
				pcontroller[0].speed = originalSpeed + boost;
				if (pcontroller[0].speed > MAX_SPEED) {
					pcontroller[0].speed = MAX_SPEED;
				}
				return;
			}
			if (boost < 0) {
				if (pcontroller[0].speed > BASE_SPEED) {
					pcontroller[0].speed = BASE_SPEED;
				} else {
					pcontroller[0].speed = originalSpeed + boost;
					if (pcontroller[0].speed <= 0) {
						pcontroller[0].speed = MIN_SPEED;
					}
				}
				return;
			}
		}
	
		// When collider happens, check if player1 can eat item (hunt it or be hunted by it)
		public bool isHitItem (string name)
		{
			//Debug.Log("!!!!!!!!!!Pass the substring: " + name);
			if (species1 == null)
				return false;
			if (GameManager.relationship [species1].ContainsKey (name)) {
				if (speedUpFlag) {
					Debug.Log (name + " is hit!!! for speed UP");
					ChangeSpeed (SPEED_INCREASE);
					speedUpFlag = false;
					return true;
				}
			} else if (GameManager.relationship [name].ContainsKey (species1)) {
				if (speedDownFlag) {
					Debug.Log (name + " is hit!!! for speed DOWN");
					ChangeSpeed (SPEED_DECREASE);
					speedDownFlag = false;
					return true;
				}
			}
			return false;
		}
		
		void OnGUI ()
		{
			GUIStyle myStyle = new GUIStyle ();
			myStyle.normal.textColor = Color.blue;
		
			GUI.Label (new Rect (Screen.width - 150, 0, 200, 100), "Running Time:  " + time, myStyle);
		
			if (GUI.Button (new Rect (10,10,120, (Screen.width / 12.8f) / 100 * 40), "Back to Lobby")) {
				Game.SwitchScene("World");
			}
		}

		void Start ()
		{
			time = 0.0f;
			mainObject = GameObject.Find ("MainObject");
			cManager = RRConnectionManager.getInstance ();
			//RRMessageQueue.getInstance ().AddCallback (Constants.SMSG_AUTH, ResponseLogin);
	
		}
	
		public void RunOnce ()
		{
			player2 = GameObject.Find ("Player_sprite_2(Clone)");
	
		}
	
		public void Player2Move (Vector2 newVect)
		{
			player2.transform.position = newVect;
		}
	
		private void HeartBeat ()
		{
			// The Heart Beat!!
		}
	
		// when player1 eats a boost, it can't eat more in next 1 second
		private IEnumerator SpeedUpDelay ()
		{
			speedUpFlag = true;
			yield return new WaitForSeconds (1f);
		}
	
		// when player1 eats a obstacle, it can't eat more in next 1 second
		private IEnumerator SpeedDownDelay ()
		{
			speedDownFlag = true;
			yield return new WaitForSeconds (1f);
		}
	
		// every 1 second, player1's speed will return towards the base value a little
		private IEnumerator SpeedReturnDelay ()
		{
			speedReturnFlag = false;
			yield return new WaitForSeconds (1f);
	
			PlayerController[] pcontroller = player1.GetComponents<PlayerController> ();
			float currentSpeed = pcontroller [0].speed;
			//Debug.Log(currentSpeed);
			if (currentSpeed == BASE_SPEED) { // this line is not necessary but can reduce operations
			} else if (currentSpeed > HIGH_SPEED) {
				pcontroller [0].speed = currentSpeed - 2;
			} else if (currentSpeed > BASE_SPEED) {
				pcontroller [0].speed = currentSpeed - 1;
			} else if (currentSpeed > BASE_SPEED - 1) {
				pcontroller [0].speed = BASE_SPEED;
			} else {
				pcontroller [0].speed = currentSpeed + 1;
			}
			//		pcontroller[0].speed = originalSpeed + boost;
	
			speedReturnFlag = true;
	
		}
	
		// comunication with server
		private IEnumerator Delay ()
		{
			flag = false;
			yield return new WaitForSeconds (4f);
	
			if (cManager) {
				RequestRRPostion rp = new RequestRRPostion ();
				rp.send ((player1.transform.position.x).ToString (), (player1.transform.position.y).ToString ());
				cManager.Send (rp);
	
			}
			flag = true;
		}
	
		public void ResponseLogin (ExtendedEventArgs eventArgs)
		{
			ResponseLoginEventArgs args = eventArgs as ResponseLoginEventArgs;
			
			if (args.status == 0) {
				Constants.USER_ID = args.user_id;
			} else {
				Debug.Log ("Login Failed");
			}
		}
	
		public void ResponseGameState (ExtendedEventArgs eventArgs)
		{
			ResponseLoginEventArgs args = eventArgs as ResponseLoginEventArgs;
			
			if (args.status == 0) {
				Constants.USER_ID = args.user_id;
			} else {
				Debug.Log ("Response GameState Failed"); // Don't know what comment should be made here
			}
		}

		public void ResponseRREndGame (ExtendedEventArgs eventArgs)
		{
			//		Debug.Log ("ResponseEndGame Called in Running.cs 1");
			ResponseRREndGameEventArgs args = eventArgs as ResponseRREndGameEventArgs;
			
			if (args.win == true) {
				PlayerPrefs.SetInt ("Win", 1);
			} else {
				PlayerPrefs.SetInt ("Win", 0);
			}
			PlayerPrefs.SetString ("Winning Time", args.winningTime);
			
			//		Debug.Log ("ResponseEndGame Called in Running.cs 2");
			Application.LoadLevel ("RREndScene");
		}

		// Update is called once per frame
		void Update ()
		{
			time += Time.deltaTime;
	
			if (flag) {
				StartCoroutine (Delay ());
			}
	
			if (!speedUpFlag) {
				StartCoroutine (SpeedUpDelay ());
			}
	
			if (!speedDownFlag) {
				StartCoroutine (SpeedDownDelay ());
			}
	
			if (speedReturnFlag) {
				StartCoroutine (SpeedReturnDelay ());
			}
	
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (cManager) {
					RequestKeyboard rk = new RequestKeyboard ();
					rk.send (1, -1);
					cManager.Send (rk);
				}
			}
			
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (cManager) {
					RequestKeyboard rk = new RequestKeyboard ();
					rk.send (1, 1);
					cManager.Send (rk);
				}
			}
			
			if (Input.GetKeyDown (KeyCode.Space)) {
				if (cManager) {
					RequestKeyboard rk = new RequestKeyboard ();
					rk.send (2, 1);
					cManager.Send (rk);
				}
			}
	
			if (Input.GetKeyUp (KeyCode.LeftArrow) && cManager) {
				RequestKeyboard rk = new RequestKeyboard ();
				rk.send (1, 0);
				cManager.Send (rk);
			}
			
			if (Input.GetKeyUp (KeyCode.RightArrow) && cManager) {
				RequestKeyboard rk = new RequestKeyboard ();
				rk.send (1, 0);
				cManager.Send (rk);
				
			}
	
			if (Input.GetKeyUp (KeyCode.Space) && cManager) {
				RequestKeyboard rk = new RequestKeyboard ();
				rk.send (2, 0);
				cManager.Send (rk);
				
			}
			if (!called) {
				RRMessageQueue.getInstance ().AddCallback (Constants.SMSG_RRENDGAME, ResponseRREndGame);
				called = true;
			}
		}
	
		public void SetGameStateOn ()
		{
			gameState = 1;
		}	
	}
}