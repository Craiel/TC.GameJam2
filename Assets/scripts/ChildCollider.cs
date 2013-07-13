using UnityEngine;
using System.Collections;

public class ChildCollider : MonoBehaviour 
{
	public GameObject Parent;
	
	public void OnTriggerEnter(Collider collider)
	{
		print("ChildOnTriggerEnter: " +this.name+" " +collider.name);
	}
	
	public void OnTriggerStay(Collider collider)
	{
		print("ChildOnTriggerStay: " +this.name+" " +collider.name);
	}
}
