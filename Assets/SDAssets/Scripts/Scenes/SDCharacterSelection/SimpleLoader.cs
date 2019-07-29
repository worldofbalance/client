//Authored by Marlo Sandoval
//Description: Temporary scene loader for the character selection screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SimpleLoader : MonoBehaviour {

	public void loadMainMenu()
    {
        SceneManager.LoadScene("SDReadyScene");
    }

    public void loadGame()
    {
        SceneManager.LoadScene("SDGameMain");
    }
}
