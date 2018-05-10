using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CW{
	public class FoodCardAction : TurnAction {
		
		private int attackersPosition, attackedPosition;
		
		public FoodCardAction(int intCount, int stringCount, List<int> intList, List<string> stringList):
		base(intCount, stringCount, intList, stringList){ }
		
		
		override public void readData(){
			attackersPosition = intList[0]; //clickedcard (food)
			attackedPosition =  intList[1]; //targetcard (species)
		}
		
		override public void execute(){
			readData ();

            GameObject obj = (GameObject)GameManager.player2.cardsInPlay[attackedPosition];

            //GameObject obj = (GameObject)GameManager.player2.cardsInPlay[attackedPosition];
            AbstractCard target = obj.GetComponent<AbstractCard> ();


            //When Player2(Client 2) used food card, this will apply to Player 1 (Client 1) but not Client 2
            //GameManager.player2.applyFoodBuff(target, 3, 3);
            //Ex p2(client) used food card on mouse 1,1 -> 2,2, p1(client) show 1,1 -> 4,4

            int food = attackersPosition;
            int species = target.cardID;
            Debug.Log("food: " + food);
            Debug.Log("species: " + species);

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
            if (food == 89 && species == 83)
            {
                Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                GameManager.player2.applyFoodBuff(target, 3, 3);
                //Decaying Materials < Tree Mouse
            }
            else if (food == 89 && species == 31)
            {
                Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                GameManager.player2.applyFoodBuff(target, 3, 3);
                //Decaying Materials < Cockroach
            }
            else if (food == 89 && species == 19)
            {
                Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                GameManager.player2.applyFoodBuff(target, 3, 3);
                //Grass and Herbs < Tree Mouse
            }
            else if (food == 96 && species == 31)
            {
                Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                GameManager.player2.applyFoodBuff(target, 3, 3);
                //Grass and Herbs < Buffalo
            }
            else if (food == 96 && species == 7)
            {
                Debug.Log("FOOD WEB SYSTEM ACTIVATED");
                GameManager.player2.applyFoodBuff(target, 3, 3);
                //Normal Food Apply
            }
            else
            {
                Debug.Log("Normal Food Apply");
                GameManager.player2.applyFoodBuff(target, 1, 1);
            }

			GameObject cardUsed = (GameObject)GameManager.player2.hand [0];
            

			GameManager.player2.hand.Remove(cardUsed);
			GameObject.Destroy (cardUsed);

			// initiate attack
			
		}
	}
}