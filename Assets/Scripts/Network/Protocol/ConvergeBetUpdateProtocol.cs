using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConvergeBetUpdateProtocol
{

	public static NetworkRequest Prepare(short betEntered, int improveValue) {
		NetworkRequest request = new NetworkRequest(NetworkCode.MC_BET_UPDATE);

		request.AddShort16(betEntered);
		request.AddInt32(improveValue);

		return request;
	}
	
	public static NetworkResponse Parse(MemoryStream dataStream) {
		ResponseConvergeBetUpdate response = new ResponseConvergeBetUpdate();
		
		using (BinaryReader br = new BinaryReader(dataStream, Encoding.UTF8)) {
			short winStatus = br.ReadInt16 ();
			int wonAmount = br.ReadInt32 ();

			// winStatus = 1 -> won round, 0 -> lost round, or didn't play
			response.winStatus = winStatus;
			response.wonAmount = wonAmount;
		}
		
		return response;
	}
}

public class ResponseConvergeBetUpdate : NetworkResponse {
	
	public short winStatus { get; set; }
	public int wonAmount { get; set; }
	
	public ResponseConvergeBetUpdate() {
		protocol_id = NetworkCode.MC_BET_UPDATE;
	}

}

