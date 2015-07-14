using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This script handles the party member status information

public class PartyMemberStatus : MonoBehaviour 
{
	public Text playerName;	//Name
	public Transform healthNode;
	public Text playerHealth; //Health Text
	public Transform shieldNode;
	public Text playerShield; //Shield Text
	public Text playerActionPoints; //Action Points

	public Slider healthSlider;
	public Slider shieldSlider;

	//Lerping Variables
	private bool lerpHealth = false;
	private float lerpCurrHealth;
	private float lerpCurrHealthBar;

	private bool lerpShield = false;
	private float lerpCurrShield;
	private float lerpCurrShieldBar;

	private bool lerpActionPoints = false;
	private float lerpCurrActionPoints;

	//Current Health, AP and Shield
	private int health; 
	private int maxHealth;

	private int shield;
	private int maxShield;

	private int AP;
	private int maxAP;

	//Public Speeds
	private float gaugeLerpSpeed = 0.1f;
	private float textLerpSpeed = 1;

	public float dampen = 0.2f;

	private PlayerCombatCharacter playerStat;

	[HideInInspector]
	public bool isCurrent = false;
	public CanvasGroup currentPanel;
	private bool currentPanelFadeIn = false;
	private bool currentPanelFadeOut = false;

	// Use this for initialization
	void Start () 
	{
		currentPanel.alpha = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(lerpHealth)
		{
			float healthPercentage = (float)health/ (float)maxHealth;

			if(healthSlider.value == 0f)
			{
				healthSlider.value = (float)health/ (float)maxHealth;
			}

			gaugeLerpSpeed = Mathf.SmoothStep (healthPercentage, lerpCurrHealthBar, Time.time);
			textLerpSpeed = Mathf.SmoothStep (health, lerpCurrHealth, Time.time);
			
			if(lerpCurrHealth > health)
			{
				lerpCurrHealth -= (textLerpSpeed * Time.deltaTime) * dampen;
			}
			else if(lerpCurrShield < health)
			{
				lerpCurrHealth += (textLerpSpeed * Time.deltaTime) * dampen;
			}
			
			
			if(lerpCurrHealthBar > healthPercentage)
			{
				lerpCurrHealthBar -= gaugeLerpSpeed * Time.deltaTime * dampen;
			}
			else if (lerpCurrHealthBar < healthPercentage)
			{
				lerpCurrHealthBar += gaugeLerpSpeed * Time.deltaTime * dampen;
			}

			if((int)lerpCurrHealth == (int)health)
			{
				lerpHealth = false;
			}

			healthSlider.value = Mathf.SmoothStep (healthPercentage, lerpCurrHealthBar, Time.time);
			playerHealth.text = (int)lerpCurrHealth +"/"+maxHealth; 
		}

		if(lerpShield)
		{
			float shieldPercentage = (float)shield/ (float)maxShield;

			gaugeLerpSpeed = Mathf.SmoothStep (shieldPercentage, lerpCurrShieldBar, Time.time);
			textLerpSpeed = Mathf.SmoothStep (shield, lerpCurrShield, Time.time);

			if(lerpCurrShield > shield)
			{
				lerpCurrShield -= (textLerpSpeed * Time.deltaTime * dampen);
			}
			else if(lerpCurrShield < shield)
			{
				lerpCurrShield += (textLerpSpeed * Time.deltaTime * dampen);
			}

			if(lerpCurrShieldBar > shieldPercentage)
			{
				lerpCurrShieldBar -= gaugeLerpSpeed * Time.deltaTime * dampen;
			}
			else if (lerpCurrShieldBar < shieldPercentage)
			{
				lerpCurrShieldBar += gaugeLerpSpeed * Time.deltaTime * dampen;
			}

			if((int)lerpCurrShield == (int)shield)
			{
				lerpShield = false;
			}

			if(shield <= 0)
			{
				lerpCurrShield = 0;
				lerpCurrShieldBar = 0f;
				shield = 0;
				shieldSlider.value = 0f;
				playerShield.text = "0/"+maxShield;

				lerpShield = false;
				lerpHealth = true;

				shieldNode.gameObject.SetActive (false);
				healthNode.gameObject.SetActive (true);
				shieldSlider.gameObject.SetActive (false);

				playerHealth.text = health +"/"+maxHealth;
				healthSlider.value = (float)health/ (float)maxHealth;
			}


			shieldSlider.value = Mathf.SmoothStep (shieldPercentage, lerpCurrShieldBar, Time.time);
			playerShield.text = (int)lerpCurrShield +"/"+maxShield; 
		}


		if(lerpActionPoints)
		{
			float APSpeed = Mathf.SmoothStep (AP, lerpCurrActionPoints, Time.time);

			if(lerpCurrActionPoints > AP)
			{
				lerpCurrActionPoints -= (APSpeed * Time.deltaTime * dampen);
			}
			else if(lerpCurrActionPoints < AP)
			{
				lerpCurrActionPoints += (APSpeed * Time.deltaTime * dampen);
			}

			if(APSpeed <= 0f)
			{
				lerpCurrActionPoints = AP;
				lerpActionPoints = false;
			}

			if((int)lerpCurrActionPoints == AP)
			{
				lerpCurrActionPoints = AP;
				lerpActionPoints = false;
			}

			if(AP == 0)
			{
				lerpCurrActionPoints = AP;
			}

			playerActionPoints.text = (int)lerpCurrActionPoints +"/"+maxAP;
		}

		if(health <= 0)
		{
			health = 0;

			lerpCurrHealth = 0;
			lerpCurrHealthBar = 0f;

			playerHealth.text = "0/"+maxHealth;
			healthSlider.value = 0f;

			//healthSlider.gameObject.SetActive (false);
		}

		if(health > 0)
		{
			if(healthSlider.value == 0f)
			{
				healthSlider.value = (float)health/ (float)maxHealth;
			}
		}

		if(shield <= 0)
		{
			lerpCurrShield = 0;
			lerpCurrShieldBar = 0f;
			shield = 0;
			shieldSlider.value = 0f;
			playerShield.text = "0/"+maxShield;
		}

		//Update Health, AP and Shield 

		//Health 
		CombatUIManager.currentPlayerHealth = healthSlider.value;

		//AP 
		float percentageAP = (float)lerpCurrActionPoints / (float)maxAP;
		CombatUIManager.currentPlayerAP = percentageAP;

		//Shield
		CombatUIManager.currentPlayerShield = shieldSlider.value;

		//Text - health, shield and ap
		if(shield > 0)
		{
			CombatUIManager.currentPlayerHealthText = playerShield.text;
		}
		else
		{
			CombatUIManager.currentPlayerHealthText = playerHealth.text;
		}

		CombatUIManager.currentPlayerAPText = playerActionPoints.text;

		if(currentPanelFadeIn)
		{
			currentPanel.alpha += Time.deltaTime;
			if(currentPanel.alpha >= 1f)
			{
				currentPanelFadeIn = false;
			}
		}

		if(currentPanelFadeOut)
		{
			currentPanel.alpha -= Time.deltaTime;
			if(currentPanel.alpha <= 0f)
			{
				currentPanelFadeOut = false;
			}
		}
	}

