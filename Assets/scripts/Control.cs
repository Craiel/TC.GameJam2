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
	
	private float bankingX;
	private float bankingY;
	
	private Vector3 tempVector;
	
	private float timeUntilUpdate = 0.01f;
	
	private Quaternion originRotation;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public float Sensitivity = 0.0001f;	
	public float EdgeBuffer = 1.0f;
	
	public float BankingFactor = 1.0f;
	public float BankingFalloff= 0.05f;
	public float BankingMax = 45.0f;
		
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
		
		this.originRotation = transform.rotation;
	}	
		
	// Update is called once per frame
	void Update ()
	{		
		float newX = Input.GetAxis("Mouse X");
		float newY = Input.GetAxis("Mouse Y");
		
		timeUntilUpdate -= Time.deltaTime;
		if(timeUntilUpdate <= 0)
		{
			this.FalloffBanking();
			timeUntilUpdate = 0.01f;
		}
		
		transform.rotation = Quaternion.AngleAxis(-this.bankingX, Vector3.up) * Quaternion.AngleAxis(-this.bankingY, Vector3.left) * this.originRotation;
		//transform.Rotate(Vector3.left, this.bankingY);
						
		this.CheckBoundsManaged(ref newX, ref newY);
		
		//print ("[0] = Stat: "+newX+"x"+newY+" | "+newX+"x"+newY);
		
		//this.CheckBounds(ref newX, ref newY);
		
		//print ("[1] = Stat: "+xMovement+"x"+yMovement+" | "+this.currentX+"x"+this.currentY+" | "+newX+"x"+newY);
				
		this.bankingX += (newX * this.BankingFactor);
		this.bankingY += (newY * this.BankingFactor);		
			
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
	
	private void FalloffBanking()
	{
		if(this.bankingX > 0.0f)
		{
			this.bankingX -= this.BankingFalloff;
			if(this.bankingX < 0.0f) { this.bankingX = 0.0f; }
		}
		
		if(this.bankingX < 0.0f)
		{
			this.bankingX += this.BankingFalloff;
			if(this.bankingX > 0.0f) { this.bankingX = 0.0f; }
		}
		
		if(this.bankingY > 0.0f)
		{
			this.bankingY -= this.BankingFalloff;
			if(this.bankingY < 0.0f) { this.bankingY = 0.0f; }
		}
		
		if(this.bankingY < 0.0f)
		{
			this.bankingY += this.BankingFalloff;
			if(this.bankingY > 0.0f) { this.bankingY = 0.0f; }
		}
				
		this.bankingX = Mathf.Clamp(this.bankingX, -this.BankingMax, this.BankingMax);
		this.bankingY = Mathf.Clamp(this.bankingY, -this.BankingMax, this.BankingMax);
	}
	
	private void CheckBoundsManaged(ref float xMovement, ref float yMovement)
	{
		if(yMovement > 0.0f && (this.transform.position.y + yMovement + this.renderer.bounds.extents.y) > this.topBorder)
		{
			yMovement = 0;
			print ("clamping ypos");
		}
		
		if(yMovement < 0.0f && (this.transform.position.y + yMovement + this.renderer.bounds.extents.y) < this.bottomBorder)
		{
			yMovement = 0;
			print ("clamping yneg");
		}
		
		if(xMovement > 0.0f && (this.transform.position.x + xMovement + this.renderer.bounds.extents.x) > this.leftBorder)
		{
			xMovement = 0;
			print ("clamping xpos");
		}
		
		if(xMovement < 0.0f && (this.transform.position.x + xMovement + this.renderer.bounds.extents.x) < this.rightBorder)
		{
			xMovement = 0;
			print ("clamping xneg");
		}
	}
	
	// Code for frustum exact border checking, currently not used (and not finished!)
	/*private void CheckBounds(ref float xMovement, ref float yMovement)
	{
		if(xMovement < 0.0f)
		{
		Vector3 closestPoint = this.leftCollider.collider.ClosestPointOnBounds(transform.position);
		Vector3 relativePoint = transform.InverseTransformPoint(this.leftCollider.transform.position);		
		float distance = Vector3.Distance(this.leftCollider.transform.position, transform.position);
		print ("DistX: " + distance + " rel: "+relativePoint.x + " mov: "+xMovement);
		if((relativePoint.x > 0.0f || distance < this.EdgeBuffer) && xMovement < 0.0f)
		{
			print ("clipping!");
			xMovement = 0;
		}
		}
		
		if(xMovement > 0.0f)
		{
		Vector3 closestPoint = this.rightCollider.collider.ClosestPointOnBounds(transform.position);
		Vector3 relativePoint = transform.InverseTransformPoint(this.rightCollider.transform.position);		
		float distance = Vector3.Distance(this.rightCollider.transform.position, transform.position);
		print ("-DistX: " + distance + " rel: "+relativePoint.x + " mov: "+xMovement);
		if((relativePoint.x < 0.0f || distance < this.EdgeBuffer) && xMovement > 0.0f)
		{
			print ("clipping!");
			xMovement = 0;
		}
		}
		
		if(yMovement < 0.0f)
		{
		Vector3 closestPoint = this.topCollider.collider.ClosestPointOnBounds(transform.position);
		Vector3 relativePoint = transform.InverseTransformPoint(this.topCollider.transform.position);		
		float distance = Vector3.Distance(this.topCollider.transform.position, transform.position);
		print ("DistY: " + distance + " rel: "+relativePoint.y + " mov: "+yMovement);
		if((relativePoint.y > 0.0f || distance < this.EdgeBuffer) && yMovement < 0.0f)
		{
			print ("clipping!");
			yMovement = 0;
		}
		}
		
		if(yMovement > 0.0f)
		{
		Vector3 closestPoint = this.topCollider.collider.ClosestPointOnBounds(transform.position);
		Vector3 relativePoint = transform.InverseTransformPoint(this.topCollider.transform.position);		
		float distance = Vector3.Distance(this.topCollider.transform.position, transform.position);
		print ("-DistY: " + distance + " rel: "+relativePoint.y + " mov: "+yMovement);
		if((relativePoint.y < 0.0f || distance < this.EdgeBuffer) && yMovement > 0.0f)
		{
			print ("clipping!");
			yMovement = 0;
		}
		}
	}*/
}
