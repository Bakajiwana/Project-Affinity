using UnityEngine;
using System.Collections;

//This script will receive information from shooter and will go towards target and send information to damage.

public class CombatProjectile : MonoBehaviour 
{
	//1 = Earth, 2 = Fire, 3 = Lightning, 3 = Water
	public int element;
	public float rotateSpeed = 20f;
	public float moveSpeed = 10f;
	private GameObject target; 
	private EnemyCombatCharacter enemy;
	private PlayerCombatCharacter player;
	private CombatStat combatStats;
	private bool isPlayer;
	private int damage;
	private float statusChance;
	private float criticalChance;
	public Transform impactParticle;

	//Aiming
	private Vector3 targetLocation;
	[Range(0f,1f)]
	public float heightPercentage = 0f;

	//Once all information is obtained then move like a projectile
	private bool launch;

	private bool miss = false;

	private bool impact = false;
	
	// Update is called once per frame
	void Update () 
	{
		//If ready to launch
		if(launch)
		{
			Vector3 targetDir = targetLocation - transform.position;
			float step = rotateSpeed * Time.deltaTime;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0f);

			float moveStep = moveSpeed * Time.deltaTime;

			if(!miss)
			{
				//Rotate towards target
				transform.rotation = Quaternion.LookRotation (newDir);

				//Move Towards target
				transform.position = Vector3.MoveTowards (transform.position, targetLocation, moveStep);
			}
			else
			{
				//Keep going
				transform.Translate (Vector3.forward * moveStep, Space.Self);
			}
		}
	}

	public void SetTarget(CombatStat _combatStats, GameObject _target, 
	                      int _damage, bool _isPlayer, float _statusChance, float _criticalChance)
	{
		combatStats = _combatStats;
		statusChance = _statusChance;
		target = _target;
		damage = _damage;
		criticalChance = _criticalChance;
		isPlayer = _isPlayer;

		//Height and location
		CapsuleCollider measurements = target.gameObject.GetComponent<CapsuleCollider>();
		float height = measurements.height * heightPercentage;
		targetLocation = new Vector3(target.transform.position.x, 
		                             target.transform.position.y + height, 
		                             target.transform.position.z);

		//Obtain necessary component information
		if(isPlayer)
		{
			player = target.GetComponent<PlayerCombatCharacter>();
		}
		else
		{
			enemy = target.GetComponent<EnemyCombatCharacter>();
		}

		launch = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == target && !impact)
		{
			//Decide whether enemy or player
			if(isPlayer)
			{
				player.HitByProjectile (gameObject);
				player.SetDamage (combatStats, element, damage, statusChance, criticalChance);
			}
			else
			{
				enemy.HitByProjectile (gameObject);
				enemy.SetDamage (combatStats, element, damage, statusChance, criticalChance);
			}

			impact = true;
		}
	}

	void TargetMiss()
	{
		miss = true;
		Destroy (gameObject, 1f);

		print ("Missed");
	}

	void TargetHit()
	{
		//Decide whether enemy or player
		if(isPlayer)
		{
			Instantiate (impactParticle,transform.position, transform.rotation);
		}
		else
		{
			Instantiate (impactParticle,transform.position, transform.rotation);
		}
		Destroy (gameObject);
	}
}
