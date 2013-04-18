using UnityEngine;
using System.Collections;

public class GUI_Player_HUD : MonoBehaviour {
	
	public Texture2D textureCarridges;
	public int numberOfCarridges;
	
	// Use this for initialization
	void Start () {

	}
	
	void OnGUI() {
		// Some simple labels
		//GUI.DrawTexture(new Rect(Screen.width - 100,0,100,50), textureCarridges);
		
		if (!textureCarridges)
		{
			Debug.LogError("Assign a Texture in the inspector.");
		}	
		
		for (int i = 0; i < numberOfCarridges; ++i)
		{
			GUI.Box(new Rect (Screen.width - i * 100, 0, 100, 50), i.ToString());
		}
		
		GUI.Box (new Rect (0,0,100,50), "Top-left");
		//GUI.Box (new Rect (Screen.width - 100,0,100,50), "Top-right");
		GUI.Box (new Rect (0,Screen.height - 50,100,50), "Bottom-left");
		GUI.Box (new Rect (Screen.width - 100,Screen.height - 50,100,50), "Bottom-right");		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
