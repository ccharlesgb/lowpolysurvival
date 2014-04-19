using UnityEngine;
using System;
using System.Collections;

public class Chunk
{
	public BlockHandle[,,] blocks;
	
	private ChunkMeshRender meshBuilder;
	
	public IntCoord gridCoord; //What location is the chunk at
	
	public Map map;
	
	public void NeedsUpdate()
	{
		meshBuilder.needsUpdate = true;
		//Debug.Log ("CHUNK REBUILDING AT " + gridCoord);
	}
	
	public bool GetNeedsUpdate()
	{
		return meshBuilder.needsUpdate;	
	}
	
	public Chunk(ChunkMeshRender rend, Map m, IntCoord pos)
	{
		gridCoord = pos;
		map = m;
		meshBuilder = rend;
		blocks = new BlockHandle[Map.chunkSize, Map.chunkSize, Map.chunkSize];	
	}
	
	public void Delete()
	{
		foreach (BlockHandle hndl in blocks)
		{
			SetBlockWorld (hndl.globalPos, BlockHandle.empty);	
		}
		//map = null;
	}
	
	static public IntCoord GetChunkGridPos(Vector3 p)
	{
		Vector3 pos = MathTools.FloorNearestVec(p, new Vector3(Map.chunkSize,Map.chunkSize,Map.chunkSize));
		pos.x /= Map.chunkSize;
		pos.y /= Map.chunkSize;
		pos.z /= Map.chunkSize; 
		return new IntCoord(pos.x, pos.y, pos.z);
	}
	static public Vector3 GetChunkBounds()
	{
		return new Vector3(Map.chunkSize, Map.chunkSize, Map.chunkSize);	
	}
	
	public IntCoord WorldToLocal(IntCoord world)
	{
		Vector3 posInBlocks = (meshBuilder.transform.position / Map.cubeSize); //chunk position in BLOCK units
		IntCoord chunkPos = posInBlocks;
		IntCoord local = world - chunkPos;
		
		return local;
	}
	
	public IntCoord LocalToWorld(IntCoord local)
	{
		Vector3 posInBlocks = (meshBuilder.transform.position / Map.cubeSize); //chunk position in BLOCK units
		IntCoord chunkPos = posInBlocks;
		IntCoord world = local + chunkPos;
		
		return world;
	}
	
	public void SetBlockWorld(IntCoord pos, BlockHandle block)
	{
		IntCoord localPos = WorldToLocal (pos);
		SetBlockLocal (localPos, block);
	}
	
	public void SetBlockLocal(IntCoord pos, BlockHandle block)
	{
		if (InLocalChunkBounds (pos))
		{
			blocks[pos.x, pos.y, pos.z] = block;
			NeedsUpdate ();
			CheckNeighbourUpdates(pos);
		}
	}
	
	public BlockHandle GetBlockLocal(IntCoord pos)
	{
		if (InLocalChunkBounds (pos))
		{
			return blocks[pos.x, pos.y, pos.z];
		}
		return map.GetBlock(LocalToWorld (pos));
	}
	
	public bool InLocalChunkBounds(IntCoord pos)
	{
		if (pos.x < 0 || pos.y < 0 || pos.z < 0)
			return false;
		if (pos.x >= Map.chunkSize || pos.y >= Map.chunkSize || pos.z >= Map.chunkSize)
			return false;
		return true;
	}
	
	public void UpdateAllBlocks()
	{
		IntCoord localPos;
		foreach (BlockHandle hndl in blocks)
		{
			localPos = WorldToLocal(hndl.globalPos);
			if (hndl.IsValid () && hndl.IsUpdateable ())
			{
				hndl.Update ();
				SetBlockLocal (localPos, hndl);
			}
		}
		NeedsUpdate ();
	}
	
	public void UpdateAllNeighbours()
	{
		Chunk neighbour;
		
		for (int i=0; i < 6; i++)
		{
			neighbour = GetNeighbourChunk(IntCoord.directions[i]);
			if (neighbour != null)
				neighbour.NeedsUpdate();

		}
	}
	
	public void CheckNeighbourUpdates(IntCoord localPos)
	{
		if (!InLocalChunkBounds(localPos))
		{
			return;	
		}
		
		Chunk neighbour;
		
		if (localPos.x == 0)
		{
			neighbour = GetNeighbourChunk(IntCoord.left);
			if (neighbour != null)
				neighbour.NeedsUpdate();
		}
		if (localPos.y == 0)
		{
			neighbour = GetNeighbourChunk(IntCoord.down);
			if (neighbour != null)
				neighbour.NeedsUpdate();
		}
		if (localPos.z == 0)
		{
			neighbour = GetNeighbourChunk(IntCoord.back);
			if (neighbour != null)
				neighbour.NeedsUpdate();
		}
		if (localPos.x == Map.chunkSize - 1)
		{
			neighbour = GetNeighbourChunk(IntCoord.right);
			if (neighbour != null)
				neighbour.NeedsUpdate();	
		}
		if (localPos.y == Map.chunkSize - 1)
		{
			neighbour = GetNeighbourChunk(IntCoord.up);
			if (neighbour != null)
				neighbour.NeedsUpdate();	
		}
		if (localPos.z == Map.chunkSize - 1)
		{
			neighbour = GetNeighbourChunk(IntCoord.forward);
			if (neighbour != null)
				neighbour.NeedsUpdate();	
		}
		
	}
	
	public Chunk GetNeighbourChunk(IntCoord dir)
	{
		return map.GetChunk(gridCoord + dir);
	}	
	
	//Hard to describe what this does.
	//If a block is on the chunk boundary (ie next to a neighbouring chunk)
	//returns the direction that the neighbour is at
	public IntCoord[] GetNeighboursOnBoundary(IntCoord localPos)
	{
		IntCoord[] neighbours = new IntCoord[4];
		int neighbourCount = 0;
		if (!InLocalChunkBounds(localPos))
		{
			return neighbours;	
		}
		if (localPos.x == 0)
		{
			neighbours[neighbourCount] = IntCoord.left;
			neighbourCount++;	
		}
		if (localPos.y == 0)
		{
			neighbours[neighbourCount] = IntCoord.down;
			neighbourCount++;	
		}
		if (localPos.z == 0)
		{
			neighbours[neighbourCount] = IntCoord.forward;
			neighbourCount++;	
		}
		if (localPos.x == Map.chunkSize - 1)
		{
			neighbours[neighbourCount] = IntCoord.right;
			neighbourCount++;	
		}
		if (localPos.y == Map.chunkSize - 1)
		{
			neighbours[neighbourCount] = IntCoord.up;
			neighbourCount++;	
		}
		if (localPos.z == Map.chunkSize - 1)
		{
			neighbours[neighbourCount] = IntCoord.back;
			neighbourCount++;	
		}
		return neighbours;
	}
}

