//Authored by Marlo Sandoval
//Description: Code for the speed boost power-up. Boosts the player's speed and adds unscored points to the player's score.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD
{
    public class SpeedBoostPowerUp : MonoBehaviour
    {

        private GameController gameController;
        private PlayerController playerController;

        //damage dealt after interacting a jellyfish
        private const int damage = 10;
        private static float originalSpeed;
        private static float originalMaxSpeed;

        void Start()
        {
            gameController = GameController.getInstance();
        }

        //when the player consumes the power-up
        private void OnTriggerEnter(Collider thing)
        {
            if (thing.CompareTag("Player"))
            {
                StartCoroutine(activateEffect(thing));

                //adds unscored points to player's score
                gameController.AddScore(gameController.GetUnscored());
                gameController.ResetUnscored();

                //restores some health upon contact
                if (gameController.GetHealth() < 100)
                {
                    gameController.UpdateHealth(damage);
                }
            }
        }

        //turns on the power-up's effects
        IEnumerator activateEffect(Collider player)
        {
            playerController = player.GetComponent<SD.PlayerController>();
            originalSpeed = playerController.baseMaxSpeed;
            originalMaxSpeed = playerController.absoluteMaxSpeedLimit;

            //changes the speed of the player
            gameController.SetIsSpeedBuffActive(true);
            playerController.baseMaxSpeed = originalSpeed * 2.5f;
            playerController.absoluteMaxSpeedLimit = playerController.absoluteMaxSpeedLimit * 2f;
            if (playerController.baseMaxSpeed > playerController.absoluteMaxSpeedLimit)
            {
                playerController.baseMaxSpeed = playerController.absoluteMaxSpeedLimit;
            }

            //makes the power-up disappear
            GetComponent<SkinnedMeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            //effects are active for 5 seconds
            yield return new WaitForSeconds(50f);

            //undoes the effects
            playerController.baseMaxSpeed = originalSpeed;
            playerController.absoluteMaxSpeedLimit = originalMaxSpeed;
            gameController.SetIsSpeedBuffActive(false);
            Destroy(gameObject);
        }
    }
}
