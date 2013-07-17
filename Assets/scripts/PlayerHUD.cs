using UnityEngine;
using System.Collections;

public class PlayerHUD : MonoBehaviour 
{
	private Player player;
	
	private int current;
	private int max = 100;
	private int progressHeight = 12;
	
	// ---------------------------------------------
	// Public
	// ---------------------------------------------
	public int Width = 300;
	
	public int Border = 5;	
	
	public Texture2D LeftTexture;
	public Texture2D RightTexture;
	public Texture2D FullTexture;
	public Texture2D EmptyTexture;
	
	public GameObject Player;
		
	public void Start()
	{
		if(this.Player != null)
		{
			this.player = this.Player.GetComponent<Player>();
		}
	}
	
	public void OnGUI()
	{
		Vector2 position = new Vector2(Screen.width * this.transform.position.x, Screen.height * this.transform.position.y);
		if(this.guiText.alignment == TextAlignment.Right)
		{
			position.x = Screen.width - this.Width;
		}
		
		if(this.player == null)
		{
			GUI.BeginGroup(new Rect(position.x, position.y, this.Width, 60));
			GUI.TextArea(new Rect(20, 8, 180, 30), "Press attack to join");
			GUI.EndGroup();
			return;
		}
		
		GUI.BeginGroup(new Rect(position.x, position.y, this.Width, 160));
 
		if(this.player.Portrait != null)
		{
			GUI.DrawTexture(new Rect(4, 4, 50, 50), this.player.Portrait);
		}
		
		GUI.TextArea(new Rect(70, 6, 80, 22), this.player.name);
		GUI.TextArea(new Rect(this.Width - 60, 6, 50, 22), this.player.Score.ToString());
		
		int progressWidth = this.Width - 55;	
		GUI.BeginGroup(new Rect(55, 32, this.Width - 20, this.progressHeight));
		this.DrawHealthBar(this.player, progressWidth, this.Width);
		GUI.EndGroup();
		
		if(this.player.CurrentTarget != null)
		{
			var target = this.player.CurrentTarget.GetComponent<CharacterEntity>();
			if(target.Portrait != null)
			{
				GUI.DrawTexture(new Rect(4, 58, 50, 50), target.Portrait);
			}
					
			GUI.TextArea(new Rect(70, 60, 80, 22), target.name);
			GUI.BeginGroup(new Rect(55, 84, this.Width - 20, this.progressHeight));
			this.DrawHealthBar(target, 100, 200);
			GUI.EndGroup();
		}
		
		GUI.EndGroup();
	}
	
	private void DrawHealthBar(CharacterEntity target, int progressWidth, int totalWidth)
	{
		float percent = (float)target.Health / (float)target.MaxHealth;
		int width = (int)((float)progressWidth * percent);
		
		GUI.DrawTexture(new Rect(Border, 0, progressWidth - (this.Border * 2), this.progressHeight), this.EmptyTexture, ScaleMode.StretchToFill );
		if(percent > 0)
		{
 			GUI.DrawTexture(new Rect(Border, 0, width - (this.Border * 2), this.progressHeight), this.FullTexture, ScaleMode.StretchToFill ); 
		}
		
		GUI.DrawTexture(new Rect(0, 0, this.LeftTexture.width, this.progressHeight), this.LeftTexture);
		GUI.DrawTexture(new Rect(progressWidth - this.RightTexture.width, 0, this.RightTexture.width, this.progressHeight), this.RightTexture);
	}
}
