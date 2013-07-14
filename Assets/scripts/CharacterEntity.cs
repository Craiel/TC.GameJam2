using UnityEngine;
using System.Collections;

public enum CharacterMovementState
{
	Idle,
	Walking,
	Jumping,
	Leveling,
	Falling,
	Landing
}

public class CharacterEntity : StageEntity 
{		
	private GameManager gameManager;
	private CapsuleCollider movementCollider;
	private Animation animation;
	
	private Vector3? startPos;
			
	private int moveTimeoutTimer;
	
	private Vector3 lastGroundedPosition;
	private Vector2 moveDirection;
	
	private bool rotate;
	
	private float upVelocity;
	private float baseY;
	
	private int currentComboProgress = -1;
	private float lastIdleTick = 0;
	private float idleDelay = 0;
	private float lastCombatTick = 0;
	private int comboStage = 0;
	private bool allowHit = false;
	
	private string lastAnimationPlayed;
			
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public int StartingHealth = 100;
	
	public float Speed = 1.0f;
	public float AirSpeed = 1.0f;
	
	public float JumpStrength = 3.0f;
	public float Gravity = 1.0f;
	public float TerminalVelocity = -0.5f;
	
	public float AnimationSpeed = 1.0f;
	
	public float IdleTime = 0.5f;
		
	public float DeathDepth = -3;
	
	public bool CanMoveInAir = false;
		
	public CharacterMovementState MovementState { get; set; }
	
	public string IdleAnimation = "Idling";
	public string WalkAnimation = "Walking";
	public string JumpAnimation = "Jumping";
	public string FallAnimation = "Falling";
	public string FallCombatAnimation = "FallingCombat";
	public string JumpCombatAnimation = "JumpCombat";
	
	public string Name = "No Name";
	
	public string[] ComboChain = null;
	public float CombatTimeout = 2;
	
	public Texture2D Portrait;
	
	public AudioClip SFXCombatMiss;
	public AudioClip SFXJump;
	public AudioClip SFXLand;
	public AudioClip SFXFalling;
	public AudioClip[] SFXComboChain;
		
	public bool IsAirborne
	{
		get
		{
			return this.MovementState == CharacterMovementState.Jumping || 
				this.MovementState == CharacterMovementState.Falling || 
					this.MovementState == CharacterMovementState.Landing || 
					this.MovementState == CharacterMovementState.Leveling;
		}
	}
	
	public virtual void Start()
	{
		this.gameManager = Camera.main.GetComponent<GameManager>();
		this.movementCollider = this.GetComponent<CapsuleCollider>();
		this.animation = this.GetComponent<Animation>();
		
		this.baseY = this.transform.position.y;
		this.Health = this.StartingHealth;
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
				
		if(this.IsAirborne)
		{
			this.UpdateAirState();
		}
		else if(this.MovementState != CharacterMovementState.Idle)
		{
			this.idleDelay+=0.1f;
			if(this.idleDelay > this.IdleTime)
			{
				this.MovementState = CharacterMovementState.Idle;
				this.idleDelay = 0;
			}
		}
		
		if(this.InCombat)
		{
			this.lastCombatTick+=0.1f;
			if(this.lastCombatTick > this.CombatTimeout)
			{
				this.LeaveCombat();
			}
		}
		
		this.UpdateAnimationState();
	}
	
	public void Initialize(Vector3 position)
	{
		this.startPos = position;
	}
	
	public void OnChildCollision(Collider collider)
	{
	}
	
	public void OnChildCollisionStay(Collider collider)
	{
		var target = collider.GetComponent<Enemy>();
		if(!this.InCombat || target == null || !this.allowHit)
		{
			// don't care or not valid target
			return;
		}		
		
		this.ResolveCombat(collider.gameObject, target);
		this.allowHit = false;
	}
	
	// ---------------------------------------------
	// Protected
	// ---------------------------------------------	
	protected GameManager GameManager
	{
		get
		{
			return this.gameManager;
		}
	}
	
	protected bool InCombat { get; private set; }
	
	protected virtual void ResolveCombat(GameObject target, Enemy targetData)
	{
		// Todo: Damage based on combo
		targetData.TakeDamage(10);
	}
	
	protected void EnterCombat(int comboStage)
	{
		print ("Entering combat");
		this.InCombat = true;
		this.lastCombatTick = 0;		
		this.comboStage = comboStage;
		this.allowHit = true;
	}
	
	protected virtual void LeaveCombat()
	{
		print("Leaving Combat");
		this.InCombat = false;
		this.comboStage = 0;
		this.allowHit = false;
	}
	
	protected void StartJump()
	{
		// No jumping if we already are or have no rigid body\\ t
		if(this.IsAirborne)
		{
			return;
		}
		
		//print ("Jumping");
		if(this.SFXJump != null)
		{
			audio.PlayOneShot(this.SFXJump);
		}
		
		this.lastGroundedPosition = this.transform.position;
		this.upVelocity = this.JumpStrength;
		this.MovementState = CharacterMovementState.Jumping;
	}
		
