using UnityEngine;
using System.Collections;

public class CombatCameraManager : MonoBehaviour 
{
	public GameObject[] arenaCameras;

	public static bool cameraOverride = false; 

	public GameObject criticalCamera;
	private GameObject previousCamera;
	private CombatCriticalCamera closeUp;

	public float criticalTimeScale = 0.5f;

	// Use this for initialization
	void Awake () 
	{
		ArenaCamerasOff ();
		ArenaCameraRandomOn ();
		criticalCamera.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void ArenaCamerasOff()
	{
		for(int i = 0; i < arenaCameras.Length; i++)
		{
			arenaCameras[i].gameObject.SetActive (false);
		}
	}

	void ArenaCameraRandomOn()
	{
		if(!cameraOverride)
		{
			int randomCamera = Random.Range (0, arenaCameras.Length);

			ArenaCamerasOff();

			arenaCameras[randomCamera].gameObject.SetActive (true);
		}
	}

	void ArenaCameraOn(int _index)
	{
		if(!cameraOverride)
		{
			if(_index > arenaCameras.Length)
			{
				_index = arenaCameras.Length;
			}

			if(_index < 0)
			{
				_index = 0;
			}

			if(!arenaCameras[_index].GetComponent<Camera>().isActiveAndEnabled)
			{
				ArenaCamerasOff ();
				
				arenaCameras[_index].gameObject.SetActive (true); 
			}
		}
	}

	void CriticalCameraOn(GameObject _target)
	{
		if(!cameraOverride)
		{
			previousCamera = Camera.main.gameObject; 
			Camera.main.gameObject.SetActive (false);
			ArenaCamerasOff ();
			criticalCamera.SetActive (true);
			closeUp = criticalCamera.GetComponent<CombatCriticalCamera>();
			cameraOverride = true;
			closeUp.CriticalCameraActivate(_target, 1);
		}
	}

	void CriticalCameraOff()
	{
		cameraOverride = false;
		criticalCamera.SetActive(false);

		if(previousCamera.activeInHierarchy)
		{
			previousCamera.SetActive (true);
		}
		else
		{
			ArenaCameraRandomOn();
		}
	}
}

