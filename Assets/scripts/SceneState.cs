using UnityEngine;
using System.Collections;

public class SceneState : MonoBehaviour
{
	private static SceneState instance = null;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public static SceneState Instance
	{
		get
		{
			if(instance == null)
			{
				print ("initializing SceneState");
				instance = (new GameObject("SceneState")).AddComponent<SceneState>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	
	public string Player1 = null;
	public string Player2 = null;
}
