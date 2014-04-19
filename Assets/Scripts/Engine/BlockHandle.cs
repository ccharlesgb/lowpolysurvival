using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Something to help with the memory management instead of throwing references around everywhere
public struct BlockHandle
{
	private Block block;
	public IntCoord globalPos;
	
	public static BlockHandle empty = new BlockHandle(null, IntCoord.zero);
	
	private bool userData;
	
	public BlockHandle(Block blk, IntCoord pos)
	{
		block = blk;
		globalPos = pos;
		userData = false;
	}
	
	public void SetUserData(bool d)
	{
		userData = d;	
	}
	
	public bool GetUserData()
	{
		return userData;	
	}
	
	public bool IsEmpty()
	{ 
		return block == null;
	}
	
	public bool IsValid()
	{
		return block != null;	
	}
	
	public Block GetBlock()
	{
		return block;	
	}
	
	public void Clear()
	{
		block.Remove ();
		block = null;	
	}
	
	public void Update()
	{
		if (IsValid () && IsUpdateable())
		{
			userData = block.Update (this);
		}
	}
	
	public bool IsUpdateable()
	{
		return IsValid() && block.isUpdateable;	
	}
	
	//Are we visible at all?
	public bool IsVisible()
	{
		return IsValid() && block.visible;	
	}
	
	//Should we collide?
	public bool IsSolid()
	{
		return IsValid() && block.shouldCollide;	
	}
	
	//Are we partially transparant or invisible? (May not be desired behaviour!)
	public bool IsTransparent()
	{
		return IsValid() && (block.transparent || !block.visible);
	}
}


