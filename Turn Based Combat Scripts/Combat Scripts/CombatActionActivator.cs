using UnityEngine;
using System.Collections;

//This script will recieve orders from the manager to activate action when its the characters turn

public class CombatActionActivator : MonoBehaviour 
{
	public bool isPlayer;

	private PlayerCombatActions playerAction;
	private EnemyCombatActions enemyAction;

	// Use this for initialization
	void Start () 
	{
		if(isPlayer)
		{
			playerAction = gameObject.GetComponent<PlayerCombatActions>();
		}
		else
		{
			enemyAction = gameObject.GetComponent<EnemyCombatActions>();
		}	
	}


	public void Activate()
	{
		Invoke ("EnableTurn", 0.01f);
	}

	//This function is called to prevent that glitch that freezes the battle
	// This function is delayed when the player or enemy is activated because if it is the 
	// players turn again, then the battle will freeze because at the same frame the turn is ended the turn is 
	// activated, causing the battle to stop.
	void EnableTurn()
	{
		if(isPlayer)
		{
			playerAction.enabled = true;
		}
		else
		{
			enemyAction.enabled = true;
		}
	}
}
