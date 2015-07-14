using UnityEngine;
using System.Collections;

//Script Objective: Activated to travel to any point to any specified scene

public class FastTravel : MonoBehaviour 
{

	private string destination;
	
	private string levelName;

	void Start()
	{
		DontDestroyOnLoad (gameObject);
	}
	
	public void SetDestination(string _levelName, string _destination)
	{
		levelName = _levelName;
		destination = _destination;
	}
	
	
	void OnLevelWasLoaded(int level)
	{
		//If the loading screen has loaded then activate async loading 
		if(level == 2)
		{
			DontDestroyOnLoad (gameObject); 
			GameObject loadManager = GameObject.Find ("loadManager01");
			LoadingScreenTravel loadingScreenTravel = loadManager.gameObject.GetComponent<LoadingScreenTravel>();
			loadingScreenTravel.LoadTargetLevel (levelName);
		}
		else
		{
			//send save file loading information
			GameObject destinationPoint = GameObject.Find (destination);
			SaveLoadManager.savePosition = destinationPoint.transform.position;
			GameObject.FindGameObjectWithTag ("Adventure Manager").SendMessage ("Autosave");
			print ("It worked");
			Destroy (gameObject);
		}
	}
}
