using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{	
	private static CameraManager instance = null;
	
	private float leftBorder = 0.0f;
	private float rightBorder = 0.0f;
	private float bottomBorder = 0.0f;
	private float topBorder = 0.0f;
	
	private Vector3 borderTempVector = Vector3.zero;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public static CameraManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = (new GameObject("CameraManager")).AddComponent<CameraManager>();
			}
			return instance;
		}
	}
	
	public float LeftBorder
	{
		get
		{
			return leftBorder;
		}
	}	
	
	public float RightBorder
	{
		get
		{
			return rightBorder;
		}
	}	
	
	public float BottomBorder
	{
		get
		{
			return bottomBorder;
		}
	}	
	
	public float TopBorder
	{
		get
		{
			return topBorder;
		}
	}
	
	public void Awake()
	{
		UpdateBorders();
	}
	
	public void Start () 
	{
	}
	
	public void Update()
	{
		UpdateBorders();
	}
	
	// ---------------------------------------------
	// Private
	// ---------------------------------------------
	private void UpdateBorders()
	{
		print ("Updating camera borders..");
		borderTempVector.Set(0,0,Camera.main.transform.position.z);
		leftBorder = Camera.main.ViewportToWorldPoint(borderTempVector).x;
		topBorder = Camera.main.ViewportToWorldPoint(borderTempVector).y;
		
		borderTempVector.Set(1,1,Camera.main.transform.position.z);
		rightBorder = Camera.main.ViewportToWorldPoint(borderTempVector).x;
		bottomBorder = Camera.main.ViewportToWorldPoint(borderTempVector).y;
	}
		
}