using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{	
	public GameManager gameManager = null;
	
	// Use this for initialization
	void Start () {
		
		// Get rid of the cursor and lock it
		Screen.lockCursor = true;
		Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			this.gameManager.StartGame();
			gameObject.SetActive(false);
		}
	}
	
	
}
