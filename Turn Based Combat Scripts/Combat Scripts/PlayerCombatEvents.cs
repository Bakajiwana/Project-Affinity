using UnityEngine;
using System.Collections;

//This script acts as a communication to the main script and handles all animation events

public class PlayerCombatEvents : MonoBehaviour 
{
	public PlayerCombatActions characterEvent;

	public CombatCharacterCamera cameraEvent;

	public PlayerCombatCharacter character;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	public void EndTurn()
	{
		characterEvent.EndTurn();
	}
	
	public void FireSingleShot(int _projectileSlot)
	{
		characterEvent.FireSingleShot (_projectileSlot);
	}
	
	public void EndTurnDelay(float _time)
	{
		characterEvent.EndTurnDelay (_time);
	}

	public void TurnOnCharacterCamera()
	{
		cameraEvent.TurnOnCharacterCamera();
	}
	
	public void SwitchBackToRandomArenaCamera()
	{
		cameraEvent.SwitchBackToRandomArenaCamera();
	}
	
	public void TurnOnRandomArenaCamera()
	{
		cameraEvent.TurnOnRandomArenaCamera();
	}
	
	public void SwitchBackToSelectedArenaCamera(int _camera)
	{
		cameraEvent.SwitchBackToSelectedArenaCamera(_camera);
	}

	public void SpawnObject(GameObject _sound)
	{
		Instantiate (_sound, transform.position,transform.rotation);
	}

	//This function is to teleport next to the enemy into melee range
	public void MoveToTarget(float _moveSpeed)
	{
		characterEvent.MoveToTarget (_moveSpeed);
	}
	
	//This function is to Look at Enemy, by activating a boolean and the update will rotate player
	public void LookAtTarget(float _rotateSpeed)
	{
		characterEvent.LookAtTarget (_rotateSpeed);
	}
	
	//These functions restores initial rotation and transform
	public void MoveToInitialPosition(float _moveSpeed)
	{
		characterEvent.MoveToInitialPosition (_moveSpeed);
	}
	
	public void LookAtInitialRotation(float _rotateSpeed)
	{
		characterEvent.LookAtInitialRotation (_rotateSpeed);
	}
	
	public void ReadyForNextFlurry()
	{
		characterEvent.ReadyForNextFlurry();
	}

	public void SetFlurryDamage()
	{
		characterEvent.SetFlurryDamage ();
	}

	public void SetChargeDamage()
	{
		characterEvent.SetChargeDamage ();
	}

	public void SetDebuff()
	{
		characterEvent.SetDebuff ();
	}

	public void SetBuff()
	{
		characterEvent.SetBuff ();
	}

	public void TeleportBackToPosition()
	{
		character.TeleportBackToPosition ();
	}
}
