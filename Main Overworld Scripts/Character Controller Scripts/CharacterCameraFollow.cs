using UnityEngine;
using System.Collections;

//Script Objective: The Camera Follow Script of the Character Controller

//Sources: -3D Buzz 3rd person character controller tutorials

public class CharacterCameraFollow : MonoBehaviour 
{
	//Instance Variables of this Object
	public static CharacterCameraFollow Instance;

	//Public Target Variable
	public Transform TargetLookAt; 

	//Camera Variables
	public float distance = 3f;
	public float distanceMin = 1f;
	public float distanceMax = 5f;
	public float distanceSmooth = 0.05f;
	public float distanceResumeSmooth = 1f; 
	public float mouseSensitivityX = 5f;
	public float mouseSensitivityY = 5f; 
	public float mouseWheelSensitivity = 5f;
	public float smoothX = 0.05f;
	public float smoothY = 0.1f; 
	public float minLimitY = -40f;
	public float maxLimitY = 70f; 
	public float occlusionDistanceStep = 0.5f;
	public int maxOcclusionChecks = 10; 

	private float mouseX = 0f;
	private float mouseY = 0f; 
	private float velX = 0f;
	private float velY = 0f;
	private float velZ = 0f;
	private float velDistance = 0f; 
	private float startDistance = 0f;
	private Vector3 position = Vector3.zero;
	private Vector3 desiredPosition = Vector3.zero;
	private float desiredDistance = 0f; 
	private float distSmooth = 0f;
	private float preOccludedDistance = 0f; 

	//Options Variables
	private bool invertX = false;
	private bool invertY = false;
	private int mouseSwap = 1;

	void Awake()
	{
		Instance = this;	//Set the instance to equal this object

		//Update controls
		UpdateCameraControls ();
	}

	// Use this for initialization
	void Start () 
	{
		distance = Mathf.Clamp (distance, distanceMin, distanceMax); 	//Initialise a clamped Distance
		startDistance = distance; 										//Initialise Starting Distance
		Reset (); 														//Call Reset Function
	}

	void LateUpdate () 
	{
		if (TargetLookAt == null)
		{
			return;
		}

		HandlePlayerInput ();

		var count = 0;
		do
		{
			CalculateDesiredPosition ();
			count++;
		}while(CheckIfOccluded (count));

		UpdatePosition (); 
	}

	//This function handles player input	
	void HandlePlayerInput()
	{
		float deadZone = 0.01f; 

		//If not paused and player presses the escape or cancel button then update controls
		if(!Pause.isPaused && Input.GetButtonDown ("Cancel"))
		{
			UpdateCameraControls ();
		}

		//If Right Mouse button is clicked
		if(!CharacterManager.isBusy && !Pause.isPaused && !GameOver.isGameOver)
		{
			if(Input.GetMouseButton (mouseSwap))
			{
				// The RMB is down get mouse axis input
				if(!invertX)
				{
					mouseX += Input.GetAxis ("Mouse X") * mouseSensitivityX;
				}
				else
				{
					mouseX -= Input.GetAxis ("Mouse X") * mouseSensitivityX;
				}
				if(!invertY)
				{
					mouseY -= Input.GetAxis ("Mouse Y") * mouseSensitivityY;
				}
				else
				{
					mouseY += Input.GetAxis ("Mouse Y") * mouseSensitivityY;
				}
			}	
		}

		// This is where we will limit mouseY 
		//Mouse Y will be clamped by the Mouse Helper Class
		mouseY = CharacterCameraHelper.ClampAngle (mouseY, minLimitY, maxLimitY);

		//If Scrolling and outside deadzone
		if(Input.GetAxis ("Mouse ScrollWheel") < -deadZone || Input.GetAxis ("Mouse ScrollWheel") > deadZone)
		{
			//Clamp the desired distance
			desiredDistance = Mathf.Clamp (distance - Input.GetAxis ("Mouse ScrollWheel") * mouseWheelSensitivity, 
			                               distanceMin, distanceMax);

			preOccludedDistance = desiredDistance;
			distSmooth = distanceSmooth; 
		}
	}

	//This function handles desired camera positions
	void CalculateDesiredPosition()
	{
		//Evaluate Distance 
		ResetDesiredDistance ();
		distance = Mathf.SmoothDamp (distance, desiredDistance, ref velDistance, distSmooth); 

		//Calculate Desired Position
		desiredPosition = CalculatePosition (mouseY , mouseX, distance);
	}

	Vector3 CalculatePosition(float rotationX, float rotationY, float dist)
	{
		Vector3 direction = new Vector3 (0, 0, -dist); 
		Quaternion rotation = Quaternion.Euler (rotationX, rotationY, 0);
		return TargetLookAt.position + rotation * direction; 
	}

	bool CheckIfOccluded (int count)
	{
		var isOccluded = false;

		var nearestDistance = CheckCameraPoints (TargetLookAt.position, desiredPosition); 

		if(nearestDistance != -1f)
		{
			if(count < maxOcclusionChecks)
			{
				isOccluded = true; 
				distance -= occlusionDistanceStep;

				if(distance < 1f)
				{
					distance = 1f;
				}
			}
			else
			{
				distance = nearestDistance - Camera.main.nearClipPlane; 
			}

			desiredDistance = distance;
			distSmooth= distanceResumeSmooth;
		}

		return isOccluded; 
	}

	float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		var nearestDistance = -1f; 

		RaycastHit hitInfo;

		CharacterCameraHelper.ClipPlanePoints clipPlanePoints = CharacterCameraHelper.ClipPlaneAtNear (to);

