using UnityEngine;
using System.Collections;

public class HitIndicatorScript : MonoBehaviour {
	
	public int Timer=3;
	public GameObject ballObject;

	// Use this for initialization
	void Start () {
		int i;
			
		for(i=0;i<7;i++)
		{
			float randX = transform.position.x-0.01f+Random.Range (0,0.02f);
			float randY = transform.position.y-0.01f+Random.Range (0,0.02f);
			float randZ = transform.position.z-0.01f+Random.Range (0,0.02f);
			var randV = new Vector3(randX,randY,randZ);
			var newball = (GameObject)Instantiate(ballObject,randV,transform.rotation);
			var rigidball = newball.GetComponent <Rigidbody>();
			rigidball.AddForce (-15+Random.Range(0,30),Random.Range(0,10),-15+Random.Range(0,30));
			
			Destroy(rigidball.gameObject, 5);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		Timer-=1;
		
		if(Timer<=0)
		{
			Destroy (gameObject);
		}
	}
}
