using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyCombatCharacter : MonoBehaviour 
{
	//Enemy Stats
	public CombatStat stat;
	[HideInInspector]
	public EnemyElementalReaction elementalReaction;

	public int shieldAffinity;
	public int shield;
	public int healthStrength; //Multiplyer that will be specific to different classes or uniqueness of stat
	public int actionPointStrength;
	public int attackStrength;
	public int defenceStrength;
	public int agilityStrength;
	public int luckStrength;
	public int accuracyStrength;
	public int speedStrength;

	[HideInInspector] //Had to make this public so Action script can use this.
	public int affinity; //0-none, 1 - Earth, 2 - Fire, 3 - Lightning, 4 - Water
	private int affinityWeakness;
	private int affinityResistance; 

	private int shieldWeakness; 
	private int shieldResistance;

	public GameObject textCanvas;
	public GameObject damageText;

	//If Hit by a projectile
	private bool hitByProjectile = false;
	private GameObject incomingProjectile;

	//Critical Integrity
	public float criticalChanceIntegrity = 1f;
	public float criticalHitIntegrity = 2f;

	//Status Effects - Debuffs
	[HideInInspector]
	public bool statusCondemned = false;
	private int statusCondemnedLength;
	
	[HideInInspector]
	public bool statusBurning = false;
	private int statusBurningLength;
	
	[HideInInspector]
	public bool statusStunned = false;
	private int statusStunnedLength;
	
	[HideInInspector]
	public bool statusRusted = false;
	private int statusRustedLength;
	
	//Status Effects - Buffs
	[HideInInspector]
	public bool statusAura = false;
	private int statusAuraLength;
	
	[HideInInspector]
	public bool statusBlazingSpirit = false;
	private int statusBlazingSpiritLength;
	
	[HideInInspector]
	public bool statusOvercharged = false;
	private int statusOverchargedLength;
	
	[HideInInspector]
	public bool statusPurify = false;
	private int statusPurifyLength;

	public int statusWeaknessDuration = 4;
	public int statusStandardDuration = 3;
	public int statusResistedDuration = 2;
	public int statusStrengthDuration = 1;

	public Transform[] statusParticles;

	private Transform condemnedParticles;
	private Transform burningParticles;
	private Transform stunnedParticles;
	private Transform rustedParticles;
	private Transform auraParticles;
	private Transform blazingSpiritParticles;
	private Transform overchargedParticles;
	private Transform purifyParticles; 

	//Integrity = The amount of which the weakness is revealed
	//As a percentage
	public int healthIntegrity = 5; //Soaked in damage
	private bool affinityRevealed = false;

	public int shieldIntegrity = 5;
	private bool shieldRuptured = false;

	//If affinity is revealed then reveal their affinity material
	public Material[] shieldAffinityMaterials;
	public GameObject[] shieldAffinitySwitchObjects;
	public Transform[] shieldHiddenReveals; 
	public Transform[] shieldHideObjects;

	public Material[] healthAffinityMaterials;
	public GameObject[] healthAffinitySwitchObjects;
	public Transform[] healthHiddenReveals; 
	public Transform[] healthHideObjects;



	// Use this for initialization
	void Start () 
	{
		//Enemies begin with a Random Affinity
		affinity = Random.Range (1, 4);

		//Describe the weakness and strength of this enemy's affinity
		switch (affinity)
		{
		case 1: //The affinity is Earth
			affinityWeakness = 2; //Weakness is Fire
			affinityResistance = 3; //Resistant to Lightning
			break;
		case 2: //The affinity is Fire
			affinityWeakness = 4; //Weakness is water
			affinityResistance = 1; //Resistant to Earth
			break;
		case 3: //The affinity is lightning
			affinityWeakness = 1; //Weakness is Earth
			affinityResistance = 4; //Resistant to water
			break;
		case 4:	//The affinity is Water
			affinityWeakness = 3; //Weakness is Lightning
			affinityResistance = 2; //Resistant to Fire
			break;
		}

		if(shield > 0) //If this enemy has a shield
		{
			//Calculate its weakness and strength
			switch(shieldAffinity)
			{
			case 1: //The affinity is Earth
				shieldWeakness = 2; //Weakness is Fire
				shieldResistance = 3; //Resistant to Lightning
				break;
			case 2: //The affinity is Fire
				shieldWeakness = 4; //Weakness is water
				shieldResistance = 1; //Resistant to Earth
				break;
			case 3: //The affinity is lightning
				shieldWeakness = 1; //Weakness is Earth
				shieldResistance = 4; //Resistant to water
				break;
			case 4:	//The affinity is Water
				shieldWeakness = 3; //Weakness is Lightning
				shieldResistance = 2; //Resistant to Fire
				break;
			}
		}

		for(int i = 0; i < healthAffinitySwitchObjects.Length; i++)
		{
			healthAffinitySwitchObjects[i].GetComponent<Renderer>().material = healthAffinityMaterials[0];
		}

		for(int i = 0; i < shieldAffinitySwitchObjects.Length; i++)
		{
			shieldAffinitySwitchObjects[i].GetComponent<Renderer>().material = shieldAffinityMaterials[0];
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	//Obtain stat information from spawn, this is done by recieving level information from the spawner
	public void InitiateEnemyStats(int _level)
	{		
		//Construct stats
		stat = new CombatStat(_level, shieldAffinity, shield, healthStrength, actionPointStrength, 
		                      attackStrength, defenceStrength, agilityStrength,
		                      luckStrength, accuracyStrength, speedStrength);

		//Calculate Integrity
		shieldIntegrity = (int)((float)shield * (float)((float)shieldIntegrity/100f));
		healthIntegrity = (int)((float)stat.health * (float)((float)healthIntegrity/100f));

		//Obtain the Elemental Reaction Component
		elementalReaction = gameObject.GetComponent<EnemyElementalReaction>();
		
		//print (character.levelMaxExperience);
		//print (stat.health);
		
	}

	//This function is to show damage text and take a string, colour and whether or not it was a critical
	public void ShowDamageText(string _text, Color _textColor, float _textSizeMultiply)
	{
		GameObject textDamage = Instantiate (damageText, textCanvas.transform.position, transform.rotation) as GameObject;
		textDamage.transform.SetParent (textCanvas.transform, false);

		Text textObject = textDamage.GetComponent<Text>();

		textObject.text = _text;
		textObject.color = _textColor;

		Vector3 textSizeMultiply = new Vector3(_textSizeMultiply,_textSizeMultiply,_textSizeMultiply);
		textObject.gameObject.transform.localScale = Vector3.Scale (textObject.gameObject.transform.localScale, 
																	textSizeMultiply);
	}

	public void SetDamage(CombatStat _opposingStat, int _damageType, int _damage, 
	                      float _statusEffectChance, float _criticalChance)
	{
		int element = _damageType;
		Color textColour = Color.white;
		float textSize = 1f;

		/*
		switch(_damageType)
		{
		case 1:
			textColour = Color.green;
			break;
		case 2:
			textColour = Color.red;
			break;
		case 3:
			textColour = Color.yellow;
			break;
		case 4:
			textColour = Color.blue;
			break;
		}
		*/


		// Calculate if dodged attack
		bool dodged = false;
		float evasionChance = 0f;

		//print ("Opposing accuracy = " + _opposingStat.accuracy);
		//print ("Agility = " + stat.agility);
		evasionChance = ((float)stat.agility / (float)_opposingStat.accuracy) * 100f;
		int randomEvasionChance = Random.Range (0, 100);
		//print ("Random Evasion Chance = " + randomEvasionChance);
		evasionChance = Random.Range (0,(int)evasionChance);
		//print ("Evasion Chance = " + evasionChance);
		if(randomEvasionChance < evasionChance)
		{
			dodged = true;

			//If Projectile 
			if(hitByProjectile && incomingProjectile)
			{
				incomingProjectile.SendMessage ("TargetMiss",SendMessageOptions.DontRequireReceiver);
				hitByProjectile = false;
			}
		}
		else
		{
			dodged = false;

			//If Projectile 
			if(hitByProjectile && incomingProjectile)
			{
				incomingProjectile.SendMessage ("TargetHit",SendMessageOptions.DontRequireReceiver);
				hitByProjectile = false;
			}

			//Send Message to Elemental Reaction Script to Activate Elemental Reaction
			if(element > 0)
			{
				elementalReaction.ActivateElementalEffect (element);
			}
		}

		if(dodged)
		{
			//Dodge attack
			ShowDamageText ("Miss", textColour, textSize);
		}
		else //Did not dodge attack, then calculate damages
		{
			int criticalHitChance = (int)((_criticalChance * (float)_opposingStat.luck) * criticalChanceIntegrity);
			//print (criticalHitChance);
			//Damage Calculations - If its a critical hit
			if(Random.Range (0,100) <= criticalHitChance)
			{
				//Then critical hit
				_damage = (int)((float)_damage * (float)criticalHitIntegrity);
				textSize = 2f;
				GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject,SendMessageOptions.DontRequireReceiver);
				//print ("CRITICAL HIT");
			}


			//No Debuff can be afflicted when shield is up
			if(shield > 0) //if shield is still active
			{
				if(shieldWeakness == element)	//If weakness
				{
					if(shieldRuptured)
					{
						_damage *= 4; //quadruple damage
						textSize += 0.5f;
					}
					else
					{
						_damage *= 2; //Double damage
					}
					shield -= _damage;	
					ShowDamageText (_damage.ToString (), textColour, textSize + 0.25f);
				}
				else if(shieldResistance == element)	//if resistance
				{
					_damage /= 2;
					shield -= _damage;	//Half damage
					ShowDamageText (_damage.ToString (), textColour, textSize - 0.25f);
				}
				else if (shieldAffinity == element)	//Shield is the same as damage, convert to health
				{
					_damage = -_damage;
					shield -= _damage;
					ShowDamageText ((-_damage).ToString (), textColour, textSize - 0.25f);
				}
				else
				{
					//Deduct the amount of damage to shield
					shield -= _damage;
					ShowDamageText (_damage.ToString (), textColour, textSize - 0.25f);
				}

				if(stat.shield < 0)
				{
					stat.shield = 0;
					//Hide shield objects
					for (int i = 0; i < healthHiddenReveals.Length; i++)
					{
						shieldHiddenReveals[i].gameObject.SetActive (false);
					}
				}
				else
				{
					if(!shieldRuptured)
					{
						shieldIntegrity -= _damage;
						if(shieldIntegrity <= 0)
						{
							shieldRuptured = true;
							RevealShieldAffinity ();
							GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject,SendMessageOptions.DontRequireReceiver);
						}
					}
				}
				if(stat.shield > stat.shieldMax)
				{
					stat.shield = stat.shieldMax;
				}
			}
			else //Damage will go to health
			{
				//Status Effect Formula: Attack StatusChance minus by enemy weak-strength.
				float statusEffectChance = 0f;
				int statusEffectVerdict = Random.Range (0, 100);
				int maxStatusDuration = 1;
				//Status effect chance should only have a limit
				if(_statusEffectChance > 2f)
				{
					_statusEffectChance = 2f;
				}

				//Defence is a resistance factor when shield is down
				_damage -= (int)((float)stat.defence/2f); 
				if(_damage <= 0)
				{
					_damage = 1;	//Make sure damage doesn't fall into a negative
				}

				if(affinityWeakness == element)
				{
					if(affinityRevealed)
					{
						_damage *= 4; //quadruple damage
						textSize += 0.5f;
					}
					else
					{
						_damage *= 2; //Double damage
					}
					stat.health -= _damage;
					ShowDamageText (_damage.ToString (), textColour,  textSize + 0.25f);
					statusEffectChance = _statusEffectChance * (((float)Random.Range (100,150))/100f);	//Calcualte Status Effect Chance
					maxStatusDuration = statusWeaknessDuration;	//Calculate a random max duration
				}
				else if (affinityResistance == element)
				{
					_damage /= 2;
					stat.health -= _damage;
					ShowDamageText (_damage.ToString (), textColour,  textSize + 0.25f);
					statusEffectChance = _statusEffectChance * (((float)Random.Range (50,100))/100f);	//Calcualte Status Effect Chance
					maxStatusDuration = statusResistedDuration;	//Calculate a random max duration
				}
				else if (affinity == element)
				{
					_damage = -_damage;
					stat.health -=  -_damage;
					ShowDamageText ("+" + (-_damage).ToString (), textColour,  textSize + 0.25f);
					statusEffectChance = _statusEffectChance * (((float)Random.Range (0,50))/100f);	//Calcualte Status Effect Chance
					maxStatusDuration = statusStrengthDuration;	//Calculate a random max duration
				}
				else
				{
					stat.health -= _damage;
					ShowDamageText (_damage.ToString (), textColour,  textSize + 0.25f);
					statusEffectChance = _statusEffectChance;	//Calcualte Status Effect Chance
					maxStatusDuration = statusStandardDuration;	//Calculate a random max duration
				}

				//-------------------If Rusted-----------------
				if(statusRusted)
				{
					statusEffectChance *= 1.5f; 
				}
				//---------------------------------------------

				//Calculate whether a debuff has been afflicted
				if(statusEffectVerdict <= (int)(statusEffectChance * 100f))
				{
					maxStatusDuration = Random.Range (1, maxStatusDuration);
					switch(element)
					{
					case 1:	//Earth Status Effect
						//Condemned: Decrease Defence and Accuracy. Prevents Healing
						//print ("I am condemned");
						SetCondemned (maxStatusDuration);
						break;
					case 2:	//Fire Status Effect
						//Burning: Fire Damage over time. Decrease attack.
						//print ("I am burning");
						SetBurning (maxStatusDuration);
						break;
					case 3:	//Lightning Status Effect
						//Stun: Disables moves
						//print ("I am stunned");
						SetStunned (maxStatusDuration);
						break;
					case 4:	//Water Status Effect
						//Rust: Increase debuff chance. Also decreases speed and agility
						//print ("I am rusted");
						SetRusted (maxStatusDuration);
						break;
					}
				}

				//Health management - if health reaches 0 - Player dies
				if(stat.health <= 0)
				{
					stat.health = 0;
					
					EnemyDown ();
				}
				else
				{
					if(!affinityRevealed)
					{
						healthIntegrity -= _damage;
						if(healthIntegrity <= 0)
						{
							RevealHealthAffinity ();
							affinityRevealed = true;
							GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject,SendMessageOptions.DontRequireReceiver);
						}
					}
				}

				if(stat.health > stat.healthMax)
				{
					stat.health = stat.healthMax;
				}
			}
			//print ("I took " + _damage +" damage....Ouch");
		}
	}

	//These are status effect setter functions
	public void SetCondemned(int _statusDuration)
	{
		if(!statusCondemned)
		{
			statusCondemned = true;
			statusCondemnedLength = _statusDuration;

			condemnedParticles = Instantiate (statusParticles[0],
			                                      transform.position,
			                                      transform.rotation) as Transform;
			condemnedParticles.SetParent (transform, true);

			//Implement Stat Debuff = Decrease Defence and Accuracy
			stat.defence -= (int)((float)stat.defenceBase * 0.25f);
			stat.accuracy -= (int)((float)stat.accuracyBase * 0.25f);
		}
		else
		{
			if(statusCondemnedLength < _statusDuration)
			{
				statusCondemnedLength = _statusDuration;
			}
		}
	}

	public void SetBurning(int _statusDuration)
	{
		if(!statusBurning)
		{
			statusBurning = true;
			statusBurningLength = _statusDuration;
			
			burningParticles = Instantiate (statusParticles[1],
			                                      transform.position,
			                                      transform.rotation) as Transform;
			burningParticles.SetParent (transform, true);

			//Implement stat debuff = decrease attack
			stat.attack -= (int)((float)stat.attackBase * 0.25f);
		}
		else
		{
			if(statusBurningLength < _statusDuration)
			{
				statusBurningLength = _statusDuration;
			}
		}
	}

	public void SetStunned(int _statusDuration)
	{
		if(!statusStunned)
		{
			statusStunned = true;
			statusStunnedLength = _statusDuration;
			
			stunnedParticles = Instantiate (statusParticles[2],
			                                      transform.position,
			                                      transform.rotation) as Transform;
			stunnedParticles.SetParent (transform, true);
		}
		//Stunned does not have a reset duration effect.
	}

	public void SetRusted(int _statusDuration)
	{
		if(!statusRusted)
		{
			statusRusted = true;
			statusRustedLength = _statusDuration;
			
			rustedParticles = Instantiate (statusParticles[3],
			                                      transform.position,
			                                      transform.rotation) as Transform;
			rustedParticles.SetParent (transform, true);

			//Implement Stat Debuff = Decrease Attack, speed and agility
			stat.speed -= (int)((float)stat.speedBase * 0.25f);
			stat.agility -= (int)((float)stat.agilityBase * 0.25f);
		}
		else
		{
			if(statusRustedLength < _statusDuration)
			{
				statusRustedLength = _statusDuration;
			}
		}
	}

	//BUFF Setters
	public void SetAura(int _statusDuration)
	{
		if(!statusAura)
		{
			statusAura = true;
			statusAuraLength = _statusDuration;
			
			auraParticles = Instantiate (statusParticles[4],
			                             transform.position,
			                             transform.rotation) as Transform;
			auraParticles.SetParent (transform, true);
			
			//Implement stat buff = Heals AP overtime. Increase Defence and Accuracy.
			stat.defence += (int)((float)stat.defenceBase * 0.5f);
			stat.accuracy += (int)((float)stat.accuracyBase * 0.5f);
		}
		else
		{
			if(statusAuraLength < _statusDuration)
			{
				statusAuraLength = _statusDuration;
			}
		}
	}
	
	public void SetBlazingSpirit(int _statusDuration)
	{
		if(!statusBlazingSpirit)
		{
			statusBlazingSpirit = true;
			statusBlazingSpiritLength = _statusDuration;
			
			blazingSpiritParticles = Instantiate (statusParticles[5],
			                                      transform.position,
			                                      transform.rotation) as Transform;
			blazingSpiritParticles.SetParent (transform, true);
			
			//Implement stat buff = Increase attack
			stat.attack += (int)((float)stat.attackBase * 0.5f);
		}
		else
		{
			if(statusBlazingSpiritLength < _statusDuration)
			{
				statusBlazingSpiritLength = _statusDuration;
			}
		}
	}
	
	public void SetOvercharged(int _statusDuration)
	{
		if(!statusOvercharged)
		{
			statusOvercharged = true;
			statusOverchargedLength = _statusDuration;
			
			overchargedParticles = Instantiate (statusParticles[6],
			                                    transform.position,
			                                    transform.rotation) as Transform;
			overchargedParticles.SetParent (transform, true);
			
			//Implement stat buff = Increase speed and agility
			stat.agility += (int)((float)stat.agilityBase * 0.5f);
			stat.speed += (int)((float)stat.speedBase * 0.5f);
		}
		else
		{
			if(statusOverchargedLength < _statusDuration)
			{
				statusOverchargedLength = _statusDuration;
			}
		}
	}
	
	public void SetPurify(int _statusDuration)
	{
		//This status buff will work like a reset
		statusPurify = false;
		statusPurifyLength = 0;
		
		purifyParticles = Instantiate (statusParticles[7],
		                               transform.position,
		                               transform.rotation) as Transform;
		purifyParticles.SetParent (transform, true);
		
		//Implement stat buff = Negate Status Ailments, heals HP overtime
		//To turn off debuffs, the duration of the debuff needs to be set to 0
		statusCondemnedLength = 0;
		statusBurningLength = 0;
		statusStunnedLength = 0;
		statusRustedLength = 0;
		statusAuraLength += 1;
		statusBlazingSpiritLength += 1;
		statusOverchargedLength += 1;
		UpdateStatusEffects ();
		
		//Moved this before the UpdateStatusEffects function is called
		//Reset loop, this is so when the updatestatus is called, this won't heal immediately...
		statusPurify = true;
		statusPurifyLength = _statusDuration;
		
		purifyParticles = Instantiate (statusParticles[7],
		                               transform.position,
		                               transform.rotation) as Transform;
		purifyParticles.SetParent (transform, true);
	}

	//Status Effect updates when character ends turn this function will be called
	public void UpdateStatusEffects()
	{
		//Update Elemental Effects
		elementalReaction.UpdateElementalEffects ();

		//Condemned Update
		if(statusCondemnedLength > 0)
		{
			statusCondemnedLength--;
		}
		else
		{
			statusCondemned = false;
			if(condemnedParticles)
			{
				//Restore Stats
				stat.defence += (int)((float)stat.defenceBase * 0.25f);
				stat.accuracy += (int)((float)stat.accuracyBase * 0.25f);
				Destroy(condemnedParticles.gameObject);
			}
		}

		//Burning Update
		if(statusBurningLength > 0)
		{
			statusBurningLength--;

			if(statusBurning)
			{
				//Inflict Burn Damage Over time
				//Burn damage is 10% of base health every turn
				float burnDamage = stat.healthBase * 0.10f; 
				//If weakness is fire than doubles burn damage
				if(affinityWeakness == 2) //2 = fire
				{
					burnDamage *= 2f;
					burnDamage = (int)burnDamage;
					stat.health -= (int)burnDamage;	//Take damage
					ShowDamageText (burnDamage.ToString (), Color.red, 0.75f); //show damage text
				}
				//If resistant to fire than half burn damage
				else if(affinityResistance == 2)
				{
					burnDamage /= 2f; 
					burnDamage = (int)burnDamage;
					stat.health -= (int)burnDamage;
					ShowDamageText (burnDamage.ToString (), Color.red, 0.75f); //show damage text
				}
				//If the same element as fire than gain health
				else if(affinity == 2)
				{
					burnDamage = (int)burnDamage;
					stat.health += (int)burnDamage;
					ShowDamageText ("+" + burnDamage.ToString (), Color.red, 0.75f); //show damage text
				}
				else
				{
					burnDamage = (int)burnDamage;
					stat.health -= (int)burnDamage;
					ShowDamageText (burnDamage.ToString (), Color.red, 0.75f); //show damage text
				}

				//Health management - if health reaches 0 - Player dies
				if(stat.health <= 0)
				{
					stat.health = 0;
					
					EnemyDown ();
				}
			}
		}
		else
		{
			statusBurning = false;
			if(burningParticles)
			{
				//Restore Stats
				stat.attack += (int)((float)stat.attackBase * 0.25f);
				Destroy (burningParticles.gameObject);
			}
		}

		//Stunned Update
		if(statusStunnedLength > 0)
		{
			statusStunnedLength--;
		}
		else
		{
			statusStunned = false;
			if(stunnedParticles)
			{
				Destroy (stunnedParticles.gameObject);
			}
		}

		//Rusted Update
		if(statusRustedLength > 0)
		{
			statusRustedLength--;
		}
		else
		{
			statusRusted = false;
			if(rustedParticles)
			{
				//Restore Stats
				stat.speed += (int)((float)stat.speedBase * 0.25f);
				stat.agility += (int)((float)stat.agilityBase * 0.25f);
				Destroy (rustedParticles.gameObject);
			}
		}

		//Aura Update - Heals AP over time and increase defence and accuracy
		if(statusAuraLength > 0)
		{
			statusAuraLength--;
			
			if(statusAura)
			{
				//Regenerate AP over time
				//Regen is 20% of base AP every turn
				RegenAP (true, 0.20f);
				//Calculate
				int regen = (int)((float)stat.actionPointBase * 0.20f);
				ShowDamageText (regen.ToString (), Color.green, 0.75f); //show damage text
			}
		}
		else
		{
			statusAura = false;
			if(auraParticles)
			{
				//Restore Stats
				stat.defence -= (int)((float)stat.defenceBase * 0.5f);
				stat.accuracy -= (int)((float)stat.accuracyBase * 0.5f);
				Destroy (auraParticles.gameObject); //destroy this particle effect
			}
		}
		
		//Blazing Spirit Update - Increases Attack
		if(statusBlazingSpiritLength > 0)
		{
			statusBlazingSpiritLength--;
		}
		else
		{
			statusBlazingSpirit = false;
			if(blazingSpiritParticles)
			{
				//Restore Stats
				stat.attack -= (int)((float)stat.attackBase * 0.5f);
				Destroy (blazingSpiritParticles.gameObject); //destroy this particle effect
			}
		}
		
		//Overcharged Update - Increases Speed and agility
		if(statusOverchargedLength > 0)
		{
			statusOverchargedLength--;
		}
		else
		{
			statusOvercharged = false;
			if(overchargedParticles)
			{
				//Restore Stats
				stat.speed -= (int)((float)stat.speedBase * 0.5f);
				stat.agility -= (int)((float)stat.agilityBase * 0.5f);
				Destroy (overchargedParticles.gameObject); //destroy this particle effect
			}
		}
		
		//Purify Update - Heals HP over time and Negates status ailments (which doesn't apply to updates)
		if(statusPurifyLength > 0)
		{
			statusPurifyLength--;
			
			if(statusPurify)
			{
				//Heal HP over time
				//Heal is 20% of base health every turn
				SetHeal (true, 0.20f);
				//Calculate
				//int heal = (int)((float)stat.healthBase * 0.20f);
				//ShowDamageText (heal.ToString (), Color.blue, 0.75f); //show damage text
			}
		}
		else
		{
			statusPurify = false;
			if(purifyParticles)
			{
				Destroy (purifyParticles.gameObject); //destroy this particle effect
			}
		}
	}

	//This Function is called to heal
	public void SetHeal(bool _percentage, float _healAmount)
	{
		if(!statusCondemned)
		{
			if(_percentage)
			{
				int heal = (int)((float)stat.healthBase * _healAmount);
				stat.health += heal;
				ShowDamageText ("+" + heal.ToString (),Color.white, 1f);
			}
			else
			{
				stat.health += (int)_healAmount;
				ShowDamageText ("+" + _healAmount.ToString (),Color.white, 1f);
			}

			if(stat.health > stat.healthMax)
			{
				stat.health = stat.healthMax;
			}
		}
		else
		{
			//Cannot heal if condemned
			ShowDamageText ("0", Color.white, 0.5f);
		}
	}

	//This function is called to regenerate AP
	public void RegenAP(bool _percentage, float _regenAmount)
	{
		if(_percentage)
		{
			int regen = (int)((float)stat.actionPointBase * _regenAmount);
			stat.actionPoints += regen;
			ShowDamageText ("+" + regen.ToString (),Color.white, 1f);
		}
		else
		{
			stat.actionPoints += (int)_regenAmount;
			ShowDamageText ("+" + _regenAmount.ToString (),Color.white, 1f);
		}
		
		if(stat.actionPoints > stat.actionPointMax)
		{
			stat.actionPoints = stat.actionPointMax;
		}
	}

	//This function is called for AP use
	public void APCost (int _cost)
	{
		stat.actionPoints -= _cost;
	}

	//This function is used to calculate the index of this player
	public int EnemyIndex()
	{
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			if(CombatManager.enemies[i] == this.gameObject)
			{
				return i;
			}
		}

		return 0;
	}

	public void HitByProjectile(GameObject _projectile)
	{
		hitByProjectile = true;
		incomingProjectile = _projectile;
	}

	void EnemyDown()
	{
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("RemoveEnemy", EnemyIndex(), SendMessageOptions.DontRequireReceiver);
		
		//Clean up Status effects
		statusAuraLength = 0;
		statusPurifyLength = 0;
		statusBlazingSpiritLength = 0;
		statusOverchargedLength = 0;
		statusCondemnedLength = 0;
		statusBurningLength = 0;
		statusStunnedLength = 0;
		statusRustedLength = 0;
		UpdateStatusEffects ();

		gameObject.SetActive (false); //Make this object inactive temporarily just in case
	}

	//This function is called when Affinity is revealed
	public void RevealShieldAffinity()
	{
		for(int i = 0; i< shieldAffinitySwitchObjects.Length; i++)
		{
			shieldAffinitySwitchObjects[i].GetComponent<Renderer>().material = shieldAffinityMaterials[shieldAffinity];
		}
		
		//Reveal hidden objects
		for (int i = 0; i < healthHiddenReveals.Length; i++)
		{
			shieldHiddenReveals[i].gameObject.SetActive (true);
		}
		
		//Hide Objects
		for(int i = 0; i < healthHideObjects.Length; i++)
		{
			shieldHideObjects[i].gameObject.SetActive(false);
		}
	}


	//This function is called when Affinity is revealed
	public void RevealHealthAffinity()
	{
		for(int i = 0; i< healthAffinitySwitchObjects.Length; i++)
		{
			healthAffinitySwitchObjects[i].GetComponent<Renderer>().material = healthAffinityMaterials[affinity];
		}

		//Reveal hidden objects
		for (int i = 0; i < healthHiddenReveals.Length; i++)
		{
			healthHiddenReveals[i].gameObject.SetActive (true);
		}

		//Hide Objects
		for(int i = 0; i < healthHideObjects.Length; i++)
		{
			healthHideObjects[i].gameObject.SetActive(false);
		}
	}
}
