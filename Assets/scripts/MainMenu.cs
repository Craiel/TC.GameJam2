using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{	
	private const string JoinString = "Press Attack to join";
	private const string JoinedString = "Move left right to select character";
	
	private Vector3 player1Position;
	private Vector3 player2Position;
	
	private int player1Selection;
	private int player2Selection;
	
	private GameObject player1Model;
	private GameObject player2Model;
	
	private bool player1Active;
	private bool player2Active;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	
	public GameObject Player1Placeholder;
	public GameObject Player2Placeholder;
	
	public List<GameObject> PlayerModels = new List<GameObject>();
	
	public GUIText Player1Text;
	public GUIText Player2Text;
	
	// Use this for initialization
	public void Start () 
	{				
		this.player1Position = this.Player1Placeholder.transform.position;
		this.player2Position = this.Player2Placeholder.transform.position;
		
		Destroy(this.Player1Placeholder);
		Destroy(this.Player2Placeholder);
		
		Player1Text.text = JoinString;
		Player2Text.text = JoinString;		
	}
		
	// Update is called once per frame
	public void Update () 
	{
		if(Input.GetAxis("Start") > 0 && (this.player1Active || this.player2Active))
		{
			if(SceneState.Instance)
			{
			}
			if(player1Active)
			{
				SceneState.Instance.Player1 = this.PlayerModels[this.player1Selection].name;
				print ("p1");
			}
			if(this.player2Active)
			{
				SceneState.Instance.Player2 = this.PlayerModels[this.player2Selection].name;
				print ("p2");
			}
			
			print ("transition into entry");
			Application.LoadLevel ("Entry");
			return;
		}
		
		this.UpdatePlayerState();
		this.UpdatePlayerSelection();	
		
		if(this.player1Model != null)
		{
			this.player1Model.transform.Rotate(Vector3.up, 0.3f);
		}
		
		if(this.player2Model != null)
		{
			this.player2Model.transform.Rotate(Vector3.up, 0.3f);
		}
	}
	
	// ---------------------------------------------
	// Private
	// ---------------------------------------------
	private void UpdatePlayerState()
	{
		float p1attackState = Input.GetAxis("Player1 Attack");
		float p1jumpState = Input.GetAxis("Player1 Jump");
		
		if(p1attackState > 0)
		{
			this.player1Active = true;
			this.Player1Text.text = JoinedString;	
			this.ChangePlayer1Model();
		}
		else if (p1jumpState > 0)
		{
			this.player1Active = false;
			this.Player1Text.text = JoinString;
			if(this.player1Model != null)
			{
				Destroy(this.player1Model);
			}
		}
		
		float p2attackState = Input.GetAxis("Player2 Attack");
		float p2jumpState = Input.GetAxis("Player2 Jump");
		if(p2attackState > 0)
		{
			this.player2Active = true;
			this.Player2Text.text = JoinedString;
			this.ChangePlayer2Model();
		}
		else if (p2jumpState > 0)
		{
			this.player2Active = false;
			this.Player2Text.text = JoinString;
			if(this.player2Model != null)
			{
				Destroy(this.player2Model);
			}
		}
	}
	
	private void UpdatePlayerSelection()
	{
		if(this.player1Active)
		{
			float p1 = Input.GetAxis("Player1 Horizontal");
			if(p1 < 0)
			{
				this.player1Selection--;
			}
			if(p1 > 0)
			{
				this.player1Selection++;
			}
			
			if(this.player1Selection > this.PlayerModels.Count - 1)
			{
				this.player1Selection = 0;
				this.ChangePlayer1Model();
			} 
			else if (this.player1Selection < 0)
			{
				this.player1Selection = this.PlayerModels.Count - 1;
				this.ChangePlayer1Model();
			}
		}
		
		if(this.player2Active)
		{
			float p2 = Input.GetAxis("Player2 Horizontal");
			if(p2 < 0)
			{
				this.player2Selection--;
			}
			if(p2 > 0)
			{
				this.player2Selection++;
			}
			
			if(this.player2Selection > this.PlayerModels.Count - 1)
			{
				this.player2Selection = 0;
				this.ChangePlayer2Model();
			} 
			else if (this.player2Selection < 0)
			{
				this.player2Selection = this.PlayerModels.Count - 1;
				this.ChangePlayer2Model();
			}
		}
	}
	
	private void ChangePlayer1Model()
	{
		if(this.player1Model != null)
		{
			Destroy(this.player1Model);
		}
		
		this.player1Model = Instantiate(this.PlayerModels[this.player1Selection], this.player1Position, Quaternion.identity) as GameObject;
		this.player1Model.transform.localScale = new Vector3(1, 1, 1);
		Destroy(this.player1Model.GetComponent<Player>());
		print (this.player1Model.name);
	}
	
	private void ChangePlayer2Model()
	{
		if(this.player2Model != null)
		{
			Destroy(this.player2Model);
		}
		
		this.player2Model = Instantiate(this.PlayerModels[this.player1Selection], this.player1Position, Quaternion.identity) as GameObject;
		this.player2Model.transform.localScale = new Vector3(1, 1, 1);
		Destroy(this.player2Model.GetComponent<Player>());
		this.player2Model.transform.position = this.player2Position;
	}
}
