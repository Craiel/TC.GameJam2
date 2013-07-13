using UnityEngine;
using System.Collections;

public class CharacterEntity : ActiveEntity 
{
	private float speed;
	
	private Vector3? startPos;
			
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
		
		this.transform.localPosition += new Vector3(0, -this.speed * Time.deltaTime, 0);
	}
	
	public void Initialize(Vector3 position, float lifeTime, float speed)
	{
		this.startPos = position;
		this.speed = speed;
	}
}
