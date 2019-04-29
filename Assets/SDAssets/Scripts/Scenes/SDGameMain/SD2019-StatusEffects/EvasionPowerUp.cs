using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD
{
    public class EvasionPowerUp : MonoBehaviour
    {

        private GameController gameController;
        private PlayerController playerController;

        //health added when getting the power-up
        private const int damage = 10;

        // Use this for initialization
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

            //makes the power-up disappear
            GetComponent<SkinnedMeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            gameController.setEvasionBoost(true);
            Debug.Log(gameController.getEvasionBoostStatus());

            //effects are active for 15 seconds
            yield return new WaitForSeconds(15f);

            //undoes the effects
            gameController.setEvasionBoost(false);
            Debug.Log(gameController.getEvasionBoostStatus());
            Destroy(gameObject);
        }
    }
}
