using UnityEngine;
using System.Collections;

public class Player : CharacterEntity 
{	
	private Camera mainCamera;
	
	private int score;
	private int lives;
	
	private int currentChain = 0;
	private bool comboDidHit = false;
		
	public Player()
	{
		this.ComboChain = new string[]{ "Punch", "Second_punch", "Elbow", "Knee", "Breaker" };
	}
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public string ControlPrefix = "Player1";
	
	public bool CameraFollows = false;
	public bool CameraFollowsY = false;	
	
	public GameObject CurrentTarget { get; set; }
	
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
	public override void Start() 
	{	
		base.Start();
		
		this.mainCamera = Camera.main;
	}
	
	public override void Update()
	{
		base.Update();
		
		float newX = Input.GetAxis(ControlPrefix+" Horizontal");
		float newZ = Input.GetAxis(ControlPrefix+" Vertical");
		//float newY = this.CurrentYPos;
		
		bool attackAction = Input.GetButtonDown(ControlPrefix + " Attack");
		bool jumpAction = Input.GetButtonDown(ControlPrefix + " Jump");

		if(attackAction)
		{
			if(this.InCombat && this.comboDidHit)
			{
				this.currentChain++;
			}
			if(this.currentChain >= this.ComboChain.Length)
			{
				this.currentChain = 0;
			}
			
			this.comboDidHit = false;
			this.EnterCombat(this.currentChain);
		}
						
		/*Animation anim = this.GetComponent<Animation>();

		// Update anims
		if(this.MovementState == CharacterMovementState.Idle && !anim.isPlaying) 
		{
			anim.PlayQueued("Idling");
			print ("Idling");
		}
				
		if(attackAction) 
		{
			if(startingNewAttack()) {
				currentComboProgress = 0;
				anim.CrossFade(comboChain[currentComboProgress],0.1f);
				print ("Starting:" + currentComboProgress);
			} else if(attackInProgress()) {
				currentComboProgress++;
				anim.CrossFade(comboChain[currentComboProgress],0.1f);
				print ("Progress:" + currentComboProgress+"/"+comboChain.Length);
			} else if(lastStrike()) {
				print ("Last strike:" + currentComboProgress);
				currentComboProgress++;
				anim.CrossFade(comboChain[currentComboProgress],0.1f);				
			}
		}
		
		if(comboCompleted()) {
			print ("Combo completed");
			currentComboProgress = -1;
			anim.CrossFadeQueued("Idling",0.5f);
		}			*/
		
		// Jump?
		if(jumpAction)
		{
			this.StartJump();
		}
		
		// Update movement and cam
		this.MoveCharacter(newX, newZ);
		
		if(this.CameraFollows && this.MovementState != CharacterMovementState.Falling)
		{
			this.mainCamera.transform.position = new Vector3(this.transform.position.x, this.mainCamera.transform.position.y, this.mainCamera.transform.position.z);
			if(this.CameraFollowsY)
			{
				this.mainCamera.transform.LookAt(this.transform);
			}
		}
	}
	
	protected override void ResolveCombat(GameObject target, Enemy targetData)
	{
		base.ResolveCombat(target, targetData);
		
		this.CurrentTarget = target;
		this.comboDidHit = true;
		
		if(targetData.IsDead)
		{
			this.score += targetData.ScoreReward;
		}
	}
	
	protected override void LeaveCombat ()
	{
		base.LeaveCombat();
		
		this.comboDidHit = false;
		this.currentChain = 0;
		this.CurrentTarget = null;
	}
	
	private bool ableToAttack() {
		return this.MovementState == CharacterMovementState.Idle
			|| this.MovementState == CharacterMovementState.Walking;
	}
	
	
	/*private bool startingNewAttack() {
		return currentComboProgress == -1;
	}
	
	private bool attackInProgress() {
		return currentComboProgress > -1 && currentComboProgress < comboChain.Length - 2;
	}
	
	private bool lastStrike() {
		return currentComboProgress == comboChain.Length - 1;
	}
	
	private bool comboCompleted() {
		return currentComboProgress == comboChain.Length;
	}*/

}
