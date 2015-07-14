using UnityEngine;
using System.Collections;

//Script Objective: When load button is clicked this object will move on to the next level without being destroyed
//and initialise the next level

public class LoadGameTravel : MonoBehaviour 
{
	private int saveFileNumber;

	private string saveLevelName;

	public bool quickload;

	public bool autoload;

	public void SetSaveNumber(int _saveFileNumber, string _saveLevelName)
	{
		saveFileNumber = _saveFileNumber;
		saveLevelName = _saveLevelName;
	}
	

	void OnLevelWasLoaded(int level)
	{
		//If the loading screen has loaded then activate async loading 
		if(level == 2)
		{
			DontDestroyOnLoad (gameObject); 
			GameObject loadManager = GameObject.Find ("loadManager01");
			LoadingScreenTravel loadingScreenTravel = loadManager.gameObject.GetComponent<LoadingScreenTravel>();
			loadingScreenTravel.LoadTargetLevel (saveLevelName);
		}
		else
		{
			//send save file loading information
			if(quickload)
			{
				GameObject.FindGameObjectWithTag ("Adventure Manager").SendMessage ("Quickload");
			}
			else if(autoload)
			{
				GameObject.FindGameObjectWithTag ("Adventure Manager").SendMessage ("Autoload");
			}
			else
			{
				GameObject.FindGameObjectWithTag ("Adventure Manager").SendMessage ("Load", saveFileNumber);
			}
		//	print ("It worked");
			Destroy (gameObject);
		}
	}
}
