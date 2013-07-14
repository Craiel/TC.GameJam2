using UnityEngine;
using System.Collections;

public class DragObject : MonoBehaviour
{
	public float Pull = 1.0f;
	
	void Start () 
	{	
		BoxCollider collider = this.collider as BoxCollider;
		if(collider != null)
		{
			Camera.main.GetComponent<GameManager>().DragEntries.Add(this.gameObject);
		}
	}
}
