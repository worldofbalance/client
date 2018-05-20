using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//Directly taken from clash of species pausemenu script
public class pauseMenu1 : MonoBehaviour
{
    //public GUISkin myskin;
    public Rect windowRect;
    public bool paused = false, waited = true;
	private ClashBattleController cbc;

    public void Start()
    {
        windowRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200);
		//cbc = GameObject.Find ("Battle Menu").GetComponent<ClashBattleController> ();

    }

    public void waiting()
    {
        waited = true;
    }

    public void ClickPause()
    {
        paused = true;
    }

    public void Update()
    {
        if (waited)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (paused)
                    paused = false;
                else
                    paused = true;

                waited = false;
                Invoke("waiting", 0.3f);
            }
        }
        Time.timeScale = paused ? 0f : 1f;
    }

    public void OnGUI()
    {
        if (paused)
            windowRect = GUI.Window(0, windowRect, windowFunc, "Pause Menu");
    }

    public void windowFunc(int id)
    {
        if (GUILayout.Button("Resume"))
        {
            paused = false;
        }

        if (GUILayout.Button("Move to Main Menu"))
        {
			paused = false;
			//cbc.Surrender ();
			Application.LoadLevel ("MainMenu");
//			cbc.ReportBattleOutcome(ClashEndBattleProtocol.BattleResult.LOSS);
            //SceneManager.LoadScene("ClashMain");
        }
		if (GUILayout.Button("Return to Lobby"))
		{
			//let's award players 30 credits for playing - Jeremy
			Game.networkManager.Send(UpdateCreditsProtocol.Prepare((short)0, 30), ProcessUpdateCredits);
			Debug.Log("old credits: " + GameState.player.credits);
			Debug.Log("player awarded 30 credits.");
			//Add in lobby credits here or gameover
			Application.LoadLevel ("Game");
		}

    }

	public void ProcessUpdateCredits(NetworkResponse response)
	{
		ResponseUpdateCredits args = response as ResponseUpdateCredits;
		Debug.Log("ResponseUpdateCredits: action= " + args.action);

		if (args.status == 0)
		{
			GameState.player.credits = args.newCredits;
			Debug.Log("new credits: " + args.newCredits);
		}
		else
			Debug.Log("failed to update credits");
	}

}
