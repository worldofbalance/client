using UnityEngine;

public class SDReadySceneUI : MonoBehaviour {

    public void BtnBackToLobbyClick() {
        Game.SwitchScene ("World");
    }
}