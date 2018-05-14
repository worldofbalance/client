using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public string levelToLoad = "LevelSelect";

	public SceneFader sceneFader;

	public void Play (){
		sceneFader.FadeTo(levelToLoad);
	}

	public void Quit (){
		Debug.Log("Exiting...");
		Application.Quit();
	}

}
