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
			int player1ID = br.ReadInt32 ();
			short betStatus1 = br.ReadInt16 ();
			int player2ID = br.ReadInt32 ();
			short betStatus2 = br.ReadInt16 ();
			int player3ID = br.ReadInt32 ();
			short betStatus3 = br.ReadInt16 ();
			int player4ID = br.ReadInt32 ();
			short betStatus4 = br.ReadInt16 ();

			response.betTime = betTime;
			response.player1ID = player1ID;
			response.betStatus1 = betStatus1;
			response.player2ID = player2ID;
			response.betStatus2 = betStatus2;
			response.player3ID = player3ID;
			response.betStatus3 = betStatus3;
			response.player4ID = player4ID;
			response.betStatus4 = betStatus4;
		}
		
		return response;
	}
}

public class ResponseConvergeGetTime : NetworkResponse {
	
	public short betTime { get; set; }
	public int player1ID { get; set; }
	public short betStatus1 { get; set; }
	public int player2ID { get; set; }
	public short betStatus2 { get; set; }
	public int player3ID { get; set; }
	public short betStatus3 { get; set; }
	public int player4ID { get; set; }
	public short betStatus4 { get; set; }
	
	public ResponseConvergeGetTime() {
		protocol_id = NetworkCode.MC_GET_TIME;
	}

}

