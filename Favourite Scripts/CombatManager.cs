using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//This script will manage the overall combat

//Manager tells character whether its their turn.

public class CombatManager : MonoBehaviour 
{
	//Start Battle Timer
	public float battleStartTime = 3f;

	//Player and Enemy Side Nodes
	public GameObject playerSide;
	public GameObject enemySide;
	public GameObject environmentInteractionSide; //Environment Interaction objects zone

	//Reference to players and enemies, using lists because it will be dynamic, so adds and removals etc
	public static List<GameObject> players = new List<GameObject>();
	public static List<GameObject> enemies = new List<GameObject>();

	//Reference Environment Interaction
	public static List<GameObject> environmentObjects = new List<GameObject>();

	//The stats of the players and enemies
	public static List<PlayerCombatCharacter> playerStats = new List<PlayerCombatCharacter>();
	public static List<EnemyCombatCharacter> enemyStats = new List<EnemyCombatCharacter>();

	//Accumulative Speeds - Determines the Time of Action
	private List<int> accPlayerSpeeds = new List<int>();
	private List<int> accEnemySpeeds = new List<int>();

	//Combat state 0=The combat is still going, 1=The player has won, 2=the player has lost
	public static int combatState = 0;

	//Applies to to all timescale lines except for pausing
	public static float currentTimeScale = 1f; 

	//Special Paradox - Life and Death Elemental Paradox
	public static bool specialParadox = false;
	private bool performingSpecial = false;

	// Use this for initialization
	void Start () 
	{
		//Acquire the players and enemies, then acquire their speeds and begin Time of Action.
		//Using Invoke because there should be a little break when battle begins to set up stats and everything
		Invoke ("InitialiseBattle", battleStartTime);
	}

	//This enumerator checks if the players or enemies have been defeated.
	IEnumerator CheckCombatState()
	{
		while(combatState == 0)
		{
			if(enemies.Count == 0)
			{
				combatState = 1;
				EndBattle ();
			}			
			else if(players.Count == 0)
			{
				combatState = 2;
				EndBattle ();
			}
			else
			{
				combatState = 0;
			}
			yield return new WaitForSeconds(1f);
		}
	}

	//This function is used to acquire players and enemies into their respective lists
	void InitialiseBattle()
	{
		//Obtain player and enemy objects - Fill the players and enemies lists
		foreach (Transform child in playerSide.transform)
		{
			players.Add (child.gameObject);
		}

		foreach (Transform child in enemySide.transform)
		{
			enemies.Add (child.gameObject);
		}

		//Obtain Environment objects - Fill EnvironmentObjects list
		foreach (Transform child in environmentInteractionSide.transform)
		{
			environmentObjects.Add (child.gameObject);
		}

		//Obtain the player and enemy stats - Fill the player and enemy stats lists
		for(int i = 0; i < players.Count; i++)
		{
			playerStats.Add (players[i].GetComponent<PlayerCombatCharacter>());	//Obtain the stat components
		}

		for(int i = 0; i < enemies.Count; i++)
		{
			enemyStats.Add (enemies[i].GetComponent<EnemyCombatCharacter>()); 	//Obtain the stat components
		}

		//Initialise the time of action - Fill the accumulative speed lists
		for(int i = 0; i < players.Count; i++)
		{
			accPlayerSpeeds.Add (0);	//Everyone starts at 0, over turns they accumulate
		}

		for(int i = 0; i < enemies.Count; i++)
		{
			accEnemySpeeds.Add (0);
		}

		//Initialise UI Panel
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("InitialisePartyPanel", SendMessageOptions.DontRequireReceiver);

		NextTurn ();		//Determine the next characters turn
		StartCoroutine (CheckCombatState ());	//Activate the Enumerator that checks who will win
	}

	//This function is to add the speeds and update the time of action
	void UpdateTimeOfAction()
	{		
		for (int i = 0; i < players.Count; i++)
		{
			accPlayerSpeeds[i] += playerStats[i].stat.speed;
			//print ("Player" + i + " = " + accPlayerSpeeds[i]);
		}
		
		for (int i = 0; i < enemies.Count; i++)
		{
			accEnemySpeeds[i] += enemyStats[i].stat.speed;
			//print ("Enemy" + i + " = " + accEnemySpeeds[i]);
		}
	}

