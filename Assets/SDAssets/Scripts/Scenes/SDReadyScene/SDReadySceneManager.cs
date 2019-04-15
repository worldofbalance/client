using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace SD {
    public class SDReadySceneManager : MonoBehaviour {

        private static SDReadySceneManager sceneManager;
        private bool _isPlayerReady, _isOpponentReady;
        private bool isPlayerReady { 
            get {
                return this._isPlayerReady;
            }
            set {
                this._isPlayerReady = value;
                if (this._isPlayerReady && this.isOpponentReady) {
                    Debug.Log ("Loading the Game Scene..");
                    try{
                    SDMain.networkManager.Ignore (NetworkCode.SD_PLAYER_POSITION, ResponseSDStartSync);
                    }
                    catch(Exception e) {
                    }
                    SceneManager.LoadScene ("SDGameMain");
                }
            }
        }
        private bool isOpponentReady {
            get {
                return this._isOpponentReady;
            }
            set {
                this._isOpponentReady = value;
                if (this._isPlayerReady && this._isOpponentReady) {
                    Debug.Log ("Loading the Game Scene..");
                    try {
                    SDMain.networkManager.Ignore (NetworkCode.SD_PLAYER_POSITION, ResponseSDStartSync);
                    }
                    catch (Exception e) {
                    }
                    SceneManager.LoadScene ("SDGameMain");
                }
            }
        }

        public static SDReadySceneManager getInstance() {
            return sceneManager;
        }

        // Use this for initialization
        void Start () {
            sceneManager = this;
            isPlayerReady = false;
            isOpponentReady = false;

            if (SDMain.networkManager != null) {
                SDMain.networkManager.Listen (NetworkCode.SD_PLAYER_POSITION, ResponseSDStartSync);
                SDMain.networkManager.Listen (NetworkCode.SD_START_GAME, ResponseSDStartGame);
            } else {
                Debug.LogWarning ("Could not obtain a connection to Sea Divided Server. Falling back to offline mode.");
            }
        }

        public void StartGame() {
            if (SDMain.networkManager != null) {
                SDMain.networkManager.Send (SDStartGameProtocol.Prepare (Constants.USER_ID));
                isPlayerReady = true;
            } else {
                Debug.LogWarning ("Starting game without server component.");
                //SceneManager.LoadScene ("SDGameMain");
                SceneManager.LoadScene ("SelectionScreen");
            }
        }

        public void ResponseSDStartGame (NetworkResponse r)
        {
            ResponseSDStartGame response = r as ResponseSDStartGame;
            Debug.Log ("ResponseSDStartGame called.");
            SDPersistentData.getInstance ().setRoundStartTime (response.startDateTime);
            if (response.status == 0) {
                // Send a request to the opponent indicating that this client is ready to play.
                SDMain.networkManager.Send (SDPlayerPositionProtocol.Prepare (
                    0.ToString (), 0.ToString (), 0.ToString()));
                Debug.Log ("Waiting for opponent to respond..");
            } else {
                Debug.Log ("Encountered an error in starting the game.");
            }
        }

        public void ResponseSDStartSync(NetworkResponse r) {
            ResponseSDPlayerPosition response = r as ResponseSDPlayerPosition;
            Debug.Log ("The opponent is ready to play, loading the game scene.");
            this.isOpponentReady = true;
        }
    }
}