	public void InitialiseStats()
	{
		playerName.text = playerStat.character.name;

		//Initiate Lerp from variables
		lerpCurrHealth = playerStat.stat.health;
		lerpCurrHealthBar = playerStat.stat.health/ playerStat.stat.healthMax;
		
		lerpCurrShield = playerStat.stat.shield;
		lerpCurrShieldBar = 1f;
		
		lerpCurrActionPoints = playerStat.stat.actionPoints;

		health = playerStat.stat.health;
		maxHealth = playerStat.stat.healthMax;

		shield = playerStat.stat.shield;
		maxShield = playerStat.stat.shieldMax;
		
		AP = playerStat.stat.actionPoints;
		maxAP = playerStat.stat.actionPointMax;

		healthSlider.value = (float)playerStat.stat.health/ (float)playerStat.stat.healthMax;
		playerHealth.text = playerStat.stat.health +"/"+playerStat.stat.healthMax; 
		healthNode.gameObject.SetActive (false);

		shieldSlider.value = (float)playerStat.stat.shield/ (float)playerStat.stat.shieldMax;
		playerShield.text = playerStat.stat.shield +"/"+playerStat.stat.shieldMax; 

		playerActionPoints.text = playerStat.stat.actionPoints +"/"+playerStat.stat.actionPointMax;
	}

	public void UpdateStatHealth()
	{
		shield = playerStat.stat.shield;
		health = playerStat.stat.health;

		if(shield > 0)
		{
			lerpShield = true;
		}
		else
		{
			lerpShield = false;
			lerpHealth = true;
			
			lerpCurrShield = 0;
			lerpCurrShieldBar = 0f;
			
			shieldNode.gameObject.SetActive (false);
			healthNode.gameObject.SetActive (true);
			shieldSlider.gameObject.SetActive (false);

			if(healthSlider.value == 0f)
			{
				healthSlider.value = (float)health/ (float)maxHealth;
			}
		}

		//print (playerName.text + " Shield is at = " + shield+ " Health is at = " + health);
	}

	public void UpdateStatActionPoints()
	{
		AP = playerStat.stat.actionPoints;
		lerpActionPoints = true;
	}

	public void SetPlayerStat(GameObject _player)
	{
		playerStat = _player.GetComponent<PlayerCombatCharacter>();
	}

	public void SetCurrent(bool _currentPlayer)
	{
		isCurrent = _currentPlayer;

		if(_currentPlayer)
		{
			currentPanelFadeIn = true;
		}
		else
		{
			currentPanelFadeOut = true;
		}
	}
}
