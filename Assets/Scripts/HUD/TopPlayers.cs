using UnityEngine;
using System;
using System.Collections;

public class TopPlayers : MonoBehaviour
{
    // This class is used to send a network request to 
    // retrieve the top rated players from the server

    private String[] topPlayerNames = new String[3];
    private int[] topPlayerScores = new int[3];
    public delegate void Callback (String[] topPlayerNames, int[] topPlayerscores);
    private Callback passedInFunc;
    
    public void requestTopPlayers(Callback callback)
    {
        passedInFunc = callback;

        NetworkManager.Send(
            TopListProtocol.Prepare(),
            ProcessTopList
       );
    }

    private void ProcessTopList(NetworkResponse response)
    {
        ResponseTopList args = response as ResponseTopList;
        topPlayerNames[0] = args.name1;
        topPlayerNames[1] = args.name2;
        topPlayerNames[2] = args.name3;
        topPlayerScores[0] = args.score1;
        topPlayerScores[1] = args.score2;
        topPlayerScores[2] = args.score3;

        passedInFunc(topPlayerNames, topPlayerScores);
        
    }

}
