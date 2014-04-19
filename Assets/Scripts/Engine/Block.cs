using UnityEngine;
using System.Collections;

public class Block
{
	protected byte[] faceIDs;
	public bool isUpdateable;
	
	public bool transparent;
	public bool visible;
	public bool shouldCollide;
	
	protected Map map;
	
	private bool flaggedForDeletion;
	
	public void Remove()
	{
		flaggedForDeletion = true;	
	}
	
	public bool IsRemoved()
	{
		return flaggedForDeletion;	
	}	
	
	//FaceIDs
	//0 = top
	//1 = front
	//2 = back
	//3 = left
	//4 = right
	//5 = bottom
	public Block()
	{
		
	}
	
	public Block(Map _map)
	{
		faceIDs = new byte[6];
		transparent = false;
		visible = true;
		shouldCollide = true;
		SetupFaces ();
		flaggedForDeletion = false;
		map = _map;
	}
	
	public void SetAllFaces(int id)
	{
		for (int i=0; i<6;i++)
		{
			faceIDs[i] = (byte)id;	
		}
	}
	
	public virtual bool ShouldRenderFace(BlockHandle neighbour)
	{
		if (!visible) return false;
		
		if (neighbour.IsEmpty())
		{
			return true;	
		}
		if (!transparent && neighbour.IsTransparent ())
		{
			return true;	
		}
		return false;
	}
	
	public virtual void SetupFaces() {}
	
	public virtual bool Update(BlockHandle data) {return false;}
	
	public virtual byte[] GetFaceIDs(BlockHandle data)
	{
		return faceIDs;	
	}
}

public class Dirt : Block
{
	byte[] grassIDs = {0,1,1,1,1,2};
	public Dirt(Map _map) : base(_map)
	{
		isUpdateable = true;
	}
	
	public override bool Update(BlockHandle data)
	{
		BlockRay ray = new BlockRay(data.globalPos, data.globalPos + new IntCoord(0,20,0));
		BlockRayResult res = BlockRayTrace.RayTraceMap (map, ray);
		data.SetUserData(false);
		//isUpdateable = false;
		if (!res.hit)
		{
			data.SetUserData(true);
			//isUpdateable = true;
		}
		else
		{
			if (res.block.IsTransparent())
			{
				data.SetUserData(true);
				//isUpdateable = true;
			}
		}
		return data.GetUserData ();
	}
	
	public override byte[] GetFaceIDs(BlockHandle data)
	{
		if (data.GetUserData())
		{
			return grassIDs;
		}
		else
		{
			return faceIDs;
		}
	}
	
	public override void SetupFaces()
	{
		//Brown
		SetAllFaces (2);
	}
}

public class Stone : Block
{
	public Stone(Map _map) : base(_map)
	{
	
	}
	
	public override void SetupFaces()
	{
		//Brown
		SetAllFaces (8);
	}
	
}

public class Water : Block
{
	public Water(Map _map) : base(_map)
	{
		transparent = true;
	}
	
	public override void SetupFaces()
	{
		//Brown
		SetAllFaces (3);
	}
	
}