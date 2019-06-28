/*
 * File Name: BaseManager.cs
 * Description: Will Handle all events between the player and the base
 *              So far,
 *              1. Player can score their unscored points staying more than 3 second in the base
 *              2. While player is in the base. The stamina stays at 100.
 * 
 */ 


using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SD {
    public class BaseManager : MonoBehaviour {

        private GameController gameController;
        private float time;
        private float secondsToScore = 2.75f;
        private float staminaRevoverRate = 0.1f;
        public GameObject baseScorePanel;
        public bool inBase = false;
        private AudioSource audioSource;
        public AudioClip scoringClip;

        // Use this for initialization
        void Start () {
            gameController = GameController.getInstance ();
            audioSource = GetComponent<AudioSource> ();
            audioSource.clip = scoringClip;
            audioSource.volume = 0.2f;
        }

        // Update is called once per frame
        void Update () {

        }

        // Runs when the player collides the base
        void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                time = secondsToScore;
                if (gameController.GetUnscored() > 0)
                {
                    audioSource.Play();
                    gameController.showBaseScorePanel();
                }
                else gameController.hideBaseScorePanel();
                gameController.setIsPlayerInBase (true);
            }
        }

        // Runs while the player is staying in the base
        void OnTriggerStay(Collider other) {
            if (other.tag == "Player") {
                
                if (gameController)
                    gameController.stamina += staminaRevoverRate;


                // After couplse seconds that is defined by timeToScore, 
                // add unscored points to the actual score
                time -= 1f * Time.deltaTime;
                if (time <= 0) {
                    gameController.Score ();
                    gameController.ResetUnscored ();
                    //gameController.stamina = 100;
                    gameController.hideBaseScorePanel ();
                }
            }
        }

        void OnTriggerExit(Collider other) {
            if (other.tag == "Player") {
                gameController.setIsPlayerInBase (false);
                gameController.hideBaseScorePanel ();
            }
        }
    }
}