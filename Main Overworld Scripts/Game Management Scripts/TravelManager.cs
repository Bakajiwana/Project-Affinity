using UnityEngine;
using System.Collections;

//Script Objective: Used to transport player across levels and fast travel.

public class TravelManager : MonoBehaviour 
{
	public GameObject levelTraveller;
	public string destination;
	public string levelName;
	private bool readyToGo = false;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(readyToGo && Input.GetKeyDown (KeyCode.E))
		{
			InitiateTravel (destination, levelName);
		}
	}

	public void InitiateTravel(string _destination, string _levelName)
	{
		GameObject levelTravel;
		levelTravel = Instantiate (levelTraveller.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		FastTravel travel = levelTravel.GetComponent<FastTravel>();
		travel.SetDestination (_levelName, _destination);
		Application.LoadLevel ("Loading Scene");
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			readyToGo = true;
			AdventureInterface.helpText = "Press E to go to " + destination;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			readyToGo = false;
			AdventureInterface.helpText = "";
		}
	}
}
