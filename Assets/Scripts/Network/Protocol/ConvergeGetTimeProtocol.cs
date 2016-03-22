using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConvergeGetTimeProtocol
{

	public static NetworkRequest Prepare() {
		NetworkRequest request = new NetworkRequest(NetworkCode.MC_GET_TIME);
		
		return request;
	}
	
	public static NetworkResponse Parse(MemoryStream dataStream) {
		ResponseConvergeGetTime response = new ResponseConvergeGetTime();
		
		using (BinaryReader br = new BinaryReader(dataStream, Encoding.UTF8)) {
			short betTime = br.ReadInt16 ();

			response.betTime = betTime;
		}
		
		return response;
	}
}

public class ResponseConvergeGetTime : NetworkResponse {
	
	public short betTime { get; set; }
	
	public ResponseConvergeGetTime() {
		protocol_id = NetworkCode.MC_GET_TIME;
	}

}

