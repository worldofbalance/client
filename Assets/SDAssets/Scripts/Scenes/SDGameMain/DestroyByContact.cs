/* 
 * File Name: DestroyByContact.cs
 * Description: Destroys attached objects if they collide with the player.
 */

using UnityEngine;
using System.Collections;

namespace SD {
    public class DestroyByContact : MonoBehaviour {

        private GameController gameController;
        private int newScoreValue = 10; // Score to be recieved by eating prey
        private GameObject mainCamera;
        private AudioSource audioSource;
        public AudioClip audioClip;

        // Use this for initialization
        void Start () {
            gameController = GameController.getInstance ();
            audioSource = GetComponent<AudioSource> ();
            mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
            audioSource.clip = audioClip;

        }
            
        
        // Update is called once per frame
        void Update () {
            //takes effect when the player picks up a point boost power-up
            /*
            if (gameController.getPointBoostStatus() == true)
            {
                newScoreValue = 50;
            }
            else
            {
                newScoreValue = 10;
            }
            */
        }

        // Destroys the attached object upon a collison with the player
        void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                int npcFishId = gameObject.GetComponentInParent<NPCFishController>().getNPCFishData().id;
                int npcFishSpeciesId = gameObject.GetComponentInParent<NPCFishController> ().getNPCFishData().speciesId;
                Debug.Log ("Consumed prey with ID: " + npcFishId);
                if (SDMain.networkManager != null) {
                    GameManager.getInstance ().DestroyNPCFish (npcFishId, npcFishSpeciesId);
                }
                AudioSource.PlayClipAtPoint (audioClip, mainCamera.transform.position);
                gameController.destroyPrey (npcFishId);

                /*
                if (npcFishSpeciesId == 5)
                {
                    gameController.AddUnscoredPoint(newScoreValue * 2);
                    //attempting to increase points based on food chain relationship
                }
                else { gameController.AddUnscoredPoint(newScoreValue); }
                */

                gameController.AddUnscoredPoint(newScoreValue);
            }
        }
    }
}
