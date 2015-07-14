using UnityEngine;
using System.Collections;

//Script Objective: The Character Controller will follow this object. This object will be customisable 
//depending whether it will be player controlled or if not, it will be a companion object to follow a point. 

//This Object is a subject to Unity's Navigation system and requires Navigation Mesh to work.

public class CharacterNavigator : MonoBehaviour 
{
	//Navigation Mesh Variables
	public Transform parentController; 
	private Quaternion parentControllerRotation = Quaternion.identity; 
	public NavMeshAgent agent; 

	//Transform variables
	private float h;
	private float v;

	//Speed Variables
	public float walkSpeed = 1.5f;
	public float runSpeed = 10f;
	public float walkSmooth = 0.5f;
	public float runSmooth = 0.02f;
	private float moveSpeedSmooth;

	//Direction Variables
	public float directionRotationSpeed = 5f;

	//After Idle Pausing Variables
	private float afterIdleTimer; 
	public float afterIdleMaxTimer; 

	//Companion Variables
	public Transform [] companionFollowPoint; 
	private int maxNumberOfCharacters = 3;
	private int followPoint = 0; 
	public bool isMain = false; 
	public Transform characterCamera;
	public int switchPriority;
	private float distanceToPlayer; 
	public float minSprintDistance = 5f;
	public float minDistanceFromPlayer = 3f;
	public float maxDistanceFromPlayer = 10f;
	public float spawnToPlayerDistance = 25f;
	private bool nextToPlayer;

	private CharacterCameraFollow cameraFollow;


	//Mecanim variables
	public Animator anim;		//A variable reference to the animator of the character


	void Awake () 
	{
		cameraFollow = characterCamera.gameObject.GetComponent <CharacterCameraFollow>();

		agent.destination = transform.position;	//The agent will follow this object

		//Make Sure Characters spawn on the intended spot. Then Turn the agent back on. 
		//This will prevent spawning under the ground. or in midair.
		agent.enabled = false;
		transform.localPosition = Vector3.forward;

		//Call the Character Camera Follow object to find a camera
		//CharacterCameraFollow.UseExistingOrCreateNewMainCamera (); //Find a camera to follow this character

		SpawnToLeader ();

		CameraChange ();	//Initialise camera
	}

