using UnityEngine;
using System.Collections;

public class Enemy : CharacterEntity 
{	
	private float attackTick;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public int ScoreReward;
	public int AggroRange = 5;
	
	public float AttackDelay = 2.0f;
	
	public bool Stationary = false;
	
	public bool IsActive { get; set; }	
	
	public override void Start ()
	{
		base.Start();
		
		this.GameManager.Enemies.Add(this.gameObject);
	}
	
	public override void Update()
	{
		base.Update();
		
		if(this.IsDead && this.GameManager.Enemies.Contains(this.gameObject))
		{
			this.GameManager.Enemies.Remove(this.gameObject);
		}
		
		if(this.IsActive)
		{
			this.PerformAI();
		}
	}
	
	private void PerformAI()
	{
		GameObject player = this.GameManager.FindClosestPlayer(this.transform.position);
		if(player == null)
		{
			return;
		}
		
		if(!this.Stationary)
		{
			print ("MoveTowards");
			if(this.MoveTowards(player))
			{
				return;
			}
			
			print ("MoveIntoPosition");
			if(this.MoveIntoPosition(player))
			{
				return;
			}
			
			print ("Attack");
			if(this.Attack(player))
			{
				return;
			}
		}
	}
	
	private bool Attack(GameObject target)
	{
		if(this.attackTick < this.AttackDelay)
		{
			this.attackTick += 0.1f;
			return false;
		}
		
		var radius = target.collider.GetComponent<CapsuleCollider>().radius;
		var distance = (target.transform.position - this.transform.position).magnitude;
		print (distance);
		if(distance <= (this.HitReach + radius + 0.2f))
		{
			this.attackTick = 0;
			target.GetComponent<CharacterEntity>().TakeDamage(1, this.gameObject);
			return true;
		}
		
		return false;
	}
	
	private bool MoveTowards(GameObject target)
	{
		var radius = target.collider.GetComponent<CapsuleCollider>().radius;
		
		// No closer than the characters boundaries
		var distance = (target.transform.position - this.transform.position).magnitude;
		if(distance > (radius + this.HitReach))
		{
			Vector3 distanceValues = (target.transform.position - this.transform.position);
			distanceValues.x = this.ClampDistance(distanceValues.x, radius);
			distanceValues.y = 0;
			distanceValues.z = this.ClampDistance(distanceValues.z, radius);
			
			Vector3 direction = distanceValues.normalized;
			this.transform.Translate(new Vector3(direction.x * this.Speed, 0, direction.z * this.Speed));
			return true;
		}
		
		return false;
	}
	
	private float ClampDistance(float source, float distance)
	{
		if(Mathf.Abs(source) > distance)
		{
			return source;
		}
		
		return 0;
	}
	
	private bool MoveIntoPosition(GameObject target)
	{
		print ("MoveIntoPosition");
		// If our Z is too far away move us closer
		float zDistance = target.transform.position.z - this.transform.position.z;
		if(Mathf.Abs(zDistance) > this.GameManager.ZRangeTollerance)
		{
			
			this.transform.Translate(new Vector3(0, 0, Mathf.Clamp(zDistance, -this.Speed, this.Speed)));
			return true;
		}
		
		return false;
	}
}
