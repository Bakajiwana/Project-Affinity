using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This script is going to update the UI screen

public class CombatUIManager : MonoBehaviour 
{
	//Player Party Layout
	public Transform partyLayoutPanel;
	public Transform partyMemberPanel;

	//Current Player UI
	public Image currHealthBar;
	public Image currAPBar;
	public Image currShieldBar;
	public Text currHealthText;
	public Text currAPText;

	public static float currentPlayerHealth;
	public static float currentPlayerAP;
	public static float currentPlayerShield;
	public static string currentPlayerHealthText;
	public static string currentPlayerAPText;

	private PartyMemberStatus[] memberStatus;

	//Level Bars
	public Transform gaugeOrganiser;

	public Transform levelBar;

	private Transform[] currentBars;
	private LevelBar[] bar;

	//Charge Slider
	public Transform chargeSliderManager;
	public Transform chargeSliderHolder;
	private Transform chargeSliderObject;
	[HideInInspector]
	public Slider chargeSlider;
	[HideInInspector]
	public float[] chargeCriticalZone = new float[3];

	//Canvas Combat UI fades
	private bool combatScreenFadeIn = false;
	public float fade = 0.5f;

	public CanvasGroup scanButton;

	//When its the first players turn the alphas will become active or not depending on turn
	private float activeAlpha = 0f;
	private bool activeLayoutFadeIn = false;
	private bool activeLayoutFadeOut = false;

	//UI End Screens
	public Transform endScreenNode;
	public Transform winScreen;
	public Transform gameOverScreen;

	public CanvasGroup combatScreen;

	//Special Screen
	public CanvasGroup specialScreen;
	private bool specialParadox = false;

	//Scan Values
	[HideInInspector]
	public int[] scanValues;

	//Scan Button Press
	public static bool scanMode = false;

	public ScanScreen scanScreen;

