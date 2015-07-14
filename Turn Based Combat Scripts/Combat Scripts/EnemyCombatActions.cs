using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCombatActions : MonoBehaviour 
{	
	//Animator 
	public Animator anim;

	private EnemyCombatCharacter combatStats;

	//ATTACKS Variables, based off the animation number
	//This is how it works, the attacks are triggered by the attack number, and the animation does all the work
	[HideInInspector]
	public int attackNumber; //0 = idle, > attack sets

	//Attack Damage
	public int[] attackDamage; 

	//Status Effects Chance of Attack, as arrays so attack 1 = attack status effect chance 1 etc
	public float[] attackStatusEffectChance; 

	//Critical Chance of attack
	public float[] attackCritChance;

	//Is attack a projectile
	public int[] attackProjectileTier;

	//Attack Projectiles
	public Transform[] projectileNode;
	public Transform[] projectile; 

	//AI Calculations
	private GameObject target;
	private int targetIndex;

	void Awake()
	{
		//Connect to Stats
		combatStats = gameObject.GetComponent<EnemyCombatCharacter>();

		//Turn this script off in case its on at the start
		this.enabled = false;

		//Set to Idle
		attackNumber = 0;
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	void OnEnable()
	{
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
			print ("I'm the Enemy and its my turn woop WOOP.. now I end my turn prematurely");

			//Calculate target to attack
			targetIndex = Random.Range (0, CombatManager.players.Count);
			target = CombatManager.players[targetIndex]; //Select Random target

			//Calculate what to attack
			attackNumber = Random.Range (1, attackNumber);

			//Attack
			anim.SetInteger ("Attack Number", attackNumber);
		}
	}

	public void FireProjectile(int _projectileSlot)
	{
		//Calculate Damage
		float damage = (((float)attackDamage[attackNumber-1] / 100f) + 1f) * combatStats.stat.attack;

		//Spawn Projectiles and specify information to projectile
		Transform shot = Instantiate (projectile[((attackProjectileTier[attackNumber-1])*(combatStats.affinity))-1],
		                                    projectileNode[_projectileSlot-1].position,
		                                    projectileNode[_projectileSlot-1].rotation) as Transform;

		//Calculate Status Chance
		float statusChance = attackStatusEffectChance[attackNumber-1];
		//Get the targets script component and set target
		CombatProjectile trajectory = shot.gameObject.GetComponent<CombatProjectile>();
		trajectory.SetTarget (combatStats.stat, target, (int)damage, true, 
		                      statusChance, attackCritChance[attackNumber-1]);
	}	

	public void SetPlayerDamage()
	{
		//Calculate Damage
		float damage = (((float)attackDamage[attackNumber-1] / 100f) + 1f) * combatStats.stat.attack;

		//Calculate Status Chance
		float statusChance = attackStatusEffectChance[attackNumber-1];

		CombatManager.playerStats[targetIndex].SetDamage (combatStats.stat, 0, (int)damage, statusChance,attackCritChance[attackNumber-1]); 
	}

	//This function ends the turn with a delay
	public void EndTurnDelay(float _time)
	{
		Invoke ("EndTurn", _time);
	}

	
	public void EndTurn()
	{
		anim.SetInteger ("Attack Number", 0);
		combatStats.UpdateStatusEffects ();
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("NextTurn", SendMessageOptions.DontRequireReceiver);
		this.enabled = false; 
	}
}
