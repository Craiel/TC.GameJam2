using UnityEngine;
using System.Collections;

public class Enemy : CharacterEntity 
{	
	public override void Start ()
	{
		this.GameManager.Enemies.Add(this.gameObject);
	}
	
	public override void Update()
	{
		base.Update();
	}
}
