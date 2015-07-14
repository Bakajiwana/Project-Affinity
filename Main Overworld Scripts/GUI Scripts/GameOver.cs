using UnityEngine;
using System.Collections;

//Script Objective: Brings up the game over screen when its game over.

public class GameOver : MonoBehaviour 
{
	//Create a public static variable for the game over so other game objects can call it
	public static bool isGameOver = false; 

	public Transform gameOverScreen;

	// Use this for initialization
	void Start () 
	{
		isGameOver = false;
		gameOverScreen.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If an object calls for game over to be true then its game over
		if(isGameOver)
		{
			Time.timeScale = 1f;	//Make sure game is not paused
			Pause.isPaused = false;
			gameOverScreen.gameObject.SetActive (true);
		}
	}
}
