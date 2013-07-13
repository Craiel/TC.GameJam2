using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {
	
	private Camera mainCamera;
	private Plane[] planes;
		
	private GameObject leftCollider;
	private GameObject rightCollider;
	private GameObject topCollider;
	private GameObject bottomCollider;
	
	private Vector3 lastGoodPosition;
		
	private float leftBorder = 0.0f;
	private float rightBorder = 0.0f;
	private float topBorder = 0.0f;
	private float bottomBorder = 0.0f;
		
	private Vector3 tempVector;
	
	private float timeUntilUpdate = 0.01f;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public float Sensitivity = 0.0001f;	
	public float EdgeBuffer = 1.0f;
			
	// Use this for initialization
	void Start ()
	{		
		this.mainCamera = Camera.main;
		this.planes = GeometryUtility.CalculateFrustumPlanes(this.mainCamera);
		this.leftCollider = this.GetColliderPlane(this.planes[0]);
		this.rightCollider = this.GetColliderPlane(this.planes[1]);
		this.topCollider = this.GetColliderPlane(this.planes[2]);
		this.bottomCollider = this.GetColliderPlane(this.planes[3]);
		
		this.UpdateBorders();
	}	
		
	// Update is called once per frame
	void Update ()
	{
		float newX = Input.GetAxis("Mouse X");
		float newY = Input.GetAxis("Mouse Y");
		
		timeUntilUpdate -= Time.deltaTime;
		if(timeUntilUpdate <= 0)
		{
			timeUntilUpdate = 0.01f;
		}
		
		transform.Translate(new Vector3(newX * Sensitivity, newY * Sensitivity, 0), null);
	}
	
	// ---------------------------------------------
	// Private
	// ---------------------------------------------
	private GameObject GetColliderPlane(Plane plane)
	{
		GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
		p.name = "Collider Plane";
  		p.transform.position = -plane.normal * plane.distance;
  		p.transform.rotation = Quaternion.FromToRotation(Vector3.up, plane.normal);
  		p.transform.localScale = Vector3.one * 20.0f;
		return p;
	}
	
	private void UpdateBorders()
	{
		print ("Updating camera borders..");
		this.tempVector.Set(0,0,Camera.main.transform.position.z);
		this.leftBorder = Camera.main.ViewportToWorldPoint(this.tempVector).x;
		this.topBorder = Camera.main.ViewportToWorldPoint(this.tempVector).y;
		
		this.tempVector.Set(1,1,Camera.main.transform.position.z);
		this.rightBorder = Camera.main.ViewportToWorldPoint(this.tempVector).x;
		this.bottomBorder = Camera.main.ViewportToWorldPoint(this.tempVector).y;		
	}	
}
