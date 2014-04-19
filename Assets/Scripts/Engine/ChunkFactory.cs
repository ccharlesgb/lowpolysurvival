using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChunkFactory : MonoBehaviour {
	
	
	public List<Transform> players; //Can be thought of as world observers, the map needs to load around them
	
	public GameObject chunkPrefab;
	
	public bool buildingChunks;
	public bool clearingChunks;
	
	public Map map;
	
	public TerrainGenerator terrainGen;
	
	public bool rebuildMap;
	
	public int columnChunkDepth = 8;
	public float chunkBuildDist = 2.0f;
	
	void Awake()
	{
		rebuildMap = false;
		terrainGen = GetComponent<TerrainGenerator>();
		map = GetComponent<Map>();
		buildingChunks = false;
	}

	
	// Use this for initialization
	void Start ()
	{
		//players.Add (Camera.main.transform);
		players.Add (GameObject.Find("Player").transform);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!buildingChunks)
		{
			StartCoroutine("BuildChunks");	
		}
		if (clearingChunks)
		{
			StartCoroutine ("CheckFarChunks");
		}
		if (!buildingChunks && rebuildMap)
		{
			map.ClearMap ();
			terrainGen.ApplyParameters();
			rebuildMap = false;
		}
	}
	
	IEnumerator CheckFarChunks()
	{
		List<IntCoord> toDelete = new List<IntCoord>();
		foreach(KeyValuePair<IntCoord, Chunk> entry in map.chunkMap )
		{
			IntCoord pos = Chunk.GetChunkGridPos(players[0].position); //TODO MAKE SURE IT WORKS FOR ALL PLAYERS
			pos.y = 0;
			int distSqr = IntCoord.MagnitudeSqr(pos - entry.Key);
			float thresholdDist = chunkBuildDist + 3;
			if (distSqr > thresholdDist * thresholdDist)
			{
				toDelete.Add (entry.Key);

			}
		}
		foreach (IntCoord coord in toDelete)
		{
			map.chunkMap[coord].Delete ();
			map.chunkMap.Remove (coord);
		}
				yield return new WaitForSeconds(0.1f);
	}
	
	IEnumerator BuildChunks()
	{
		buildingChunks = true;

		for (int i=0; i<players.Count; i++)
		{
			IntCoord pos = Chunk.GetChunkGridPos(players[i].position);
			pos.y = 0;
			IntCoord? chunkToBuild = map.chunkMap.GetNearestUnBuilt(pos, chunkBuildDist);
			clearingChunks = false;
			while (chunkToBuild != null)
			{
				clearingChunks = true; //If we are building new chunks maybe we need to delete some old ones?
				if (chunkToBuild != null)
				{
					yield return StartCoroutine ("BuildColumnAt", chunkToBuild);	
				}
				chunkToBuild = map.chunkMap.GetNearestUnBuilt(pos, chunkBuildDist);
			}
		}
		buildingChunks = false;
	}
	
	IEnumerator BuildColumnAt(IntCoord pos)
	{
		for (int height = columnChunkDepth - 1; height >= 0; height--)
		{
			pos.y = height;
			yield return StartCoroutine ("BuildChunkAt", pos);	
		}
		for (int height = columnChunkDepth - 1; height >= 0; height--)
		{
			Chunk chunk = map.GetChunk (new IntCoord(pos.x,height,pos.z));	
			if (chunk != null)
			{
				chunk.UpdateAllBlocks();	
			}
		}
	}
	
	IEnumerator BuildChunkAt(IntCoord pos)
	{
		Vector3 spawnPos = MathTools.ScalarMultiply(pos.ToVector3(),Chunk.GetChunkBounds()); //Find the transform pos
		GameObject newChunk = Instantiate (chunkPrefab, spawnPos, Quaternion.identity) as GameObject; //Create the chunk fab
		newChunk.name = "Chunk " + pos; //Name it its grid chunk location
		
		ChunkMeshRender chunkRender = newChunk.GetComponent<ChunkMeshRender>();
		
		Chunk newChunkObj = new Chunk(chunkRender, map, pos);
		chunkRender.parentChunk = newChunkObj;
		map.chunkMap.Add (pos, newChunkObj);
		newChunk.transform.parent = transform;
		
		terrainGen.BuildChunkBlocks(newChunkObj);
		yield return null;
		newChunkObj.UpdateAllNeighbours ();
	}
}	
