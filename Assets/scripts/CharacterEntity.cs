using UnityEngine;
using System.Collections;

public class CharacterEntity : ActiveEntity 
{	
	private Vector3? startPos;
	
	private bool isJumping;
	private float currentHeight;
	private float currentYAccelleration;
	private float currentYFalloff;
			
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public float Speed = 1.0f;
	
	public float JumpStrength = 3.0f;	
	public float JumpFalloff = 0.1f;
	
	public override void Update()
	{
		base.Update();
		
		if(this.startPos != null)
		{
			this.transform.position = (Vector3)this.startPos;
			this.startPos = null;
		}
		
		if(this.isJumping)
		{
			this.currentHeight += this.currentYAccelleration;
			this.currentYAccelleration -= this.currentYFalloff;
			print (this.currentYAccelleration);
			if(this.currentHeight < 0)
			{
				this.currentHeight = 0;
				this.isJumping = false;
			}
		}
	}
	
	public void Initialize(Vector3 position)
	{
		this.startPos = position;
	}
	
	// ---------------------------------------------
	// Protected
	// ---------------------------------------------
	protected float CurrentYPos
	{
		get
		{
			return this.currentHeight;
		}
	}
	
	protected void StartJump()
	{
		if(this.isJumping)
		{
			return;
		}
		
		this.isJumping = true;
		this.currentYAccelleration = this.JumpStrength;
		this.currentYFalloff = this.JumpFalloff;
	}
}
