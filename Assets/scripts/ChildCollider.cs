using UnityEngine;
using System.Collections;

public class ChildCollider : MonoBehaviour 
{
	private CharacterEntity host;
	
	public GameObject Parent;
	
	public void Start()
	{
		this.host = this.Parent.GetComponent<CharacterEntity>();
	}
	
	public void OnTriggerEnter(Collider collider)
	{
		if(this.host == null)
		{
			return;
		}
		
		print ("ChildTrigger -> going to parent");
		this.host.OnChildCollision(collider);
	}
	
	public void OnTriggerStay(Collider collider)
	{
		if(this.host == null)
		{
			return;
		}
		
		this.host.OnChildCollisionStay(collider);
	}
}
