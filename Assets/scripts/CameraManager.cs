using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
#region Class Singleton
	private static CameraManager m_Instance = null;

	public static CameraManager Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = (new GameObject("CameraManager")).AddComponent<CameraManager>();
			}
			return m_Instance;
		}
	}
#endregion

#region Properties
	private float m_LeftBorder = 0.0f;
	public float LeftBorder
	{
		get
		{
			return m_LeftBorder;
		}
	}

	
	private float m_RightBorder = 0.0f;
	public float RightBorder
	{
		get
		{
			return m_RightBorder;
		}
	}
	
	private float m_BottomBorder = 0.0f;
	public float BottomBorder
	{
		get
		{
			return m_BottomBorder;
		}
	}
	
	private float m_TopBorder = 0.0f;
	public float TopBorder
	{
		get
		{
			return m_TopBorder;
		}
	}
#endregion
	
	private Vector3 m_BorderTempVector = Vector3.zero;
	
	void Awake()
	{
		UpdateBorders();
	}
	
	// Use this for initialization
	void Start () 
	{
	}
	
	void Update()
	{
		UpdateBorders();
	}
	
	void UpdateBorders()
	{
		//print ("Updating camera borders..");
		m_BorderTempVector.Set(0,0,Camera.main.transform.position.z);
		m_LeftBorder = Camera.main.ViewportToWorldPoint(m_BorderTempVector).x;
		m_TopBorder = Camera.main.ViewportToWorldPoint(m_BorderTempVector).y;
		
		m_BorderTempVector.Set(1,1,Camera.main.transform.position.z);
		m_RightBorder = Camera.main.ViewportToWorldPoint(m_BorderTempVector).x;
		m_BottomBorder = Camera.main.ViewportToWorldPoint(m_BorderTempVector).y;		
		//print ("Borders:" +m_LeftBorder+", "+m_RightBorder+", "+m_TopBorder+", "+m_BottomBorder);
	}
		
}