using UnityEngine;
using System.Collections;

public class GUI_Player_HUD : MonoBehaviour {
	
	public Texture2D[] textureCarridges = new Texture2D[3];
	public int numberOfCarridges;
	
	private bool isMenu = false;
	private float menuBoxSize = 200;
	private float menuButtonIndent = 50;
	
	// Use this for initialization
	void Start () {

	}
	
	void OnGUI() {
		
		for (int i = 0; i < numberOfCarridges; ++i)
		{
			GUI.DrawTexture(new Rect (Screen.width - i * 100, 0, 100, 100), textureCarridges[0]);
			//GUI.Box(new Rect (Screen.width - i * 100, 0, 100, 50), i.ToString());
		}
			
		if (isMenu)
		{
			GUI.Box(new Rect(Screen.width / 2 - (menuBoxSize / 2), Screen.height / 2 - (menuBoxSize / 2), 
				menuBoxSize, menuBoxSize), "Game Menu");
			
			GUI.Button(new Rect(Screen.width / 2 - (menuBoxSize / 2) + menuButtonIndent, Screen.height / 2 - (menuBoxSize / 2) + menuButtonIndent,
				100, 50), "Leave Game");
		}	
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Menu
		if (Input.GetKey(KeyCode.Escape))
		{
			print ("We have hit the menu key");
			//Application.Quit();
			//Debug.Break();
			isMenu = true;
			Screen.showCursor = true;
		}
	}
}
