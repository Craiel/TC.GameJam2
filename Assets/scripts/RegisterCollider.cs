using UnityEngine;
using System.Collections;

public class RegisterCollider : MonoBehaviour
{
	void Start () 
	{	
		CapsuleCollider collider = this.collider as CapsuleCollider;
		if(collider != null)
		{
			Camera.main.GetComponent<GameManager>().CollidingGeometry.Add(this.gameObject);
		}
	}
}
