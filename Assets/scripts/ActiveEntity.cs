using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActiveEntity : MonoBehaviour 
{
	public delegate void NotifyDelegate();
		
	private float health;
	private float maxHealth;
	
	private bool isDead = false;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public event NotifyDelegate OnDying;
	public event NotifyDelegate OnTakenDamage;
	
	public float CollisionDamage;
	
	public bool CollisionEnabled;
	
	public bool IsDead
	{
		get
		{
			return this.isDead;
		}
	}
	
	public float MaxHealth
	{
		get
		{
			return this.maxHealth;
		}
	}
	
	public float Health
	{
		get
		{
			return this.health;
		}
		
		set
		{
			this.health = value;
			this.maxHealth = value;
			this.isDead = this.health <= 0.0f;
		}
	}
	
	public void TakeDamage(float damage, GameObject source = null)
	{
		if(this.isDead)
		{
			return;
		}
				
		// Todo: Add fancy damage calculations here
		this.health -= damage;
		if(this.health <= 0)
		{
			this.isDead = true;
			if(this.OnDying != null)
			{
				this.OnDying();
			}
			
			return;
		}
		
		if(this.OnTakenDamage != null)
		{
			this.OnTakenDamage();
		}
	}
		
	// Update is called once per frame
	public virtual void Update () 
	{
	}
	
	public void SetCollision(float damage)
	{
		this.CollisionDamage = damage;
		this.CollisionEnabled = damage > 0.0f;
	}
	
	// ---------------------------------------------
	// Private
	// ---------------------------------------------
	private void ProcessCollision(Collider collider)
	{
		ActiveEntity component = collider.gameObject.GetComponent(typeof(ActiveEntity)) as ActiveEntity;
		if(component != null && component.GetType() != this.GetType())
		{
			if(!component.CollisionEnabled)
			{
				return;
			}
						
			this.TakeDamage(component.CollisionDamage, collider.gameObject);
		}
	}
	
	private void OnTriggerEnter(Collider collider)
	{
		this.ProcessCollision(collider);
	}
	
	private void OnTriggerStay(Collider collider)
	{
		this.ProcessCollision(collider);
	}
}
