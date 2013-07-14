using UnityEngine;
using System.Collections;

public class Enemy : CharacterEntity 
{	
	public int ScoreReward;
	
	public override void Start ()
	{
		base.Start();
		
		this.GameManager.Enemies.Add(this.gameObject);
	}
	
	public override void Update()
	{
		base.Update();
		
		if(this.IsDead)
		{
			this.GameManager.Enemies.Remove(this.gameObject);
			Destroy(this.gameObject);
		}
	}
}
