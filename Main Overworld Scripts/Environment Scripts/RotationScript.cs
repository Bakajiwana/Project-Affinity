using UnityEngine;
using System.Collections;

public class RotationScript : MonoBehaviour 
{
	public float x;
	public float y;
	public float z;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Make the mainframe ball rotate
		transform.Rotate (new Vector3(x,y,z) * Time.deltaTime);
	}
}
