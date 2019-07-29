//Authored by Marlo Sandoval
//Description: Code for the effects when interacting with a jellyfish (hazard). Reduces player's speed and gives damage.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD
{
    public class JellyfishStatusEffects : MonoBehaviour
    {
        private GameController gameController;
        private PlayerController playerController;

        //damage dealt after interacting a jellyfish
        private const int damage = -10;
        private float lastDamage = 0;
        private static float originalSpeed;

        void Start()
        {
            gameController = GameController.getInstance();

            //hides the hitbox
            GetComponent<MeshRenderer>().enabled = false;
        }

        //when the player makes contact with a jellyfish for a moment
        private void OnTriggerEnter(Collider thing)
        {
            if (thing.CompareTag("Player"))
            {
                StartCoroutine(activateEffect(thing));
                if (gameController.GetHealth() > 0)
                {
                    gameController.UpdateHealth(damage);
                }
            }
        }

        //when the player makes contact with a jellyfish and stays there
        private void OnTriggerStay(Collider thing)
        {
            if (thing.CompareTag("Player"))
            {
                lastDamage += Time.deltaTime;
                if (lastDamage >= 2)
                {
                    StartCoroutine(activateEffect(thing));
                    if (gameController.GetHealth() > 0)
                    {
                        gameController.UpdateHealth(damage);
                    }
                    lastDamage = 0;
                }
            }
        }

        //turns on the jellyfish's effects
        IEnumerator activateEffect(Collider player)
        {
            playerController = player.GetComponent<SD.PlayerController>();
            originalSpeed = playerController.baseMaxSpeed;
            //changes the speed of the player
            playerController.baseMaxSpeed = originalSpeed * .5f;
            if (playerController.baseMaxSpeed < 10)
            {
                playerController.baseMaxSpeed = 10;
            }

            //effects are active for 5 seconds
            yield return new WaitForSeconds(5f);

            //undoes the effects
            playerController.baseMaxSpeed = originalSpeed;
         }
    }
}
