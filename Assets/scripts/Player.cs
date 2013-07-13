using UnityEngine;
using System.Collections;

public class Player : CharacterEntity 
{	
	private Camera mainCamera;
	
	private int score;
	private int lives;
		
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public float StartingHealth = 100.0f;
	
	public string ControlPrefix = "Player1";
	
	public bool CameraFollows = false;
	
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
		this.mainCamera = Camera.main;
		this.Health = this.StartingHealth;
	}
	
	public override void Update()
	{
		base.Update();
		
		float newX = Input.GetAxis(ControlPrefix+" Horizontal");
		float newZ = Input.GetAxis(ControlPrefix+" Vertical");
		float newY = this.CurrentYPos;
		
		float attackState = Input.GetAxis(ControlPrefix+" Attack");
		float jumpState = Input.GetAxis(ControlPrefix+" Jump");
		
		this.CheckAction(attackState > 0, jumpState > 0);
		transform.position = new Vector3(transform.position.x, newY, transform.position.z);
		transform.Translate(new Vector3(newX, newY, newZ), null);
		
		if(this.CameraFollows)
		{
			this.mainCamera.transform.LookAt(this.transform);
			this.mainCamera.transform.Translate(new Vector3(newX, 0, 0), null);
		}
	}
	
	private void CheckAction(bool attack, bool jump)
	{
		if(jump)
		{
			this.StartJump();
		}
	}
	
	private void UpdateVisuals()
	{
		// Todo:
		// 
	}
}
