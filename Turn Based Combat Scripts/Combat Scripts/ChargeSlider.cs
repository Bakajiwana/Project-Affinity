using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This script handles the Charging Attack Mode Slider

public class ChargeSlider : MonoBehaviour 
{
	public Transform [] sliders;
	public Transform [] critBars;

	// Use this for initialization
	void Awake () 
	{
		for(int i = 0; i < sliders.Length; i++)
		{
			sliders[i].gameObject.SetActive (false);
			critBars[i].gameObject.SetActive (false);
		}
	}

	public void ActivateChargeSlider(int _element)
	{
		_element--;
		sliders[_element].gameObject.SetActive (true);
		GameObject.FindGameObjectWithTag("Combat UI").SendMessage ("SetElementSlider", sliders[_element].gameObject, SendMessageOptions.DontRequireReceiver);

		//Layout the crit bars
		CreateCriticalPoints (_element);
	}

	//This function is called by the CreateChargeSlider to create 3 randomly placed critical bars
	void CreateCriticalPoints(int _element)
	{
		//Show Crit Zone
		critBars[_element].gameObject.SetActive (true);

		//Calculate 3 random zones
		float [] randomZones = new float[critBars[_element].childCount];

		float min = 0f;
		float accumulate = (100f/ critBars[_element].childCount) / 100f;
		float max = accumulate;

		for(int i = 0; i < randomZones.Length; i++)
		{
			randomZones[i] = Random.Range (min, max);
			min += accumulate;
			max += accumulate;
		}

		//Obtain Sliders
		Slider critSlider;
		int loopCount = 0;

		foreach(Transform child in critBars[_element])
		{
			critSlider = child.gameObject.GetComponent<Slider>();

			//Place into position
			critSlider.value = randomZones[loopCount];

			loopCount++;
		}

		//Send Randomzones to UI manager
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetCriticalZones", randomZones,SendMessageOptions.DontRequireReceiver);
	}
}
