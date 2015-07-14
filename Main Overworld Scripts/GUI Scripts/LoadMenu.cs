using UnityEngine;
using UnityEngine.UI;
using System.IO; 
using System.Collections;

//Script Objective Load Menu Management

public class LoadMenu : MonoBehaviour 
{
	private RectTransform rectTransform;
	private float top = 0.5f;
	private float bottom = 200f;
	
	public GameObject button;
	public GameObject autosaveButton;
	public GameObject quicksaveButton;

	public bool isSaveMenu = false;

	void Awake()
	{
		SetUpMenu ();
	}

	// Use this for initialization
	void Start () 
	{

	}
	
	public void SetUpMenu()
	{
		//Clean up menu and then recreate all the buttons again
		foreach (Transform child in transform)
		{
			GameObject.Destroy (child.gameObject);
		}

		//Update Menu Rect Transform
		top = 0.5f;
		bottom = 200f;

		//Count the amount of save files there are
		string _FileLocation= Application.dataPath; 
		DirectoryInfo  di = new DirectoryInfo (_FileLocation);		
		int numXML = di.GetFiles("*.xml", SearchOption.TopDirectoryOnly).Length;

		//Find autosave xml and quick save files and take that number away from numXML so it can instantiate correct number of save games
		int autosaveXML = di.GetFiles ("Autosave.xml", SearchOption.TopDirectoryOnly).Length;
		int quicksaveXML = di.GetFiles ("Quicksave.xml", SearchOption.TopDirectoryOnly).Length;

		SaveLoadManager[] saveLoadManager = new SaveLoadManager[numXML];
		long[] latestTime = new long[numXML];

		numXML = numXML - autosaveXML - quicksaveXML;

		GameObject[] loadButtons = new GameObject[numXML];

		//This is not a save menu then show Autosave and quick save buttons
		if(!isSaveMenu)
		{
			if(autosaveXML > 0)
			{
				GameObject autosave = Instantiate (autosaveButton, Vector3.zero, Quaternion.identity) as GameObject;
				SaveLoadManager autosaveManager = autosave.GetComponent<SaveLoadManager>();
				autosave.transform.SetParent (this.transform, false);
				autosaveManager.LoadButtonUpdate (0);	//int doesn't matter
				saveLoadManager[numXML + autosaveXML - 1] = autosave.gameObject.GetComponent<SaveLoadManager>();
			}
			if(quicksaveXML >0)
			{
				GameObject quicksave = Instantiate (quicksaveButton, Vector3.zero, Quaternion.identity) as GameObject;
				SaveLoadManager quicksaveManager = quicksave.GetComponent<SaveLoadManager>();
				quicksave.transform.SetParent (this.transform, false);
				quicksaveManager.LoadButtonUpdate (0);	//int doesn't matter
				saveLoadManager[numXML + autosaveXML + quicksaveXML - 1] = quicksave.gameObject.GetComponent<SaveLoadManager>();
			}
		}

		//If there is an xml file
		if(numXML > 0)
		{
			//Instantiate Buttons into arrays
			for (int i = 0; i < numXML; i++)
			{
				loadButtons[i] = Instantiate (button, Vector3.zero, Quaternion.identity) as GameObject;
				//Get Components
				saveLoadManager[i] = loadButtons[i].gameObject.GetComponent<SaveLoadManager>();
				
				//Parent the arrays into this object
				loadButtons[i].transform.SetParent(this.transform, false);
				//loadButtons[i].transform.parent = this.transform;
				
				//Send Save Numbers according to their array 
				saveLoadManager[i].LoadButtonUpdate (i + 1);
				
				bottom -= 100f;	//Make the window size according to the amound of buttons. -200 per button
			}

			int latest = 0;
			if(!isSaveMenu)
			{
				//Find the most recent save 
				for (int l = 0; l < saveLoadManager.Length; l++)
				{
					latestTime[l] = saveLoadManager[l].saveLatestTime;
					//print ("File "+l+"= "+latestTime[l]);
				}
			}
			else
			{
				//Find the most recent save 
				for (int l = 0; l < numXML; l++)
				{
					latestTime[l] = saveLoadManager[l].saveLatestTime;
					//print ("File "+l+"= "+latestTime[l]);
				}
			}

			//print (saveLoadManager.Length + " " + latestTime.Length);
			latest = (int)MaxValue(latestTime);
			saveLoadManager[latest].MarkLatest ();
			//print (latest);

			//Check for the latest save file whether quick save, auto save or normal and mark the latest
			if(!isSaveMenu)
			{
				if(latest == numXML + autosaveXML + quicksaveXML - 1)
				{
					SaveLoadManager.latestQuicksave = true;
					SaveLoadManager.latestAutosave = false;
				}
				else if (latest == numXML + autosaveXML - 1)
				{
					SaveLoadManager.latestAutosave = true;
					SaveLoadManager.latestQuicksave = false;
				}
				else
				{
					SaveLoadManager.latestSave = latest + 1; 
					SaveLoadManager.latestAutosave = false;
					SaveLoadManager.latestQuicksave = false;
				}
			}
		}
		
		//Set Transform of the Scrollview so that each button will fit
		rectTransform = (RectTransform)transform; 
		rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
		rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, top);
	}

	//Find the Highest element in an array
	long MaxValue (long[] intArray)
	{
		long max = intArray[0];
		long maxElement = 0;
		for (int i = 1; i < intArray.Length; i++) 
		{
			if(intArray[i] > max)
			{
				max = intArray[i];
				maxElement = i;
			}
		}
		return maxElement;
	}
}
