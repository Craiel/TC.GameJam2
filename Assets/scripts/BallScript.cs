using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	public GameObject ballObject;
	
	// Use this for initialization
	void Start () {
		
		//rigidbody.AddForce (100, 0, 100);
		
		var newball = Instantiate(ballObject,transform.position,transform.rotation);
	
	}
	
	void FixedUpdate () {
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
