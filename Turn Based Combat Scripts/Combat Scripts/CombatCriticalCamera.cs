using UnityEngine;
using System.Collections;

//This script controls the critical camera object

public class CombatCriticalCamera : MonoBehaviour 
{
	private bool criticalCloseIn = false;
	private GameObject target;
	public float critTimeScale;
	private int cameraIndex;

	public GameObject cam; 

	private Vector3 targetPos;

	public float speedUpStartMax = 0.2f;
	private float speedUpStartTimer = 0.2f;

	public float minSlowDownDistance = 0.2f;

	public float speed;

	private Animator anim;

	// Use this for initialization
	void Start () 
	{
		anim = gameObject.GetComponent<Animator>();
		speedUpStartTimer = speedUpStartMax;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(criticalCloseIn)
		{
			if(target.activeInHierarchy)
			{
				//Move to target
				float step = speed * Time.deltaTime;
				transform.position = Vector3.MoveTowards (transform.position,targetPos, step);

				//Have the same rotation as the target
				Vector3 targetRotation = new Vector3 (target.transform.rotation.x,
				                                      -target.transform.rotation.y,
				                                      target.transform.rotation.z);
				transform.eulerAngles = targetRotation;

				//Get Camera to look at target
				cam.transform.LookAt (targetPos);

				float dist = Vector3.Distance (targetPos, transform.position);

				if(dist < minSlowDownDistance)
				{
					anim.SetInteger ("Critical Number", cameraIndex);

					speedUpStartTimer -= Time.deltaTime;
					if(speedUpStartTimer <= 0f)
					{
						Time.timeScale = 1f;
					}
					else
					{
						//Slowdown time
						Time.timeScale = critTimeScale;
					}
				}
			}
			else
			{
				Time.timeScale = 1f;

				//Target no longer exist, turn off critical hit camera
				TurnOffCritical ();
			}
		}
	}

	public void CriticalCameraActivate(GameObject _criticalTarget, int _cameraIndex)
	{
		criticalCloseIn = true;
		cameraIndex = _cameraIndex;

		//Calculate Target Properly
		CapsuleCollider targetMeasurement = _criticalTarget.GetComponent<CapsuleCollider>();
		float height = targetMeasurement.height / 1.5f;
		float distance = targetMeasurement.radius;

		targetPos = new Vector3(_criticalTarget.transform.position.x,
		                        height, _criticalTarget.transform.position.z + distance);
		target = _criticalTarget;
	}

	public void TurnOffCritical()
	{
		speedUpStartTimer = speedUpStartMax;
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOff", SendMessageOptions.DontRequireReceiver);
	}
}
