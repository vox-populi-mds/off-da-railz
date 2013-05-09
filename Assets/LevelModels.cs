using UnityEngine;
using System.Collections;

public class LevelModels : MonoBehaviour
{
	public Transform cube;
	public Transform tunnel;
	
	GameObject[] Cubes = new GameObject[10];
	
	// Use this for initialization
	void Start ()
	{
		float x = -243;
		for(int i = 0; i < 1; i++)
		{
			Transform cubeTransform = (Transform) Instantiate(cube, new Vector3(x,10,263), Quaternion.identity);
			
			Vector3 Scale = new Vector3(100,100,100);
			
			cubeTransform.localScale = Scale;
			
			
			//x += 100;
			
			//Transform cubeTransform2 = (Transform) Instantiate(cube, new Vector3(x,10,263), Quaternion.identity);
						
			//cubeTransform2.localScale = Scale;
			
		}
		// Spawn Areas
		PlayerSpawnArea();

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void PlayerSpawnArea()
	{
		// SPAWN TUNNEL A
		Transform SpawnPointA = (Transform) Instantiate(tunnel, new Vector3(695,94,582), Quaternion.identity);
		SpawnPointA.localScale = new Vector3(10, 10, 20);
		Vector3 RotateYA = new Vector3(0, 46, 0);
		SpawnPointA.Rotate(RotateYA);
		SpawnPointA.name = "SpawnA";
		
		// SPAWN TUNNEL B
		Transform SpawnPointB = (Transform) Instantiate(tunnel, new Vector3(-664,72,658), Quaternion.identity);
		SpawnPointB.localScale = new Vector3(10, 10, 20);
		Vector3 RotateYB = new Vector3(0, 306, 0);
		SpawnPointB.Rotate(RotateYB);
		SpawnPointB.name = "SpawnB";
		
		// SPAWN TUNNEL C
		Transform SpawnPointC = (Transform) Instantiate(tunnel, new Vector3(-627,76,-672), Quaternion.identity);
		SpawnPointC.localScale = new Vector3(10, 10, 20);
		Vector3 RotateYC = new Vector3(0, -150, 0);
		SpawnPointC.Rotate(RotateYC);
		SpawnPointC.name = "SpawnC";		
		
		// SPAWN TUNNEL D
		Transform SpawnPointD = (Transform) Instantiate(tunnel, new Vector3(661, 67 ,-590), Quaternion.identity);
		SpawnPointD.localScale = new Vector3(10, 10, 20);
		Vector3 RotateYD = new Vector3(0, 144, 0);
		SpawnPointD.Rotate(RotateYD);
		SpawnPointD.name = "SpawnD";
	}
}
