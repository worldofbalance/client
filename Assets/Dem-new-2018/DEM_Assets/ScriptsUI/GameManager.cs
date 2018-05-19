using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static bool GameIsOver;

	public GameObject gameOverUI;
	public GameObject completeLevelUI;

	void Start ()
	{
		GameIsOver = false;
	}

	// Update is called once per frame
	void Update () {
		if (GameIsOver)
			return;

		if (TreeOfLifeBehavior.treeHealth <= 0)
		{
			// Start a coroutine Die to let the tree of life have time to react to dying
			StartCoroutine (EndGame());
		}
	}

	public virtual IEnumerator EndGame()
	{
		yield return new WaitForSeconds(2.0f);
		GameIsOver = true;
		gameOverUI.SetActive(true);
	}

	public void WinLevel ()
	{
		GameIsOver = true;
		completeLevelUI.SetActive(true);
	}

}
