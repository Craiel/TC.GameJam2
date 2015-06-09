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
			GetComponent<Rigidbody>().AddForce(0f,10f,0f);
		}
		else if(transform.position.y>2.5f)
		{
			
		}
		else if(transform.position.y<1.0)
		{
			GetComponent<Rigidbody>().AddForce(0f,17f,0f);
		}
		else
		{
			GetComponent<Rigidbody>().AddForce(0f,7f,0f);
		}
		
		
		
		
		
		
		if(transform.position.y<1.6f)
		{
			transform.position=new Vector3(transform.position.x,1.6f,transform.position.z);
		}
		
		if(transform.position.y>2.2f)
		{
			transform.position=new Vector3(transform.position.x,2.2f,transform.position.z);
		}
	}
}
