using UnityEngine;
using System;

namespace CW
{
	public class InHand : AbstractCardHandler
	{
        public InHand (AbstractCard card, BattlePlayer player) : base(card, player)
		{
        }

		override public void affect ()
		{

            //DebugConsole.Log("affect in hand, diet="+card.diet+card.dietChar);

			//set Max card in field.
			if (player.isActive && player.currentMana >= card.getManaCost () && player == GameManager.player1) 
            {
		
				GameObject removeCard = (GameObject)player.hand [player.hand.IndexOf (card.gameObject)];
		
				int temp = 0, temp2 = 0, count = 0;
				string cardName, cardName2, newCardName, newCardName2;
                

				if (player.cardsInPlay.Count < 6 && IsAnimal(card)) {
                    card.isInHand = false;
                    card.isInPlay = true;
					player.hand.Remove (removeCard);
					player.currentMana -= card.getManaCost ();
					player.cardsInPlay.Add (removeCard.gameObject);
					card.setCanAttack (false);
					card.handler = new InPlay (card, player);
					Vector3 position = new Vector3 (card.transform.position.x, card.transform.position.y, card.transform.position.z);
					player.reposition ();
					Vector3 destination = new Vector3 (card.transform.position.x, card.transform.position.y, card.transform.position.z);
                    card.effect = true;
                    card.transform.position = position;
					card.calculateDirection (destination, false);
                    

					GameManager.player1.getProtocolManager ().sendSummon (player.playerID, card.cardID, card.dietChar, 
			                                        card.level, card.dmg, card.maxHP, 
			                                        card.name, card.type, 
			                                        card.description);
                }
                else if(card.diet == AbstractCard.DIET.WEATHER)
                {
                    
                    player.applyWeather(card.cardID, true);
                    GameManager.player2.applyWeather(card.cardID,false);
                    player.getProtocolManager().sendWeatherCard(player.playerID, card.cardID);
                    player.hand.Remove (removeCard);
                    GameObject.Destroy(removeCard);
                    player.currentMana -= card.getManaCost ();               

                }
                else if(card.diet == AbstractCard.DIET.FOOD){
                    
					//player.hand.Remove (removeCard);
					//player.currentMana -= card.getManaCost ();
					startFoodCard();
				}

			}
            
	
		}
		void startFoodCard(){
			BattlePlayer currentPlayer = GameManager.curPlayer;
			
			if(currentPlayer.clickedCard == null && player.isActive){
				player.clickedCard = card;
				if(player.player1){
					Debug.Log("Player 1 food card prep");
				} else {
					Debug.Log("Player 2 food card prep");
				}	
                	
				
			}else if(currentPlayer.clickedCard != null){
				/*currentPlayer.targetCard = card;
				Debug.Log ("Before apply food buff. Target: " + currentPlayer.ta);
				player.applyFoodBuff(currentPlayer.targetCard, 1, 1);
				Debug.Log ("After food buff");
				//currentPlayer.clickedCard.applyFood(currentPlayer.targetCard, 1, 1);
				currentPlayer.getProtocolManager().sendFoodBuff(currentPlayer.playerID, currentPlayer.targetCard.fieldIndex);
				
				currentPlayer.clickedCard = null;
				currentPlayer.targetCard = null;*/
				
			}
			
			if(currentPlayer.clickedCard != null && currentPlayer == player && card != currentPlayer.clickedCard){
				currentPlayer.clickedCard = null;	
			}
		}

		override public void clicked ()
		{
	
			if (player.isActive)
				affect ();
			
		
		}

		public static bool IsAnimal(AbstractCard card){
			if((card.diet.Equals(AbstractCard.DIET.HERBIVORE)) || (card.diet.Equals(AbstractCard.DIET.OMNIVORE)) || (card.diet.Equals(AbstractCard.DIET.CARNIVORE)))
				return true;
			else 
				return false;
		}
	}

}
	

