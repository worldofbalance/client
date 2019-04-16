//Authored by Marlo Sandoval

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour {

    //list of charcter models
    private List<GameObject> characters;

    //default selection index of the model
    private int selectionIndex = 0;

    // Use this for initialization
    private void Start () {
        characters = new List<GameObject>();
        //adds models to list
        foreach(Transform t in transform)
        {
            characters.Add(t.gameObject);
            //sets active to false; models don't appear
            t.gameObject.SetActive(false);
        }
        //sets active to true for the character at a specific index; model will appear
        characters[selectionIndex].SetActive(true);
    }

    public void FixedUpdate()
    {

    }

    //function handles with selecting and swapping the models
    public void Select(int index)
    {
        //exits the function if index is out-of-bounds or matches the current selectionIndex
        if(index < 0 || index >= characters.Count || index == selectionIndex)
        {
            return;
        }

        //makes current model disappear
        characters[selectionIndex].SetActive(false);

        //changes the index and makes the model with that index appear
        selectionIndex = index;
        characters[selectionIndex].SetActive(true);
    }
	
}
