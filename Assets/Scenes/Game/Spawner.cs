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
			
			SetupQuadrants();
			
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
		
		m_CurrentSpawnQuad = 0;
	}
	
	void SpawnNewCarriage()
	{
		GameObject CarriageGO = ((Transform) Network.Instantiate(m_Carriage, Vector3.zero, Random.rotation, 0)).gameObject;
			
		Vector3 v3RandomPosition = new Vector3(Random.Range(m_listSpawnQuadrants[m_CurrentSpawnQuad].bottomLeft.x, 
															m_listSpawnQuadrants[m_CurrentSpawnQuad].topRight.x), 300.0f, 
											   Random.Range(m_listSpawnQuadrants[m_CurrentSpawnQuad].bottomLeft.z, 
															m_listSpawnQuadrants[m_CurrentSpawnQuad].topRight.z));
		CarriageGO.transform.position = v3RandomPosition;
		
		m_listCarriages.Add(CarriageGO.GetComponent<Carriage>());
		
		// Change spawwning quadrant.
		if(m_CurrentSpawnQuad != m_listSpawnQuadrants.Count - 1)
		{
			m_CurrentSpawnQuad += 1;
		}
		else
		{
			m_CurrentSpawnQuad = 0;
		}
	}
	
	public float 			m_CarriageSpawnRate = 2.0f;
	public int 				m_InitialSpawnedCarriages = 5;
	public Transform		m_Carriage;
	
	private float 			m_fSpawnTimer;
	private float			m_ArenaWidthHeight = 1000.0f;
	private List<Carriage> 	m_listCarriages;
	private List<Quadrant>	m_listSpawnQuadrants;
	private	int				m_CurrentSpawnQuad;
}
