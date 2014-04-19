using UnityEngine;
using System.Collections;

public class HitInfo
{
	public bool hit = false;
	public Vector3 normal = Vector3.zero;
	public float dist = 0;
	public Vector3 minTranslation = Vector3.zero;
	public Vector3 point = Vector3.zero;
}	

public class CharacterCollision
{
	public static void GetContactPoint(PlayerController ctrl, Map map)
	{
		IntCoord blockPos = Map.RoundPositionToBlock(ctrl.transform.position);
		
		IntCoord offset;
		for (int x=-1; x<=1; x++)
		{
			for (int z=-1; z<=1; z++)
			{
				for (int y=-1; y<=0; y++)
				{
					offset.x = x;
					offset.y = y - 1;
					offset.z = z;
					BlockHandle testBlock = map.GetBlock(blockPos + offset);
					if (!testBlock.IsEmpty () && testBlock.IsSolid ())
					{
						CheckCollision (testBlock, ctrl, 1);
					}
					offset.y = y + 1;
					testBlock = map.GetBlock(blockPos + offset);
					if (!testBlock.IsEmpty () && testBlock.IsSolid ())
					{
						CheckCollision (testBlock, ctrl, 0);
					}
				}
			}
		}
	}
	
	
	static void CheckCollision(BlockHandle block, PlayerController ctrl, int collideID)
	{
		float radius = ctrl.GetColliderRadius();
		Vector3 blockOrigin = block.globalPos.ToVector3 ();
		Vector3 relPos = ctrl.transform.position - blockOrigin;
		
		relPos += ctrl.GetColliderPos(collideID);
		
		Vector3 boxMin = new Vector3(-0.5f,-0.5f, -0.5f);
		Vector3 boxMax = -boxMin;
		Vector3 boxPoint;
		//X
		if (relPos.x < boxMin.x)
			boxPoint.x = boxMin.x;
		else if (relPos.x > boxMax.x)
			boxPoint.x = boxMax.x;
		else
			boxPoint.x = relPos.x;
		//Y		
		if (relPos.y < boxMin.y)
			boxPoint.y = boxMin.y;
		else if (relPos.y > boxMax.y)
			boxPoint.y = boxMax.y;
		else
			boxPoint.y = relPos.y;
		//Z	
		if (relPos.z < boxMin.z)
			boxPoint.z = boxMin.z;
		else if (relPos.z > boxMax.z)
			boxPoint.z = boxMax.z;
		else
			boxPoint.z = relPos.z;
	
		Vector3 distance = relPos - boxPoint;
		
		if (distance.sqrMagnitude < radius * radius)
		{
			HitInfo hit = new HitInfo();
			hit.hit = true;
			hit.dist = distance.magnitude;
			hit.normal = distance.normalized;
			hit.minTranslation = hit.normal * (radius - hit.dist);
			hit.point = boxPoint + blockOrigin;
			ctrl.OnMapCollide (hit);
		}
	}
}

