using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the controller for the point boost buff.
/// Player receives an extra unscored point for ever buff stack per fish consumed
/// while under the effect of this buff.
/// </summary>
namespace SD
{
    public class PointBuffController : BuffBackend
    {
        // Variables that can be adjusted for balance.
        public float perStackBonus;
        public float buffDuration;
        public int maxBuffStacks;

        // Get the player and their collider.
        private PlayerController player;
        private GameController gameController;

        void Start()
        {
            player = gameObject.GetComponent<PlayerController>();
            gameController = GameController.getInstance();

            SetBaseStat(0.0f);
            SetMaxStackAmount(maxBuffStacks);
            SetMaxBuffDuration(buffDuration);
            SetBuffMechanism(BuffMechanism.FLAT);
            SetInitialBuffBonus(0.0f);
            SetBuffBonusPerStack(perStackBonus);
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Collided with: " + other.gameObject.tag);

            // If the collision target is a buff fish, adjust stacks.
            if (other.gameObject.tag == "PointBuffFish")
            {
                // Add a stack of the buff, get the recalculated buff results.
                ApplyBuff();
            }
        }

        void Update()
        {
            // Round the point bonus to avoid truncation errors, then cast to int.
            gameController.SetPointBonusAmount((int)Math.Round(GetAdjustedStatAmount()));

            if (GetBuffStackAmount() > 0)
            {
                gameController.SetIsPointBuffActive(true);
            }
            else
            {
                gameController.SetIsPointBuffActive(false);
            }
        }
    }
}