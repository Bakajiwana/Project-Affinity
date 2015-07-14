using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Player Actions.... Can be re used to specify different animator, time etc. Don't need stuff like EinarAction etc

public class PlayerCombatActions : MonoBehaviour 
{
	//Animator
	public Animator anim;

	//This variable determines and initialises combat variables only once when combat has properly been setup
	private bool start = false;

	private Transform selector;
	private bool isSelecting;
	private int objectSelection = 1; //0 = Players, 1 = Enemies, 2 = Environment
	private int [] objectCurrentSelection = new int[3];

	//Move to and Rotate Functions
	private bool lookAtTarget = false;
	private bool moveToTarget = false;
	private bool lookAtInitialRotation = false;
	private bool moveToInitialPosition = false;

	private float lookAtTargetTimer = 1f;
	private float targetWidth;

	private float rotateSpeed;
	private float moveSpeed;
	public float teleportDashSpeed = 50f;
	public float teleportRotateSpeed = 5f;

	private PlayerCombatCharacter combatStats;

	//Player button feedback variables
	/*
	 * 0 = Idle
	 * 1 = Ready
	 * 2 = Single Attack
	 * 3 = Flurry Attack
	 * 4 = Charge Attack
	*/
	private int attackPhase; 
	
	public float readyMaxTimer = 0.5f;
	private float readyTimer;
	private float idleDelay = 0f; //This function is used to create a delay if reset to idle

	private bool singleShot;
	private bool charging;
	private bool environmentInteract;

	public float flurryMaxTimer = 1f;
	private float flurryTimer;
	private int flurryNextEnemy = 0;
	private int[] flurryNextElements; 
	private int flurryCurrentButton = 0;

	private float charge = 0f; 
	private float chargeMax;
	public float chargeSpeed = 2f;
	private float chargeBarCount = 0f;
	private float chargeAPCount = 0f;
	private int chargeCurrentBar = 0;
	private bool chargeInstantCritical = false;

	//AP Costs
	public int flurryAPCost = 5;
	public int chargeAPCost = 5;

	//Attack Accuracy
	[Range(0f, 1f)]
	public float flurryAccuracy = 1f;
	[Range(0f, 1f)]
	public float chargeAccuracy = 1f;

	//Status Effect Chances
	[Range(0f, 1f)]
	public float singleShotStatusChance = 0.5f;
	[Range(0f, 1f)]
	public float flurryStatusChance = 0.10f;
	[Range(0f, 1f)]
	public float chargeStatusChance = 0.25f;
	[Range(0f, 1f)]
	public float curseStatusChance = 0.80f;

	//Critical Chances
	[Range(0f, 1f)]
	public float singleShotCritChance = 0.40f;
	[Range(0f, 1f)]
	public float flurryCritChance = 0.10f;
	[Range(0f, 1f)]
	public float chargeCritChance = 0.75f;

	//Attack Phase Initiators
	private int elementalStance = 0; //1 = Earth, 2 = Fire, 3 = Lightning, 4 = Water
	private bool flurryActivate = false;
	private bool flurryReady = false;

	//Combat Effects
	public Transform[] projectileNode;

	//Single Shot Projectiles
	public Transform[] singleShotProjectiles;

	//Level Bar Limits
	private int[] levelBarLimit = new int[5]; //0 - normal, 1 - earth, 2 - fire, 3 - lightning, 4 - water
	private int currentLevelBar = 0;

	//Attack Mode Level Unlock
	public int flurryLevelUnlock = 2;
	public int chargeLevelUnlock = 3; 

	//Renderers To Hide and Effects/ Objects to Show during teleport 
	public GameObject[] teleportHideObjects;
	public Renderer[] teleportHideRenderers;
	public GameObject[] teleportRevealEffects;

	//Scan Mode Variables
	private float scanModeDelay = 0.5f;
	private bool scanModeActivate = false;

	//Action Modes: 0 - Attack, 1 - Defend, 2 - Support, 3 - Curse
	private int actionMode = 0;
	private bool actionModeLock = false;
	
	private bool defendMode = false;
	private bool supportMode = false;
	private bool curseMode = false;

	void Awake()
	{
		//Connect to stats
		combatStats = gameObject.GetComponent<PlayerCombatCharacter>();

		//Find and reference the selector
		selector = GameObject.Find("Selector").transform;

		//Turn this script off in case its on at the start
		this.enabled = false;
	}

