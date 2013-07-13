using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	private Player player1 = null;
	private Player player2 = null;
	
	private bool isAnimating = false;
	private float animatingStartTime = 0.0f;
	private float animatingDuration = 3.0f;
	private Vector3 animatingStartPoint = Vector3.zero;
	
	private const int ENEMY_COUNT = 10;
	private Enemy[] enemies = new Enemy[ENEMY_COUNT];
	private List<StageEntity> destroyableObjects = new List<StageEntity>();
	
	private int maxLives = 3;	
	
	private bool isAnimatingMessage = false;
	private float textAnimationTime = 2.0f;
	private float textAnimationStartTime = 0.0f;
	
	private GameObject Player1Model;
	private GameObject Player2Model;
	
	public Vector3 SpawnPositionPlayer1 = Vector3.zero;
	public Vector3 SpawnPositionPlayer2 = Vector3.zero;
	
	public Rect LivesRect;
	public Rect ScoreRect;
		
	public TextMesh MessageText = null;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------		
	public void Start()
	{
		print ("Starting");
		if(SceneState.Instance.Player1 != null)
		{
			this.Player1Model = Instantiate(Resources.Load(SceneState.Instance.Player1), this.SpawnPositionPlayer1, Quaternion.identity) as GameObject;
			this.player1 = this.Player1Model.GetComponent<Player>();
			this.player1.CameraFollows = true;
			this.player1.OnDying += OnPlayerDying;
		}
		
		if(SceneState.Instance.Player2 != null)
		{
			this.Player2Model = Instantiate(Resources.Load(SceneState.Instance.Player2), this.SpawnPositionPlayer2, Quaternion.identity) as GameObject;
			this.player2 = this.Player2Model.GetComponent<Player>();
			this.player2.OnDying += OnPlayerDying;
			this.player2.ControlPrefix = "Player2";
		}
			
		print ("animate");
		this.Animate();
	}
	
	public void Update()
	{
		if(this.isAnimating)
		{
			this.Animate();
		}
		
		this.UpdateEnemies();
		
		if(this.isAnimatingMessage)
		{
			this.AnimateText();
		}
		
		this.HandleInput();
	}
	
	void UpdateEnemies()
	{
		/*for(int i=0;i<ENEMY_COUNT;i++)
		{
			Enemy current = this.enemies[i];
			if(current.IsDead)
			{
				m_Score += (int)current.MaxHealth;
			}
			
			if(current.LifeTime <= 0 || current.IsDead)
			{
				this.ResetEnemy(i);
				continue;
			}
			
			if(Random.value < 0.2f)
			{
				current.Fire(true, (m_Ship.transform.position - current.transform.position).normalized);
			}
		}*/
	}	
		
	void HandleInput()
	{
		if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
		
	void OnGUI()
	{
		if(!this.isAnimating)
		{
			/*GUI.TextArea(this.LivesRect,"Lives: " + this.player1.Lives);
			this.ScoreRect.width = 60 + 5*this.player1.Score.ToString().Length;
			GUI.TextArea(this.ScoreRect, "Score: " + this.player1.Score);*/
		}
	}
	
	public void OnPlayerDying()
	{
		if((this.player1 == null || this.player1.IsDead) && (this.player2 == null || this.player2.IsDead))
		{
			ShowText("GAME OVER	\nr to restart", false);
		}
	}
	
	public void Animate()
	{
		if(!this.isAnimating)
		{
			return;
		}
		
		/*Vector3 entryPath = (m_ShipSpawnPosition - m_EntryStartPoint);
		
		if(Time.time - m_EntryStartTime > m_EntryDuration)
		{
			m_Ship.transform.localPosition = m_ShipSpawnPosition;
			m_IsAnimatingEntry = false;
			m_Ship.GetComponent<Control>().enabled = true;
			m_IsPlaying = true;
			InitEnemies();
		}
		else
		{
			m_Ship.transform.localPosition = m_EntryStartPoint +  entryPath * ((Time.time - m_EntryStartTime)/m_EntryDuration);
		}*/
	}
	
	void ShowText(string text, bool fade=true)
	{
		this.MessageText.text = text;
		if(fade)
		{
			this.textAnimationStartTime = Time.time;
			this.isAnimatingMessage = true;
		}
		else
		{
			this.isAnimatingMessage = false;
			this.MessageText.renderer.material.color = new Color(this.MessageText.renderer.material.color.r, 
				this.MessageText.renderer.material.color.g, 
				this.MessageText.renderer.material.color.b, 
				1);
		}
	}
	
	void HideText()
	{
		this.MessageText.text = "";
	}
		
	
	public void AnimateText()
	{
		if(Time.time - this.textAnimationStartTime > this.textAnimationTime)
		{
			this.MessageText.text = "";
			this.MessageText.renderer.material.color = new Color(this.MessageText.renderer.material.color.r, 
				this.MessageText.renderer.material.color.g, 
				this.MessageText.renderer.material.color.b, 
				1);
			this.isAnimatingMessage = false;
		}
		else
		{
			this.MessageText.renderer.material.color = new Color(this.MessageText.renderer.material.color.r, 
				this.MessageText.renderer.material.color.g, 
				this.MessageText.renderer.material.color.b, 
				1.0f - (Time.time - this.textAnimationStartTime)/this.textAnimationTime);
		}
	}
}
