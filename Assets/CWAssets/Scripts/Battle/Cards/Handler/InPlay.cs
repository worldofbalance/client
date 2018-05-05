using System;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CW
{
	public class InPlay : AbstractCardHandler
	{
		//player is owner of the card
		public InPlay (AbstractCard card, BattlePlayer player) : base(card, player)
		{
			
		}
		
		public override void affect ()
		{
			BattlePlayer currentPlayer = GameManager.curPlayer;
            
            
			if (currentPlayer.clickedCard == null && player.isActive && card.canAttack()) 
            {
				player.clickedCard = card;
				if (player.player1) {
					Debug.Log ("Player 1 is ready to attack");
				} else {
					Debug.Log ("Player 2 is ready to attack");
				}		
				
			} 
            else if (currentPlayer.clickedCard != null) 
            {
                
                if (currentPlayer.clickedCard.diet == AbstractCard.DIET.FOOD) 
                {
                    if (player.isActive) {
                      
                        currentPlayer.targetCard = card;

                        currentPlayer.applyFoodBuff (currentPlayer.targetCard, 1, 1);

                        //currentPlayer.clickedCard.applyFood(currentPlayer.targetCard, 1, 1);
                        //currentPlayer.getProtocolManager ().sendFoodBuff (currentPlayer.cardID, currentPlayer.targetCard.fieldIndex);
                        currentPlayer.getProtocolManager ().sendFoodBuff (currentPlayer.playerID, currentPlayer.targetCard.fieldIndex);
                        currentPlayer.hand.Remove (currentPlayer.clickedCard.gameObject);
                        GameObject.Destroy (currentPlayer.clickedCard.gameObject);
                        currentPlayer.currentMana -= currentPlayer.clickedCard.getManaCost ();
                        currentPlayer.clickedCard = null;
                        currentPlayer.targetCard = null;
                    } else {
                        currentPlayer.clickedCard = null;
                    }
					
				} 
                
                //else if (currentPlayer != player && currentPlayer.clickedCard.diet != AbstractCard.DIET.HERBIVORE) 
                else if (currentPlayer != player && (currentPlayer.clickedCard.diet == AbstractCard.DIET.CARNIVORE || currentPlayer.clickedCard.diet == AbstractCard.DIET.OMNIVORE))
                {
                    
                    currentPlayer.targetCard = card;	
					bool attackback = false;
					if (currentPlayer.targetCard.diet != AbstractCard.DIET.HERBIVORE) {
						attackback = true;
					}

                    //2018 Spring semester WoB Ecosystem
                    //Lion 86, Buffalo 7, Grass 96, Bush Pig 83, Tree Mouse 31, 
                    //Cockroach 19, Decaying Material 89
                    int[] ecoID1 = { 86, 7, 83, 31, 19 };
                    int[] ecoID2 = { 89, 96 };
                    SpeciesData speciesData;

                    //Check If the current clicked card is in the 2018 Spring semester WoB Ecosystem
                    //for species
                    if (ecoID1.Contains(currentPlayer.clickedCard.cardID))
                    {
                        //Call the unity db and retrieve the predator and prey list
                        speciesData = SpeciesTable.speciesList[currentPlayer.clickedCard.cardID];
                        List<string> predatorList = new List<string>(speciesData.predatorList.Values);
                        predatorList.Sort();
                        string[] pdlist = predatorList.ToArray();
                        //Debug.Log("Predator: " + pdlist[0]);

                        List<string> preyList = new List<string>(speciesData.preyList.Values);
                        preyList.Sort();
                        string[] prelist = preyList.ToArray();
                        //Debug.Log("Prey: " + prelist[0]);

                        //if player species is predator of opponent species
                        if (preyList.Contains(currentPlayer.targetCard.name))
                        {
                            currentPlayer.clickedCard.dmg += 3;
                            currentPlayer.clickedCard.hp += 3;
                        }


                        //if player species is prey of opponent species
                        if (pdlist.Contains(currentPlayer.targetCard.name))
                        {
                            currentPlayer.clickedCard.dmg -= 3;
                            currentPlayer.clickedCard.hp -= 3;
                        }
                    }
                    
                    //for food
                    if (ecoID2.Contains(currentPlayer.clickedCard.cardID))
                    {
                        speciesData = SpeciesTable.speciesList[currentPlayer.clickedCard.cardID];
                        List<string> predatorList = new List<string>(speciesData.predatorList.Values);
                        predatorList.Sort();
                        string[] pdlist = predatorList.ToArray();
                        //Debug.Log("Predator: " + pdlist[0]);

                        List<string> preyList = new List<string>(speciesData.preyList.Values);
                        preyList.Sort();
                        string[] prelist = preyList.ToArray();
                        //Debug.Log("Prey: " + prelist[0]);

                        if (preyList.Contains(currentPlayer.targetCard.name))
                        {
                            currentPlayer.clickedCard.hp += 3;
                        }


                        //if player species is prey of opponent species
                        if (pdlist.Contains(currentPlayer.targetCard.name))
                        {
                            currentPlayer.clickedCard.hp -= 3;
                        }

                    }

                    currentPlayer.clickedCard.attack (currentPlayer.clickedCard, currentPlayer.targetCard, attackback);
					
					currentPlayer.getProtocolManager ().sendCardAttack (currentPlayer.playerID, currentPlayer.clickedCard.fieldIndex, currentPlayer.targetCard.fieldIndex);
					currentPlayer.clickedCard = null;
					currentPlayer.targetCard = null;
				}
				
			}
			
			if (currentPlayer.clickedCard != null && currentPlayer == player && card != currentPlayer.clickedCard) {
				currentPlayer.clickedCard = null;	
               
			}
			
		}
		
		public override void clicked ()
		{
			
			affect ();
		}
	}
}

