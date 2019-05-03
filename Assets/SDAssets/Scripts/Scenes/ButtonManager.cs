using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound player script for button mouseovers and clicks.
/// </summary>
public class ButtonManager : MonoBehaviour
{
    public List<AudioClip> mouseoverSounds;
    public List<AudioClip> buttonClickSounds;
    public List<AudioClip> transistionSounds;

    private AudioSource audioSource;
    private System.Random rng;

	void Start ()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        rng = new System.Random();
	}

    // Play a random sound on mouseover.
    public void BtnPlayMouseoverSound()
    {
        audioSource.PlayOneShot(mouseoverSounds[rng.Next(0, (mouseoverSounds.Count))]);
    }

    // Play button click sound on mouse down.
    public void BtnPlayButtonClickSound()
    {
        audioSource.PlayOneShot(buttonClickSounds[rng.Next(0, (buttonClickSounds.Count))]);
    }

    // Play button click sound on transition screen.
    public void BtnPlaySceneTransitionSound()
    {
        audioSource.PlayOneShot(transistionSounds[rng.Next(0, (transistionSounds.Count))]);
    }
}
