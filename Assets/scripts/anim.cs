using UnityEngine;
using System.Collections;

public class anim : MonoBehaviour {

	private Animator animator;
	
	private int comboStage = 0;
	
	private float comboDelay = 0;
	private bool inCombo;
	
	// Use this for initialization
	void Start () {
		this.animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		bool attackState = Input.GetButton("Player1 Attack");
		bool jumpState = Input.GetButton("Player1 Jump");
		
		if(this.inCombo)
		{
			this.comboDelay+= 0.1f;
		}
		if(attackState)
		{
			if(this.comboDelay > 5f)
			{
				this.comboDelay = 0;
				this.comboStage++;
				if(this.comboStage > 3)
				{
					this.comboStage = 0;
					this.inCombo = false;
				}
			} 
			else if (!this.inCombo)
			{
				this.comboDelay = 0;
				this.comboStage = 1;
				this.inCombo = true;
			}
		} else
		{
			if(this.comboDelay > 5f)
			{
				this.comboStage = 0;
			}
		}
		
		this.animator.SetInteger("ComboStage", this.comboStage);
	}
}
