using UnityEngine;
using System.Collections;

using LibNoise.Unity;

public class TerrainGenerator : MonoBehaviour
{
	LibNoise.Unity.Generator.Perlin heightmapGen;
	LibNoise.Unity.Operator.ScaleBias heightBias;
	
	public float heightScale;
	public float heightOffset;
	
	public float terrainScale = 10.0f;
	
	public float heightFreq = 0.5f;
	public float heightLacunarity = 1.5f;
	public int heightOctaves = 3;
	public float heightPersistence = 0.2f;
	public int genSeed = 1;
	
	private Map map;
	
	private Block dirt;
	private Block water;
	private Block stone;
	
	void Awake()
	{
		map = GetComponent<Map>();
		QualityMode quality = QualityMode.High;
		heightmapGen = new LibNoise.Unity.Generator.Perlin(heightFreq, heightLacunarity, heightPersistence, heightOctaves, genSeed, quality);
		
		heightBias = new LibNoise.Unity.Operator.ScaleBias(heightScale, heightOffset, heightmapGen);
		
		dirt = new Dirt(map);
		water = new Water(map);
		stone = new Stone(map);
	}
	
	public void ApplyParameters()
	{
		heightmapGen.Frequency = heightFreq;
		heightmapGen.Lacunarity = heightLacunarity;
		heightmapGen.Persistence = heightPersistence;
		heightmapGen.OctaveCount = heightOctaves;
		heightmapGen.Seed = genSeed;
		
		heightBias.Bias = heightOffset;
		heightBias.Scale = heightScale;
	}
	
	public float GetHeight(IntCoord pos)
	{
		Vector3 coord = pos.ToVector3() / terrainScale;
		coord.y = 0;
		return (float)heightBias.GetValue(coord);
	}

	public void BuildChunkBlocks(Chunk chunk)
	{
		IntCoord gridPos = chunk.gridCoord;
		for (int x = 0; x < Map.chunkSize; x++)
		{
			for (int z = 0; z < Map.chunkSize; z++)
			{
				IntCoord localCoord = new IntCoord(x,0,z);
				IntCoord worldCoords = chunk.LocalToWorld(localCoord);
				
				float heightFrac = GetHeight (worldCoords);
				
				//if (Random.Range(0,500) == 0)
					//Debug.Log (height);
				
				int groundHeight = Mathf.RoundToInt (map.GetMapHeight() * heightFrac); //Height of terrain in blocks
				
				for (int y = 0; y < Map.chunkSize; y++)
				{
					localCoord.y = y;
					worldCoords = chunk.LocalToWorld(localCoord);
					if (worldCoords.y >= groundHeight)
					{		
						if (worldCoords.y < map.GetMapHeight() / 4.0f)
						{
							map.SetBlock (worldCoords, water);
						}	
					}
					else if (worldCoords.y < groundHeight)
					{
						int heightAbove = worldCoords.y - groundHeight;
						if (heightAbove >= -5)
						{
							map.SetBlock (worldCoords, dirt);
						}
						else
						{
							map.SetBlock (worldCoords, stone);
						}
					}

				}
			}
		}
	}
}

