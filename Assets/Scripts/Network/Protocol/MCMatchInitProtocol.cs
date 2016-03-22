using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

// DH change
// New script

public class MCMatchInitProtocol
{
			
	public static NetworkRequest Prepare (int playerID, int roomID, short host)
	{
		NetworkRequest request = new NetworkRequest (NetworkCode.MC_MATCH_INIT);
		request.AddInt32 (playerID);
		request.AddInt32 (roomID);
		request.AddShort16 (host);
	
		return request;
	}
	
	public static NetworkResponse Parse (MemoryStream dataStream)
	{
		ResponseMCMatchInit response = new ResponseMCMatchInit ();
		response.status = DataReader.ReadShort (dataStream);
	
		if (response.status == 0) {
			response.matchID = DataReader.ReadInt (dataStream);
		}
		return response;
	}
}
	
public class ResponseMCMatchInit : NetworkResponse
{
		
	public short status { get; set; }

	public int matchID { get; set; }
		
	public ResponseMCMatchInit ()
	{
		protocol_id = NetworkCode.MC_MATCH_INIT;
	}
}
