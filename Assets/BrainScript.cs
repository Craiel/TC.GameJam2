using UnityEngine;
using System.Collections;

public class BrainScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if(transform.position.y<1.9f)
		{
			//rigidbody.AddForce(0f,5f+Random.Range (0f,5f),0f);
			rigidbody.AddForce(0f,13f,0f);
		}
		else if(transform.position.y>2.5f)
		{
			
		}
		else if(transform.position.y<1.0)
		{
			rigidbody.AddForce(0f,17f,0f);
		}
		else
		{
			rigidbody.AddForce(0f,7f,0f);
		}
		
		if(transform.position.y<0.5f)
		{
			transform.position=new Vector3(transform.position.x,0.5f,transform.position.z);
		}
	}
}
