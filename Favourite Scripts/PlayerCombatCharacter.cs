using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCombatCharacter : MonoBehaviour 
{	
	public Character character; 
	public CombatStat stat;
	[HideInInspector]
	public PlayerElementalReaction elementalReaction;

	public Animator anim;

	public int healthStrength; //Multiplyer that will be specific to different classes or uniqueness of stat
	public int actionPointStrength;
	public int attackStrength;
	public int defenceStrength;
	public int agilityStrength;
	public int luckStrength;
	public int accuracyStrength;
	public int speedStrength;

	private int shieldWeakness;
	private int shieldResistance;

	//Text Canvas for Information Output
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
	
	public Transform[] statusParticles;

	//Debuff Particles
	private Transform condemnedParticles;
	private Transform burningParticles;
	private Transform stunnedParticles;
	private Transform rustedParticles;

	//Buff Particles
	private Transform auraParticles;
	private Transform blazingSpiritParticles;
	private Transform overchargedParticles;
	private Transform purifyParticles; 

	//Defend Mode
	[HideInInspector]
	public bool defend;
	private float defendResistance;
	private int defendElement;

	//Overwatch 
	[HideInInspector]
	public Transform overwatchTarget;
	private float overwatchTeleportTimer;
	private float overwatchTeleportMaxTimer = 0.5f;
	private bool overwatchCall = false;

	//Overwatch Protected
	[HideInInspector]
	public bool overwatchProtected = false;
	private int overwatchIndex;
	private int overwatchElement;
	private bool overwatchReturn = false;

	//Access UI
	[HideInInspector]
	public CombatUIManager ui;
	[HideInInspector]
	public PartyMemberStatus statusUI;


	// Use this for initialization
	void Start () 
	{
		//Initialise Combat UI Manager
		ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();

		if(stat.shieldMax > 0) //If this has a shield
		{
			//Calculate its weakness and strength
			switch(stat.shieldAffinity)
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
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If recieved message to protect an ally
		if(overwatchCall)
		{
			if(overwatchTeleportTimer > 0f)
			{
				overwatchTeleportTimer -= Time.deltaTime;

				//Teleport to ally
				float step = 80f * Time.deltaTime;
				anim.transform.position = Vector3.MoveTowards (anim.transform.position, 
				                                          overwatchTarget.position, step);
			}
			else
			{
				//Check for enemies nearby
				if(CheckForNearbyEnemy() != null)
				{
					//If there is an enemy nearby go into a counter attack
					anim.SetTrigger ("Counter");

					//print ("Counter");

					//Counterattack damage
					CounterAttack (overwatchElement);
				}
				else
				{
					//print ("block");

					//If there is no enemy nearby block animation
					anim.SetTrigger ("Block");
				}
				//print ("Check");
				overwatchCall = false;
			}
		}

		if(overwatchReturn)
		{
			if(overwatchTeleportTimer > 0f)
			{
				overwatchTeleportTimer -= Time.deltaTime;

				//Teleport to initial position
				float step = 80f * Time.deltaTime;
				anim.gameObject.transform.position = Vector3.MoveTowards (anim.gameObject.transform.position,
				                                                          transform.position, step);
			}
			else
			{
				overwatchReturn = false;
			}
		}
	}
	
	//Obtain stat information from spawn, this is done by recieving level information from the spawner
	public void InitiatePlayerStats(Character _character)
	{
		//This character now defined with its character stats
		character = _character;

		//Construct stats
		stat = new CombatStat(character.level, character.currentShieldAffinity,
		                      character.currentShield, healthStrength, actionPointStrength, 
		                      attackStrength, defenceStrength, agilityStrength,
		                      luckStrength, accuracyStrength, speedStrength);

		//Make sure player character health is at current health 
		stat.health = character.currentHealth;

		if(stat.health > stat.healthMax)
		{
			stat.health = stat.healthMax;
		}

		//Obtain the Elemental Reaction Component
		elementalReaction = gameObject.GetComponent<PlayerElementalReaction>();

		//print (character.levelMaxExperience);
		//print (stat.health);
		//print (stat.shield);
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

			//Play Dodge Animation
			anim.SetTrigger ("Dodge");
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
			
		//If Not overwatch protected
		if(!overwatchProtected)
		{
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
					//print ("CRITICAL HIT");
					GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject, SendMessageOptions.DontRequireReceiver);
				}
				
				
				//No Debuff can be afflicted when shield is up
				if(stat.shield > 0) //if shield is still active
				{
					if(shieldWeakness == element)	//If weakness
					{
						_damage *= 2; //Double damage
						stat.shield -= _damage;	
						textSize += 0.25f;
					}
					else if(shieldResistance == element)	//if resistance
					{
						_damage /= 2;
						stat.shield -= _damage;	//Half damage
						textSize -= 0.25f;
					}
					else if (stat.shieldAffinity == element)	//Shield is the same as damage, convert to health
					{
						_damage /= 4;
						stat.shield -= _damage;
						textSize -= 0.25f;
					}
					else
					{
						//Deduct the amount of damage to shield
						stat.shield -= _damage;
						textSize -= 0.25f;
					}

					if(stat.shield > stat.shieldMax)
					{
						stat.shield = stat.shieldMax;
					}

					if(stat.shield < 0)
					{
						stat.shield = 0;
					}

					//If Defending
					if(defend)
					{
						if(element == defendElement)
						{
							_damage /= (int)(defendResistance * 2f); 
						}
						else
						{
							_damage /= (int)defendResistance; 
						}

						//Check for enemies nearby
						if(CheckForNearbyEnemy() != null)
						{
							//If there is an enemy nearby go into a counter attack
							anim.SetTrigger ("Counter");

							//Counterattack damage
							CounterAttack (defendElement);
						}
						else
						{
							//If there is no enemy nearby block animation
							anim.SetTrigger ("Block");
						}
					}

					//Show text damage
					ShowDamageText (_damage.ToString (), textColour, textSize);
				}
				else //Damage will go to health
				{
					//Status Effect Formula: Attack StatusChance minus by enemy weak-strength.
					float statusEffectChance = 0f;
					int statusEffectVerdict = Random.Range (0, 100);
					int maxStatusDuration = 3;
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
					
					//Calculate damage
					stat.health -= _damage;

					//If Defending
					if(defend)
					{
						if(element == defendElement)
						{
							_damage /= (int)(defendResistance * 2f); 
						}
						else
						{
							_damage /= (int)defendResistance; 
						}

						//Check for enemies nearby
						if(CheckForNearbyEnemy() != null)
						{
							//If there is an enemy nearby go into a counter attack
							anim.SetTrigger ("Counter");
							
							//Counterattack damage
							CounterAttack (defendElement);
						}
						else
						{
							//If there is no enemy nearby block animation
							anim.SetTrigger ("Block");
						}
					}

					//Show Damage Text
					ShowDamageText (_damage.ToString (), textColour, 0.75f);

					//Calcualte Status Effect Chance
					statusEffectChance = _statusEffectChance;	

					
					//-------------------If Rusted-----------------
					if(statusRusted)
					{
						statusEffectChance *= 1.5f; 
					}
					//---------------------------------------------
					
					//Calculate whether a debuff has been afflicted and if not defending
					if(statusEffectVerdict <= (int)(statusEffectChance * 100f) && !defend)
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
					if(stat.health > stat.healthMax)
					{
						stat.health = stat.healthMax;
					}

					if(stat.health <= 0)
					{
						stat.health = 0;

						statusUI.UpdateStatHealth();	//Update health UI

						PlayerDown ();
					}
				}
				//print ("I took " + _damage +" damage....Ouch");
				if(stat.health > 0)
				{
					statusUI.UpdateStatHealth();	//Update health UI
				}
			}
		}
		else
		{
			//Call overwatch to protect
			CombatManager.playerStats[overwatchIndex].TeleportToProtect (gameObject);

			//Overwatch dodge animation
			anim.SetTrigger ("Next");

			//Calculate Damage output here
			float reducedDamage = 2f;

			if(_damageType == overwatchElement)
			{
				reducedDamage = 4f;
			}

			switch (overwatchElement)
			{
			case 1:	//Earth
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.earthAffinity / 100f;
				break;
			case 2:	//Fire
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.fireAffinity / 100f;
				break;
			case 3:	//Lightning
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.lightningAffinity / 100f;
				break;
			case 4:	//Water
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.waterAffinity / 100f;
				break;
			}

			_damage /= (int)reducedDamage; 

			//Send Damage Output to Overwatch Volunteer
			CombatManager.playerStats[overwatchIndex].SetDamage (_opposingStat, _damageType, _damage, 0f, 0f);

			//No Longer overwatched
			overwatchProtected = false;
		}
	}
	
	//These are status effect setter functions
	//DEBUFF setters
	public void SetCondemned(int _statusDuration)
	{
		if(!statusCondemned)
		{
			statusCondemned = true;
			statusCondemnedLength = _statusDuration;
			
			condemnedParticles = Instantiate (statusParticles[0],
			                                  anim.transform.position,
			                                  anim.transform.rotation) as Transform;
			condemnedParticles.SetParent (anim.transform, true);
			
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
			                                anim.transform.position,
			                                anim.transform.rotation) as Transform;
			burningParticles.SetParent (anim.transform, true);
			
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
			                                anim.transform.position,
			                                anim.transform.rotation) as Transform;
			stunnedParticles.SetParent (anim.transform, true);
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
			                               anim.transform.position,
			                               anim.transform.rotation) as Transform;
			rustedParticles.SetParent (anim.transform, true);
			
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
			                             anim.transform.position,
			                             anim.transform.rotation) as Transform;
			auraParticles.SetParent (anim.transform, true);

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
			                             anim.transform.position,
			                             anim.transform.rotation) as Transform;
			blazingSpiritParticles.SetParent (anim.transform, true);
			
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
			                                      anim.transform.position,
			                                      anim.transform.rotation) as Transform;
			overchargedParticles.SetParent (anim.transform, true);
			
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
		                               anim.transform.position,
		                               anim.transform.rotation) as Transform;
		purifyParticles.SetParent (anim.transform, true);
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
				int burnDamage = (int)((float)stat.healthBase * 0.10f); 
				//Fire Damage Inflict Calculations
				stat.health -= burnDamage;
				ShowDamageText (burnDamage.ToString (), Color.red, 0.75f); //show damage text

				//Health management - if health reaches 0 - Player dies
				if(stat.health <= 0)
				{
					stat.health = 0;
					statusUI.UpdateStatHealth();	//Update health UI
					PlayerDown ();
				}
				else
				{
					statusUI.UpdateStatHealth();	//Update health UI
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

			statusUI.UpdateStatHealth();	//Update health UI
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
			ShowDamageText ("+" + regen.ToString (),Color.cyan, 0.5f);
		}
		else
		{
			stat.actionPoints += (int)_regenAmount;
			ShowDamageText ("+" + _regenAmount.ToString (),Color.cyan, 0.5f);
		}

		if(stat.actionPoints > stat.actionPointMax)
		{
			stat.actionPoints = stat.actionPointMax;
		}

		statusUI.UpdateStatActionPoints();	//Update AP UI
	}

	//This function is called for AP use
	public void APCost (int _cost)
	{
		stat.actionPoints -= _cost;
		statusUI.UpdateStatActionPoints();	//Update AP UI
	}

	//This function is used to calculate the index of this player
	public int PlayerIndex()
	{
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			if(CombatManager.players[i] == this.gameObject)
			{
				return i;
			}
		}

		return 0;
	}

	public void SetPartyUI(PartyMemberStatus _partyMemberStatus)
	{
		statusUI = _partyMemberStatus; 
		statusUI.SetPlayerStat (this.gameObject);
	}

	//This function is called to set up defend mode
	public void SetDefend(int _defendElement, float _defendRate)
	{
		defend = true;
		defendResistance = _defendRate; 
		defendElement = _defendElement;
	}

	public void HitByProjectile(GameObject _projectile)
	{
		hitByProjectile = true;
		incomingProjectile = _projectile;
	}

	public void TeleportToProtect(GameObject _target)
	{
		overwatchTarget = _target.transform;
		overwatchCall = true;
		overwatchTeleportTimer = overwatchTeleportMaxTimer;
	}

	public void TeleportBackToPosition()
	{
		overwatchReturn = true;
		overwatchCall = false;
		overwatchTeleportTimer = overwatchTeleportMaxTimer;
	}

	public void SetOverwatch(int _index, int _element)
	{
		overwatchIndex = _index;
		overwatchElement = _element;
		overwatchProtected = true;
	}

	GameObject CheckForNearbyEnemy()
	{
		//5 Meters
		for(int i = 0; i < CombatManager.enemies.Count; i ++)
		{
			//Get the distance of the enemy
			float dist = Vector3.Distance (CombatManager.enemies[i].transform.position,  anim.transform.position);

			if(dist < 5f)
			{
				return CombatManager.enemies[i];
			}
		}

		return null;
	}

	void CounterAttack(int _element)
	{
		//Calculate damage
		float damage = (((float)character.earthAffinity / 100f) + 1f) * stat.defence;

		EnemyCombatCharacter enemyStat = CheckForNearbyEnemy().GetComponent<EnemyCombatCharacter>();

		enemyStat.SetDamage (stat, _element, (int)damage, 0f, 0f); 
	}

	//This function is called when character is taken out
	void PlayerDown()
	{
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("RemovePlayer", PlayerIndex(), SendMessageOptions.DontRequireReceiver);
		
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
	}
}