	void OnEnable()
	{
		//Initialise Once
		if(!start)
		{
			//Calculate the middle Selections
			//Player Selection
			objectCurrentSelection[0] = (CombatManager.players.Count / 2);
			//Enemy Selection
			objectCurrentSelection[1] = (CombatManager.enemies.Count / 2);
			//Object Selection
			objectCurrentSelection[2] = 0;

			SetLevelBarSizes ();

			start = true;	//End Initialisation
		}

		//If Stunned - End Turn Immediately OR battered - chance of ending turn
		if(combatStats.statusStunned || combatStats.elementalReaction.elementalEffect[3] > 0)
		{
			//If Stunned
			if(combatStats.statusStunned)
			{
				print ("I am stunned..... shit.");
				combatStats.ShowDamageText ("Stun", Color.yellow, 1f);
				EndTurnDelay (0.1f);
			}

			//If Battered
			else if(combatStats.elementalReaction.elementalEffect[3] > 0)
			{
				float batteredChance = Random.Range (0f, 1f);

				if(batteredChance > 0.5f)
				{
					print ("I'm battered bruh");
					combatStats.ShowDamageText ("Battered", Color.white, 1f);
					EndTurnDelay (0.11f);
				}
				else
				{
					//Snapped out of it
					combatStats.elementalReaction.elementalEffect[3] = 0;

					//Re Call OnEnable
					OnEnable ();
				}
			}
		}
		else
		{
			print ("I'm the Player and its my turn HUZZAH.. ");

			//As this will be the Main Player Script activate show the scan button
			combatStats.ui.ActiveUISwitch (true);	//Turn on UI elements
			combatStats.statusUI.SetCurrent (true);	//Set the UI that this character is the current

			//Default Action Variables
			lookAtTarget = false;
			moveToTarget = false;	
			lookAtInitialRotation = false;
			moveToInitialPosition = false;
			lookAtTargetTimer = 1f;
			
			//Default Attack phase 
			attackPhase = 0; //Idle
			readyTimer = readyMaxTimer;
			idleDelay = 0f;
			flurryTimer = flurryMaxTimer;
			elementalStance = 0;
			flurryActivate = false; 
			flurryReady = false;
			flurryNextEnemy = 0;
			flurryCurrentButton = 0;
			currentLevelBar = 0;

			singleShot = false;
			charging = false;
			environmentInteract = false;
			charge = 0f; 
			chargeBarCount = 0f;
			chargeAPCount = 0f;
			chargeCurrentBar = 0;
			chargeInstantCritical = false;

			//Animation Default
			anim.SetInteger ("Attack Phase", 0);
			anim.SetInteger ("Index", 0);

			//Set up the Object Selection
			UpdateObjectSelection();

			//Default Scan Mode Variables
			scanModeDelay = 0.5f;
			scanModeActivate = false;

			//Action Mode Default Variables
			actionModeLock = false;
			defendMode = false;
			supportMode = false;
			curseMode = false;

			//Set the Defend Mode Boolean back to false
			combatStats.defend = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//When this script is updating press a button to do something
		//If there are still enemies and not in scan mode and not activating scan mode (stop buttons being pressed)
		if(CombatManager.enemies.Count > 0 && 
		   !CombatUIManager.scanMode && !scanModeActivate) 
		{
			//-------------------------------------Selection------------------------------------------
			if(isSelecting)
			{
				switch(objectSelection)
				{
				case 0:	//Player Selection
					if(Input.GetKeyDown (KeyCode.A))
					{
						objectCurrentSelection[0]--; 
						
						if(objectCurrentSelection[0] < 0)
						{
							objectCurrentSelection[0] = CombatManager.players.Count - 1;
						}
						
						selector.position = CombatManager.players[objectCurrentSelection[0]].transform.position;
					}
					
					if(Input.GetKeyDown (KeyCode.D))
					{
						objectCurrentSelection[0]++; 
						
						if(objectCurrentSelection[0] >= CombatManager.players.Count)
						{
							objectCurrentSelection[0] = 0;
						}			
	
						selector.position = CombatManager.players[objectCurrentSelection[0]].transform.position;
					}
					break;
				case 1:	//Enemy Selection
					if(Input.GetKeyDown (KeyCode.A))
					{
						objectCurrentSelection[1]--; 
						
						if(objectCurrentSelection[1] < 0)
						{
							if(CombatManager.environmentObjects.Count != 0)
							{
								//Move to Environment Selection
								objectSelection = 2;
								objectCurrentSelection[2] = CombatManager.environmentObjects.Count - 1;
								selector.position = CombatManager.environmentObjects[objectCurrentSelection[2]].transform.position;
							}
							else
							{
								objectCurrentSelection[1] = CombatManager.enemies.Count -1;
								selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
							}
						}
						else
						{						
								selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
						}
					}
					
					if(Input.GetKeyDown (KeyCode.D))
					{
						objectCurrentSelection[1]++; 
						
						if(objectCurrentSelection[1] >= CombatManager.enemies.Count)
						{
							if(CombatManager.environmentObjects.Count != 0)
							{
								//Move to Environment Selection
								objectSelection = 2;
								objectCurrentSelection[2] = 0;
								selector.position = CombatManager.environmentObjects[objectCurrentSelection[2]].transform.position;
							}
							else
							{
								objectCurrentSelection[1] = 0;
								selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
							}
						}		
						else
						{
							selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
						}
					}
					break;
				case 2:	//Environment Selection
					if(Input.GetKeyDown (KeyCode.A))
					{
						objectCurrentSelection[2]--; 
						
						if(objectCurrentSelection[2] < 0)
						{
							//Move to Environment Selection
							objectSelection = 1;
							objectCurrentSelection[1] = CombatManager.enemies.Count - 1;
							selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
						}
						else
						{						
							selector.position = CombatManager.environmentObjects[objectCurrentSelection[2]].transform.position;
						}
					}
					
					if(Input.GetKeyDown (KeyCode.D))
					{
						objectCurrentSelection[2]++; 
						
						if(objectCurrentSelection[2] >= CombatManager.environmentObjects.Count)
						{
							//Move to Environment Selection
							objectSelection = 1;
							objectCurrentSelection[1] = 0;
							selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
						}		
						else
						{
							selector.position = CombatManager.environmentObjects[objectCurrentSelection[2]].transform.position;
						}
					}
					break;
				}
			}

			//-------------------------------------Action Phases------------------------------------------
			switch(attackPhase)
			{
			case 0:	//Idle
				if(idleDelay <= 0f)
				{
					if(Input.GetKeyDown (KeyCode.E))
					{
						elementalStance = 1; //Earth Elemental Stance

						switch(actionMode)
						{
						case 0:	//Attack Mode
							if(objectSelection == 1)	//If in Enemy Selection
							{
								attackPhase = 1;
								anim.SetInteger ("Attack Phase", 1);	//Set Attack Phase Animation to Ready
							}
							else if(objectSelection == 2)//If in environment selection
							{
								attackPhase = 5;
							}
							break;
						case 1: //Defend Mode
							attackPhase = 6;
							break;
						case 2:	//Support Mode
							attackPhase = 7;
							break;
						case 3:	//Curse Mode
							attackPhase = 8;
							break;
						}
					}
					else if(Input.GetKeyDown (KeyCode.Q))
					{
						elementalStance = 2; //Fire Elemental Stance

						switch(actionMode)
						{
						case 0: //Attack Mode
							if(objectSelection == 1)
							{
								attackPhase = 1;
								anim.SetInteger ("Attack Phase", 1);	//Set Attack Phase Animation to Ready
							}
							else if(objectSelection == 2)
							{
								attackPhase = 5;
							}
							break;
						case 1: //Defend Mode
							attackPhase = 6;
							break;
						case 2:	//Support Mode
							attackPhase = 7;
							break;
						case 3:	//Curse Mode
							attackPhase = 8;
							break;
						}
					}
					else if(Input.GetKeyDown (KeyCode.R))
					{
						elementalStance = 3; //Lightning Elemental Stance

						switch(actionMode)
						{
						case 0: //Attack Mode
							if(objectSelection == 1)
							{
								attackPhase = 1;
								anim.SetInteger ("Attack Phase", 1);	//Set Attack Phase Animation to Ready
							}
							else if(objectSelection == 2)
							{
								attackPhase = 5;
							}
							break;
						case 1: //Defend Mode
							attackPhase = 6;
							break;
						case 2:	//Support Mode
							attackPhase = 7;
							break;
						case 3:	//Curse Mode
							attackPhase = 8;
							break;
						}
					}
					else if(Input.GetKeyDown (KeyCode.W))
					{
						elementalStance = 4; //Water Elemental Stance

						switch(actionMode)
						{
						case 0: //Attack Mode
							if(objectSelection == 1)
							{
								attackPhase = 1;
								anim.SetInteger ("Attack Phase", 1);	//Set Attack Phase Animation to Ready
							}
							else if (objectSelection == 2)
							{
								attackPhase = 5;
							}
							break;
						case 1: //Defend Mode
							attackPhase = 6;
							break;
						case 2:	//Support Mode
							attackPhase = 7;
							break;
						case 3:	//Curse Mode
							attackPhase = 8;
							break;
						}
					}

					//Move to Scan Mode
					else if(Input.GetKeyDown(KeyCode.Space))
					{
						combatStats.ui.ScanModeSwitch (true);
						combatStats.ui.scanScreen.RevealScanScreen(true);
					}

					//Change Modes
					else if(Input.GetKeyDown (KeyCode.Tab))
					{
						SwitchToNextMode ();
					}
				}
				else
				{
					idleDelay -= Time.deltaTime;
				}
				break;
			case 1: //Ready
				HideSelection (); //Hide Selector

				readyTimer -= Time.deltaTime; 

				//Move to Single Shot
				if(readyTimer <= 0f)
				{
					//When Ready Timer is complete and ready, Determine whether a charge attack or single shot

					//If Still holding specific charge button after idle timer is completed
					//Move to Charge
					if(Input.GetKey (KeyCode.E))		//Earth Charge Mode
					{
						if(CanCharge())
						{
							elementalStance = 1;
							attackPhase = 4;
							anim.SetInteger ("Attack Phase", attackPhase);

							//Create UI - Charge Bar
							combatStats.ui.CreateChargeSlider(elementalStance);
							combatStats.ui.CreateLevelBars (levelBarLimit[elementalStance]);

							//Charging, in case when player finishes charging, cannot re charge during anim
							charging = true;
							//Calculate Max Charge with the levelbar limit
							chargeMax = levelBarLimit[elementalStance] * chargeAPCost;
						}
						else
						{
							ReturnToIdle ();
						}
					}
					else if(Input.GetKey (KeyCode.Q))	//Fire Charge Mode
					{
						if(CanCharge ())
						{
							elementalStance = 2;
							attackPhase = 4;
							anim.SetInteger ("Attack Phase", attackPhase);

							//Create UI - Charge Bar
							combatStats.ui.CreateChargeSlider(elementalStance);
							combatStats.ui.CreateLevelBars (levelBarLimit[elementalStance]);

							//Charging, in case when player finishes charging, cannot re charge during anim
							charging = true;
							//Calculate Max Charge with the levelbar limit
							chargeMax = levelBarLimit[elementalStance] * chargeAPCost;
						}
						else
						{
							ReturnToIdle ();
						}
					}
					else if(Input.GetKey (KeyCode.R))	//Lightning Charge Mode
					{
						if(CanCharge ())
						{
							elementalStance = 3;
							attackPhase = 4;
							anim.SetInteger ("Attack Phase", attackPhase);

							//Create UI - Charge Bar
							combatStats.ui.CreateChargeSlider(elementalStance);
							combatStats.ui.CreateLevelBars (levelBarLimit[elementalStance]);

							//Charging, in case when player finishes charging, cannot re charge during anim
							charging = true;
							//Calculate Max Charge with the levelbar limit
							chargeMax = levelBarLimit[elementalStance] * chargeAPCost;
						}
						else
						{
							ReturnToIdle ();
						}
					}
					else if(Input.GetKey (KeyCode.W))	//Water Charge Mode
					{
						if(CanCharge ())
						{
							elementalStance = 4;
							attackPhase = 4;
							anim.SetInteger ("Attack Phase", attackPhase);

							//Create UI - Charge Bar
							combatStats.ui.CreateChargeSlider(elementalStance);
							combatStats.ui.CreateLevelBars (levelBarLimit[elementalStance]);

							//Charging, in case when player finishes charging, cannot re charge during anim
							charging = true;
							//Calculate Max Charge with the levelbar limit
							chargeMax = levelBarLimit[elementalStance] * chargeAPCost;
						}
						else
						{
							ReturnToIdle ();
						}
					}
					else
					{
						//Move to Single Shot Attack Mode
						attackPhase = 2; //Single Shot
						singleShot = true;
					}
				}


				//Move to a Flurry
				if(!flurryActivate)
				{
					if(Input.GetKey(KeyCode.Q)|| Input.GetKey(KeyCode.W)||
					   Input.GetKey(KeyCode.E)|| Input.GetKey(KeyCode.R))
					{
						flurryActivate = true;

						//Store the first flurry element
						//Create button storage of elements array and assign first button element
						flurryNextElements = new int[levelBarLimit[0]];
						currentLevelBar = 1;
						flurryNextElements[0] = elementalStance;
					}
				}
				else
				{
					//Initialise the Flurry Attack Mode

					//Teleport to target and hit with specified element
					if(Input.GetKeyDown (KeyCode.E))
					{
						if(CanFlurry())
						{
							attackPhase = 3;	//Flurry Attack Mode Animation State
							anim.SetInteger ("Attack Phase", attackPhase); //Play Flurry Animation

							//Teleport to enemy
							MoveToTarget (teleportDashSpeed);
							LookAtTarget (teleportRotateSpeed);

							//Create button storage of elements array and assign first button element
							flurryCurrentButton = 1;

							//Create flurry UI Level Bars
							combatStats.ui.CreateLevelBars (levelBarLimit[0]);
							combatStats.ui.ActivateLevelBar (flurryNextElements[0], currentLevelBar);

							currentLevelBar = 2;	//Move up the current bar and activate the next
							flurryNextElements[1] = 1;
							combatStats.ui.ActivateLevelBar (flurryNextElements[1], currentLevelBar);


							//AP Cost
							combatStats.APCost (flurryAPCost);
						}
						else
						{
							ReturnToIdle ();
						}

					}
					else if(Input.GetKeyDown (KeyCode.Q))
					{
						if(CanFlurry ())
						{
							attackPhase = 3;
							anim.SetInteger ("Attack Phase", attackPhase);

							//Teleport to enemy
							MoveToTarget (teleportDashSpeed);
							LookAtTarget (teleportRotateSpeed);

							//Create button storage of elements array and assign first button element
							flurryCurrentButton = 1;
							
							//Create flurry UI Level Bars
							combatStats.ui.CreateLevelBars (levelBarLimit[0]);
							combatStats.ui.ActivateLevelBar (flurryNextElements[0], currentLevelBar);
							
							currentLevelBar = 2;	//Move up the current bar and activate the next
							flurryNextElements[1] = 2;
							combatStats.ui.ActivateLevelBar (flurryNextElements[1], currentLevelBar);

							//AP Cost
							combatStats.APCost (flurryAPCost);
						}
						else
						{
							ReturnToIdle ();
						}
					}
					else if(Input.GetKeyDown (KeyCode.R))
					{
						if(CanFlurry ())
						{
							attackPhase = 3;
							anim.SetInteger ("Attack Phase", attackPhase);

							//Teleport to enemy
							MoveToTarget (teleportDashSpeed);
							LookAtTarget (teleportRotateSpeed);

							//Create button storage of elements array and assign first button element
							flurryCurrentButton = 1;
							
							//Create flurry UI Level Bars
							combatStats.ui.CreateLevelBars (levelBarLimit[0]);
							combatStats.ui.ActivateLevelBar (flurryNextElements[0], currentLevelBar);
							
							currentLevelBar = 2;	//Move up the current bar and activate the next
							flurryNextElements[1] = 3;
							combatStats.ui.ActivateLevelBar (flurryNextElements[1], currentLevelBar);

							//AP Cost
							combatStats.APCost (flurryAPCost);
						}
						else
						{
							ReturnToIdle ();
						}
					}
					else if(Input.GetKeyDown (KeyCode.W))
					{
						if(CanFlurry ())
						{
							attackPhase = 3;
							anim.SetInteger ("Attack Phase", attackPhase);

							//Teleport to enemy
							MoveToTarget (teleportDashSpeed);
							LookAtTarget (teleportRotateSpeed);

							//Create button storage of elements array and assign first button element
							flurryCurrentButton = 1;
							
							//Create flurry UI Level Bars
							combatStats.ui.CreateLevelBars (levelBarLimit[0]);
							combatStats.ui.ActivateLevelBar (flurryNextElements[0], currentLevelBar);
							
							currentLevelBar = 2;	//Move up the current bar and activate the next
							flurryNextElements[1] = 4;
							combatStats.ui.ActivateLevelBar (flurryNextElements[1], currentLevelBar);

							//AP Cost
							combatStats.APCost (flurryAPCost);
						}
						else
						{
							ReturnToIdle ();
						}
					}
				}
				break;
			case 2: //Single Shot Attack Mode
				if(singleShot)
				{
					anim.SetInteger ("Index", 0); //*For now using 0 as variation*
					anim.SetInteger ("Attack Phase", attackPhase);
					singleShot = false;
				}
				break;
			case 3: //Flurry Attack Mode
				//The Flurry Attack Mode has been activated and this is where the flurry code works in
				if(flurryReady)
				{
					flurryTimer -= Time.deltaTime; //Flurry Timer Countdown

					if(flurryTimer > 0f && flurryCurrentButton < currentLevelBar)
					{
						elementalStance = flurryNextElements[flurryCurrentButton];
						anim.SetTrigger("Next"); //Move to next Chain Animation
						flurryReady = false;
						flurryTimer = flurryMaxTimer;

						DetermineFlurryTeleport (); 	//Calculate if character needs to be teleported

						//Increase current bar storage
						flurryCurrentButton++;

						//print ("The Current Element is" + elementalStance + "in "+ flurryCurrentButton);
					}
					else 
					{
						//If flurry is finished, return to position and end the turn
						//Teleporting back to Initial Position will be called by animation clip
						//End turn will be called in the finishing flurry animation
						anim.SetTrigger ("End"); //End the chain
						combatStats.ui.ClearLevelBars ();
						flurryReady = false;
					}
				}

				//Store and acquire Button presses
				if(currentLevelBar < levelBarLimit[0]
				&& combatStats.stat.actionPoints >= flurryAPCost)
				{
					if(Input.GetKeyDown (KeyCode.E))	//If earth
					{
						flurryNextElements[currentLevelBar] = 1;

						//Activate Next Level Bar
						currentLevelBar ++;
						combatStats.ui.ActivateLevelBar (1, currentLevelBar);

						//AP Cost
						combatStats.APCost (flurryAPCost);
					}
					else if(Input.GetKeyDown (KeyCode.Q))	//if fire
					{
						flurryNextElements[currentLevelBar] = 2;

						//Activate Next Level Bar
						currentLevelBar ++;
						combatStats.ui.ActivateLevelBar (2, currentLevelBar);

						//AP Cost
						combatStats.APCost (flurryAPCost);
					}
					else if(Input.GetKeyDown (KeyCode.R))	//If Lightning
					{
						flurryNextElements[currentLevelBar] = 3;

						//Activate Next Level Bar
						currentLevelBar ++;
						combatStats.ui.ActivateLevelBar (3, currentLevelBar);

						//AP Cost
						combatStats.APCost (flurryAPCost);
					}
					else if(Input.GetKeyDown (KeyCode.W))	//If Water
					{
						flurryNextElements[currentLevelBar] = 4;

						//Activate Next Level Bar
						currentLevelBar ++;
						combatStats.ui.ActivateLevelBar (4, currentLevelBar);

						//AP Cost
						combatStats.APCost (flurryAPCost);
					}
				}

				//Gather Direction input when the player wants to flurry the next enemy etc
				if(Input.GetKeyDown(KeyCode.A))
				{
					//If flurry is a positive number, reset to 0
					if(flurryNextEnemy > 0)
					{
						flurryNextEnemy = 0;
					}

					flurryNextEnemy --; 
				}
				if(Input.GetKeyDown (KeyCode.D))
				{
					//If flurry is a negative number, reset to 0
					if(flurryNextEnemy < 0)
					{
						flurryNextEnemy = 0;
					}

					flurryNextEnemy ++;
				}

				break;
			case 4: //Charging Attack Mode
				if(charging)
				{
					switch(elementalStance)
					{
					case 1:	//if Charging with Earth
						if(Input.GetKey (KeyCode.E))
						{
							//Increase Charge and decrease AP
							float chargeCalculations = Time.deltaTime * chargeSpeed;
							charge += chargeCalculations;
							chargeBarCount += chargeCalculations;
							chargeAPCount += chargeCalculations;

							//Activate Level Bar when Ap cost reached
							if(chargeBarCount >= chargeAPCost)
							{
								chargeCurrentBar++;
								combatStats.ui.ActivateLevelBar (elementalStance, chargeCurrentBar);
								anim.SetInteger ("Index", chargeCurrentBar);	//Next Stance
								chargeBarCount = 0f;
							}

							//Drain AP
							if(chargeAPCount >= 1f)
							{
								combatStats.APCost (1);
								chargeAPCount = 0f;
							}

							combatStats.stat.actionPoints -= (int)chargeCalculations;
							//Charge Slider will equal charge 
							combatStats.ui.chargeSlider.value = (float)(charge / chargeMax);
						}

						//If button released or Charge is over Max charge or if no AP
						if(Input.GetKeyUp (KeyCode.E) ||
						   combatStats.stat.actionPoints <= 0 ||
						   charge >= chargeMax)
						{
							//Activate the Charged Attack
							anim.SetTrigger ("Next");

							//Destroy the Charge Level UI Bar
							combatStats.ui.ClearChargeSlider ();

							//Determine whether in critical zone
							chargeInstantCritical = DetermineChargeCritical ();

							//Turn off Charging
							charging = false;
						}
						break;
					case 2:	//if Charging with Fire
						if(Input.GetKey (KeyCode.Q))
						{
							//Increase Charge and decrease AP
							float chargeCalculations = Time.deltaTime * chargeSpeed;
							charge += chargeCalculations;
							chargeBarCount += chargeCalculations;
							chargeAPCount += chargeCalculations;
							
							//Activate Level Bar when Ap cost reached
							if(chargeBarCount >= chargeAPCost)
							{
								chargeCurrentBar++;
								combatStats.ui.ActivateLevelBar (elementalStance, chargeCurrentBar);
								anim.SetInteger ("Index", chargeCurrentBar);	//Next Stance
								chargeBarCount = 0f;
							}
							
							//Drain AP
							if(chargeAPCount >= 1f)
							{
								combatStats.APCost (1);
								chargeAPCount = 0f;
							}
							
							combatStats.stat.actionPoints -= (int)chargeCalculations;
							//Charge Slider will equal charge 
							combatStats.ui.chargeSlider.value = (float)(charge / chargeMax);
						}

						//If button released or Charge is over Max charge or if no AP
						if(Input.GetKeyUp (KeyCode.Q) ||
						   combatStats.stat.actionPoints <= 0 ||
						   charge >= chargeMax)
						{
							//Activate the Charged Attack
							anim.SetTrigger ("Next");
							
							//Destroy the Charge Level UI Bar
							combatStats.ui.ClearChargeSlider ();

							//Determine whether in critical zone
							chargeInstantCritical = DetermineChargeCritical ();
							
							//Turn off Charging
							charging = false;
						}
						break;
					case 3:	//if Charging with Lightning
						if(Input.GetKey (KeyCode.R))
						{
							//Increase Charge and decrease AP
							float chargeCalculations = Time.deltaTime * chargeSpeed;
							charge += chargeCalculations;
							chargeBarCount += chargeCalculations;
							chargeAPCount += chargeCalculations;
							
							//Activate Level Bar when Ap cost reached
							if(chargeBarCount >= chargeAPCost)
							{
								chargeCurrentBar++;
								combatStats.ui.ActivateLevelBar (elementalStance, chargeCurrentBar);
								anim.SetInteger ("Index", chargeCurrentBar);	//Next Stance
								chargeBarCount = 0f;
							}
							
							//Drain AP
							if(chargeAPCount >= 1f)
							{
								combatStats.APCost (1);
								chargeAPCount = 0f;
							}
							
							combatStats.stat.actionPoints -= (int)chargeCalculations;
							//Charge Slider will equal charge 
							combatStats.ui.chargeSlider.value = (float)(charge / chargeMax);
						}

						//If button released or Charge is over Max charge or if no AP
						if(Input.GetKeyUp (KeyCode.R) ||
						   combatStats.stat.actionPoints <= 0 ||
						   charge >= chargeMax)
						{
							//Activate the Charged Attack
							anim.SetTrigger ("Next");
							
							//Destroy the Charge Level UI Bar
							combatStats.ui.ClearChargeSlider ();

							//Determine whether in critical zone
							chargeInstantCritical = DetermineChargeCritical ();
							
							//Turn off Charging
							charging = false;
						}
						break;
					case 4:	//if Charging with Water
						if(Input.GetKey (KeyCode.W))
						{
							//Increase Charge and decrease AP
							float chargeCalculations = Time.deltaTime * chargeSpeed;
							charge += chargeCalculations;
							chargeBarCount += chargeCalculations;
							chargeAPCount += chargeCalculations;
							
							//Activate Level Bar when Ap cost reached
							if(chargeBarCount >= chargeAPCost)
							{
								chargeCurrentBar++;
								combatStats.ui.ActivateLevelBar (elementalStance, chargeCurrentBar);
								anim.SetInteger ("Index", chargeCurrentBar);	//Next Stance
								chargeBarCount = 0f;
							}
							
							//Drain AP
							if(chargeAPCount >= 1f)
							{
								combatStats.APCost (1);
								chargeAPCount = 0f;
							}
							
							combatStats.stat.actionPoints -= (int)chargeCalculations;
							//Charge Slider will equal charge 
							combatStats.ui.chargeSlider.value = (float)(charge / chargeMax);
						}

						//If button released or Charge is over Max charge or if no AP
						if(Input.GetKeyUp (KeyCode.W) ||
						   combatStats.stat.actionPoints <= 0 ||
						   charge >= chargeMax)
						{
							//Activate the Charged Attack
							anim.SetTrigger ("Next");
							
							//Destroy the Charge Level UI Bar
							combatStats.ui.ClearChargeSlider ();

							//Determine whether in critical zone
							chargeInstantCritical = DetermineChargeCritical ();
							
							//Turn off Charging
							charging = false;
						}
						break;
					}
				}
				break;
			case 5:	//Environment Interaction
				if(!environmentInteract)
				{
					CombatManager.environmentObjects[objectCurrentSelection[2]].SendMessage ("EnvironmentInteract", elementalStance, 
					                                                         SendMessageOptions.DontRequireReceiver);
					//Hide Selection
					HideSelection ();
					EndTurn ();
					environmentInteract = true;
				}
				break;
			case 6:	//Defend Mode
				if(!defendMode)
				{
					//Hide Selection
					HideSelection ();

					if(objectCurrentSelection[0] == combatStats.PlayerIndex ())
					{
						//Calculate Resistance
						float defendRating = 2f;
						
						switch(elementalStance)
						{
						case 1:	//If Earth
							defendRating += combatStats.character.earthAffinity / 100f; 
							print ("Defend with Earth");
							break;
						case 2:	//If Fire
							defendRating += combatStats.character.fireAffinity / 100f; 
							print ("Defend with Fire");
							break;
						case 3:	//If Lightning
							defendRating += combatStats.character.lightningAffinity / 100f; 
							print ("Defend with Lightning");
							break;
						case 4:	//If Water
							defendRating += combatStats.character.waterAffinity / 100f; 
							print ("Defend with Water");
							break;
						}
						
						//Activate and send defend message to the PlayerCombatCharacter Script
						combatStats.SetDefend(elementalStance, defendRating);
						
						//Move into the Defend Animation
						anim.SetInteger ("Attack Phase", attackPhase);
						anim.SetInteger ("Index", elementalStance);
						
						//Restore 30% AP
						combatStats.RegenAP (true, 0.30f);
					}
					else
					{
						//Mark as Overwatch Protected
						//print ("PROVIDING OVERWATCH SIR");

						//Send Message to Overwatch Target
						CombatManager.playerStats[objectCurrentSelection[0]].SetOverwatch (combatStats.PlayerIndex(), 
						                                                                   elementalStance);
						EndTurn ();
					}

					defendMode = true;
				}
				break;
			case 7:	//Support Mode
				if(!supportMode)
				{
					//Hide Selection
					HideSelection ();

					//Move into the Buff Animation
					anim.SetInteger ("Attack Phase", attackPhase);
					anim.SetInteger ("Index", 0);

					supportMode = true;
					print ("Support");
				}
				break;
			case 8:	//Curse Mode
				if(!curseMode)
				{
					//Hide Selection
					HideSelection ();

					//Move into the Debuff Animation
					anim.SetInteger ("Attack Phase", attackPhase);
					anim.SetInteger ("Index", 0);

					curseMode = true;
					print ("Curse");
				}
				break; 
			}

			//Look At Target function 
			if(lookAtTarget)
			{
				//Rotate towards countdown 
				lookAtTargetTimer -= Time.deltaTime; 

				if(lookAtTargetTimer > 0f)
				{
					//Rotate towards target
					Vector3 targetDir = CombatManager.enemies[objectCurrentSelection[1]].transform.position - 
										anim.gameObject.transform.position;
					targetDir.y = 0;
					float step = rotateSpeed * Time.deltaTime;
					Vector3 newDir = Vector3.RotateTowards (anim.gameObject.transform.forward, targetDir, step, 0.0f);
					anim.gameObject.transform.rotation = Quaternion.LookRotation (newDir);
				}
				else
				{
					lookAtTarget = false;
					lookAtTargetTimer = 1f;
				}
			}

			if(lookAtInitialRotation)
			{
				anim.gameObject.transform.rotation = Quaternion.RotateTowards (anim.gameObject.transform.rotation,
				                                                      			transform.rotation, rotateSpeed);


				if(anim.gameObject.transform.localEulerAngles == Vector3.zero)
				{
					lookAtInitialRotation = false;
				}
			}

			//Move to Target Function
			if(moveToTarget)
			{
				//Calculate Distance
				if(CombatManager.enemies[objectCurrentSelection[1]])
				{
					float dist = Vector3.Distance (CombatManager.enemies[objectCurrentSelection[1]].transform.position,
					                               anim.gameObject.transform.position);
					if(dist > targetWidth/2f + 2f)
					{
						//Move Towards target
						float moveStep = moveSpeed * Time.deltaTime;
						anim.gameObject.transform.position = Vector3.MoveTowards (anim.gameObject.transform.position,
						                                                          CombatManager.enemies[objectCurrentSelection[1]].transform.position,
						                                                          moveStep);
					}
					else
					{
						moveToTarget = false;

						//if the move speed was a teleport than reveal self again
						if(moveSpeed >= 30f)
						{
							TeleportEffectToggle(false);
						}
					}
				}
				else
				{
					objectCurrentSelection[1] = 0;
				}
			}

			if(moveToInitialPosition)
			{
				//Move Towards target
				float moveStep = moveSpeed * Time.deltaTime;
				anim.gameObject.transform.position = Vector3.MoveTowards (anim.gameObject.transform.position,
				                                                          transform.position, moveStep);

				if(anim.gameObject.transform.localPosition == Vector3.zero)
				{
					moveToInitialPosition = false;

					//if the move speed was a teleport than reveal self again
					if(moveSpeed >= 30f)
					{
						TeleportEffectToggle(false);
					}
				}
			}
		}

		//If Scan Mode is On
		if(CombatUIManager.scanMode)
		{
			//Delay Input
			if(scanModeDelay <= 0f)
			{
				if(!scanModeActivate)
				{
					//If Left Mouse button is clicked - Submit 
					if(Input.GetMouseButtonUp(0))
					{
						//Send message to Combat UI Manager to retrieve scan values and initiate scan
						combatStats.ui.SubmitScan ();
						//Hide Selection
						HideSelection ();
						//Activate scanModeActivate to prevent input
						scanModeActivate = true;
						//Use the Trigger End to activate the Scan Animation 
						anim.SetInteger("Attack Phase", 0);
						anim.SetInteger ("Index", 0);	//Choose animation
						anim.SetTrigger ("End");
					}
					//If Right mouse button or escape is clicked - exit interface
					if(Input.GetMouseButtonUp (1) || Input.GetKeyUp (KeyCode.Escape))
					{
						//Send message to Combat UI Manager to exit scan mode
						combatStats.ui.ExitScan ();
						//Reset Delay
						scanModeDelay = 0.5f;
						//Show Selection
						ShowSelection();
						//Return to idle
						ReturnToIdle ();
						//Unlock the Action Modes
						actionModeLock = false;
					}
				}
			}
			else
			{
				//Countdown delay
				scanModeDelay -= Time.deltaTime;

				//Hide Selection
				HideSelection ();
			}
		}
	}

	//This function is to fire the single shot projectile, activated by the animator
	public void FireSingleShot(int _projectileSlot)
	{
		//Fire
		if(elementalStance == 2)
		{
			//Calculate damage
			float damage = (((float)combatStats.character.fireAffinity / 100f) + 1f) * combatStats.stat.attack;

			//Spawn Projectile and specify information to projectile
			Transform projectile = Instantiate (singleShotProjectiles[elementalStance-1],
			                                    projectileNode[_projectileSlot-1].position, 
			                                    projectileNode[_projectileSlot-1].rotation) as Transform;
			//Calculate status chance
			float statusChance =  CalculateStatusChance(singleShotStatusChance, combatStats.character.fireAffinity, 2f);
			//Get the targets script component and set target
			CombatProjectile trajectory = projectile.gameObject.GetComponent<CombatProjectile>();
			trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
			                      statusChance, singleShotCritChance);
		}
		
		//Water
		if(elementalStance == 4)
		{
			//Calculate damage
			float damage = (((float)combatStats.character.waterAffinity / 100f) + 1f) * combatStats.stat.attack;
			
			//Spawn Projectile and specify information to projectile
			Transform projectile = Instantiate (singleShotProjectiles[elementalStance-1],
			                                    projectileNode[_projectileSlot-1].position, 
			                                    projectileNode[_projectileSlot-1].rotation) as Transform;
			//Calculate status chance
			float statusChance =  CalculateStatusChance(singleShotStatusChance, combatStats.character.waterAffinity, 2f);
			//Get the targets script component and set target
			CombatProjectile trajectory = projectile.gameObject.GetComponent<CombatProjectile>();
			trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
			                     statusChance, singleShotCritChance);
		}
		
