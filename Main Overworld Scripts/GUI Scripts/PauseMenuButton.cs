using UnityEngine;
using System.Collections;

//Script Objective control pause button functions

public class PauseMenuButton : MonoBehaviour 
{
	public Animator pauseMenuSwitch;
	public Animator optionsSwitchAnim;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	//Switch Options Menu
	public void OptionsMenuSwitch(int _optionNumber)
	{
		optionsSwitchAnim.SetInteger ("Options Number", _optionNumber);
	}

	public void PauseMenuSwitch(int _menuNumber)
	{
		pauseMenuSwitch.SetInteger ("Menu Number", _menuNumber);
	}

	public void LoadLevel(string _level)
	{
		Application.LoadLevel (_level);
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