	void Start ()
	{
		//Make sure combatScreen alpha is off
		combatScreen.alpha = 0f;
		activeAlpha = 0f;
		scanButton.alpha = 0f;

		scanMode = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currHealthBar.fillAmount = currentPlayerHealth;
		currShieldBar.fillAmount = currentPlayerShield;
		currAPBar.fillAmount = currentPlayerAP;
		currHealthText.text = currentPlayerHealthText;
		currAPText.text = currentPlayerAPText;

		//Combat Screen Fade In start of battle
		if(combatScreenFadeIn)
		{
			combatScreen.alpha += Time.deltaTime * fade;
			
			if(combatScreen.alpha >= 1f)
			{
				combatScreenFadeIn = false;
			}
		}		
		
		if(activeLayoutFadeIn)
		{
			//activeLayoutFadeOut = false; //Overwrite fade out

			activeAlpha += Time.deltaTime * fade;

			//Affected Alphas
			scanButton.alpha = activeAlpha;
			
			if(activeAlpha >= 1f)
			{
				activeLayoutFadeIn = false;
			}
		}
		
		if(activeLayoutFadeOut)
		{
			activeLayoutFadeIn = false; //Overwrite fade in

			activeAlpha -= Time.deltaTime * fade;

			//Affected Alphas
			scanButton.alpha = activeAlpha;
			
			if(activeAlpha <= 0f)
			{
				activeLayoutFadeOut = false;
			}
		}

		if(specialParadox && CombatManager.combatState == 0)
		{
			if(Input.GetMouseButtonDown (0))
			{
				specialParadox = false;

				combatScreen.alpha = 1f;

				specialScreen.alpha = 0f;

				GameObject.FindGameObjectWithTag("Combat Manager").SendMessage ("LifeParadox", SendMessageOptions.DontRequireReceiver);
			}
			else if (Input.GetMouseButtonDown (1))
			{
				specialParadox = false;

				combatScreen.alpha = 1f;
				
				specialScreen.alpha = 0f;

				GameObject.FindGameObjectWithTag("Combat Manager").SendMessage ("DeathParadox", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	void InitialisePartyPanel()
	{
		memberStatus = new PartyMemberStatus[CombatManager.players.Count];
		
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			Transform panel = Instantiate (partyMemberPanel, partyLayoutPanel.position, partyLayoutPanel.rotation) as Transform;
			panel.SetParent(partyLayoutPanel, true);
			
			memberStatus[i] = panel.gameObject.GetComponent<PartyMemberStatus>();

			//Initialise the UI and player together <3

			CombatManager.playerStats[i].SetPartyUI (memberStatus[i]);

			memberStatus[i].InitialiseStats();

			//The combat screen should appear
			combatScreenFadeIn = true;
		}
	}

	//This function creates bars depending on [0] elements and [1] how many bars to spawn
	public void CreateLevelBars(int _bars)
	{
		//Create new size of array 
		currentBars = new Transform[_bars]; 
		bar = new LevelBar[_bars]; 

		//For every bar that needs to be spawned
		for(int i = 0; i < _bars; i++)
		{
			//spawn bar
			currentBars[i] = Instantiate (levelBar, gaugeOrganiser.position, gaugeOrganiser.rotation) as Transform;
			//Parent Bar to UI Bar Organiser
			currentBars[i].SetParent (gaugeOrganiser, true);
			//Obtain Level Bar Script
			bar[i] = currentBars[i].gameObject.GetComponent<LevelBar>(); 
		}
	}

	public void ActivateLevelBar(int _element, int _index)
	{
		_index--; //Set to array readability
	
		bar[_index].RevealBar (_element);	//Reveal specified bar
	}

	public void ClearLevelBars()
	{
		//Destroy UI objects
		for(int i = 0; i < currentBars.Length; i++)
		{
			Destroy (currentBars[i].gameObject);
		}
	}

	void SetElementSlider(GameObject _slider)
	{
		chargeSlider = _slider.GetComponent<Slider>();
	}

	void SetCriticalZones(float[] _critZone)
	{
		chargeCriticalZone = _critZone; 
	}

	public void CreateChargeSlider(int _element)
	{
		chargeSliderObject = Instantiate (chargeSliderManager, 
		                                  chargeSliderHolder.position, 
		                                  chargeSliderHolder.rotation) as Transform;
		chargeSliderObject.SetParent (chargeSliderHolder, true);
		chargeSliderObject.gameObject.SendMessage ("ActivateChargeSlider", _element, SendMessageOptions.DontRequireReceiver);
	}

	public void ClearChargeSlider()
	{
		Destroy (chargeSliderObject.gameObject);
		ClearLevelBars();
	}

	void SwitchToLoseScreen()
	{
		combatScreen.alpha = 0f;
		specialScreen.alpha = 0f;
		Transform screen = Instantiate (gameOverScreen, 
		                                endScreenNode.position, 
		                                endScreenNode.rotation) as Transform;
		screen.SetParent (endScreenNode, true);
	}

	void SwitchToWinScreen()
	{
		combatScreen.alpha = 0f;
		specialScreen.alpha = 0f;
		Transform screen = Instantiate (winScreen, 
		                                endScreenNode.position, 
		                                endScreenNode.rotation) as Transform;
		screen.SetParent (endScreenNode, true);
	}

	public void ActiveUISwitch(bool _active)
	{
		if(_active)
		{
			activeLayoutFadeIn = true;

			CombatInteractionSwitch(true);
		}
		else
		{
			activeLayoutFadeOut = true;

			CombatInteractionSwitch(false);
		}
	}

	public void CombatInteractionSwitch(bool _switch)
	{
		if(_switch)
		{
			//Turn on Interactivity
			combatScreen.blocksRaycasts = true;
			scanButton.blocksRaycasts = true;
		}
		else
		{
			//Turn off Interactivity
			combatScreen.blocksRaycasts = false;
			scanButton.blocksRaycasts = false;
		}
	}

	//The scan button uses this function to change scan mode to true
	public void ScanModeSwitch(bool _switch)
	{
		if(_switch)
		{
			scanMode = true;
			ActiveUISwitch(false);
		}
		else
		{
			scanMode = false;
			ActiveUISwitch (true);
		}
	}

	//This function is to activate the scan and store the values
	void SetScanValues(int[] _values)
	{
		scanValues = _values;
	}

	//This function is called from the player action script when the scan mode is on
	public void SubmitScan()
	{
		//This function will activate the scan:

		//Sending a message to Scan screen to send its scan values
		//Top, Bottom, Right, Left
		scanScreen.SubmitScanValues(); //The Player Action Script will access the scanValues values.

		//Turning off the Scan screen
		scanScreen.RevealScanScreen (false);

		ScanModeSwitch (false);
	}

	public void ExitScan()
	{
		//Exit the scan mode
		ScanModeSwitch (false);
		scanScreen.RevealScanScreen (false);
	}

	public void ActivateSpecialParadoxScreen()
	{
		if(CombatManager.combatState == 0)
		{
			specialScreen.alpha = 1f;
			combatScreen.alpha = 1f;
			specialParadox = true;
		}
	}
}
