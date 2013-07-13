using UnityEngine;
using System.Collections;

public class Player : CharacterEntity 
{
	private int score;
	private int lives;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public float StartingHealth = 100.0f;
	
	public int Score
	{
		get
		{
			return this.score;
		}
		
		set
		{
			if(this.score != value)
			{
				this.score = value;
			}
		}
	}
	
	public int Lives
	{
		get
		{
			return this.lives;
		}
		
		set
		{
			if(this.lives != value)
			{
				this.lives = value;
			}
		}
	}
	
	// Use this for initialization
	public void Start () 
	{	
		this.Health = this.StartingHealth;
	}
	
	public override void Update()
	{
		base.Update();
	}
}
