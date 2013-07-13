using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	public GameObject ballObject;
	
	// Use this for initialization
	void Start () {
		int i;
		
		/*for(i=0;i<40;i++)
		{
			//var randV = Vector3(transform.position-0.01
			var newball = (GameObject)Instantiate(ballObject,transform.position,transform.rotation);
			var rigidball = newball.GetComponent <Rigidbody>();
			rigidball.AddForce (-15+Random.Range(0,30),Random.Range(0,10),-15+Random.Range(0,30));
		}*/
	
	}
	
	void FixedUpdate () {
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
