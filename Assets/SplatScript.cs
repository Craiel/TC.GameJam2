using UnityEngine;
using System.Collections;

public class SplatScript : MonoBehaviour {
	
	public float currentScale=0.005f;
	
	// Use this for initialization
	void Start () {
		float startScale=0.01f+Random.Range (0f,0.3f);
		transform.localScale= new Vector3(startScale,0.1f,startScale);
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.localScale += new Vector3(currentScale, 0.0f, currentScale);
		
		if(currentScale>0.0035f)
		{
			currentScale=currentScale*0.85f;
		}
		
		
		if(transform.localScale.x>1.2f)
		{
        	transform.Translate(Vector3.down * Time.deltaTime*0.125f, Space.World);
			
			if(transform.position.y<0.4f)
			{
				Destroy(gameObject);
				Debug.Log ("destroyed");
			}
		}
		
		
	}
}
