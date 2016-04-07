using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConvergeHostConfigProtocol
{

    public static NetworkRequest Prepare(short numRounds, short timeWindow, short betAmount, short ecoNumber ) {
		NetworkRequest request = new NetworkRequest(NetworkCode.MC_HOST_CONFIG);

        request.AddShort16(numRounds);
        request.AddShort16(timeWindow);
        request.AddShort16(betAmount);
        request.AddShort16(ecoNumber);

		return request;
	}
	
	public static NetworkResponse Parse(MemoryStream dataStream) {
		ResponseConvergeHostConfig response = new ResponseConvergeHostConfig();
		
		using (BinaryReader br = new BinaryReader(dataStream, Encoding.UTF8)) {
			short status = br.ReadInt16 ();

			// status = 0 -> successful, 1 -> not successful
			response.status = status;
		}
		
		return response;
	}
}

public class ResponseConvergeHostConfig : NetworkResponse {
	
	public short status { get; set; }
	
	public ResponseConvergeHostConfig() {
		protocol_id = NetworkCode.MC_HOST_CONFIG;
	}

}

