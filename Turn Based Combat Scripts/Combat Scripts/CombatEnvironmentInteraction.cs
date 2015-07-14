using UnityEngine;
using System.Collections;

//This Script Will Control the Environment Interactible Objects

public class CombatEnvironmentInteraction : MonoBehaviour 
{
	private Animator anim;

	// Use this for initialization
	void Start () 
	{
		anim = gameObject.GetComponent<Animator>();
	}
	
	void EnvironmentInteract(int _element) //1 = Earth, 2 = Fire, 3 = Lightning, 4 = Water
	{
		switch(_element)
		{
		case 1:
			anim.SetInteger ("Earth", anim.GetInteger ("Earth")+ 1);
			break;
		case 2:
			anim.SetInteger ("Fire", anim.GetInteger ("Fire")+ 1);
			break;
		case 3:
			anim.SetInteger ("Lightning", anim.GetInteger ("Lightning")+ 1);
			break;
		case 4:
			anim.SetInteger ("Water", anim.GetInteger ("Water")+ 1);
			break;
		}
	}

	public void DamageClosestEnemy()
	{
		float closest = 0;
		int closestEnemy = 0;
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			float dist = Vector3.Distance (CombatManager.enemies[i].transform.position, transform.position);
			if(dist > closest)
			{
				closest = dist;
				closestEnemy = i;
			}
		}

		CombatManager.enemyStats[closestEnemy].SetStunned (3);
	}
}