	// Start is called for initialisation
	void Start()
	{
		afterIdleTimer = afterIdleMaxTimer; 		//The After Idle Timer
		agent.enabled = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If Player presses the Tab button, this should switch between characters
		//As long it is not paused, busy or in game over
		if(!CharacterManager.isBusy && !Pause.isPaused && !GameOver.isGameOver)
		{
			if(Input.GetKeyDown (KeyCode.Tab))
			{
				SwitchPlayer();
			}
		}
		
		//----------------------------------------------------------------------------------------------

		//If Agent is active
		if(agent.gameObject.activeSelf)
		{
			//Find location of agent
			CheckLocation();
		}

		//----------------------------------------------------------------------------------------------
		
		//Distance of follow target when this object isn't the main 
		if(companionFollowPoint[followPoint])
		{
			distanceToPlayer = Vector3.Distance (companionFollowPoint[followPoint].position, agent.transform.position);
		}

		//----------------------------------------------------------------------------------------------

		//Assign the Parent Controller to follow the Characters position and the cameras rotations
		if(parentController)
		{
			parentController.transform.position = agent.gameObject.transform.position; 

			if(Camera.main)
			{
				parentControllerRotation.eulerAngles = new Vector3 (0f, Camera.main.transform.eulerAngles.y, 0f);
				parentController.transform.eulerAngles = parentControllerRotation.eulerAngles; 
			}
		}

		//----------------------------------------------------------------------------------------------

		//Control the movement of this object using the Horizontal and Vertical Axis
		//else follow a point and be a componanion
		if(isMain)
		{
			//If not busy and not in pause and game over (just in case)
			if(!CharacterManager.isBusy && !Pause.isPaused && !GameOver.isGameOver)
			{
				Vector3 directionVector = new Vector3 (Input.GetAxis ("Horizontal"), 1f, Input.GetAxis ("Vertical")); 
				transform.localPosition = directionVector;
			}
			else
			{
				transform.localPosition = Vector3.zero; //stop if player is busy, paused or in game over state.
			}
		}
		else
		{
			if(distanceToPlayer < minDistanceFromPlayer)
			{
				nextToPlayer = true;
			}

			if(distanceToPlayer > maxDistanceFromPlayer)
			{
				nextToPlayer = false;
			}

			if(nextToPlayer)
			{
				transform.position = agent.transform.position;
			}
			else
			{
				transform.position = companionFollowPoint[followPoint].position;
			}

			//If too far from player
			if(distanceToPlayer > spawnToPlayerDistance)
			{
				SpawnToLeader ();
			}
		}

		h = transform.localPosition.x;
		v = transform.localPosition.z;

		//----------------------------------------------------------------------------------------------

		//Assign the correct Smooth Speed
		//This is determined if the player is sprinting then he suddenly stops there will be
		//a greater fidelity of smoothing, whereas just walking just stops faster.
		if(agent.speed > walkSpeed)
		{
			moveSpeedSmooth = runSmooth;
		}
		else
		{
			moveSpeedSmooth = walkSmooth; 
		}

		//----------------------------------------------------------------------------------------------

		//Assign Agent Speed
		//If the player presses a movement button and idle timer is finished then move
		if(h < 0f || h > 0f || v < 0f || v > 0f)
		{
			if(afterIdleTimer <= 0f)
			{
				//The player is moving
				if(!Input.GetKey(KeyCode.LeftShift) && isMain)	//If Player is not holding left shift
				{
					agent.speed = Mathf.Lerp (agent.speed, walkSpeed, moveSpeedSmooth); 	//Walk
				}
				else if(Input.GetKey(KeyCode.LeftShift) && isMain)
				{	
					agent.speed = Mathf.Lerp (agent.speed, runSpeed, moveSpeedSmooth);		//Run
					anim.SetLayerWeight (1, 0f);	//Turn Layer off
				}

				//If not player controlled
				//if not within min distance, sprint
				if(distanceToPlayer > minSprintDistance && !isMain)
				{
					agent.speed = Mathf.Lerp (agent.speed, runSpeed, moveSpeedSmooth);		//Run
					anim.SetLayerWeight (1, 0f);	//Turn Layer off
				}
				else if (distanceToPlayer < minSprintDistance && distanceToPlayer > 0f && !isMain)
				{
					agent.speed = Mathf.Lerp (agent.speed, walkSpeed, moveSpeedSmooth); 	//Walk
				}
			}
			else
			{
				//There shouldn't be any pausing if player wants to start running or just walking forward
				if(Input.GetKey (KeyCode.LeftShift))
				{
					afterIdleTimer = 0f;
					anim.SetLayerWeight (1, 0f);	//Turn Layer off
				}
				else
				{
					afterIdleTimer -= Time.deltaTime; 	//Idle Pausing timer countdown
					anim.SetLayerWeight (1, 1f);	//Turn Layer On
				}
			}
		}
		else
		{
			//The player is not moving
			agent.speed = Mathf.Lerp (agent.speed, 0f, moveSpeedSmooth);

			afterIdleTimer = afterIdleMaxTimer; 
		}

		anim.SetFloat ("Speed", agent.speed); 	//Assign the player speed to the Mecanim Speed

		//----------------------------------------------------------------------------------------------

		if(afterIdleTimer > 0f)
		{
			agent.updatePosition = false;
		}
		else
		{
			agent.updatePosition = true;
		}

		//----------------------------------------------------------------------------------------------

		//Access Mecanim, so this character is animating according to the movement of this script
		anim.SetFloat ("Horizontal", h);	//Horizontal mecanim variable will equal the Horizontal axis
		anim.SetFloat ("Vertical", v); 		//Vertical mecanim variable will equal the vertical axis

		//----------------------------------------------------------------------------------------------

		//Calculate the direction of the Player and this object in degrees
		//This will be achieved by having this navigator object look at the agent and 
		//Compare the two Y rotations and this will give us the degrees
		float angle;
		if(h < 0f || h > 0f || v < 0f || v > 0f)
		{
			Vector3 agentDir = agent.transform.position - transform.position;
			float directionSpeed = directionRotationSpeed * Time.deltaTime;
			Vector3 agentLookAt = Vector3.RotateTowards (transform.forward, -agentDir, directionSpeed, 0f);
			transform.rotation = Quaternion.LookRotation (agentLookAt);

			// Old Solution - Very Inaccurate
			if(afterIdleTimer < afterIdleMaxTimer / 1.25f)
			{
				angle = transform.eulerAngles.y - agent.transform.localEulerAngles.y;
				angle = Mathf.Abs (angle);
			}
			else
			{
				angle = 0f;
			}

			//angle = (2f * Mathf.PI * agent.transform.eulerAngles.y) / transform.localEulerAngles.y;
			//angle = angle * Mathf.Rad2Deg;
			//Vector3 from = new Vector3 (0f, agent.transform.eulerAngles.y, 0f);
			//Vector3 to = new Vector3 (0f, transform.eulerAngles.y, 0f);
			//angle = Vector3.Angle (from, to);
			//angle = Mathf.Abs (angle);

			//angle = 180f - Quaternion.Angle (agent.transform.rotation, transform.rotation);
		}
		else
		{
			angle = 0f;
		}


		anim.SetFloat ("Direction", angle);	//Update Mecanim's Direction

		//----------------------------------------------------------------------------------------------

		//If Direction is between the forward points 340 - 360 and 0 - 20 then just turn off after idle pause
		if(angle > 0f && angle < 20f || angle > 340f && angle < 360f)
		{
			afterIdleTimer = 0f;
		}
	}

