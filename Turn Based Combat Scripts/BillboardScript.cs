using UnityEngine;
using System.Collections;

public class BillboardScript : MonoBehaviour 
{

	// Update is called once per frame
	void Update () 
	{
		transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
	}
}
