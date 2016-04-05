using UnityEngine;

using System;
using System.Collections;

public class Login : MonoBehaviour {
	
	private int window_id = Constants.LOGIN_WIN;
	// Window Properties
	private float left;
	private float top;
	private float width = 280;
	private float height = 190;
	// Other
	public Texture background;
	private Texture2D bgTexture;
	private Font font;
	private string user_id = "";
	private string password = "";
	private Rect windowRect;
	private bool isActive = true;
	private bool isInitial = true;
	private String[] topPlayers;
	private int playerRank = -1;
	
	void Awake() {
		//DontDestroyOnLoad(gameObject);
		
		left = (Screen.width - width) / 2;
		top = (Screen.height - height) / 2;
		
		windowRect = new Rect(left, top, width, height);
		
		bgTexture = Resources.Load<Texture2D>(Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
		font = Resources.Load<Font>("Fonts/" + "Chalkboard");

		RR.RRMessageQueue.getInstance().AddCallback (RR.Constants.SMSG_AUTH, RR_ResponseLogin);
	}
	
	// Use this for initialization
	void Start() {
		//StartCoroutine("AutoLogin");

	}
	
	// Update is called once per frame
	void Update() {
		
	}
	
	void OnDestroy() {
		
	}
	
	void OnGUI() {
		// Background
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.ScaleAndCrop);
		
		// Client Version Label
		GUI.Label(new Rect(Screen.width - 75, Screen.height - 30, 65, 20), "v" + Constants.CLIENT_VERSION + " Beta");
		
		// Login Interface
		if (isActive) {
			windowRect = GUI.Window(window_id, windowRect, MakeWindow, "Login", GUIStyle.none);
			
			if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return) {
				Submit();
			}
		}

