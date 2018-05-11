using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CW
{
	public class InPlay : AbstractCardHandler
	{
        public bool foodactive = false;
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

                        /*
                        //2018 Spring semester WoB Ecosystem
                        //Lion (86) > Bufflao, Bush Pig
                        //Buffalo (7) > Grass and Herbs
                        //Bush Pig (83) > Decayling Material, Tree Mouse
                        //Tree Mouse (31) > Grass and Herbs, Decaying Materials, Cockroach
                        //Cockroach (19) > Decaying Materials
                        //Decaying Materials (89) 
                        //Grass and Herbs (96)
                        */

                        //Decaying Materials < Bush Pig
                        if (currentPlayer.clickedCard.cardID == 89 && currentPlayer.targetCard.cardID == 83)
                        {
                            Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                            currentPlayer.applyFoodBuff(currentPlayer.targetCard, 3, 3);
                        //Decaying Materials < Tree Mouse
                        }
                        else if (currentPlayer.clickedCard.cardID == 89 && currentPlayer.targetCard.cardID == 31)
                        {
                            Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                            currentPlayer.applyFoodBuff(currentPlayer.targetCard, 3, 3);
                        //Decaying Materials < Cockroach
                        }
                        else if (currentPlayer.clickedCard.cardID == 89 && currentPlayer.targetCard.cardID == 19)
                        {
                            Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                            currentPlayer.applyFoodBuff(currentPlayer.targetCard, 3, 3);
                        //Grass and Herbs < Tree Mouse
                        }
                        else if (currentPlayer.clickedCard.cardID == 96 && currentPlayer.targetCard.cardID == 31)
                        {
                            Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                            currentPlayer.applyFoodBuff(currentPlayer.targetCard, 3, 3);
                        //Grass and Herbs < Buffalo
                        }
                        else if (currentPlayer.clickedCard.cardID == 96 && currentPlayer.targetCard.cardID == 7)
                        {
                            Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                            currentPlayer.applyFoodBuff(currentPlayer.targetCard, 3, 3);
                        //Normal Food Apply
                        }
                        else
                        {
                            Debug.Log("Normal Food Apply");
                            currentPlayer.applyFoodBuff(currentPlayer.targetCard, 1, 1);
                        }

                        //currentPlayer.clickedCard.applyFood(currentPlayer.targetCard, 1, 1);
                        //currentPlayer.getProtocolManager ().sendFoodBuff (currentPlayer.cardID, currentPlayer.targetCard.fieldIndex);
                        currentPlayer.getProtocolManager ().sendFoodBuff (currentPlayer.playerID, currentPlayer.clickedCard.cardID, currentPlayer.targetCard.fieldIndex);
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

                    /*
                    //2018 Spring semester WoB Ecosystem
                    //Lion (86) > Bufflao, Bush Pig
                    //Buffalo (7) > Grass and Herbs
                    //Bush Pig (83) > Decayling Material, Tree Mouse
                    //Tree Mouse (31) > Grass and Herbs, Decaying Materials, Cockroach
                    //Cockroach (19) > Decaying Materials
                    //Decaying Materials (89) 
                    //Grass and Herbs (96)
                    */

                    
                    //Predator > Prey
                    //Lion > Bufflao
                    if(currentPlayer.clickedCard.cardID == 86 && currentPlayer.targetCard.cardID == 7)
                    {
                        Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                        Debug.Log("Species: " + currentPlayer.clickedCard.name + " > " + currentPlayer.targetCard.name);
                        Debug.Log("Before Buffed");
                        Debug.Log("Old Predator (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("Old Prey (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        currentPlayer.clickedCard.dmg += 3;
                        currentPlayer.clickedCard.hp += 5;
                        Debug.Log("After Buffed");
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        Debug.Log("After Attack");
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") hp : " + "Predator hp (" + currentPlayer.clickedCard.hp + ") - Prey dmg (" + currentPlayer.targetCard.dmg + ") = "
                            + (currentPlayer.clickedCard.hp - currentPlayer.targetCard.dmg));
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") dmg : " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") hp : " + "prey hp (" + currentPlayer.targetCard.hp + ") - Predator dmg (" + currentPlayer.clickedCard.dmg + ") = "
                            + (currentPlayer.targetCard.hp - currentPlayer.clickedCard.dmg));
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") dmg : " + currentPlayer.targetCard.dmg);

                        //Lion > Bush Pig
                    } else if (currentPlayer.clickedCard.cardID == 86 && currentPlayer.targetCard.cardID == 83)
                    {
                        Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                        Debug.Log("Species: " + currentPlayer.clickedCard.name + " > " + currentPlayer.targetCard.name);
                        Debug.Log("Before Buffed");
                        Debug.Log("Old Predator (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("Old Prey (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        currentPlayer.clickedCard.dmg += 3;
                        currentPlayer.clickedCard.hp += 5;
                        Debug.Log("After Buffed");
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        Debug.Log("After Attack");
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") hp : " + "Predator hp (" + currentPlayer.clickedCard.hp + ") - Prey dmg (" + currentPlayer.targetCard.dmg + ") = "
                            + (currentPlayer.clickedCard.hp - currentPlayer.targetCard.dmg));
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") dmg : " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") hp : " + "prey hp (" + currentPlayer.targetCard.hp + ") - Predator dmg (" + currentPlayer.clickedCard.dmg + ") = "
                            + (currentPlayer.targetCard.hp - currentPlayer.clickedCard.dmg));
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") dmg : " + currentPlayer.targetCard.dmg);

                        //Bush Pig > Tree Mouse
                    } else if (currentPlayer.clickedCard.cardID == 83 && currentPlayer.targetCard.cardID == 31)
                    {
                        Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                        Debug.Log("Species: " + currentPlayer.clickedCard.name + " > " + currentPlayer.targetCard.name);
                        Debug.Log("Before Buffed");
                        Debug.Log("Old Predator (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("Old Prey (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        currentPlayer.clickedCard.dmg += 3;
                        currentPlayer.clickedCard.hp += 5;
                        Debug.Log("After Buffed");
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        Debug.Log("After Attack");
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") hp : " + "Predator hp (" + currentPlayer.clickedCard.hp + ") - Prey dmg (" + currentPlayer.targetCard.dmg + ") = "
                            + (currentPlayer.clickedCard.hp - currentPlayer.targetCard.dmg));
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") dmg : " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") hp : " + "prey hp (" + currentPlayer.targetCard.hp + ") - Predator dmg (" + currentPlayer.clickedCard.dmg + ") = "
                            + (currentPlayer.targetCard.hp - currentPlayer.clickedCard.dmg));
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") dmg : " + currentPlayer.targetCard.dmg);

                        //Tree Mouse > Cockroach
                    } else if (currentPlayer.clickedCard.cardID == 31 && currentPlayer.targetCard.cardID == 19)
                    {
                        Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                        Debug.Log("Species: " + currentPlayer.clickedCard.name + " > " + currentPlayer.targetCard.name);
                        Debug.Log("Before Buffed");
                        Debug.Log("Old Predator (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("Old Prey (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        currentPlayer.clickedCard.dmg += 3;
                        currentPlayer.clickedCard.hp += 5;
                        Debug.Log("After Buffed");
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        Debug.Log("After Attack");
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") hp : " + "Predator hp (" + currentPlayer.clickedCard.hp + ") - Prey dmg (" + currentPlayer.targetCard.dmg + ") = "
                            + (currentPlayer.clickedCard.hp - currentPlayer.targetCard.dmg));
                        Debug.Log("New Predator (" + currentPlayer.clickedCard.name + ") dmg : " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") hp : " + "prey hp (" + currentPlayer.targetCard.hp + ") - Predator dmg (" + currentPlayer.clickedCard.dmg + ") = "
                            + (currentPlayer.targetCard.hp - currentPlayer.clickedCard.dmg));
                        Debug.Log("New Prey (" + currentPlayer.targetCard.name + ") dmg : " + currentPlayer.targetCard.dmg);
                    }

                    //Prey < Predator
                    //Bufflao < Lion
                    if (currentPlayer.clickedCard.cardID == 7 && currentPlayer.targetCard.cardID == 86)
                    {
                        Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                        Debug.Log("Species: " + currentPlayer.clickedCard.name + " < " + currentPlayer.targetCard.name);
                        Debug.Log("Before Buffed");
                        Debug.Log("Old Prey (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("Old Predator (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        currentPlayer.clickedCard.dmg -= 3;
                        currentPlayer.clickedCard.hp -= 5;
                        Debug.Log("After Buffed");
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        Debug.Log("After Attack");
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") hp : " + "prey hp (" + currentPlayer.clickedCard.hp + ") - Predator dmg (" + currentPlayer.targetCard.dmg + ") = "
                            + (currentPlayer.clickedCard.hp - currentPlayer.targetCard.dmg));
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") dmg : " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") hp : " + "Predator hp (" + currentPlayer.targetCard.hp + ") - Prey dmg (" + currentPlayer.clickedCard.dmg + ") = "
                            + (currentPlayer.targetCard.hp - currentPlayer.clickedCard.dmg));
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") dmg : " + currentPlayer.targetCard.dmg);

                    //Bush Pig < Lion
                    }
                    else if (currentPlayer.clickedCard.cardID == 83 && currentPlayer.targetCard.cardID == 86)
                    {
                        Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                        Debug.Log("Species: " + currentPlayer.clickedCard.name + " < " + currentPlayer.targetCard.name);
                        Debug.Log("Before Buffed");
                        Debug.Log("Old Prey (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("Old Predator (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        currentPlayer.clickedCard.dmg -= 3;
                        currentPlayer.clickedCard.hp -= 5;
                        Debug.Log("After Buffed");
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        Debug.Log("After Attack");
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") hp : " + "prey hp (" + currentPlayer.clickedCard.hp + ") - Predator dmg (" + currentPlayer.targetCard.dmg + ") = "
                            + (currentPlayer.clickedCard.hp - currentPlayer.targetCard.dmg));
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") dmg : " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") hp : " + "Predator hp (" + currentPlayer.targetCard.hp + ") - Prey dmg (" + currentPlayer.clickedCard.dmg + ") = "
                            + (currentPlayer.targetCard.hp - currentPlayer.clickedCard.dmg));
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") dmg : " + currentPlayer.targetCard.dmg);

                    //Tree Mouse < Bush Pig
                    }
                    else if (currentPlayer.clickedCard.cardID == 31 && currentPlayer.targetCard.cardID == 83)
                    {
                        Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                        Debug.Log("Species: " + currentPlayer.clickedCard.name + " < " + currentPlayer.targetCard.name);
                        Debug.Log("Before Buffed");
                        Debug.Log("Old Prey (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("Old Predator (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        currentPlayer.clickedCard.dmg -= 3;
                        currentPlayer.clickedCard.hp -= 5;
                        Debug.Log("After Buffed");
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        Debug.Log("After Attack");
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") hp : " + "prey hp (" + currentPlayer.clickedCard.hp + ") - Predator dmg (" + currentPlayer.targetCard.dmg + ") = "
                            + (currentPlayer.clickedCard.hp - currentPlayer.targetCard.dmg));
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") dmg : " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") hp : " + "Predator hp (" + currentPlayer.targetCard.hp + ") - Prey dmg (" + currentPlayer.clickedCard.dmg + ") = "
                            + (currentPlayer.targetCard.hp - currentPlayer.clickedCard.dmg));
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") dmg : " + currentPlayer.targetCard.dmg);

                    //Cockroach < Tree Mouse 
                    }
                    else if (currentPlayer.clickedCard.cardID == 19 && currentPlayer.targetCard.cardID == 31)
                    {
                        Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                        Debug.Log("Species: " + currentPlayer.clickedCard.name + " < " + currentPlayer.targetCard.name);
                        Debug.Log("Before Buffed");
                        Debug.Log("Old Prey (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("Old Predator (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        currentPlayer.clickedCard.dmg -= 3;
                        currentPlayer.clickedCard.hp -= 5;
                        Debug.Log("After Buffed");
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") - hp: " + currentPlayer.clickedCard.hp + " dmg: " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") - hp: " + currentPlayer.targetCard.hp + " dmg: " + currentPlayer.targetCard.dmg);
                        Debug.Log("After Attack");
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") hp : " + "prey hp (" + currentPlayer.clickedCard.hp + ") - Predator dmg (" + currentPlayer.targetCard.dmg + ") = "
                            + (currentPlayer.clickedCard.hp - currentPlayer.targetCard.dmg));
                        Debug.Log("New Prey (" + currentPlayer.clickedCard.name + ") dmg : " + currentPlayer.clickedCard.dmg);
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") hp : " + "Predator hp (" + currentPlayer.targetCard.hp + ") - Prey dmg (" + currentPlayer.clickedCard.dmg + ") = "
                            + (currentPlayer.targetCard.hp - currentPlayer.clickedCard.dmg));
                        Debug.Log("New Predator (" + currentPlayer.targetCard.name + ") dmg : " + currentPlayer.targetCard.dmg);
                    }
                    

                    /*
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
                    */

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

