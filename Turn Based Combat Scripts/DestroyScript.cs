using UnityEngine;
using System.Collections;

public class DestroyScript : MonoBehaviour 
{
	public float destroyTime;

	public GameObject specificObject;

	// Use this for initialization
	void Start () 
	{
		if(specificObject)
		{
			Destroy (specificObject, destroyTime);
		}
		else
		{
			Destroy (gameObject, destroyTime);
		}
	}
}
