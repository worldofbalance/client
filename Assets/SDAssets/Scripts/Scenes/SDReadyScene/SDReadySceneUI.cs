using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SD {
    public class SDReadySceneUI : MonoBehaviour {

        private Button btnPlaySdv;
        private Text txtWaitingForOpponent;
        private static SDReadySceneUI sdReadySceneUI;
        public GameObject howToPlayPanal, logo, menu;

        void OnAwake() {
            sdReadySceneUI = this;
        }

        void OnEnable () {
            btnPlaySdv = GameObject.Find ("BtnPlaySDV").GetComponent<Button> ();
            txtWaitingForOpponent = GameObject.Find ("TxtWaiting").GetComponent<Text> ();
        }

        public void BtnBackToLobbyClick() {
            Game.SwitchScene ("World");
        }

        public void BtnPlaySDVClick() {
            SDReadySceneManager.getInstance().StartGame ();
            btnPlaySdv.interactable = false;
        }

        public void BtnHowToPlayClick() {
            howToPlayPanal.SetActive (true);
            logo.SetActive(false);
            menu.SetActive(false);
        }

        public void BtnHowToPlayCloseClick() {
            howToPlayPanal.SetActive (false);
            logo.SetActive(true);
            menu.SetActive(true);
        }

        public static SDReadySceneUI getInstance() {
            return sdReadySceneUI;
        }

        public Text getWaitingText() {
            return txtWaitingForOpponent;
        }
    }
}
