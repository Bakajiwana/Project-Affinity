using UnityEngine;
using System.Collections;

//Script Objective: Holds all the functions of the Menu Buttons, this script will be attached to the menu manager

public class MainMenuButton : MonoBehaviour 
{
	public Animator mainMenuAnim;
	public Animator menuSwitchAnim;
	public Animator optionsSwitchAnim;
	public float loadLevelTimer = 0.5f;

	void Start()
	{
		//Make sure main menu isn't paused
		Time.timeScale = 1f;
	}

	void Update()
	{
		//If Player hits escape or the exit button then move back a menu
		if(Input.GetButtonDown ("Cancel") && menuSwitchAnim.GetInteger ("Menu Number") > 0)
		{
			//If leaving the Options Menu then Save PlayerPrefs
			if(menuSwitchAnim.GetInteger ("Menu Number") == 2)
			{
				SavePlayerPrefs();
			}

			//Move back by one menu
			menuSwitchAnim.SetInteger ("Menu Number", 0);
		}
	}

	//Continue Game
	public void Continue()
	{
		mainMenuAnim.SetTrigger ("Fade Out");
		Invoke ("LoadLevel", loadLevelTimer);
	}

	//New Game
	public void NewGame()
	{
		mainMenuAnim.SetTrigger ("Fade Out");
		Invoke ("LoadLevel", loadLevelTimer);
	}

	//Load Level
	void LoadLevel()
	{
		Application.LoadLevel (3);
	}

	//Switch Menu
	public void SwitchMenu(int _menuNumber)
	{
		menuSwitchAnim.SetInteger ("Menu Number", _menuNumber);
	}

	//Switch Options Menu
	public void OptionsMenuSwitch(int _optionNumber)
	{
		optionsSwitchAnim.SetInteger ("Options Number", _optionNumber);
	}

	//Quit Button
	public void Quit()
	{
		Application.Quit ();
	}

	public void SavePlayerPrefs()
	{
		PlayerPrefs.Save ();
	}
}
