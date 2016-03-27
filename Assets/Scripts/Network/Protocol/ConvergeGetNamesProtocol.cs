using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConvergeGetNamesProtocol
{

	public static NetworkRequest Prepare() {
		NetworkRequest request = new NetworkRequest(NetworkCode.MC_GET_NAMES);
		
		return request;
	}

	// DH change
	// These are the player IDs and names for upto 4 opponents.
	// This is used to display information about the opponents
	// The player is not returned by the server
	// If less than 4 opponents exist then 0 and "" are returned

	public static NetworkResponse Parse(MemoryStream dataStream) {
		ResponseConvergeGetNames response = new ResponseConvergeGetNames();
		
		using (BinaryReader br = new BinaryReader(dataStream, Encoding.UTF8)) {
			int player1ID = br.ReadInt32 ();
			short fldSize = br.ReadInt16 ();
			string player1Name = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));
			int player2ID = br.ReadInt32 ();
			fldSize = br.ReadInt16 ();
			string player2Name = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));
			int player3ID = br.ReadInt32 ();
			fldSize = br.ReadInt16 ();
			string player3Name = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));
			int player4ID = br.ReadInt32 ();
			fldSize = br.ReadInt16 ();
			string player4Name = System.Text.Encoding.UTF8.GetString (br.ReadBytes (fldSize));

			response.player1ID = player1ID;
			response.player1Name = player1Name;
			response.player2ID = player2ID;
			response.player2Name = player2Name;
			response.player3ID = player3ID;
			response.player3Name = player3Name;
			response.player4ID = player4ID;
			response.player4Name = player4Name;
		}
		
		return response;
	}
}

public class ResponseConvergeGetNames : NetworkResponse {
	
	public int player1ID { get; set; }
	public string player1Name { get; set; }
	public int player2ID { get; set; }
	public string player2Name { get; set; }
	public int player3ID { get; set; }
	public string player3Name { get; set; }
	public int player4ID { get; set; }
	public string player4Name { get; set; }
	
	public ResponseConvergeGetNames() {
		protocol_id = NetworkCode.MC_GET_NAMES;
	}

}

