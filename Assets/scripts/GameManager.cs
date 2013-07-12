using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public GameObject m_Ship = null;
	public GameObject m_EnemyShip = null;
	public Vector3 m_ShipSpawnPosition = Vector3.zero;
	
	public Rect m_LivesRect;
	public Rect m_ScoreRect;
	
	public float m_WaveDuration = 10.0f;
	
	public TextMesh m_MessageText = null;
	bool m_IsAnimatingMessage = false;
	float m_TextAnimationTime = 2.0f;
	float m_TextAnimationStartTime = 0.0f;
	
	private int m_Score = 0;
	public int Score
	{
		get{return m_Score;}
		set{m_Score = value;}
	}
	
	private int m_Wave = 1;
	public int Wave
	{
		get{return m_Wave;}
	}
	
	private int m_Lives = 1;
	public int Lives
	{
		get{return m_Lives;}
	}
	
	private int m_MaxLives = 3;
	
	private bool m_IsPlaying = false;
	
	private float m_TimeUntilNextWave = 0.0f;
	
	private Player m_Player = null;
	
	private bool m_IsAnimatingEntry = false;
	private float m_EntryStartTime = 0.0f;
	private float m_EntryDuration = 3.0f;
	private Vector3 m_EntryStartPoint = Vector3.zero;
	
	//Enemy Spawning
	public GameObject ShotHolder;
	public GameObject EnemyHolder;
	
	private const int ENEMY_COUNT = 10;
	private const int MAX_SHOTS = 500;
	
	private GameObject[] m_Enemies = new GameObject[ENEMY_COUNT];
	private List<GameObject> shots = new List<GameObject>();	
		
	void Start()
	{
		m_Player = m_Ship.GetComponent<Player>();
		m_Player.OnDying += OnPlayerDying;
	}
	
	private void InitEnemies()
	{
		for(int i=0;i<ENEMY_COUNT;i++)
		{
			this.m_Enemies[i] = Instantiate(m_EnemyShip) as GameObject;
			this.m_Enemies[i].transform.parent = this.EnemyHolder.transform;
			this.m_Enemies[i].AddComponent<Enemy>();
			//this.m_Enemies[i].AddComponent<BoxCollider>();
			this.m_Enemies[i].GetComponent<BoxCollider>().isTrigger = true;
			this.m_Enemies[i].AddComponent<Rigidbody>();
			this.m_Enemies[i].GetComponent<Rigidbody>().useGravity = false;
			this.m_Enemies[i].GetComponent<Rigidbody>().isKinematic = true;
			this.m_Enemies[i].GetComponent<Enemy>().CollisionDamage = Random.value * 5;
			this.m_Enemies[i].GetComponent<Enemy>().CollisionInterval = 0.1f;
			this.m_Enemies[i].name = "Enemy "+i;
			
			var weapon = WeaponSingleDumbFire.Create();
			weapon.GetComponent<Weapon>().Cooldown = 2f;
			weapon.GetComponent<Weapon>().Source = ShotSource.Foe;
			this.m_Enemies[i].GetComponent<Enemy>().AddWeapon(weapon);
			
			this.ResetEnemy(i);
		}
	}
	
	public int GetDifficulty(int wave)
	{
		//TODO: Tweak actual difficulty curve not to be linear. Consider batching waves of non-linear curve
		return wave;
	}
	
	public void StartGame()
	{
		m_Ship.transform.localPosition = m_ShipSpawnPosition;
		m_Wave = 0;
		AnimateEntry();
	}
	
	void Update()
	{
		if(m_IsAnimatingEntry)
		{
			AnimateEntry();
		}
		
		if(m_IsPlaying)
		{
			UpdateWave();
			UpdateEnemies();
			UpdateProjectiles();
		}
		
		if(m_IsAnimatingMessage)
		{
			AnimateText();
		}
		
		HandleInput();		
	}
	
	void UpdateEnemies()
	{
		for(int i=0;i<ENEMY_COUNT;i++)
		{
			Enemy current = this.m_Enemies[i].GetComponent<Enemy>();
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
		}
	}	
	
	void UpdateProjectiles()
	{
		IList<GameObject> activeShots = new List<GameObject>(this.shots);
		foreach(GameObject obj in activeShots)
		{			
			Shot current = obj.GetComponent<Shot>();
			if(current.LifeTime <= 0)
			{				
				this.shots.Remove(obj);
				DestroyObject(obj);
			}
		}
	}
	
	void HandleInput()
	{
		if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		else if(Input.GetKeyDown(KeyCode.R))
		{
			m_IsPlaying = false;
			m_Lives = m_MaxLives;
			m_TimeUntilNextWave = 0.0f;
			StartGame();
		}
	}
	
	void UpdateWave()
	{
		m_TimeUntilNextWave -= Time.deltaTime;
		if(m_TimeUntilNextWave < 0.0f)
		{
			SpawnWave();
		}
	}
	
	void SpawnWave()
	{
		
		m_Wave++;
		ShowText("Wave " + m_Wave);
		m_TimeUntilNextWave = m_WaveDuration;
		
		//TODO: Select next wave from enemy types and difficulty budget
	}
	
	
	
	void OnGUI()
	{
		if(m_IsPlaying)
		{
			GUI.TextArea(m_LivesRect,"Lives: " + m_Lives);
			m_ScoreRect.width = 60 + 5*m_Score.ToString().Length;
			GUI.TextArea(m_ScoreRect, "Score: " + m_Score);
		}
	}
	
	public void OnPlayerDying()
	{
		m_Lives--;
		if(m_Lives == 0)
		{
			ShowText("GAME OVER	\nr to restart", false);
		}
		else
		{
			m_Player.Health = m_Player.MaxHealth;
			StartGame();
		}
	}
	
	public void AnimateEntry()
	{
		if(!m_IsAnimatingEntry)
		{
			m_EntryStartPoint = m_ShipSpawnPosition - new Vector3(0,50,0);
			m_IsAnimatingEntry = true;
			m_EntryStartTime = Time.time;
		}
		
		Vector3 entryPath = (m_ShipSpawnPosition - m_EntryStartPoint);
		
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
		}
	}
	
	void ShowText(string text, bool fade=true)
	{
		m_MessageText.text = text;
		if(fade)
		{
			m_TextAnimationStartTime = Time.time;
			m_IsAnimatingMessage = true;
		}
		else
		{
			m_IsAnimatingMessage = false;
			m_MessageText.renderer.material.color = new Color(m_MessageText.renderer.material.color.r, 
				m_MessageText.renderer.material.color.g, 
				m_MessageText.renderer.material.color.b, 
				1);
		}
	}
	
	void HideText()
	{
		m_MessageText.text = "";
	}
		
	
	public void AnimateText()
	{
		if(Time.time - m_TextAnimationStartTime > m_TextAnimationTime)
		{
			m_MessageText.text = "";
			m_MessageText.renderer.material.color = new Color(m_MessageText.renderer.material.color.r, 
				m_MessageText.renderer.material.color.g, 
				m_MessageText.renderer.material.color.b, 
				1);
			m_IsAnimatingMessage = false;
		}
		else
		{
			m_MessageText.renderer.material.color = new Color(m_MessageText.renderer.material.color.r, 
				m_MessageText.renderer.material.color.g, 
				m_MessageText.renderer.material.color.b, 
				1.0f - (Time.time - m_TextAnimationStartTime)/m_TextAnimationTime);
		}
	}
	
	public void AddShot(GameObject newShot)
	{
		newShot.transform.parent = this.ShotHolder.transform;
		if(this.shots.Count > MAX_SHOTS)
		{
			var oldShot = this.shots[0];
			this.shots.RemoveAt(0);
			DestroyObject(oldShot);
		}
		
		newShot.GetComponent<Shot>().IsActive = true;
		this.shots.Add(newShot);
	}
	
	public List<GameObject> GetShotsWithin(Vector3 center, float radius)
	{
		List<GameObject> result = new List<GameObject>();
		List<GameObject> list = new List<GameObject>(this.shots);
		for(int i=0;i<list.Count;i++)
		{
			if(list[i].GetComponent<Shot>().LifeTime <= 0 || list[i].GetComponent<Shot>().Source == ShotSource.Friend)
			{
				continue;
			}
		
			if((list[i].collider.bounds.center - center).magnitude < radius)
			{
				result.Add(list[i]);
			}
		}
		
		print("In Well: "+result.Count);
		return result;
	}
	
	private void ResetEnemy(int slot)
	{
		float pos = Random.Range(CameraManager.Instance.LeftBorder, CameraManager.Instance.RightBorder)*0.9f;
		Debug.Log ("POS: " + pos);
		this.m_Enemies[slot].GetComponent<Enemy>().Initialize(new Vector3(pos, CameraManager.Instance.TopBorder, -75.0f), 15.0f, 20.0f + Random.value * 20.0f);
		this.m_Enemies[slot].GetComponent<Enemy>().SetCollision(5.0f, 0.2f);
		this.m_Enemies[slot].GetComponent<Enemy>().Health = 1.0f + Random.value * 10.0f;
	}
}
