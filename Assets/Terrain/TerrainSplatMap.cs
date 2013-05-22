using UnityEngine;
using System.Collections;
using UnityEditor;

public class TerrainSplatMap : MonoBehaviour 
{
	void Start () 
	{	
		// Allow reading of this texture.
		string path = AssetDatabase.GetAssetPath(m_SplatMap);
		TextureImporter ti = (TextureImporter) TextureImporter.GetAtPath(path);
		ti.isReadable = true;
		AssetDatabase.ImportAsset(path);
		
		// Get the alpha map data
		TerrainData td = GetComponent<Terrain>().terrainData;	
		float[,,] alphamaps = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
	
		for(int y = 0; y < td.alphamapHeight; y++) 
		{
            for(int x = 0; x < td.alphamapWidth; x++) 
			{
				Color SplatColor = m_SplatMap.GetPixel(x, y);
				float a0 = SplatColor.a;
				float a1 = SplatColor.r;
				float a2 = SplatColor.g;
				float a3 = SplatColor.b;
				
                alphamaps[x, y, 0] = a0;
                alphamaps[x, y, 1] = a1;
				alphamaps[x, y, 2] = a2;
				alphamaps[x, y, 3] = a3;
            }
        }
        td.SetAlphamaps(0, 0, alphamaps);
	}
	
	public Texture2D m_SplatMap = null;
}
