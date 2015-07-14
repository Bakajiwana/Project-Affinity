using UnityEngine;
using System.Collections;

public class EnemyCombatEvents : MonoBehaviour 
{
	public EnemyCombatActions characterEvent;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void EndTurn()
	{
		characterEvent.EndTurn();
	}

	public void FireProjectile(int _projectileSlot)
	{
		characterEvent.FireProjectile (_projectileSlot);
	}

	public void EndTurnDelay(float _time)
	{
		characterEvent.EndTurnDelay (_time);
	}

	public void SpawnObject(GameObject _object)
	{
		Instantiate (_object, transform.position,transform.rotation);
	}

	public void SetPlayerDamage()
	{
		characterEvent.SetPlayerDamage ();
	}
}
