using UnityEngine;
using System.Collections;

public class ActiveEntity : MonoBehaviour 
{
	public float CollisionDamage;
	public float CollisionInterval;
	
	public bool CollisionEnabled;
	
	public void SetCollision(float damage, float interval)
	{
		this.CollisionDamage = damage;
		this.CollisionInterval = interval;
		this.CollisionEnabled = damage > 0.0f;
	}
}
