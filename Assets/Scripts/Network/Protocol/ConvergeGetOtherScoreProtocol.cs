using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConvergeGetOtherScoreProtocol
{

	public static NetworkRequest Prepare(int id_otherPlayer) {
		NetworkRequest request = new NetworkRequest(NetworkCode.MC_GET_OTHER_SCORE);

		request.AddInt32(id_otherPlayer);
		
		return request;
	}
	
	public static NetworkResponse Parse(MemoryStream dataStream) {
		ResponseConvergeGetOtherScore response = new ResponseConvergeGetOtherScore();
		
		using (BinaryReader br = new BinaryReader(dataStream, Encoding.UTF8)) {
			int score0 = br.ReadInt32 ();
			int score1 = br.ReadInt32 ();
			int score2 = br.ReadInt32 ();
			int score3 = br.ReadInt32 ();
			int score4 = br.ReadInt32 ();

			response.score0 = score0;
			response.score1 = score1;
			response.score2 = score2;
			response.score3 = score3;
			response.score4 = score4;
		}
		
		return response;
	}
}

public class ResponseConvergeGetOtherScore : NetworkResponse {

	public int score0 { get; set; }
	public int score1 { get; set; }
	public int score2 { get; set; }
	public int score3 { get; set; }
	public int score4 { get; set; }
	
	public ResponseConvergeGetOtherScore() {
		protocol_id = NetworkCode.MC_GET_OTHER_SCORE;
	}

}

