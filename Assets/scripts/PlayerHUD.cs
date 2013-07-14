using UnityEngine;
using System.Collections;

public class PlayerHUD : MonoBehaviour 
{
	private Player player;
	
	private int current;
	private int max = 100;
	private int progressHeight = 12;
	
	private bool needUpdate = true;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public Vector2 Position = new Vector2(0, 0);
	public int Width = 300;
	
	public int Border = 5;	
	
	public Texture2D LeftTexture;
	public Texture2D RightTexture;
	public Texture2D FullTexture;
	public Texture2D EmptyTexture;
	
	public GameObject Player;
		
	public void Start()
	{
		this.player = this.Player.GetComponent<Player>();
	}
	
	public void OnGUI()
	{
		GUI.BeginGroup(new Rect(this.Position.x, this.Position.y, this.Width, 60));
 
		if(this.player.Portrait != null)
		{
			GUI.DrawTexture(new Rect(4, 4, 50, 50), this.player.Portrait);
		}
		
		GUI.TextArea(new Rect(70, 6, 80, 22), this.player.name);
		GUI.TextArea(new Rect(this.Width - 60, 6, 50, 22), this.player.Score.ToString());
		
		int progressWidth = this.Width - 55;
		GUI.BeginGroup(new Rect(55, 32, this.Width - 20, this.progressHeight));
		float percent = (float)this.player.Health / (float)this.player.MaxHealth;
		int width = (int)((float)progressWidth * percent);
		
		GUI.DrawTexture(new Rect(Border, 0, progressWidth - (this.Border * 2), this.progressHeight), this.EmptyTexture, ScaleMode.StretchToFill );
		if(percent > 0)
		{
 			GUI.DrawTexture(new Rect(Border, 0, width - (this.Border * 2), this.progressHeight), this.FullTexture, ScaleMode.StretchToFill ); 
		}
		
		GUI.DrawTexture(new Rect(0, 0, this.LeftTexture.width, this.progressHeight), this.LeftTexture);
		GUI.DrawTexture(new Rect(progressWidth - this.RightTexture.width, 0, this.RightTexture.width, this.progressHeight), this.RightTexture);
		GUI.EndGroup();
		
		GUI.EndGroup();
	}
}
