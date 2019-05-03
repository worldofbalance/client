using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the speed buff controller script.
/// Inherits from the BuffTemplate class.
/// </summary>
public class SpeedBuffController : BuffTemplate
{
    // Variables that can be adjusted for balance.
    public float perStackMultiplier;
    public float buffDuration;
    public int maxBuffStacks;

    // Get the player and their collider.
    private SD.PlayerController player;
    private Collider playerCollider;

    private void Awake()
    {
        SetBaseStat(player.GetBaseSpeed());
        SetMaxStackAmount(maxBuffStacks);
        SetMaxBuffDuration(buffDuration);
        SetBuffMechanism(0);
        SetMultiplerPerBuffStack(perStackMultiplier);

        player.gameObject.GetComponent<SD.PlayerController>();
        playerCollider = player.GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the collision target is a buff fish, adjust stacks.
        if(collision.gameObject.tag == "SpeedBuffFish")
        {
            // Add a stack of the buff, get the recalculated results, and set the player speed limit to the new limit.
            player.SetCurrentSpeed(AddStack());
        }
    }

    private void Update()
    {
        player.SetCurrentSpeed(GetAdjustedStatAmount());
    }
}
