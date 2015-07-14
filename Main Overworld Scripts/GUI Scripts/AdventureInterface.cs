using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Script Objective Control the Player User Interface

public class AdventureInterface : MonoBehaviour 
{
	public static bool showSaveIcon = false;
	public static string helpText = "";

	public Transform saveIcon;
	public Text helpDescription;

	// Use this for initialization
	void Start () 
	{
		//Start coroutine for performance
		StartCoroutine (PerformanceUpdate ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If the show save icon is true, enable the icon and disable it within a time limit
		if(showSaveIcon)
		{
			if(saveIcon)
			{
				saveIcon.gameObject.SetActive (true);
				Invoke ("HideSaveIcon", 2f);
			}
			showSaveIcon = false;
		}
	}

	//This function is called when save icon is true
	void HideSaveIcon()
	{
		saveIcon.gameObject.SetActive (false);
	}

	//This coroutine is just used to save performance instead of every frame it is every second
	IEnumerator PerformanceUpdate()
	{
		while(true) //loop forever
		{
			yield return new WaitForSeconds(1f);

			//Objectives and functions here

			//Display the help text
			if(helpDescription)
			{
				helpDescription.text = helpText;
			}
		}
	}
}
