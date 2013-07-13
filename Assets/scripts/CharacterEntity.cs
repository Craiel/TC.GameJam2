using UnityEngine;
using System.Collections;

public enum CharacterMovementState
{
	Idle,
	Punching,
	Walking,
	Running,
	Jumping,
	Leveling,
	Falling
}

public class CharacterEntity : StageEntity 
{		
	private GameManager gameManager;
	
	private Vector3? startPos;
			
	private CharacterMovementState movementState;
	
	private int levelingTimer;
	private int moveTimeoutTimer;
	
	private Vector3 lastGroundedPosition;
	private Vector2 moveDirection;
	
	private bool rotate;
			
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public float Speed = 1.0f;
	public float AirSpeed = 1.0f;
	public float RunSpeed = 2.0f;
	
	public float JumpStrength = 3.0f;
	
	public float DeathDepth = -3;
	
	public bool CanMoveInAir = false;
	
	public bool IsRunning { get; set; }
	
	public virtual void Start()
	{
		this.gameManager = Camera.main.GetComponent<GameManager>();
	}
	
	public override void Update()
	{
		base.Update();
		//print ("Falling");
		if(this.startPos != null)
		{
			this.transform.position = (Vector3)this.startPos;
			this.startPos = null;
		}
		
		if (this.movementState == CharacterMovementState.Running || this.movementState == CharacterMovementState.Idle || this.movementState == CharacterMovementState.Walking || this.movementState == CharacterMovementState.Punching)
		{
			this.lastGroundedPosition = this.transform.position;
		}
		
		if(this.movementState == CharacterMovementState.Jumping || this.movementState == CharacterMovementState.Falling || this.movementState == CharacterMovementState.Leveling)
		{
			this.UpdateAirState();
		}
		else if (this.movementState != CharacterMovementState.Falling)
		{
			// Todo: Check if we are starting to fall here
		}	
	}
	
	public void Initialize(Vector3 position)
	{
		this.startPos = position;
	}
	
	public virtual void ForceDeath()
	{
	}
	
	// ---------------------------------------------
	// Protected
	// ---------------------------------------------	
	protected void StartJump()
	{
		// No jumping if we already are or have no rigid body\\ t
		if(this.movementState == CharacterMovementState.Jumping || this.movementState == CharacterMovementState.Falling || this.movementState == CharacterMovementState.Leveling)
		{
			return;
		}
		
		//print ("Jumping");
		this.lastGroundedPosition = this.transform.position;
		// Todo: Add actual jump movement
		this.movementState = CharacterMovementState.Jumping;
		this.levelingTimer = 0;
	}
	
	protected void MoveCharacter(float newX, float newZ)
	{		
		if(this.CanMoveInAir || this.movementState == CharacterMovementState.Running || this.movementState == CharacterMovementState.Idle || this.movementState == CharacterMovementState.Walking)
		{
			// Clear out depth movement if we are airborne
			if(this.movementState == CharacterMovementState.Falling || this.movementState == CharacterMovementState.Jumping || this.movementState == CharacterMovementState.Leveling)
			{
				newZ = 0;
			}
						
			Vector3 newVector;
			if(this.IsRunning)
			{
				//print ("Running");
				if(this.movementState == CharacterMovementState.Idle || this.movementState == CharacterMovementState.Walking)
				{
					this.movementState = CharacterMovementState.Running;
				}
				
				newVector = new Vector3(this.CheckXMovementBounds(this.transform.position.x, newX * this.RunSpeed), 
					0, 
					this.CheckZMovementBounds(this.transform.position.z, newZ * this.RunSpeed));
			}
			else
			{
				//print ("Walking");
				if(this.movementState == CharacterMovementState.Idle || this.movementState == CharacterMovementState.Running)
				{
					this.movementState = CharacterMovementState.Walking;
				}
				
				newVector = new Vector3(this.CheckXMovementBounds(this.transform.position.x, newX * this.Speed), 
					0, 
					this.CheckZMovementBounds(this.transform.position.z, newZ * this.Speed));
			}
			
			transform.Translate(newVector, null);
			
			this.moveDirection = new Vector2(newX, newZ);
			bool newState = newX < 0;
			if((newX < 0 || newX > 0) && this.rotate != newState)
			{
				this.rotate = newState;
				this.transform.Rotate(Vector3.up, 180);
			}		
		}
	}
	
	protected override void HandleCollision(Collider collider)
	{
	}
	
	
	
	// ---------------------------------------------
	// Protected
	// ---------------------------------------------	
	private void UpdateAirState()
	{
		transform.Translate(new Vector3(this.moveDirection.x * AirSpeed, 0, this.moveDirection.y * AirSpeed), null);
		
		if(this.transform.position.y < DeathDepth)
		{
			this.ForceDeath();
			this.transform.position = this.lastGroundedPosition;
			return;
		}
		
		// Todo: Check the state of air and move us into level or idle
	}
	
	private float CheckXMovementBounds(float origin, float translation)
	{
		if((origin + translation) < this.gameManager.StageBounds.x && translation < 0)
		{
			this.transform.position = this.transform.position + new Vector3(Mathf.Abs(translation), 0, 0);
			//return this.gameManager.StageBounds.x;
		}
		
		if((origin + translation) > this.gameManager.StageBounds.width && translation > 0)
		{
			this.transform.position = this.transform.position - new Vector3(translation, 0, 0);
			//return this.gameManager.StageBounds.width;
		}
		
		return translation;
	}
	
	private float CheckZMovementBounds(float origin, float translation)
	{
		print (origin+translation+" "+translation+ " -- "+this.gameManager.StageBounds);
		if((origin + translation) < this.gameManager.StageBounds.y && translation < 0)
		{
			this.transform.position = this.transform.position + new Vector3(0, 0, Mathf.Abs(translation));
			//return this.gameManager.StageBounds.y;
		}
		
		if((origin + translation) > this.gameManager.StageBounds.height && translation > 0)
		{
			this.transform.position = this.transform.position - new Vector3(0, 0, translation);
			//return this.gameManager.StageBounds.height;
		}
		
		return translation;
	}
}
