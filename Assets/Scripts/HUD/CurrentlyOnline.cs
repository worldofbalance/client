using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class CurrentlyOnline : MonoBehaviour
{
    // This class retrieves the currently online players
    public delegate void Callback(Dictionary<int, Player> playerList);
    private Callback passedInFunc; 

    public void requestOnlinePlayers(Callback callback)
    {
        passedInFunc = callback;

        NetworkManager.Send(
            PlayersProtocol.Prepare(),
            ProcessOnlinePlayers
        );
    }

    private void ProcessOnlinePlayers(NetworkResponse response)
    {
        ResponsePlayers args = response as ResponsePlayers;
        passedInFunc(args.playerList);

    }

}