		//Earth 
		if(elementalStance == 1)
		{
			//Calculate damage
			float damage = (((float)combatStats.character.earthAffinity / 100f) + 1f) * combatStats.stat.attack;
			
			//Spawn Projectile and specify information to projectile
			Transform projectile = Instantiate (singleShotProjectiles[elementalStance-1],
			                                    projectileNode[_projectileSlot-1].position, 
			                                    projectileNode[_projectileSlot-1].rotation) as Transform;
			//Calculate status chance
			float statusChance =  CalculateStatusChance(singleShotStatusChance, combatStats.character.earthAffinity, 2f);
			//Get the targets script component and set target
			CombatProjectile trajectory = projectile.gameObject.GetComponent<CombatProjectile>();
			trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
			                      statusChance, singleShotCritChance);
		}
		
		//Lightning
		if(elementalStance == 3)
		{
			//Calculate damage
			float damage = (((float)combatStats.character.lightningAffinity / 100f) + 1f) * combatStats.stat.attack;
			
			//Spawn Projectile and specify information to projectile
			Transform projectile = Instantiate (singleShotProjectiles[elementalStance-1],
			                                    projectileNode[_projectileSlot-1].position, 
			                                    projectileNode[_projectileSlot-1].rotation) as Transform;
			//Calculate status chance
			float statusChance =  CalculateStatusChance(singleShotStatusChance, combatStats.character.lightningAffinity, 2f);
			//Get the targets script component and set target
			CombatProjectile trajectory = projectile.gameObject.GetComponent<CombatProjectile>();
			trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
			                      statusChance, singleShotCritChance);
		}
	}

	public void SetFlurryDamage()
	{
		//Special Note: The accuracy stat will be temporarily increased when using flurry (x2)
		combatStats.stat.accuracy += (int)((float)combatStats.stat.accuracyBase * (float)flurryAccuracy);

		//Calculate damage and status chance
		float damage = 0f;
		float statusChance = 0f;

		//Fire
		if(elementalStance == 2)
		{
			//Calculate damage: for Flurry the damage will be halved
			damage = (((float)combatStats.character.fireAffinity / 100f) + 1f) * (combatStats.stat.attack / 2f);

			//Calculate status chance
			statusChance =  CalculateStatusChance(flurryStatusChance, combatStats.character.fireAffinity, 1f);
		}
		
		//Water
		if(elementalStance == 4)
		{
			//Calculate damage
			damage = (((float)combatStats.character.waterAffinity / 100f) + 1f) * (combatStats.stat.attack / 2f);

			//Calculate status chance
			statusChance =  CalculateStatusChance(flurryStatusChance, combatStats.character.waterAffinity, 1f);
		}
		
		//Earth 
		if(elementalStance == 1)
		{
			//Calculate damage
			damage = (((float)combatStats.character.earthAffinity / 100f) + 1f) * (combatStats.stat.attack / 2f);

			//Calculate status chance
			statusChance =  CalculateStatusChance(flurryStatusChance, combatStats.character.earthAffinity, 1f);
		}
		
		//Lightning
		if(elementalStance == 3)
		{
			//Calculate damage
			damage = (((float)combatStats.character.lightningAffinity / 100f) + 1f) * (combatStats.stat.attack / 2f);

			//Calculate status chance
			statusChance =  CalculateStatusChance(flurryStatusChance, combatStats.character.lightningAffinity, 1f);
		}

		CombatManager.enemyStats[objectCurrentSelection[1]].SetDamage (combatStats.stat, elementalStance, (int) damage,
		                                               statusChance, flurryCritChance);

		//Accuracy is restored
		combatStats.stat.accuracy -= (int)((float)combatStats.stat.accuracyBase * (float)flurryAccuracy);
	}

	public void SetChargeDamage()
	{
		//Special Note: The accuracy stat will be temporarily increased when using charged (x2)
		combatStats.stat.accuracy += (int)((float)combatStats.stat.accuracyBase * (float)chargeAccuracy);

		//Calculate damage and status chance
		float damage = 0f;
		float statusChance = 0f;
		
		//Fire
		if(elementalStance == 2)
		{
			//Calculate damage: for charged, depending on how many charge is multiplied to the damage / 10f 
			damage = (((float)combatStats.character.fireAffinity / 100f) + 1f) * 
				((float)combatStats.stat.attack * (charge / 10f));
			
			//Calculate status chance
			statusChance =  CalculateStatusChance(chargeStatusChance, combatStats.character.fireAffinity, 2f);
		}
		
		//Water
		if(elementalStance == 4)
		{
			//Calculate damage
			damage = (((float)combatStats.character.waterAffinity / 100f) + 1f) * 
				((float)combatStats.stat.attack * (charge / 10f));
			
			//Calculate status chance
			statusChance =  CalculateStatusChance(chargeStatusChance, combatStats.character.waterAffinity, 2f);
		}
		
		//Earth 
		if(elementalStance == 1)
		{
			//Calculate damage
			damage = (((float)combatStats.character.earthAffinity / 100f) + 1f) * 
				((float)combatStats.stat.attack * (charge / 10f));
			
			//Calculate status chance
			statusChance =  CalculateStatusChance(chargeStatusChance, combatStats.character.earthAffinity, 2f);
		}
		
		//Lightning
		if(elementalStance == 3)
		{
			//Calculate damage
			damage = (((float)combatStats.character.lightningAffinity / 100f) + 1f) * 
				((float)combatStats.stat.attack * (charge / 10f));
			
			//Calculate status chance
			statusChance =  CalculateStatusChance(chargeStatusChance, combatStats.character.lightningAffinity, 2f);
		}

		//Decide whether critical hit, during charge the player may let go at the correct moment and score an
		//instant critical
		float criticalHitChance = 0f;

		if(chargeInstantCritical)
		{
			criticalHitChance = 100f; 
			print ("ITS AN INSTANT CRITICAL");
		}
		else
		{
			criticalHitChance = chargeCritChance;
		}
		
		CombatManager.enemyStats[objectCurrentSelection[1]].SetDamage (combatStats.stat, elementalStance, (int) damage,
		                                               statusChance, criticalHitChance);

		//Accuracy is restored
		combatStats.stat.accuracy -= (int)((float)combatStats.stat.accuracyBase * (float)chargeAccuracy);
	}

	public void SetDebuff()
	{
		//Special Note: The accuracy stat will be temporarily increased when using debuff (x2)
		combatStats.stat.accuracy += (int)((float)combatStats.stat.accuracyBase * 2f);
		
		//Calculate damage and status chance
		float damage = 0f;

		float statusChance = curseStatusChance;	//Very High chance of inflicting status chance
		
		//Fire
		if(elementalStance == 2)
		{
			//Calculate damage: for Cursing the damage will be put down by 4 times
			damage = (((float)combatStats.character.fireAffinity / 100f) + 1f) * (combatStats.stat.attack / 4f);
		}
		
		//Water
		if(elementalStance == 4)
		{
			//Calculate damage
			damage = (((float)combatStats.character.waterAffinity / 100f) + 1f) * (combatStats.stat.attack / 4f);
		}
		
		//Earth 
		if(elementalStance == 1)
		{
			//Calculate damage
			damage = (((float)combatStats.character.earthAffinity / 100f) + 1f) * (combatStats.stat.attack / 4f);
		}
		
		//Lightning
		if(elementalStance == 3)
		{
			//Calculate damage
			damage = (((float)combatStats.character.lightningAffinity / 100f) + 1f) * (combatStats.stat.attack / 4f);
			//The status chance of stunning should be lower and should be at least 10% more than single shots chance
			statusChance = curseStatusChance - 0.1f;	
		}
		
		CombatManager.enemyStats[objectCurrentSelection[1]].SetDamage (combatStats.stat, elementalStance, (int) damage,
		                                                               statusChance, 0f);	//no criticals
		
		//Accuracy is restored
		combatStats.stat.accuracy -= (int)((float)combatStats.stat.accuracyBase * 2f);
	}

	public void SetBuff()
	{
		//Depending on Element Buff target character 

		//Status Durations is the magic number 3
		switch(elementalStance)
		{
		case 1:	//If Earth
			CombatManager.playerStats[objectCurrentSelection[0]].SetAura (3);
			break;
		case 2:	//If Fire
			CombatManager.playerStats[objectCurrentSelection[0]].SetBlazingSpirit (3);
			break;
		case 3:	//If Lightning
			CombatManager.playerStats[objectCurrentSelection[0]].SetOvercharged (3);
			break;
		case 4:	//If Water
			CombatManager.playerStats[objectCurrentSelection[0]].SetPurify (3);
			break;
		}
	}

	float CalculateStatusChance(float _chance, float _affinityLevel, float _multiplier)
	{
		float statusChance;
		statusChance = ((_chance * (_affinityLevel * _multiplier)) / 100f) + _chance;
		return statusChance;
	}

	//Special Note: Using anim.gameObject.transform to move the animation, not this object this script is attached

	//This function is to teleport next to the enemy into melee range
	public void MoveToTarget(float _moveSpeed)
	{
		moveSpeed = _moveSpeed;

		//Calculate Target Location
		CapsuleCollider targetMeasurement = CombatManager.enemies[objectCurrentSelection[1]].GetComponent<CapsuleCollider>();
		targetWidth = targetMeasurement.radius;

		moveToInitialPosition = false;
		moveToTarget = true;

		//Determine if teleport by using move speed, if over the speed of light then yes.... its a teleport
		if(moveSpeed >= 30f)
		{
			TeleportEffectToggle(true);
		}
	}

	//This function is to Look at Enemy, by activating a boolean and the update will rotate player
	public void LookAtTarget(float _rotateSpeed)
	{
		rotateSpeed = _rotateSpeed;
		lookAtTargetTimer = 1f;
		lookAtInitialRotation = false;
		lookAtTarget = true;
	}

	//These functions restores initial rotation and transform
	public void MoveToInitialPosition(float _moveSpeed)
	{
		moveSpeed = _moveSpeed;

		moveToTarget = false;
		moveToInitialPosition = true;

		//Determine if teleport by using move speed, if over the speed of light then yes.... its a teleport
		if(moveSpeed >= 30f)
		{
			TeleportEffectToggle(true);
		}
	}

	public void LookAtInitialRotation(float _rotateSpeed)
	{
		rotateSpeed = _rotateSpeed;

		lookAtTarget = false;
		lookAtInitialRotation = true;
	}

	//This function prepares the Next Flurry Attack during the Flurry Attack Mode. Called in animation clip
	public void ReadyForNextFlurry()
	{
		flurryReady = true;
		flurryTimer = flurryMaxTimer;
		//print ("Next Flurry");
	}

	//This function Calculates whether the character needs to teleport to the next enemy during a Flurry
	void DetermineFlurryTeleport()
	{
		bool requireTeleport = false;	//Determines whether to teleport at the end of this function

		//selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
		if(flurryNextEnemy > 0) //if Positive
		{
			flurryNextEnemy --;
			objectCurrentSelection[1]++; 	
			requireTeleport = true;
		}

		if(flurryNextEnemy < 0)	//if negative
		{
			flurryNextEnemy ++;
			objectCurrentSelection[1]--;
			requireTeleport = true;
		}

		//Ensure Selection is not over or lesser than size of list		
		if(objectCurrentSelection[1] >= CombatManager.enemies.Count)
		{
			objectCurrentSelection[1] = 0;
		}	
		if(objectCurrentSelection[1] < 0)
		{
			objectCurrentSelection[1] = CombatManager.enemies.Count -1;
		}

		//Extra Measures: If there is one enemy left than ignore teleport to enemy request
		if(CombatManager.enemies.Count == 1)
		{
			requireTeleport = false;
		}

		//Check Distance in case enemy dies and new enemy needs to be teleported to

		//Calculate Distance
		if(CombatManager.enemies[objectCurrentSelection[1]])
		{
			float dist = Vector3.Distance (CombatManager.enemies[objectCurrentSelection[1]].transform.position,
			                               anim.gameObject.transform.position);
			if(dist > targetWidth/2f + 2f)
			{
				requireTeleport = true;
			}
			else
			{
				requireTeleport = false;
			}
		}
		else
		{
			objectCurrentSelection[1] = 0;
			requireTeleport = true;
		}

		if(requireTeleport)
		{
			//Teleport to enemy
			MoveToTarget (teleportDashSpeed);
			LookAtTarget (teleportRotateSpeed);
		}
	}

	//These functions calculates the size limit of the level bars
	void SetLevelBarSizes()
	{
		levelBarLimit[0] = GetLevelBarSize(combatStats.character.level);			//Determine Normal Level Bar
		levelBarLimit[1] = GetLevelBarSize(combatStats.character.earthAffinity);	//Determine Earth Level Bar
		levelBarLimit[2] = GetLevelBarSize(combatStats.character.fireAffinity);		//Determine Fire Level Bar
		levelBarLimit[3] = GetLevelBarSize(combatStats.character.lightningAffinity);//Determine Lightning Level Bar
		levelBarLimit[4] = GetLevelBarSize(combatStats.character.waterAffinity);	//Determine Water Level Bar
	}

	int GetLevelBarSize(int _level)
	{
		//Determine how many bars are needed. Extra Note: The max 
		if(_level >= 0)
		{
			return 3;
		}
		else if (_level >= 5)
		{
			return 6;
		}
		else if (_level >= 10)
		{
			return 9;
		}
		else if (_level >= 15)
		{
			return 12;
		}
		else if (_level >= 20)
		{
			return 15;
		}
		
		return 0;
	}

	//This function is called to determine if it is possible to initiate the flurry attack mode
	bool CanFlurry()
	{
		//Decide Whether its possible to move into the flurry Mode
		if(combatStats.character.level >= flurryLevelUnlock &&
		   combatStats.stat.actionPoints >= flurryAPCost &&
		   levelBarLimit[0] > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//This function is called to determine if it is possible to initiate the charging attack mode
	bool CanCharge()
	{
		//Decide whether its possible to move into the charge attack mode
		if(combatStats.character.level >= chargeLevelUnlock &&
		   combatStats.stat.actionPoints >= 1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	bool DetermineChargeCritical()
	{
		bool instantCritical = false;

		float chargePercentage = (float)((float)charge / (float)chargeMax);

		float[] criticalZones = combatStats.ui.chargeCriticalZone;

		for(int i = 0; i < criticalZones.Length ; i++)
		{
			if(criticalZones[i] < chargePercentage + 0.02f &&
			   criticalZones[i] > chargePercentage - 0.02f)
			{
				instantCritical = true;
				break;
			}
			else
			{
				instantCritical = false;
			}
		}

		return instantCritical;
	}

	//This procedure returns the attack phase back to idle
	void ReturnToIdle()
	{
		//Cannot go into flurry, move back to idle
		attackPhase = 0;
		anim.SetInteger ("Attack Phase", 0);
		ShowSelection ();
		flurryActivate = false;
		readyTimer = readyMaxTimer;
		idleDelay = 1f;
	}

	void HideSelection()
	{
		isSelecting = false;
		selector.gameObject.SetActive (false);
		combatStats.ui.ActiveUISwitch (false);
		actionModeLock = true;
	}

	void ShowSelection ()
	{
		isSelecting = true;
		selector.gameObject.SetActive (true);
		combatStats.ui.ActiveUISwitch (true);
		actionModeLock = false;
	}

	void TeleportEffectToggle(bool _reveal)
	{
		if(_reveal)
		{
			//Hide renderers and reveal effects
			for(int i = 0; i< teleportHideObjects.Length; i++)
			{
				teleportHideObjects[i].SetActive (false);
			}
			for(int i = 0; i < teleportHideRenderers.Length; i++)
			{
				teleportHideRenderers[i].enabled = false;
			}
			for(int i = 0; i < teleportRevealEffects.Length; i++)
			{
				teleportRevealEffects[i].SetActive (true);
			}
		}
		else
		{
			//Hide renderers and reveal effects
			for(int i = 0; i< teleportHideObjects.Length; i++)
			{
				teleportHideObjects[i].SetActive (true);
			}
			for(int i = 0; i < teleportHideRenderers.Length; i++)
			{
				teleportHideRenderers[i].enabled = true;
			}
			for(int i = 0; i < teleportRevealEffects.Length; i++)
			{
				teleportRevealEffects[i].SetActive (false);
			}
		}
	}

	//This function is to Switch to the next Action Mode
	void SwitchToNextMode()
	{
		if(!actionModeLock)
		{
			actionMode ++; //Switch to the next Mode

			if(actionMode > 3)
			{
				actionMode = 0;
			}

			UpdateObjectSelection ();
		}
	}

	void SwitchToMode(int _mode)
	{
		if(!actionModeLock)
		{
			actionMode = _mode;
		}
	}

	void UpdateObjectSelection()
	{
		//Ensure Selections aren't out of Range
		//Player
		if(objectCurrentSelection[0] >= CombatManager.players.Count)
		{
			objectCurrentSelection[0] = 0;
		}
		//Enemy
		if(objectCurrentSelection[1] >= CombatManager.enemies.Count)
		{
			objectCurrentSelection[1] = 0;
		}
		//Object Interaction
		if(objectCurrentSelection[2] >= CombatManager.environmentObjects.Count)
		{
			objectCurrentSelection[2] = 0;
		}

		switch(actionMode)
		{
		case 0:	//If Attack Mode
			ShowSelection ();
			objectSelection = 1;
			selector.gameObject.SetActive (true);
			isSelecting = true;
			selector.position = CombatManager.enemies[objectCurrentSelection[objectSelection]].transform.position;
			print ("Switched to Attack");
			break;
		case 1:	//If Defend Mode
			ShowSelection ();
			objectSelection = 0;
			selector.gameObject.SetActive (true);
			isSelecting = true;
			selector.position = CombatManager.players[objectCurrentSelection[objectSelection]].transform.position;
			print ("Switched to Defend");
			break;
		case 2:	//If Support Mode
			ShowSelection ();
			objectSelection = 0;
			selector.gameObject.SetActive (true);
			isSelecting = true;
			selector.position = CombatManager.players[objectCurrentSelection[objectSelection]].transform.position;
			print ("Switched to Support");
			break;
		case 3:	//If Curse Mode
			ShowSelection ();
			objectSelection = 1;
			selector.gameObject.SetActive (true);
			isSelecting = true;
			selector.position = CombatManager.enemies[objectCurrentSelection[objectSelection]].transform.position;
			print ("Switched to Curse");
			break;
		}
	}

	//This function ends the turn with a delay
	public void EndTurnDelay(float _time)
	{
		Invoke ("EndTurn", _time);
	}
	
	//This function will end the players turn
	public void EndTurn()
	{
		//If Defending then don't change back to idle
		if(!combatStats.defend)
		{
			anim.SetInteger ("Attack Phase", 0);
		}
		//Update Status Effects
		combatStats.UpdateStatusEffects ();
		//Hide Selection
		HideSelection ();
		//Send Message to Combat Manager to start next turn
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("NextTurn", SendMessageOptions.DontRequireReceiver);
		//Turn off this script
		this.enabled = false; 

		//As this will be the Main Player Script De Activate the scan button
		combatStats.ui.ActiveUISwitch (false);	//Turn off UI elements
		combatStats.statusUI.SetCurrent (false);
	}
}
