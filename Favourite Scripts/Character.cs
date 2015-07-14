using UnityEngine;
using System.Collections;

public class Character
{
	//Exponential growth values x(t) = x * (1 + r)^t   x= 200 and r = 0.2 and t = level...

	public string name;

	public int level;
	public long levelExperience;
	public long levelMaxExperience;

	public int currentShieldAffinity;
	public int currentShield;
	public int currentHealth;

	public int fireAffinity;
	public long fireExperience;
	public long fireMaxExperience;

	public int waterAffinity;
	public long waterExperience;
	public long waterMaxExperience;

	public int lightningAffinity;
	public long lightningExperience;
	public long lightningMaxExperience;

	public int earthAffinity;
	public long earthExperience;
	public long earthMaxExperience; 

	public Character(string newName, int newLevel, int newShieldAffinity, int newShield,
	                 int newHealth,long newLevelExp, 	           
	                 int fire, long fireExp,int water, long waterExp, 
	                 int lightning, long lightningExp, int earth, long earthExp)
	{
		int x = 200;
		float r = 0.20f;

		name = newName;

		level = newLevel;
		levelExperience = newLevelExp;
		levelMaxExperience = (long)(x * Mathf.Pow (1 + r, level)); 

		currentShieldAffinity = newShieldAffinity; 
		currentShield = newShield;
		currentHealth = newHealth;

		fireAffinity = fire;
		fireExperience = fireExp;
		fireMaxExperience = (long)(x * Mathf.Pow (1 + r, fireAffinity)); 

		waterAffinity = water;
		waterExperience = waterExp;
		waterMaxExperience = (long)(x * Mathf.Pow (1 + r, waterAffinity)); 

		lightningAffinity = lightning;
		lightningExperience = lightningExp;
		lightningMaxExperience = (long)(x * Mathf.Pow (1 + r, lightningAffinity)); 

		earthAffinity = earth;
		earthExperience = earthExp;
		earthMaxExperience = (long)(x * Mathf.Pow (1 + r, earthAffinity)); 
	}
}