		// Draw Lines in the editor to make it easier to visualize
		Debug.DrawLine (from, to + transform.forward * -camera.nearClipPlane, Color.red);
		Debug.DrawLine (from, clipPlanePoints.UpperLeft);
		Debug.DrawLine (from, clipPlanePoints.LowerLeft);
		Debug.DrawLine (from, clipPlanePoints.UpperRight);
		Debug.DrawLine (from, clipPlanePoints.LowerRight);

		Debug.DrawLine (clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		Debug.DrawLine (clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		Debug.DrawLine (clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		Debug.DrawLine (clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

		if(Physics.Linecast (from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
		{
			nearestDistance = hitInfo.distance; 
		}

		if(Physics.Linecast (from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
		{
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance; 
			}
		}

		if(Physics.Linecast (from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player")
		{
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance; 
			}
		}

		if(Physics.Linecast (from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player")
		{
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance; 
			}
		}

		if(Physics.Linecast (from, to + transform.forward * -camera.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
		{
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance; 
			}
		}

		return nearestDistance; 
	}

	void ResetDesiredDistance()
	{
		if(desiredDistance < preOccludedDistance)
		{
			var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);

			var nearestDistance = CheckCameraPoints (TargetLookAt.position, pos);

			if(nearestDistance == -1 || nearestDistance > preOccludedDistance) 
			{
				desiredDistance = preOccludedDistance;
			}
		}
	}

	void UpdatePosition()
	{
		float posX = Mathf.SmoothDamp (position.x, desiredPosition.x, ref velX, smoothX);
		float posY = Mathf.SmoothDamp (position.y, desiredPosition.y, ref velY, smoothY);
		float posZ = Mathf.SmoothDamp (position.z, desiredPosition.z, ref velZ, smoothX);
		position = new Vector3 (posX, posY, posZ); 

		transform.position = position; 

		transform.LookAt (TargetLookAt);
	}

	//Reset Values on Start
	void Reset()
	{
		mouseX = 0f; 
		mouseY = 10f; 

		distance = startDistance; 
		desiredDistance = distance; 

		preOccludedDistance = distance; 
	}

	//This function takes control of an existing camera or creates a new main camera
	public static void UseExistingOrCreateNewMainCamera()
	{
		//Function Variables
		GameObject tempCamera;				//Temporary Camera Game Object
		GameObject targetLookAt;			//Target Point Game Object
		CharacterCameraFollow myCamera; 	//This Object

		//----------------------------------------------------------------------------------------------

		//If Main Camera is found
		if(Camera.main != null)
		{
			tempCamera = Camera.main.gameObject; 	//Grab that existing main camera
		}
		else 											//Otherwise if no main camera then
		{
			tempCamera = new GameObject("Main Camera");	//Create Camera Object
			tempCamera.AddComponent ("Camera"); 		//Add Camera Component
			tempCamera.tag = "MainCamera"; 				//Tag as Main Camera
		}

		//----------------------------------------------------------------------------------------------

		//After initialising our camera, we need to add this object or 'script' into the temporary camera
		tempCamera.AddComponent ("CharacterCameraFollow");	//Add the Instance into the component
		myCamera = tempCamera.GetComponent ("CharacterCameraFollow") as CharacterCameraFollow; //myCamera gets Instance

		//----------------------------------------------------------------------------------------------

		//We now need to find our Target for this object or camera to look at
		targetLookAt = GameObject.Find ("targetLookAt") as GameObject; 	//Find a target to look at

		//If we didn't find a target to look at
		if(targetLookAt == null)
		{
			targetLookAt = new GameObject ("targetLookAt");	//Create a target to look at
			targetLookAt.transform.position = Vector3.zero; //At 0,0,0
			targetLookAt = GameObject.Find ("targetLookAt") as GameObject; 	//Find a target to look at
		}

		myCamera.TargetLookAt = targetLookAt.transform; 	//The local target will equal the public target to look at

		//----------------------------------------------------------------------------------------------

	}


	//Create a function that will update itself from the Options Specified
	public void UpdateCameraControls()
	{
		//Invert X
		if(PlayerPrefs.HasKey ("InvertX"))
		{
			int currentInvertX = PlayerPrefs.GetInt ("InvertX");
			if(currentInvertX == 1)
			{
				invertX = true;
			}
			else
			{
				invertX = false;
			}
		}
		
		//Invert Y
		if(PlayerPrefs.HasKey ("InvertY"))
		{
			int currentInvertY = PlayerPrefs.GetInt ("InvertY");
			if(currentInvertY == 1)
			{
				invertY = true;
			}
			else
			{
				invertY = false;
			}
		}
		
		//Mouse Swap Toggle
		if(PlayerPrefs.HasKey ("MouseSwap"))
		{
			mouseSwap = PlayerPrefs.GetInt ("MouseSwap");

			if(mouseSwap == 0)
			{
				mouseSwap = 1;
			}
			else
			{
				mouseSwap = 0;
			}
		}
		
		//Mouse Sensitivity
		if(PlayerPrefs.HasKey ("MouseSensitivity"))
		{
			float sensitivity = PlayerPrefs.GetFloat ("MouseSensitivity");
			mouseSensitivityX = sensitivity;
			mouseSensitivityY = sensitivity;
		}

		
		//Mouse Smooth X
		if(PlayerPrefs.HasKey ("MouseSmoothX"))
		{
			smoothX = PlayerPrefs.GetFloat ("MouseSmoothX");
		}
		
		
		//Mouse Smooth Y	
		if(PlayerPrefs.HasKey ("MouseSmoothY"))
		{
			smoothY = PlayerPrefs.GetFloat ("MouseSmoothY");
		}
	}
}
