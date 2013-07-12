using UnityEngine;
using System.Collections;

public class Enemy : ShipBase 
{
	private float lifeTime;
	private float speed;
	
	private Vector3? startPos;
		
	public float LifeTime
	{
		get
		{
			return this.lifeTime;
		}
	}
	
	public float Speed
	{
		get
		{
			return this.speed;
		}
	}
	
	public override void Update()
	{
		base.Update();
		
		if(this.startPos != null)
		{
			this.transform.position = (Vector3)this.startPos;
			this.startPos = null;
		}
		
		this.lifeTime -= Time.deltaTime;
		//this.transform.Translate(0, -this.speed * Time.deltaTime, 0, Space.Self);
		this.transform.localPosition += new Vector3(0, -this.speed * Time.deltaTime, 0);
	}
	
	public void Initialize(Vector3 position, float lifeTime, float speed)
	{
		this.startPos = position;
		this.lifeTime = lifeTime;
		this.speed = speed;
	}
	
	protected override bool AcceptShotSource (ShotSource source)
	{
		return source == ShotSource.Friend;
	}
}
