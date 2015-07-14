using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This script will handle the spawning of the characters when combat begins.
//Also send stat information from previous scene

public class CombatSpawner : MonoBehaviour 
{
	//The Characters to spawn
	private GameObject [] playerCharacters;
	private GameObject [] enemyCharacters;

	//Stats of the characters
	private int[] enemyLevel;
	public List<Character> character = new List<Character>();
	private int[] playerHealths;

	//Reference the spawned characters
	private GameObject[] player;
	private GameObject[] enemy;

	// Use this for initialization
	void Start () 
	{
		//SpawnPlayers ();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnLevelWasLoaded(int level)
	{
		//print ("Hello I'm ready to spawn things");
		SpawnPlayers();
	}

	//This function is to spawn the characters into the battlefield
	public void SpawnPlayers()
	{
		//Find and reference the player and enemy side
		GameObject playerSide = GameObject.Find ("Player Combat Side");
		GameObject enemySide = GameObject.Find ("Enemy Combat Side"); 

		//Place every position node into an array for players to spawn
		GameObject playerPositions = GameObject.Find ("Player Spawn Positions");
		GameObject enemyPositions = GameObject.Find ("Enemy Spawn Positions"); 

		player = new GameObject[playerCharacters.Length];
		enemy = new GameObject[enemyCharacters.Length];


		//Firstly spawn all the players into the battlefield using thier array and positon arrays
		for(int i = 0; i < playerCharacters.Length; i++)
		{
			player[i] = Instantiate (playerCharacters[i], playerPositions.transform.GetChild(i).position, playerPositions.transform.GetChild (i).rotation) as GameObject;
			player[i].transform.SetParent (playerSide.transform, true);

			//print ("Character "+i+" is "+ character[i].name);

			//print (character[i]);

			//Send the character list to the player character
			player[i].SendMessage ("InitiatePlayerStats", character[i], SendMessageOptions.DontRequireReceiver);
		}

		//Secondly Spawn all enemies into the battlefield
		for (int i = 0; i < enemyCharacters.Length; i++)
		{
			enemy[i] = Instantiate (enemyCharacters[i], enemyPositions.transform.GetChild (i).position, enemyPositions.transform.GetChild(i).rotation) as GameObject;
			enemy[i].transform.SetParent (enemySide.transform, true);

			//print ("Enemy "+ i+ " is level " + enemyLevel[i]);

			//Send the enemy its specified level
			enemy[i].SendMessage ("InitiateEnemyStats", enemyLevel[i], SendMessageOptions.DontRequireReceiver);
		}
	}

	//This function is for the player to communicate to this spawner its array that will spawn in the battlefield
	public void AddPlayers(GameObject[] players, List<Character> characterStat)
	{
		playerCharacters = players;
		character = characterStat;
	}

	//This function is for the enemy to communicate to this spawner its array that will spawn in the battlefield
	public void AddEnemies (GameObject[] enemies, int[] levels)
	{
		enemyCharacters = enemies;
		enemyLevel = levels;
	}
}
