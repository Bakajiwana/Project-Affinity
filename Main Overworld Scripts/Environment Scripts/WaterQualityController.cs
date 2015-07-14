using UnityEngine;
using System.Collections;

//Script Objective control the quality of water in the scene

public class WaterQualityController : MonoBehaviour 
{
	public GameObject lowQualityWater;
	public GameObject medQualityWater;
	public GameObject highQualityWater;

	// Use this for initialization
	void Start () 
	{
		//Start coroutine for performance
		StartCoroutine (PerformanceUpdate ());
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	//This coroutine is just used to save performance instead of every frame it is every second
	IEnumerator PerformanceUpdate()
	{
		while(true) //loop forever
		{
			yield return new WaitForSeconds(5f);

			//If the water quality is low then turn on low etc....
			if(EnvironmentManager.waterQuality == 0)
			{
				lowQualityWater.SetActive (true);
				medQualityWater.SetActive (false);
				highQualityWater.SetActive (false);
			}
			else if(EnvironmentManager.waterQuality == 1)
			{
				lowQualityWater.SetActive (false);
				medQualityWater.SetActive (true);
				highQualityWater.SetActive (false);
			}
			else if (EnvironmentManager.waterQuality == 2)
			{
				lowQualityWater.SetActive (false);
				medQualityWater.SetActive (false);
				highQualityWater.SetActive (true);
			}
		}
	}
}
