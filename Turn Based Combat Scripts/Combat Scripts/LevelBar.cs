using UnityEngine;
using System.Collections;

//This script controls the Level Bars 

public class LevelBar : MonoBehaviour 
{
	public Transform[] barFills;

	// Use this for initialization
	void Awake () 
	{
		for(int i = 0; i < barFills.Length; i++)
		{
			if(barFills[i])
			{
				barFills[i].gameObject.SetActive (false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void RevealBar(int _element)
	{
		if(barFills[_element])
		{
			barFills[_element].gameObject.SetActive (true);
		}
	}
}
