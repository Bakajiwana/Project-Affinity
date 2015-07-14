using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatTest : MonoBehaviour {

	public CombatSpawner spawner;

	public GameObject [] playerCharacters;
	public List<Character> characterStats = new List<Character>();

	public GameObject [] enemyCharacters;
	public int [] enemyLevels;

	// Use this for initialization
	void Start () {
		characterStats.Add(new Character("Einar", 8, 2, 50, 200, 0, 7, 0, 7, 0, 7, 0, 7, 0));
		characterStats.Add (new Character("Smith",8, 2, 50, 200, 0, 7, 0, 7, 0, 7, 0, 7, 0));

		spawner.AddPlayers(playerCharacters, characterStats);
		spawner.AddEnemies(enemyCharacters, enemyLevels);

		spawner.SpawnPlayers();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