	//Find location of the nav mesh agent
	void CheckLocation()
	{
		//Mark this object as the agents destination
		agent.destination = transform.position;
	}

	//This function will switch the player
	void SwitchPlayer()
	{
		switchPriority--; 

		if(switchPriority < 0)
		{
			switchPriority = maxNumberOfCharacters - 1;
		}

		if(switchPriority == 0)
		{
			isMain = true;
		}
		else
		{
			isMain = false;
		}

		CameraChange ();	//Check if camera needs a change

		if(!isMain && maxNumberOfCharacters != 1)
		{
			if(maxNumberOfCharacters == 2)
			{
				followPoint++;

				if(followPoint >= companionFollowPoint.Length - 1)
				{
					followPoint = 0;

					if(!companionFollowPoint[followPoint].gameObject.activeInHierarchy)
					{
						followPoint++;
					}
				}
			}
			else if(maxNumberOfCharacters == 3)
			{
				//Change person to follow
				if(followPoint >= companionFollowPoint.Length - 1)
				{
					followPoint = 0;
				}
				else
				{
					followPoint++;
				}
			}
		}
	}

	//This function will initialise this script to work together to form a solid switch character system
	public void SetCharacter(int _priority, int maxCharacters, bool _leader)
	{
		switchPriority = _priority;				//Set Switch Priority	
		maxNumberOfCharacters = maxCharacters;	//Set the maximum number of characters
		isMain = _leader;						//Set the leader
		SpawnToLeader ();						//If not the leader spawn to leader
		CameraChange ();						//Turn on the appropriate character camera
	}

	//Spawn towards the leader
	public void SpawnToLeader()
	{
		if(!isMain)
		{
			agent.gameObject.transform.position = companionFollowPoint[followPoint].position;
		}
	}

	//This turns off the camera if not main else turn it on
	void CameraChange()
	{
		if(!isMain)
		{
			characterCamera.gameObject.SetActive (false);
		}
		else
		{
			characterCamera.gameObject.SetActive (true);
			cameraFollow.UpdateCameraControls ();
		}
	}
}
