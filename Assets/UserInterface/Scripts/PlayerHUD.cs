using UnityEngine;
using System.Collections;

public class PlayerHUD : MonoBehaviour {
	
	public Texture2D[] textureCarridges = new Texture2D[3];
	public Texture2D textureWeaponShotgun;
	public int numberOfCarridges = 1;
	
	private bool isMenu = false;
	private float menuBoxSize = 200;
	private float menuBoxWidth = 200;
	private float menuBoxHeight = 250;
	private float menuButtonIndent = 50;
	
	public int NumberOfCarridges
	{
		get{ return numberOfCarridges; }
		set{ numberOfCarridges = value; }
	}
	
	// Use this for initialization
	void Start () {

	}
	
	void OnGUI() {
		
		for (int i = 0; i < numberOfCarridges; ++i)
		{
			GUI.DrawTexture(new Rect (Screen.width - ((i * 100) + 100), 0, 100, 100), textureCarridges[0]);
			//GUI.Box(new Rect (Screen.width - i * 100, 0, 100, 50), i.ToString());
		}
			
		if (isMenu)
		{
			GUI.Box(new Rect(Screen.width / 2 - (menuBoxWidth / 2), Screen.height / 2 - (menuBoxSize / 2), 
				menuBoxSize, menuBoxSize), "Game Menu");
			
			GUI.Button(new Rect(Screen.width / 2 - (menuBoxWidth / 2) + menuButtonIndent, Screen.height / 2 - (menuBoxHeight / 2) + menuButtonIndent,
				100, 50), "Leave Game");
			
			if (GUI.Button (new Rect(Screen.width / 2 - (menuBoxWidth / 2) + menuButtonIndent, Screen.height / 2 - (menuBoxHeight / 2) + menuButtonIndent * 3,
				100, 50), "Continue"))
			{
				isMenu = false;	
			}
		}
		
		DrawWeapons();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Menu
		if (Input.GetKeyUp(KeyCode.Escape) && isMenu == false)
		{
			//Application.Quit();
			//Debug.Break();
			isMenu = true;
			Screen.showCursor = true;
		}
		else if (Input.GetKeyUp(KeyCode.Escape) && isMenu == true)
		{
			isMenu = false;
			Screen.showCursor = false;	
		}
	}
	
	private void DrawWeapons()
	{
		GUI.DrawTexture(new Rect((textureWeaponShotgun.width / 2) - 100, Screen.height - (textureWeaponShotgun.height / 2),
			(textureWeaponShotgun.width / 2), (textureWeaponShotgun.height / 2)), textureWeaponShotgun);
	}
}
