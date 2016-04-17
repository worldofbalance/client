using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConvergeNonHostConfigProtocol
{

	public static NetworkRequest Prepare() {
		NetworkRequest request = new NetworkRequest(NetworkCode.MC_NONHOST_CONFIG);
		
		return request;
	}
	
	public static NetworkResponse Parse(MemoryStream dataStream) {
		ResponseConvergeNonHostConfig response = new ResponseConvergeNonHostConfig();
		
        using (BinaryReader br = new BinaryReader (dataStream, Encoding.UTF8)) {
            short numRounds = br.ReadInt16 (); 
            short timeWindow = br.ReadInt16 ();
            short betAmount = br.ReadInt16 ();
            short ecoNumber = br.ReadInt16 ();

            response.numRounds = numRounds;
            response.timeWindow = timeWindow;
            response.betAmount = betAmount;
            response.ecoNumber = ecoNumber;
        }
		
		return response;
	}
}

public class ResponseConvergeNonHostConfig : NetworkResponse {
	
    public short numRounds { get; set; }
    public short timeWindow { get; set; }
    public short betAmount { get; set; }
    public short ecoNumber { get; set; }

	public ResponseConvergeNonHostConfig() {
        protocol_id = NetworkCode.MC_NONHOST_CONFIG;
	}
}

