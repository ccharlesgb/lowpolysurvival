using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkDictionary : Dictionary<IntCoord, GameObject>
{
	public ChunkDictionary ()
	{
		
	}
	
	public IntCoord? GetNearestUnBuilt(IntCoord pos, float maxDist)
	{
		GameObject tryChunk;
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
						GameObject testChunk;
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

