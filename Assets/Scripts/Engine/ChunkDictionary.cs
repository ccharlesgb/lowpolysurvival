using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkDictionary : Dictionary<IntCoord, Chunk>
{
	public ChunkDictionary ()
	{
		
	}
	
	public IntCoord? GetNearestUnBuilt(IntCoord pos, float maxDist)
	{
		Chunk tryChunk;
		//Is our current position unbuilt?
		if (!TryGetValue(pos, out tryChunk))
		{
			return pos;
		}
		for (int xDist=0; xDist<maxDist; xDist++)
		{
			for (int zDist=0; zDist<maxDist; zDist++)
			{
				for (int xOffset = -xDist; xOffset < xDist; xOffset++)
				{
					for (int zOffset = -zDist; zOffset < zDist; zOffset++)
					{
						IntCoord testPos = pos + new IntCoord(xOffset,0,zOffset);
						Chunk testChunk;
						if (!TryGetValue(testPos, out testChunk))
						{
							return testPos;
						}
					}
				}
				
			}		
		}
		return null;
	}

}

