using UnityEngine;
using System.Collections;
namespace CW{
public class ProtocolManager : MonoBehaviour{


	// Use this for initialization
	void Start () {
		// Start pollilng 
		StartCoroutine(PollAction(Constants.UPDATE_RATE));
	}
	
	// Update is called once per frame
	void Update () {

	}



	public void init() {
		
		//StartCoroutine(PollAction(Constants.UPDATE_RATE));
	}

	private IEnumerator PollAction(float time) {
		
		while (true) {
			
			CWGame.networkManager.Send(MatchActionProtocol.Prepare(GameManager.player1.playerID), processMatchAction );
			yield return new WaitForSeconds(time);
		}
	}
	
	


	public void processMatchAction(NetworkResponse response){
		ResponseMatchAction args = response as ResponseMatchAction;
				short actionID = args.actionID;

		if(actionID != 0){
			TurnAction action = args.action;
			action.execute();
		}

	}





	
	private void initMatch() {
		// matchID variable is accessible via the GameState 
		// adding here for clarity
		GameManager.player1.isReady = true;
		GameManager.matchID = GameState.matchID;
		
		setMatchStatus();
		
	}
	
	
	// Let server know that you are logged into the game
	public void setMatchStatus() {
		//Debug.Log("Setting match for playerID: " + GameManager.player1.playerID);
		//Informs the Server we are ready for take off
		CWGame.networkManager.Send (
			MatchStatusProtocol.Prepare (GameManager.player1.playerID, GameManager.player1.playerName), 
			ProcessMatchStatus);
	}
	
	
	
	//Response to the server's ready request
	public void ProcessMatchStatus(NetworkResponse response) {

		short status;
		//Retrieve the response arguments
		ResponseMatchStatus args = response as ResponseMatchStatus;
		status = args.status;	
		if(status == Constants.STATUS_SUCCESS){//Request the deck from Server
			GetDeck ();	
			//Set variables passed in from server related to the match's status
			GameManager.player1.isActive = args.isActive;	
			GameManager.opponentIsReady = args.opponentIsReady;
			
		//Display the arguments passed from server
		Debug.Log("GameManager.player1 isActive: " + GameManager.player1.isActive +
			                    " opponentIsReady " + GameManager.opponentIsReady );
		} else {
			Debug.Log("Failed to acquire Match Status");
            setMatchStatus();
		}

	}
	
	public void sendSummon(int playerID, int cardID,  string diet, 
	                      int level, int attack, int health, 
	                       string species_name,  string type, 
	                      string description){

			CWGame.networkManager.Send (
			SummonCardProtocol.Prepare (playerID, cardID,  diet, 
		                             level, attack,  health, 
		                            species_name,  type, 
		                            description), 
				ProcessSummonCard);

	}
	
	public void ProcessSummonCard(NetworkResponse response) {
		ResponseSummonCard args = response as ResponseSummonCard;
		

	}

	
	public void GetDeck () {
		
		CWGame.networkManager.Send ( GetDeckProtocol.Prepare (GameManager.player1.playerID), 
		                     ProcessGetDeck);
	}

	// Get deck for player1 -- player2 only if Single player
	public void ProcessGetDeck(NetworkResponse response){
		ResponseGetDeck args = response as ResponseGetDeck;
		int numFields = args.numFields;
		int numCards = args.numCards;
		DeckData deck = args.deck;
		//CardData card = deck.popCard();
		
		GameManager.player1.setDeck(deck);
		GameManager.player1.dealCard(3);
		// Don't allow player1 to move cards until they hasDeck
		GameManager.player1.hasDeck = true;
		GameManager.player2.dealDummyCard(3);
		// Only deal cards for opponent if 
		if (Constants.SINGLE_PLAYER) {
			Debug.Log ("Deal player2 Cards");
			GameManager.player2.setDeck (deck);
			GameManager.player2.dealCard (3);
			
		} 
		
		Debug.Log("Protocols Deck Response: deck count: " + deck.getSize() + 
		          " numCards: " + numCards + " num fields: " + numFields); 
		//Debug.Log ("Card data: cardID: " + card.cardID + " diet Type: " +
		//          card.dietType + " SpeciesName: " + card.speciesName);
		
	}



	
	public void sendDealCard( int playerID, int handPosition){
	// Probably not needed -- can automatically set player2 cards dealt
	// on turn switch
		
		CWGame.networkManager.Send ( DealCardProtocol.Prepare (playerID, handPosition), 
		                     ProcessDealCard);	
	}
	

