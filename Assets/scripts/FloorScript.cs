using UnityEngine;
using System.Collections;

public class FloorScript : MonoBehaviour {
	
	public GameObject splatObject;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// Destroy everything that enters the trigger
    void OnTriggerEnter(Collider other) {
        
		Debug.Log ("Disappeared!");
		
		var splatpos = new Vector3(other.attachedRigidbody.transform.position.x,0.5f,other.attachedRigidbody.transform.position.z);
		var splatrot = new Quaternion(0,0,0,0);
		
		var newsplat = (GameObject)Instantiate (splatObject,splatpos,splatrot);
		
		Destroy(other.gameObject);
    }
}