	protected void MoveCharacter(float newX, float newZ)
	{	
		// Skip out of 0 movement requests
		if(Mathf.Abs(newX) < Mathf.Epsilon && Mathf.Abs(newZ) < Mathf.Epsilon)
		{
			this.moveDirection = new Vector2(0, 0);
			return;
		}
		
		if(this.CanMoveInAir || this.MovementState == CharacterMovementState.Idle || this.MovementState == CharacterMovementState.Walking)
		{
			this.idleDelay = 0;
			if(this.MovementState == CharacterMovementState.Idle)
			{				
				this.MovementState = CharacterMovementState.Walking;
			}
			
			// Clear out depth movement if we are airborne
			if(this.IsAirborne)
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
					if(this.SFXFalling != null)
					{
						audio.PlayOneShot(this.SFXFalling);
					}
					
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
		this.upVelocity -= this.Gravity;
		if(this.upVelocity < this.TerminalVelocity)
		{
			this.upVelocity = this.TerminalVelocity;
		}
		
		this.transform.Translate(Mathf.Abs(this.moveDirection.x * AirSpeed), this.upVelocity, 0);
		
		// Stop air movement if we are hitting the boundaries
		if(this.transform.position.x < this.gameManager.StageBounds.x)
		{
			this.moveDirection = new Vector2(0, 0);
			this.transform.position = new Vector3(this.gameManager.StageBounds.x, this.transform.position.y, this.transform.position.z);
		}
		else if(this.transform.position.x > this.gameManager.StageBounds.width)
		{
			this.moveDirection = new Vector2(0, 0);
			this.transform.position = new Vector3(this.gameManager.StageBounds.width, this.transform.position.y, this.transform.position.z);
		}
		
		if(this.upVelocity < 0.1f && this.MovementState == CharacterMovementState.Jumping)
		{
			print ("Leveling");
			this.MovementState = CharacterMovementState.Leveling;
			return;
		}
		
		if(this.upVelocity < -0.01f && this.MovementState == CharacterMovementState.Leveling)
		{
			print ("Landing");
			this.MovementState = CharacterMovementState.Landing;
			return;
		}
		
		if(this.transform.position.y < this.baseY && this.MovementState == CharacterMovementState.Landing)
		{
			print ("Landed");
			if(this.SFXJump != null)
			{
				audio.PlayOneShot(this.SFXLand);
			}
			
			this.upVelocity = 0;
			this.transform.position = new Vector3(this.transform.position.x, this.baseY, this.transform.position.z);
			this.MovementState = CharacterMovementState.Idle;
			this.LeaveCombat();
			return;
		}
		
		if(this.transform.position.y < DeathDepth)
		{
			print ("Death from falling");
			this.TakeDamage(this.gameManager.FallDamage, null);
			this.upVelocity = 0;
			this.transform.position = new Vector3(this.lastGroundedPosition.x, this.baseY, this.lastGroundedPosition.z);
			this.MovementState = CharacterMovementState.Idle;
			return;
		}
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
	
	private void UpdateAnimationState()
	{
		if(this.animation == null)
		{
			return;
		}
		
		if(this.MovementState == CharacterMovementState.Idle && !this.InCombat)
		{
			if(this.lastIdleTick < this.AnimationSpeed)
			{
				this.lastIdleTick += 0.1f;
				return;
			}
			
			this.PlayAnimation(this.IdleAnimation, false, true);
			this.lastIdleTick = 0;
			return;
		}
		
		if(this.MovementState == CharacterMovementState.Walking && !this.InCombat)
		{
			this.PlayAnimation(this.WalkAnimation, false, true);
			return;
		}
		
		if(this.MovementState == CharacterMovementState.Jumping)
		{
			if(this.InCombat)
			{
				this.PlayAnimation(this.JumpCombatAnimation, true, false);
				return;
			}
			
			this.PlayAnimation(this.JumpAnimation, true, true);
			return;
		}
		
		if(this.MovementState == CharacterMovementState.Falling)
		{
			if(this.InCombat)
			{
				this.PlayAnimation(this.FallCombatAnimation, true, false);
				return;
			}
			
			this.PlayAnimation(this.FallAnimation, true, true);
			return;
		}
	}
	
	private void PlayAnimation(string animation, bool onceOnly = true, bool transition = true)
	{
		if(this.lastAnimationPlayed == animation)
		{
			if(onceOnly || this.animation.isPlaying)
			{
				return;
			}
		}
		
		print("PlayAnim: "+animation);
		this.animation[animation].speed = this.AnimationSpeed;
		if(!onceOnly)
		{
			this.animation[animation].wrapMode = WrapMode.Loop;
		}
		
		if(transition)
		{
			this.animation.CrossFade(animation);
		} else
		{
			this.animation.Stop();
			this.animation.Play(animation);
		}
		this.lastAnimationPlayed = animation;
	}
}
