using UnityEngine;
using System.Collections;

//Script Objective: This script will manage the characters in the game

public class CharacterManager : MonoBehaviour 
{
	//Character Variables
	private GameObject characterIona;
	private GameObject characterTaven;
	private GameObject characterAiren;

	//Character Navigator Variables
	private GameObject ionaController;
	private GameObject tavenController;
	private GameObject airenController;

	private CharacterNavigator ionaNavigator;
	private CharacterNavigator tavenNavigator;
	private CharacterNavigator airenNavigator;

	//Busy Static Bool to be used during "Busy" moments to prevent certain interactions
	public static bool isBusy = false;

	// Use this for initialization
	void Start () 
	{
		//Initialise Characters
		characterIona = GameObject.Find("characterIona01");
		characterTaven = GameObject.Find ("characterTaven01");
		characterAiren = GameObject.Find ("characterAiren01");

		ionaController = GameObject.Find ("characterIona01/Controller/Iona's Navigator");
		tavenController = GameObject.Find ("characterTaven01/Controller/Taven's Navigator");
		airenController = GameObject.Find ("characterAiren01/Controller/Airen's Navigator");

		if(ionaController)
		{
			ionaNavigator = ionaController.GetComponent<CharacterNavigator>();
		}

		if(tavenController)
		{
			tavenNavigator = tavenController.GetComponent<CharacterNavigator>();
		}

		if(airenController)
		{
			airenNavigator = airenController.GetComponent<CharacterNavigator>();
		}

		/*
		//Debugging: Check if characters are found and initialised
		print ("Iona = " + (bool)characterIona);
		print ("Taven = " + (bool)characterTaven);
		print ("Airen = " + (bool)characterAiren);

		print ("Iona's Controller = " + (bool)ionaController);
		print ("Taven's Controller = " + (bool)tavenController);
		print ("Airen's Controller = " + (bool)airenController);

		print ((bool)ionaNavigator);
		print ((bool)tavenNavigator);
		print ((bool)airenController);
		*/

		//Later on during story: This will be case switched depending on story level
		ActivateCharacters (true,true,true, SaveLoadManager.savePosition);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void ActivateCharacters(bool airenActive, bool ionaActive, bool tavenActive, Vector3 _location)
	{
		//Update characters position
		characterAiren.transform.position = _location;
		characterIona.transform.position = _location;
		characterTaven.transform.position = _location;

		//If Airen, Iona and Taven is active
		if(airenActive && ionaActive && tavenActive)
		{
			int maxCharacters = 3;

			characterAiren.SetActive (true);
			characterIona.SetActive (true);
			characterTaven.SetActive (true);

			airenNavigator.SetCharacter (0, maxCharacters, true);
			ionaNavigator.SetCharacter (1, maxCharacters, false);
			tavenNavigator.SetCharacter (2, maxCharacters, false);
		}
		//If Airen and Iona is active
		if(airenActive && ionaActive && !tavenActive)
		{
			int maxCharacters = 2;

			characterAiren.SetActive (true);
			characterIona.SetActive (true);
			characterTaven.SetActive (false);
			
			airenNavigator.SetCharacter (0, maxCharacters, true);
			ionaNavigator.SetCharacter (1, maxCharacters, false);
		}
		//If Airen and Taven is active
		if(airenActive && !ionaActive && tavenActive)
		{
			int maxCharacters = 2;
			
			characterAiren.SetActive (true);
			characterIona.SetActive (false);
			characterTaven.SetActive (true);
			
			airenNavigator.SetCharacter (0, maxCharacters, true);
			tavenNavigator.SetCharacter (1, maxCharacters, false);
		}
		//If Iona and Taven is active
		if(!airenActive && ionaActive && tavenActive)
		{
			int maxCharacters = 2;
			
			characterAiren.SetActive (false);
			characterIona.SetActive (true);
			characterTaven.SetActive (true);

			ionaNavigator.SetCharacter (0, maxCharacters, true);
			tavenNavigator.SetCharacter (1, maxCharacters, false);
		}
		//If Airen is active only
		if(airenActive && !ionaActive && !tavenActive)
		{
			int maxCharacters = 1;
			
			characterAiren.SetActive (true);
			characterIona.SetActive (false);
			characterTaven.SetActive (false);
			
			airenNavigator.SetCharacter (0, maxCharacters, true);
		}
		//If Iona is Active only
		if(!airenActive && ionaActive && !tavenActive)
		{
			int maxCharacters = 1;
			
			characterAiren.SetActive (false);
			characterIona.SetActive (true);
			characterTaven.SetActive (false);

			ionaNavigator.SetCharacter (0, maxCharacters, true);
		}
		//If Taven is Active only
		if(!airenActive && !ionaActive && tavenActive)
		{
			int maxCharacters = 1;
			
			characterAiren.SetActive (false);
			characterIona.SetActive (false);
			characterTaven.SetActive (true);

			tavenNavigator.SetCharacter (0, maxCharacters, true);
		}
	}

	//Save the players position during save
	public void SavePlayerPosition()
	{
		if(airenController.gameObject.activeInHierarchy)
		{
			SaveLoadManager.savePosition = airenController.transform.position;
		}
		else if(ionaController.gameObject.activeInHierarchy)
		{
			SaveLoadManager.savePosition = ionaController.transform.position;
		}
		else if (tavenController.gameObject.activeInHierarchy)
		{
			SaveLoadManager.savePosition = tavenController.transform.position;
		}
	}
}
