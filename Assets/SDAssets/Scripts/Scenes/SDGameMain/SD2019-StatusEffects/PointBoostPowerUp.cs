using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD
{
    public class PointBoostPowerUp : MonoBehaviour
    {

        private GameController gameController;
        
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
                //adds unscored points to player's score
                gameController.AddScore(gameController.GetUnscored());
                gameController.ResetUnscored();

                StartCoroutine(activateEffect(thing));

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
            //makes the power-up disappear
            GetComponent<SkinnedMeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            gameController.setPointBoost(true);

            //effects are active for 10 seconds
            yield return new WaitForSeconds(100f);

            //undoes the effects
            gameController.setPointBoost(false);
            Destroy(gameObject);
        }
    }
}
