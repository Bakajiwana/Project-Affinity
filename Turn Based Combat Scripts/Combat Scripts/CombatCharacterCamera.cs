using UnityEngine;
using System.Collections;

//This script controls the cameras from this character intended for animation event use

public class CombatCharacterCamera : MonoBehaviour 
{
	public GameObject characterCamera;

	void Awake()
	{
		characterCamera.gameObject.SetActive (false);
	}

	public void TurnOnCharacterCamera()
	{
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("ArenaCamerasOff", SendMessageOptions.DontRequireReceiver);
		characterCamera.gameObject.SetActive (true);
	}

	public void SwitchBackToRandomArenaCamera()
	{
		characterCamera.gameObject.SetActive (false);
		TurnOnRandomArenaCamera();
	}

	public void TurnOnRandomArenaCamera()
	{
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("ArenaCameraRandomOn", SendMessageOptions.DontRequireReceiver);
	}

	public void SwitchBackToSelectedArenaCamera(int _camera)
	{
		characterCamera.gameObject.SetActive (false);
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("ArenaCameraOn", _camera, SendMessageOptions.DontRequireReceiver);
	}
}
