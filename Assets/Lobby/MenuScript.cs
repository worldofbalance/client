using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {
    public bool menuOpen = false;
    public GameObject whosOnlineMenu;
    private Object activeGuiObject;
    public GameObject statusContainer;
    public Text[] playerArray;
    public bool statusOpen;
    public Text player1box;
    private string name;
    private string score;
    public ShopPopUP shopPopUp;

    void awake(){
        statusOpen = false;
        playerArray = new Text[18];
        //shopPopUp.UpdateTextFields ();
    }
    void update(){
        // Debug.Log ("IN MENU SCRIPT");
    }

    public void OpenWhosOnline(){
        CloseAllMenus ();
        GameObject.Find("Local Object").GetComponent<WorldMouse>().popOversEnabled = false;
        disableDropDown();
        //Call method to request players 
        CurrentlyOnline online = this.GetComponent<CurrentlyOnline>();
        Debug.Log("Currently Online instantiated");
        online.requestOnlinePlayers(handleOnlinePlayers);
        Debug.Log ("You Pressed WHOS ONLINE?");
        //EventSystemManager sets this item to take priority over bckground objects (mouseEvents)
        EventSystem.current.SetSelectedGameObject(whosOnlineMenu);
        whosOnlineMenu.SetActive (true);
        menuOpen = true;
    }

    private void handleOnlinePlayers(Dictionary<int, Player> playerList) {
        //Display the onlinePlayer Response
        int i =0;
        foreach (KeyValuePair<int, Player> entry in playerList)
        {
            score = entry.Value.xp.ToString();
            name = entry.Value.name;
            playerArray [i].text = name + "            " +score;
            i++;
        }
    }

    public void showTopBar() {
      GameObject.Find("dropdown").transform.localScale = new Vector3(1, 1, 1);
      GameObject.Find("btn_whosOnline").transform.localScale = new Vector3(1, 1, 1);
      GameObject.Find("btn_status").transform.localScale = new Vector3(1, 1, 1);
    }

    public void hideTopBar() {
      GameObject.Find("dropdown").transform.localScale = new Vector3(0, 0, 0);
      GameObject.Find("btn_whosOnline").transform.localScale = new Vector3(0, 0, 0);
      GameObject.Find("btn_status").transform.localScale = new Vector3(0, 0, 0);
    }

    public void enableDropdown() {
      GameObject.Find("container").transform.localScale = new Vector3(1, 1, 1);
    }

    public void disableDropDown() {
      GameObject.Find("container").transform.localScale = new Vector3(0, 0, 0);
    }

    public void showCoreUI() {
        GameObject.Find("ChatPanel").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("dropdown").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("btn_whosOnline").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("btn_status").transform.localScale = new Vector3(1, 1, 1);
    }
  
    public void OpenStatus() {
        GameObject.Find("ChatPanel").transform.localScale = new Vector3(0, 0, 0);
        GameObject.Find("dropdown").transform.localScale = new Vector3(0, 0, 0);
        GameObject.Find("btn_whosOnline").transform.localScale = new Vector3(0, 0, 0);
        GameObject.Find("btn_status").transform.localScale = new Vector3(0, 0, 0);

        CloseAllMenus ();
        GameObject.Find("Local Object").GetComponent<WorldMouse>().popOversEnabled = false;
        disableDropDown();
        statusContainer.SetActive (true);
        Transform t  = statusContainer.GetComponent(typeof (Transform)) as Transform;
        //t.SetAsFirstSibling();
        menuOpen = true;
		GameObject.Find ("BarChart").GetComponent<BarCharScript>().ReloadData ();
    }

    public void OpenMiniGames() {
        CloseAllMenus ();
        GameObject.Find("Local Object").GetComponent<WorldMouse>().popOversEnabled = false;
        disableDropDown();
        Debug.Log ("You Pressed MiniGames");
        menuOpen = true;
    }


	public void TogglePressed() {
		Debug.Log ("Toggle Pressed: Inside MenuScript");
		GameObject.Find ("BarChart").GetComponent<BarCharScript>().ToggleScores ();

	}




    public void OpenConvergence() {
        Game.SwitchScene("Converge");
    }

    public void OpenCardsfWild() {
        Game.SwitchScene ("ClashSplash");
    }

    public void OpenDontEatMe() {
		Game.SwitchScene ("MainMenu");
    }
    public void OpenMultiplayerGames() {
        CloseAllMenus();
        disableDropDown();
        GameObject.Find("Local Object").GetComponent<WorldMouse>().popOversEnabled = false;
        menuOpen = true;
        Debug.Log("You Pressed Open Multiplayer Games");
        this.activeGuiObject =  (Object) gameObject.AddComponent <MultiplayerGames>();
        this.enabled = false;
    }


    public void OpenClashOfSpecies() {
        Destroy (this);
        Game.SwitchScene ("ClashSplash");
    }
    

    public void CloseAllMenus() {
        if (this.activeGuiObject != null) {
          Destroy(this.activeGuiObject);
        }
        whosOnlineMenu.SetActive (false);
        statusContainer.SetActive(false);
        GameObject.Find("Local Object").GetComponent<WorldMouse>().popOversEnabled = true;
        this.enableDropdown();
        menuOpen = false;
        statusOpen = false;
    }

    public bool checkIfOpen() {
        Debug.Log ("STATUS WINDOW OPEN: "+statusOpen);
        return statusOpen;
    }
}
