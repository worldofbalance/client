using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;


//this is my protocol to update the player's credits on the client, server, and db
//action =0 adds credits
//action =1 subtracts credits
//-Jeremy
public class UpdateCreditsProtocol : MonoBehaviour
{

    public static NetworkRequest Prepare(short action, int credits)
    {
        NetworkRequest request = new NetworkRequest(NetworkCode.UPDATE_CREDITS);
        Debug.Log("UpdateCredits preparing: action= " + action + ",credits= " + credits);
        request.AddShort16(action);
        request.AddInt32(credits);


        return request;
    }

    public static NetworkResponse Parse(MemoryStream dataStream)
    {
        ResponseUpdateCredits response = new ResponseUpdateCredits();

        response.action = DataReader.ReadShort(dataStream);
        response.status = DataReader.ReadShort(dataStream);
        response.newCredits = DataReader.ReadInt(dataStream);

        Debug.Log("UpdateCreditsProtocol parsing: action= "+ response.action + ", status= " + response.status + ", newCredits= "+response.newCredits);

        

        return response;
    }
}

public class ResponseUpdateCredits : NetworkResponse
{

    public short action { get; set; }
    public short status { get; set; }
    public int newCredits { get; set; }

    public ResponseUpdateCredits()
    {
        protocol_id = NetworkCode.UPDATE_CREDITS;
    }
}
