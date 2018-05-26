using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour {

	public SceneFader fader;

	public Button[] levelButtons;

	void Start ()
	{
		int levelReached = PlayerPrefs.GetInt("levelReached", 1);
		// commented out because the levelButtons array is never initialized
		// so levelButtons[i] is trying to access a null object
		// I do not know what should be there so I could not fix it
		// the code works without this
		/**
		for (int i = 0; i < levelButtons.Length; i++)
		{
			if (i + 1 > levelReached)
            levelButtons[i].interactable = false;
		}
		**/
	}

	public void Select (string levelName)
	{
		fader.FadeTo(levelName);
	}

}
