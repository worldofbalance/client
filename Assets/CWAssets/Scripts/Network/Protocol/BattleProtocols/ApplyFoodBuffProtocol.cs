using System.IO;

namespace CW
{
	public class ApplyFoodBuffProtocol
	{

		public static NetworkRequest Prepare (int playerID, int cardID, int target)
		{
		
			NetworkRequest request = new NetworkRequest (NetworkCode.APPLY_FOOD);
			request.AddInt32 (playerID);
            request.AddInt32(cardID);
            request.AddInt32 (target);
			return request;
		}
	
		public static NetworkResponse Parse (MemoryStream dataStream)
		{
			ResponseFoodBuff response = new ResponseFoodBuff ();
		
			response.status = DataReader.ReadShort (dataStream);
		
			return response;
		}


	}

	public class ResponseFoodBuff : NetworkResponse
	{
		
		public short status { get ; set; }
		
		public ResponseFoodBuff ()
		{
			protocol_id = NetworkCode.APPLY_FOOD;
		}
	}
}