	//This function will update the time of action
	void NextTurn()
	{
		//If not performing a special then continue to next turn
		if(!performingSpecial)
		{
			if(players.Count > 0 && enemies.Count > 0)
			{
				//Update the Time of Action 
				UpdateTimeOfAction ();

				//Calculate the accumulated values and determine the highest turn
				int _nextPlayerSpeed = accPlayerSpeeds[0];
				int _nextPlayer = 0;
				for (int i = 0; i < accPlayerSpeeds.Count; i++)
				{
					if(accPlayerSpeeds[i] > _nextPlayerSpeed)
					{
						_nextPlayerSpeed = accPlayerSpeeds[i];
						_nextPlayer = i;
					}
				}

				int _nextEnemySpeed = accEnemySpeeds[0];
				int _nextEnemy = 0;
				for (int i = 0; i < accEnemySpeeds.Count; i++)
				{
					if(accEnemySpeeds[i] > _nextEnemySpeed)
					{
						_nextEnemySpeed = accEnemySpeeds[i];
						_nextEnemy = i;
					}
				}

				//Then compare the players highest value and the enemies highest value 
				//print (_nextPlayer + " " + _nextPlayerSpeed);
				//print (_nextEnemy + " " + _nextEnemySpeed);
				if(_nextPlayerSpeed >= _nextEnemySpeed)
				{
					//Activate Specified Player
					//print ("It is Player " + _nextPlayer + " Turn, with a speed of " + _nextPlayerSpeed);
					players[_nextPlayer].SendMessage ("Activate", SendMessageOptions.DontRequireReceiver);

					//Reset that specific player's accumulated speed
					accPlayerSpeeds[_nextPlayer] = 0; 
				}
				else
				{
					//Activate Specified Enemy
					//print ("It is Enemy " + _nextEnemy + " Turn, with a speed of " + _nextEnemySpeed);
					enemies[_nextEnemy].SendMessage ("Activate", SendMessageOptions.DontRequireReceiver);

					//Reset that specific Enemy's accumulated speed
					accEnemySpeeds[_nextEnemy] = 0;
				}
			}
		}
		else
		{
			//If performing special delay turn and show special screen
			GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("ActivateSpecialParadoxScreen", SendMessageOptions.DontRequireReceiver);
		}
	}

	//These functions are called to remove and insert players and enemies to the list this manager depends on
	public void RemovePlayer(int _index)
	{
		players.RemoveAt (_index);
		playerStats.RemoveAt (_index);
		accPlayerSpeeds.RemoveAt (_index);
	}

	public void InsertPlayer(int _index)
	{
		int count = 0;

		foreach (Transform child in playerSide.transform)
		{
			if(count == _index)
			{
				players.Insert (_index, child.gameObject);
				break;
			}
			count++;
		}

		playerStats.Insert (_index, players[_index].GetComponent<PlayerCombatCharacter>());
		accPlayerSpeeds.Insert (_index, 0);
	}

	public void RemoveEnemy(int _index)
	{
		enemies.RemoveAt (_index);
		enemyStats.RemoveAt (_index);
		accEnemySpeeds.RemoveAt (_index);
	}

	public void InsertEnemy(int _index)
	{
		int count = 0;
		
		foreach (Transform child in enemySide.transform)
		{
			if(count == _index)
			{
				enemies.Insert (_index, child.gameObject);
				break;
			}
			count++;
		}

		enemyStats.Insert (_index, enemies[_index].GetComponent<EnemyCombatCharacter>());
		accEnemySpeeds.Insert (_index, 0);
	}

	//This function removes environment interactibles
	public void RemoveEnvironmentObject(int _index)
	{
		environmentObjects.RemoveAt (_index);
	}

	//This function is called when special paradox is ready
	public void SpecialParadoxActivate()
	{
		print ("activate paradox");
		performingSpecial = true;
	}

	public void LifeParadox()
	{
		//All players are healed and resurrected and all buffs applied
		for(int i = 0; i < playerStats.Count; i++)
		{
			//Heal
			playerStats[i].SetHeal (true, 0.5f);

			//Apply Buffs
			playerStats[i].SetPurify (3);
			playerStats[i].SetAura (3);
			playerStats[i].SetBlazingSpirit (3);
			playerStats[i].SetOvercharged (3);
		}

		specialParadox = true;
		performingSpecial = false;

		NextTurn ();
	}

	public void DeathParadox()
	{
		//All enemies are damaged and all debuffs applied
		for(int i = 0; i < enemyStats.Count; i++)
		{
			//Damage
			int damage = (int)((float)enemyStats[i].stat.healthBase * 0.5f);
			enemyStats[i].stat.health -= damage;
			enemyStats[i].ShowDamageText (damage.ToString (), Color.black, 1f);

			//Set all Debuffs
			enemyStats[i].SetCondemned (3);
			enemyStats[i].SetBurning (3);
			enemyStats[i].SetRusted (3);
			enemyStats[i].SetStunned (3);
			enemyStats[i].elementalReaction.SetJammed ();
			enemyStats[i].elementalReaction.SetBattered ();
			enemyStats[i].elementalReaction.SetMagnetised ();
			enemyStats[i].elementalReaction.SetMolten ();
			enemyStats[i].elementalReaction.SetPowerSurge ();
			enemyStats[i].elementalReaction.SetSteamCloud ();
		}

		specialParadox = true;
		performingSpecial = false;

		NextTurn ();
	}

	//These functions are called if the player or enemy has won
	void EndBattle()
	{
		//1 = win, 2 = lost
		if(combatState == 1)
		{
			GameObject.FindGameObjectWithTag("Combat UI").SendMessage("SwitchToWinScreen", SendMessageOptions.DontRequireReceiver);
		}
		else if (combatState == 2)
		{
			GameObject.FindGameObjectWithTag("Combat UI").SendMessage("SwitchToLoseScreen", SendMessageOptions.DontRequireReceiver);
		}
	}
}
