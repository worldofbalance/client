using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConvergeCheckPlayersProtocol
{

	public static NetworkRequest Prepare() {
		NetworkRequest request = new NetworkRequest(NetworkCode.MC_CHECK_PLAYERS);
		
		return request;
	}
	
	public static NetworkResponse Parse(MemoryStream dataStream) {
		ResponseConvergeCheckPlayers response = new ResponseConvergeCheckPlayers();
		
		using (BinaryReader br = new BinaryReader(dataStream, Encoding.UTF8)) {
			short status = br.ReadInt16 ();

			response.status = status;
		}
		
		return response;
	}
}

public class ResponseConvergeCheckPlayers : NetworkResponse {
	
	public short status { get; set; }
	
	public ResponseConvergeCheckPlayers() {
		protocol_id = NetworkCode.MC_CHECK_PLAYERS;
	}

}

