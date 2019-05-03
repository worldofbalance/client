using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SD {
    public class SDGameEndUI : MonoBehaviour {

        private SDPersistentData persistentObject;

        public void BtnBackToLobbyClick() {
            SD.Constants.PLAYER_NUMBER = -1;
            Game.SwitchScene ("World");
        }

        public void BtnPlayAgainClick() {
            // Start another round of the game with the same players.
            SceneManager.LoadScene("SDReadyScene");
            persistentObject = SDPersistentData.getInstance();
            if (persistentObject) 
                persistentObject.initializeData ();
        }

    }
}
