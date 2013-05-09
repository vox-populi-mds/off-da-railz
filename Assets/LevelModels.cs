using UnityEngine;
using System.Collections;

public class LevelModels : MonoBehaviour
{
	public GameObject[] Cubes = new GameObject[10];
	
	// Use this for initialization
	void Start ()
	{
		
		for(int i = 0; i < 10; i++)
		{
			Cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
		}
		//Vector3 Scale = new Vector3(100,100,100);
			
		//Cubes[0].transform.localScale = Scale;
		//Cubes[0].transform.position = new Vector3(-243,10,263);
		//Cubes[0].name = "Tjingy";
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