	public void ProcessDealCard(NetworkResponse response){
		ResponseDealCard args = response as ResponseDealCard;
		
		Debug.Log ("DealCard status" + args.status);
		// TODO: 
		//dealDummyCard(1);
		
	}



	public void sendEndTurn (int playerID){
		CWGame.networkManager.Send ( EndTurnProtocol.Prepare (playerID), 
		                     ProcessEndTurn);
		GameManager.player1.isActive = false;
	}
	

	public void ProcessEndTurn(NetworkResponse response){
		ResponseEndTurn args = response as ResponseEndTurn;

		//Debug.Log ("End Turn Response: isActive :" + args.status);
	}


	// Sent when attacking tree
	public void sendTreeAttack(int playerID, int fieldPosition){
		CWGame.networkManager.Send (
			TreeAttackProtocol.Prepare (playerID, fieldPosition), 
			ProcessTreeAttack);
	}

	
	public void ProcessTreeAttack(NetworkResponse response) {
		ResponseTreeAttack args = response as ResponseTreeAttack;
		
		Debug.Log("TreeAttack attack: " + args.status );
		
	}


	public void sendFoodBuff(int playerId, int cardId, int targetPosition){
            
			CWGame.networkManager.Send (
                ApplyFoodBuffProtocol.Prepare(playerId, cardId, targetPosition),
				ProcessFoodBuff);
		}

	public void ProcessFoodBuff(NetworkResponse response){
				ResponseFoodBuff args = response as ResponseFoodBuff;

		}
	// Sent when attacking opponents cards
	public void sendCardAttack(int playerID, int attackersPosition, int attackedPosition){
		CWGame.networkManager.Send (
			CardAttackProtocol.Prepare (playerID, attackersPosition, attackedPosition), 
			ProcessCardAttack);
	}
	
	public void ProcessCardAttack(NetworkResponse response) {
		ResponseCardAttack args = response as ResponseCardAttack;
		

		
	}
	
	public void sendMatchOver (int playerID, int wonGame){
        CWGame.networkManager.Send(MatchOverProtocol.Prepare(playerID, wonGame), ProcessQuitMatch);
//		CWGame.networkManager.Send ( QuitMatchProtocol.Prepare (playerID), 
//            ProcessQuitMatch);
	}

    // Send if "leave/quit match" button clicked
    public void sendQuitMatch (int playerID){
		CWGame.networkManager.Send ( QuitMatchProtocol.Prepare (playerID), 
            ProcessQuitMatch);
	}
	
	public void ProcessQuitMatch(NetworkResponse response){
		ResponseQuitMatch args = response as ResponseQuitMatch;
		bool opponentReadyResponse = false;

	}

	// Sent if player wins game
	public void TestGameOver(int playerID){
		int wonGame = 1;
		CWGame.networkManager.Send ( MatchOverProtocol.Prepare (playerID, wonGame), 
		                     ProcessGameOver);	
	}
	
	public void ProcessGameOver(NetworkResponse response) {
		ResponseMatchOver args = response as ResponseMatchOver;
		short statusResponse = args.status;
		int creditsWon = args.creditsWon;
		Debug.Log ("MatchOver creditsWon: " + args.creditsWon );
		
	}

	public void sendReturnToLobby(){
	CWGame.networkManager.Send ( ReturnLobbyProtocol.Prepare (), 
		                     ProcessReturnToLobby);	
	}

	public void ProcessReturnToLobby(NetworkResponse response){
		ResponseReturnLobby args = response as ResponseReturnLobby;
		Debug.Log ("Return To Lobby processed" + args.status );
	}
    public void sendWeatherCard(int playerID, int card_id){
            
            CWGame.networkManager.Send (
                ApplyWeatherProtocol.Prepare (playerID, card_id),
                ProcessWeatherCard);
        }

    public void ProcessWeatherCard(NetworkResponse response){
            ResponseWeatherCard args = response as ResponseWeatherCard;
            Debug.Log ("Weather Card: " + args.status);
        }

}
}