		if (playerRank != -1) {
			switch (playerRank) {
			case 1:
				GUI.Button(new Rect (Screen.width/2 - 200, Screen.height/2 - 100, 400, 150), "Congratulations, you're player number 1!!");
				StartCoroutine (wait(true));
				break;
			case 2:
				GUI.Button(new Rect (Screen.width/2 - 200, Screen.height/2 - 100, 400, 150), "Congratulations, you're player number 2!!");
				StartCoroutine (wait(true));
				break;
			case 3:
				GUI.Button(new Rect (Screen.width/2 - 200, Screen.height/2 - 100, 400, 150), "Congratulations, you're player number 3!!");
				StartCoroutine (wait(true));
				break;
			default:
				StartCoroutine (wait(false));
				break;
			}
		}
	}
	
	void MakeWindow(int id) {
		Functions.DrawBackground(new Rect(0, 0, width, height), bgTexture);
		
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.font = font;
		style.fontSize = 16;
		
		GUI.Label(new Rect((windowRect.width - 100) / 2, 0, 100, 30), "Login", style);
		
		GUI.BeginGroup(new Rect(10, 25, 300, 100));
		{
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = 14;
			GUI.Label(new Rect(0, 0, 300, 30), "User ID (Display Name or Email)", style);
			GUI.SetNextControlName("username_field");
			user_id = GUI.TextField(new Rect(0, 25, windowRect.width - 20, 25), user_id, 25);
		}
		GUI.EndGroup();
		
		GUI.BeginGroup(new Rect(10, 80, 300, 100));
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = 14;
		GUI.Label(new Rect(0, 0, 100, 30), "Password", style);
		GUI.SetNextControlName("password_field");
		password = GUI.PasswordField(new Rect(0, 25, windowRect.width - 20, 25), password, "*"[0], 25);
		GUI.EndGroup();
		
		if (isInitial) {  // && GUI.GetNameOfFocusedControl() == "") {
			GUI.FocusControl("username_field");
			isInitial = false;
		}
		
		if (GUI.Button(new Rect(width / 2 - 110, 145, 100, 30), "Log In")) {
			Submit();
		}
		
		if (GUI.Button(new Rect(width / 2 + 10, 145, 100, 30), "Register")) {
			SwitchToRegister();
		}
	}
	
	public void Submit() {
		user_id = user_id.Trim();
		password = password.Trim();
		
		if (user_id.Length == 0) {
			//			mainObject.GetComponent<Main>().CreateMessageBox("User ID Required");
			GUI.FocusControl("username_field");
		} else if (password.Length == 0) {
			//			mainObject.GetComponent<Main>().CreateMessageBox("Password Required");
			GUI.FocusControl("password_field");
		} else {
			
			NetworkManager.Send(
				LoginProtocol.Prepare(user_id, password), ProcessLogin
			);

			RR.RRConnectionManager cManager = RR.RRConnectionManager.getInstance();
			cManager.Send(RR_RequestLogin(user_id, password));
		}
	}
	
	public IEnumerator AutoLogin() {
		while (true) {
			user_id = "1";
			password = "1";
			Submit();
			
			yield return new WaitForSeconds(1.0f);
		}
	}
	
	public void ProcessLogin(NetworkResponse response) {
		ResponseLogin args = response as ResponseLogin;
		
		if (args.status == 0) {
			GameState.account = args.account;

			NetworkManager.Send(
				TopListProtocol.Prepare(),
				ProcessTopList
				);
			NetworkManager.Send(
				PlayerSelectProtocol.Prepare(0),
				ProcessPlayerSelect
				);
			NetworkManager.Send(
				CW.PlayerSelectProtocol.Prepare(0),
				CW_ProcessPlayerSelect
				);
		} else {
			Debug.Log ("login failed, server message = " + args.status);
			//mainObject.GetComponent<Main>().CreateMessageBox("Login Failed");
		}
	}

	public void ProcessTopList(NetworkResponse response) {
		ResponseTopList args = response as ResponseTopList;
		//client team -- use this data for the toplist functionality
		topPlayers = new string[3];
		topPlayers [0] = args.name1;
		topPlayers [1] = args.name2;
		topPlayers [2] = args.name3;
		Debug.Log ("rank 1 player: " + args.name1 + " with " + args.score1 + " points.");
		Debug.Log ("rank 2 player: " + args.name2 + " with " + args.score2 + " points.");
		Debug.Log ("rank 3 player: " + args.name3 + " with " + args.score3 + " points.");
	}
	
	public void ProcessPlayerSelect(NetworkResponse response) {
		ResponsePlayerSelect args = response as ResponsePlayerSelect;
		
		if (args.status == 0) {
			isActive = false;
			GameState.player = args.player;
			playerRank = PlayerIsTop(user_id);
		}
	}

	public void CW_ProcessPlayerSelect(NetworkResponse response) {
		CW.ResponsePlayerSelect args = response as CW.ResponsePlayerSelect;
	}

	IEnumerator wait(Boolean condition)
	{
		if (condition) {
			yield return new WaitForSeconds (1f);
		}
		Game.SwitchScene("World"); //"World");
	}

	public int PlayerIsTop(String name) {
		if (name.Equals(topPlayers[0]))
			return 1;
		else if (name.Equals(topPlayers[1]))
			return 2;
		else if (name.Equals (topPlayers [2]))
			return 3;
		else
			return 0;
	}
	
	public void SwitchToRegister() {
		isActive = false;
		gameObject.AddComponent<Register>();
	}
	
	public void SetActive(bool active) {
		this.isActive = active;
		//reset GUI focus if reactivating login.
		this.isInitial = this.isInitial || this.isActive;
	}

	public RR.RequestLogin RR_RequestLogin (string username, string password)
	{
		RR.RequestLogin request = new RR.RequestLogin ();
		request.send (username, password);
		return request;
	}
	
	public void RR_ResponseLogin (RR.ExtendedEventArgs eventArgs)
	{
		RR.ResponseLoginEventArgs args = eventArgs as RR.ResponseLoginEventArgs;
		
		if (args.status == 0) {
			RR.Constants.USER_ID = args.user_id;
		} else {
			Debug.Log ("RR: Login Failed");
		}
	}
}
