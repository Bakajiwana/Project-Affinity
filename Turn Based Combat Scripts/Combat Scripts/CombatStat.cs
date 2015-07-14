using UnityEngine;
using System.Collections;

//This will be used by every character because every characters have their own stat, perfect Monobehaviour re-usable script
//The Management of Combat Stats

public class CombatStat
{
	//AFFINITY, 1 - Earth, 2 - Fire, 3 - Lightning, 4 - Water
	//Every character will use these variables which are:

	//Shield 
	public int shieldAffinity;
	public int shieldMax;
	public int shield;

	//Health
	public int healthBase;	//Obtained initially at the start of battle, from spawner
	public int healthMax; //Maximum health, used in conjunction to buffing etc
	public int health; //The health that will be used and displayed

	//Action Points 
	public int actionPointBase;
	public int actionPointMax;
	public int actionPoints;

	//Attack, the overall damage you can do with addition to affinity level, apparel, buffs (from item or actual buff)
	public int attackBase;
	public int attackMax;
	public int attack;

	//Defence
	public int defenceBase;
	public int defenceMax;
	public int defence; 

	//Agility: gives chance to dodge attacks
	public int agilityBase;
	public int agilityMax;
	public int agility;

	//Luck: the chance of rare loot AND the increased chance of a critical hit with flurries
	public int luckBase;
	public int luckMax;
	public int luck;

	//Accuracy: Minises the chance of enemy dodging
	public int accuracyBase;
	public int accuracyMax;
	public int accuracy; 

	//Speed: the speed of taking turns
	public int speedBase;
	public int speedMax;
	public int speed;

	public CombatStat(int level, int shieldElement, int shieldHealth, int healthPower, int actionPointsPower, int attackPower,
	                  int defencePower, int agilityPower, int luckPower, int accuracyPower, int speedPower)
	{
		//Shield
		shieldAffinity = shieldElement;
		shieldMax = shieldHealth;
		shield = shieldMax;

		//Linear Rising Gap from - http://gamedev.stackexchange.com/questions/13638/algorithm-for-dynamically-calculating-a-level-based-on-experience-points

		//Equation: x = power * sqrt(level) 

		healthBase = (int)((float)healthPower * Mathf.Sqrt(level));
		healthMax = healthBase;
		health = healthMax;

		actionPointBase = (int)((float)actionPointsPower * Mathf.Sqrt(level));//*100;//times 100 for easy
		actionPointMax = actionPointBase;
		actionPoints = actionPointMax;

		attackBase = (int)((float)attackPower * Mathf.Sqrt(level));
		attackMax = attackBase;
		attack = attackMax;

		defenceBase = (int)((float)defencePower * Mathf.Sqrt(level));
		defenceMax = defenceBase;
		defence = defenceMax; 

		agilityBase = (int)((float)agilityPower * Mathf.Sqrt(level));
		agilityMax = agilityBase;
		agility = agilityMax;

		luckBase = (int)((float)luckPower * Mathf.Sqrt(level));
		luckMax = luckBase;
		luck = luckMax;

		accuracyBase = (int)((float)accuracyPower * Mathf.Sqrt(level));
		accuracyMax = accuracyBase;
		accuracy = accuracyMax; 

		speedBase = (int)((float)speedPower * Mathf.Sqrt(level));
		speedMax = speedBase;
		speed = speedMax;
	}

}
