using UnityEngine;
using System.Collections;

public enum CharacterMovementState
{
	Idle,
	Punching,
	Walking,
	Jumping,
	Leveling,
	Falling
}

public class CharacterEntity : StageEntity 
{		
	private GameManager gameManager;
	private CapsuleCollider movementCollider;
	
	private Vector3? startPos;
			
	private int levelingTimer;
	private int moveTimeoutTimer;
	
	private Vector3 lastGroundedPosition;
	private Vector2 moveDirection;
	
	private bool rotate;
	
	private float upVelocity;
	private float baseY;
			
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public float Speed = 1.0f;
	public float AirSpeed = 1.0f;
	
	public float JumpStrength = 3.0f;
	public float Gravity = 1.0f;
	public float TerminalVelocity = -0.5f;
		
	public float DeathDepth = -3;
	
	public bool CanMoveInAir = false;
		
	public CharacterMovementState MovementState { get; set; }
	
	public virtual void Start()
	{
		this.gameManager = Camera.main.GetComponent<GameManager>();
		this.movementCollider = this.GetComponent<CapsuleCollider>();
		
		this.baseY = this.transform.position.y;
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
				
		if(this.MovementState == CharacterMovementState.Jumping || this.MovementState == CharacterMovementState.Falling || this.MovementState == CharacterMovementState.Leveling)
		{
			this.UpdateAirState();
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
		if(this.MovementState == CharacterMovementState.Jumping || this.MovementState == CharacterMovementState.Falling || this.MovementState == CharacterMovementState.Leveling)
		{
			return;
		}
		
		//print ("Jumping");
		this.lastGroundedPosition = this.transform.position;
		this.upVelocity = this.JumpStrength;
		this.MovementState = CharacterMovementState.Jumping;
		this.levelingTimer = 0;
	}
	
	protected void MoveCharacter(float newX, float newZ)
	{		
		if(this.CanMoveInAir || this.MovementState == CharacterMovementState.Idle || this.MovementState == CharacterMovementState.Walking)
		{
			// Clear out depth movement if we are airborne
			if(this.MovementState == CharacterMovementState.Falling || this.MovementState == CharacterMovementState.Jumping || this.MovementState == CharacterMovementState.Leveling)
			{
				newZ = 0;
			}
						
			Vector3 newVector;
			
			//print ("Walking");
			if(this.MovementState == CharacterMovementState.Idle)
			{
				this.MovementState = CharacterMovementState.Walking;
			}
			
			newVector = new Vector3(this.CheckXMovementBounds(this.transform.position.x, newX * this.Speed), 
				0, 
				this.CheckZMovementBounds(this.transform.position.z, newZ * this.Speed));
			
			// Test the new vector against colliding geometry
			Vector3 target = this.movementCollider.transform.position + this.movementCollider.center + new Vector3(newX, 0, newZ);
			foreach(var geometry in this.gameManager.CollidingGeometry)
			{
				CapsuleCollider test = geometry.collider as CapsuleCollider;
				if(test == null)
				{
					continue;
				}
				
				float distance = (target - (test.transform.position + test.center)).magnitude;
				float requiredDistance = (this.movementCollider.radius * this.movementCollider.transform.localScale.x) + (test.radius * test.transform.localScale.x);
				if(distance < requiredDistance)
				{
					// We are too close to a collider, bail out
					return;
				}
			}
			
			// Test if we are moving into a drag entry
			bool wasDragged = false;
			foreach(var drag in this.gameManager.DragEntries)
			{
				BoxCollider test = drag.collider as BoxCollider;				
				if(test.bounds.Contains(target))
				{
					this.upVelocity = -drag.GetComponent<DragObject>().Pull;
					this.MovementState = CharacterMovementState.Falling;
					wasDragged = true;
				}
			}
			
			if(!wasDragged && (this.MovementState == CharacterMovementState.Walking || this.MovementState == CharacterMovementState.Idle))
			{
				this.lastGroundedPosition = this.transform.position;
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
	
	// ---------------------------------------------
	// Protected
	// ---------------------------------------------	
	private void UpdateAirState()
	{
		transform.Translate(new Vector3(this.moveDirection.x * AirSpeed, 0, this.moveDirection.y * AirSpeed), null);
		
		if(this.transform.position.y < DeathDepth)
		{
			this.ForceDeath();
			this.upVelocity = 0;
			this.transform.position = new Vector3(this.lastGroundedPosition.x, this.baseY, this.lastGroundedPosition.z);
			this.MovementState = CharacterMovementState.Idle;
			return;
		}
		
		this.upVelocity -= this.Gravity;
		if(this.upVelocity < this.TerminalVelocity)
		{
			this.upVelocity = this.TerminalVelocity;
		}
		
		this.transform.Translate(0, this.upVelocity, 0);
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
//		print (origin+translation+" "+translation+ " -- "+this.gameManager.StageBounds);
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
