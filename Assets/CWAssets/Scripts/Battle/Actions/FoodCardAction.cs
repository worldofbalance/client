using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CW{
	public class FoodCardAction : TurnAction {
		
		private int attackersPosition, attackedPosition;
		
		public FoodCardAction(int intCount, int stringCount, List<int> intList, List<string> stringList):
		base(intCount, stringCount, intList, stringList){ }
		
		
		override public void readData(){
			attackersPosition = intList[0];
			attackedPosition =  intList[1];
		}
		
		override public void execute(){
			readData ();

            GameObject obj = (GameObject)GameManager.player2.cardsInPlay[attackersPosition];


            //GameObject obj = (GameObject)GameManager.player2.cardsInPlay[attackedPosition];
            AbstractCard target = obj.GetComponent<AbstractCard> ();


            //When Player2(Client 2) used food card, this will apply to Player 1 (Client 1) but not Client 2
            //GameManager.player2.applyFoodBuff(target, 3, 3);
            //Ex p2(client) used food card on mouse 1,1 -> 2,2, p1(client) show 1,1 -> 4,4

            GameManager.player2.applyFoodBuff(target, 1, 1);

			GameObject cardUsed = (GameObject)GameManager.player2.hand [0];
            

			GameManager.player2.hand.Remove(cardUsed);
			GameObject.Destroy (cardUsed);

			// initiate attack
			
		}
	}
}