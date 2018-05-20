using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public static int Money;
	public int startMoney = 7;

	public static int Lives;

	public static int Rounds;

	void Start ()
	{
		Money = startMoney;
		Lives = TreeOfLifeBehavior.treeHealth;

		Rounds = 0;
	}

}
