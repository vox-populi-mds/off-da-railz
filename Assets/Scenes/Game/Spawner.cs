using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour 
{
	class Quadrant
	{
		public Vector3 bottomLeft;
		public Vector3 topRight;
	}
	
	void Start() 
	{
		if(Network.isServer)
		{
			m_listCarriages = new List<Carriage>();
			
			// Random dropping spawn.
			if(m_RandomSpawnOverride)
			{
				SetupQuadrants();
			}
			else
			{
				SetupSpawnLocations();
			}
			
			// Spawn initial carriages.
			for(int i = 0; i < m_InitialSpawnedCarriages; ++i)
			{
				SpawnNewCarriage();
			}
		}
	}
	
	void Update() 
	{
		if(Network.isServer)
		{
			m_fSpawnTimer += Time.deltaTime;
			
			if(m_fSpawnTimer > m_CarriageSpawnRate)
			{
				SpawnNewCarriage();
				m_fSpawnTimer = 0.0f;
			}
		}
	}
	
	void SetupSpawnLocations()
	{
		m_listSpawnLocations = new List<Transform>();
		
		foreach(Transform t in m_SpawnLocations)
		{
			m_listSpawnLocations.Add(t);
		}
	}
	
	void SetupQuadrants()
	{
		float fHalfArena = m_ArenaWidthHeight * 0.5f;
		int iQuadsWideHigh = 2;
		m_listSpawnQuadrants = new List<Quadrant>();
		
		for(int i = 0; i < iQuadsWideHigh; ++i)
		{
			for(int j = 0; j < iQuadsWideHigh; ++j)
			{
				Quadrant q = new Quadrant();
				
				q.bottomLeft.x = i * (m_ArenaWidthHeight/iQuadsWideHigh) - fHalfArena;
				q.bottomLeft.z = j * (m_ArenaWidthHeight/iQuadsWideHigh) - fHalfArena;
				
				q.topRight.x = (i + 1) * (m_ArenaWidthHeight/iQuadsWideHigh) - fHalfArena;
				q.topRight.z = (j + 1) * (m_ArenaWidthHeight/iQuadsWideHigh) - fHalfArena;
				
				m_listSpawnQuadrants.Add(q);
			}
		}
		
		m_CurrentSpawnLocation = 0;
	}
	
	void SpawnNewCarriage()
	{	
		if(m_RandomSpawnOverride)
		{
			GameObject CarriageGO = ((Transform) Network.Instantiate(m_Carriage, Vector3.zero, Random.rotation, 0)).gameObject;
				
			Vector3 v3RandomPosition = new Vector3(Random.Range(m_listSpawnQuadrants[m_CurrentSpawnLocation].bottomLeft.x, 
																m_listSpawnQuadrants[m_CurrentSpawnLocation].topRight.x), 300.0f, 
												   Random.Range(m_listSpawnQuadrants[m_CurrentSpawnLocation].bottomLeft.z, 
																m_listSpawnQuadrants[m_CurrentSpawnLocation].topRight.z));
			CarriageGO.transform.position = v3RandomPosition;
			
			m_listCarriages.Add(CarriageGO.GetComponent<Carriage>());
			
			// Change spawning location.
			if(m_CurrentSpawnLocation != m_listSpawnQuadrants.Count - 1)
			{
				m_CurrentSpawnLocation += 1;
			}
			else
			{
				m_CurrentSpawnLocation = 0;
			}
		}
		else
		{
			Transform transformSpawner = m_listSpawnLocations[m_CurrentSpawnLocation];
			
			Quaternion RotationSpawn = transformSpawner.rotation;
			Vector3 PositionSpawn = transformSpawner.position;
			
			GameObject CarriageGO = ((Transform) Network.Instantiate(m_Carriage, PositionSpawn, RotationSpawn, 0)).gameObject;
			
			float ForceAmmount = transformSpawner.GetComponent<SpawnForce>().m_Force;
			ForceAmmount += Random.Range(-ForceAmmount/5.0f, ForceAmmount/5.0f);
			
			CarriageGO.rigidbody.AddForce((RotationSpawn * Vector3.forward) * ForceAmmount * CarriageGO.rigidbody.mass, ForceMode.Impulse);
			
			m_listCarriages.Add(CarriageGO.GetComponent<Carriage>());
			
			// Change spawning location.
			int iRandomPlus = Random.Range(1, 3);
			if(m_CurrentSpawnLocation < m_listSpawnLocations.Count - iRandomPlus)
			{
				m_CurrentSpawnLocation += iRandomPlus;
			}
			else
			{
				m_CurrentSpawnLocation = (m_CurrentSpawnLocation + iRandomPlus) - m_listSpawnLocations.Count;
			}
		}
	}
	
	public float 			m_CarriageSpawnRate = 2.0f;
	public int 				m_InitialSpawnedCarriages = 5;
	public Transform		m_Carriage;
	
	public Transform		m_SpawnLocations;
	public bool 			m_RandomSpawnOverride = false;
	
	private float 			m_fSpawnTimer;
	private float			m_ArenaWidthHeight = 1000.0f;
	private List<Carriage> 	m_listCarriages;
	
	private List<Transform>	m_listSpawnLocations;
	private List<Quadrant>	m_listSpawnQuadrants;
	private	int				m_CurrentSpawnLocation;
}
