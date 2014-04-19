using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
	ChunkFactory chunkFac;
	public ChunkDictionary chunkMap;
	
	public int tasks;
	
	static public int chunkSize = 16;
	static public int columnCount = 4;
	
	static public float cubeSize = 1.0f;
	
	public int GetMapHeight()
	{
		return chunkSize * columnCount;	
	}
	
	public Map ()
	{
		chunkMap = new ChunkDictionary();	
	}
	
	public void ClearMap()
	{
		foreach(KeyValuePair<IntCoord, Chunk> entry in chunkMap )
		{
			entry.Value.Delete ();
		}
		chunkMap.Clear ();
		System.GC.Collect ();
	}
	
	public void RemoveBlock(IntCoord globalPos)
	{
		BlockHandle block = GetBlock (globalPos);
		if (!block.IsEmpty ())
		{
			RemoveBlock (block);
		}
	}
	
	public void RemoveBlock(BlockHandle block)
	{
		if (block.IsValid ())
		{
			SetBlock (block.globalPos, null);
			//block.Clear (); //Flags it for deletion
			//SetBlock (block.pos, null); //Remove the reference from the chunk
		}
	}
	
	void Update()
	{
	}	
	
	public static IntCoord RoundPositionToBlock(Vector3 pos)
	{
		IntCoord blockWorldPos;
		blockWorldPos.x = Mathf.RoundToInt (pos.x);
		blockWorldPos.y = Mathf.RoundToInt (pos.y);
		blockWorldPos.z = Mathf.RoundToInt (pos.z);
		return blockWorldPos;
	}
	
	public void SetBlock(IntCoord globalPos, Block block)
	{
		Chunk chunk = GetChunkWhichContains(globalPos);
		if (chunk != null)
		{
			BlockHandle handle = new BlockHandle(block, globalPos);
			chunk.SetBlockWorld(globalPos, handle);
		}
	}
	
	public BlockHandle GetBlockNearest(Vector3 pos)
	{
		return GetBlock(RoundPositionToBlock (pos));
	}
	
	public BlockHandle GetBlock(IntCoord globalPos)
	{
		Chunk chunk = GetChunkWhichContains (globalPos);
		if (chunk != null)
		{
			return chunk.GetBlockLocal (chunk.WorldToLocal (globalPos));	
		}
		return BlockHandle.empty;	
	}
	
	//Get the chunk instance that contains this block position
	public Chunk GetChunkWhichContains(IntCoord globalPos)
	{
		IntCoord gridPos = Chunk.GetChunkGridPos(globalPos.ToVector3());
		Chunk chunkInst = GetChunk (gridPos);
		return chunkInst;
	}
	
	//Gets the chunk object at the int coord position 'pos'
	public Chunk GetChunk(IntCoord gridPos)
	{
		Chunk chunk;
		if (chunkMap.TryGetValue(gridPos, out chunk))
		{
			return chunkMap[gridPos];
		}
		return null;	
	}
	//Check if the chunk exists at a specific position
	public bool ChunkExists(IntCoord pos)
	{
		Chunk chunk;
		if (chunkMap.TryGetValue(pos, out chunk))
		{
			return true;
		}	
		return false;
	}
	
	void RemoveChunk(IntCoord pos)
	{
		if (!ChunkExists(pos))
		{
			Debug.Log ("Warning: Tried to remove non existing chunk at " + pos);
			return;
		}
		chunkMap.Remove (pos);	
	}
}

