using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper script to find and access the persistent button sound player.
/// </summary>
public class ButtonController : MonoBehaviour
{
    private GameObject soundManagerObject;
    
	void Start ()
    {
        soundManagerObject = GameObject.Find("Button Sound Manager");
        Console.WriteLine("Ate speedBuffFish.");
    }
	
    public void PlayMouseoverSound()
    {
        soundManagerObject.GetComponent<ButtonManager>().BtnPlayMouseoverSound();
    }

    public void PlayButtonClickSound()
    {
        soundManagerObject.GetComponent<ButtonManager>().BtnPlayButtonClickSound();
    }
}
