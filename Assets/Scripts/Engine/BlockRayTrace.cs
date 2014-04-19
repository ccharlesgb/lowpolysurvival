using UnityEngine;
using System.Collections;

public struct BlockRayResult
{
	public BlockHandle block;
	public float distance;
	public bool hit;
	public IntCoord hitPos;
}

public struct BlockRay
{
	public Vector3 start;
	public Vector3 end;
	public Vector3 dir;
	
	public BlockRay(IntCoord p1, IntCoord p2)
	{
		start = p1.ToVector3();
		end = p2.ToVector3 ();
		dir = (end-start).normalized;
	}
	
	public BlockRay(Vector3 p1, Vector3 p2)
	{
		start = p1;
		end = p2;
		dir = (p2-p1).normalized;
	}
	
	public BlockRay(Vector3 origin, Vector3 direc, float dist)
	{
		start = origin;
		end = origin + (direc * dist);
		dir = direc;
	}
	
	public BlockRay(IntCoord origin, Vector3 direc)
	{
		start = origin.ToVector3 ();
		end = start + (direc * BlockRayTrace.BLOCK_RAY_MAX_DIST);
		dir = direc;
	}
}

public class BlockRayTrace
{
	public static float BLOCK_RAY_MAX_DIST = 256;
	public static int BLOCK_RAY_PRECISION = 0;
	
	public static BlockRayResult RayTraceMap(Map map, BlockRay ray)
	{
		return Bresenham (map, ray.start, ray.end, BLOCK_RAY_PRECISION);
	}
	
	public static BlockRayResult Bresenham(Map map, Vector3 p1, Vector3 p2, int precision)
	{
		BlockRayResult result;
		result.hit = false;
		result.block = BlockHandle.empty;
		result.distance = float.MaxValue;
		result.hitPos = new IntCoord(0,0,0);
		
		IntCoord P1;
		IntCoord P2;
		
		P1.x = Mathf.RoundToInt (p1.x * Mathf.Pow (10,precision));
		P1.y = Mathf.RoundToInt (p1.y * Mathf.Pow (10,precision));
		P1.z = Mathf.RoundToInt (p1.z * Mathf.Pow (10,precision));
		P2.x = Mathf.RoundToInt (p2.x * Mathf.Pow (10,precision));
		P2.y = Mathf.RoundToInt (p2.y * Mathf.Pow (10,precision));
		P2.z = Mathf.RoundToInt (p2.z * Mathf.Pow (10,precision));
		
		int d = Mathf.Max(IntCoord.Abs(P2-P1).ToArray ()) + 1;
		float[] X = new float[d];
		float[] Y = new float[d];
		float[] Z = new float[d];
		
		int x1 = P1.x;
		int y1 = P1.y;
		int z1 = P1.z;
		int x2 = P2.x;
		int y2 = P2.y;
		int z2 = P2.z;
		
		//if (map.GetBlock (new IntCoord(x1,y1,z1)) != null)
		//{
			//result.distance = 0;
			//result.block = map.GetBlock (new IntCoord(x1,y1,z1));
			//result.hit = true;
			//return result;	
		//}
		
		int dx = x2 - x1;
   		int dy = y2 - y1;
  		int dz = z2 - z1;

		int ax = System.Math.Abs(dx)*2;
  	 	int ay = System.Math.Abs(dy)*2;
   		int az = System.Math.Abs(dz)*2;
		
		int sx = System.Math.Sign(dx);
   		int sy = System.Math.Sign(dy);
  	 	int sz = System.Math.Sign(dz);
		
		int x = x1;
		int y = y1;
		int z = z1;
		int idx = 0;
		
		int distToHit = 0;
		
		Vector3 curBlockPos;
		
		if(ax >= System.Math.Max(ay,az))	// x dominant
		{
			int yd = ay - ax/2;
     		int zd = az - ax/2;	
			
			while(true)
			{
				X[idx] = (float)x;
				Y[idx] = (float)y;
				Z[idx] = (float)z;
				idx = idx + 1;
				
				if(x == x2)
					break;
				
				if(yd >= 0)		// move along y
				{
					y = y + sy;
					yd = yd - ax;
				}
				
				if(zd >= 0)		// move along z
				{
					z = z + sz;
					zd = zd - ax;
				}
				
				x  = x  + sx;	// move along x
				yd = yd + ay;
				zd = zd + az;
				
				curBlockPos.x = x / Mathf.Pow (10,precision);
				curBlockPos.y = y / Mathf.Pow (10,precision);
				curBlockPos.z = z / Mathf.Pow (10,precision);
		
				BlockHandle block = map.GetBlockNearest(curBlockPos);
				if (!block.IsEmpty ())
				{
					result.distance = idx - 1;
					result.block = block;
					result.hit = true;
					result.hitPos = curBlockPos;
					return result;	
				}
			}
		}
		else if(ay>=System.Math.Max(ax,az))		// y dominant
		{
			int xd = ax - ay/2;
			int zd = az - ay/2;
			
			while(true)
			{
				X[idx] = (float)x;
				Y[idx] = (float)y;
				Z[idx] = (float)z;
				idx = idx + 1;
				
				if(y == y2)		// end
					break;
				
				if(xd >= 0)		// move along x
				{
					x = x + sx;
					xd = xd - ay;
				}
				
				if(zd >= 0)	// move along z	
				{		
					z = z + sz;
					zd = zd - ay;
				}
				
				y  = y  + sy;		// move along y
				xd = xd + ax;
				zd = zd + az;
				
				curBlockPos.x = x / Mathf.Pow (10,precision);
				curBlockPos.y = y / Mathf.Pow (10,precision);
				curBlockPos.z = z / Mathf.Pow (10,precision);
		
				BlockHandle block = map.GetBlockNearest(curBlockPos);
				if (!block.IsEmpty ())
				{
					result.distance = idx - 1;
					result.block = block;
					result.hit = true;
					result.hitPos = curBlockPos;
					return result;	
				}
			}
		}
		else if(az>=System.Math.Max(ax,ay))		// z dominant
		{
			int xd = ax - az/2;
			int yd = ay - az/2;
			
			while(true)
			{
				X[idx] = (float)x;
				Y[idx] = (float)y;
				Z[idx] = (float)z;
				idx = idx + 1;
			
				if(z == z2)		// end
					break;
				
				if(xd >= 0)		// move along x
				{
					x = x + sx;
					xd = xd - az;
				}
				
				if(yd >= 0)		// move along y
				{
					y = y + sy;
					yd = yd - az;
				}
				
				z  = z  + sz;		// move along z
		        xd = xd + ax;
		        yd = yd + ay;
				
				curBlockPos.x = x / Mathf.Pow (10,precision);
				curBlockPos.y = y / Mathf.Pow (10,precision);
				curBlockPos.z = z / Mathf.Pow (10,precision);
		
				BlockHandle block = map.GetBlockNearest(curBlockPos);
				if (!block.IsEmpty ())
				{
					result.distance = idx - 1;
					result.block = block;
					result.hit = true;
					result.hitPos = curBlockPos;
					return result;	
				}
			}
		}
		if (precision != 0)
		{
			for (int i=0; i < d; i++)
			{
				X[i] = X[i] / Mathf.Pow (10,precision);
      			Y[i] = Y[i] / Mathf.Pow (10,precision);
				Z[i] = Z[i] / Mathf.Pow (10,precision);
			}
		}
		return result;
	}
}

/*
function [X,Y,Z] = bresenham_line3d(P1, P2, precision)

   if ~exist('precision','var') | isempty(precision) | round(precision) == 0
      precision = 0;
      P1 = round(P1);
      P2 = round(P2);
   else
      precision = round(precision);
      P1 = round(P1*(10^precision));
      P2 = round(P2*(10^precision));
   end

   d = max(abs(P2-P1)+1);
   X = zeros(1, d);
   Y = zeros(1, d);
   Z = zeros(1, d);

   x1 = P1(1);
   y1 = P1(2);
   z1 = P1(3);

   x2 = P2(1);
   y2 = P2(2);
   z2 = P2(3);

   dx = x2 - x1;
   dy = y2 - y1;
   dz = z2 - z1;

   ax = abs(dx)*2;
   ay = abs(dy)*2;
   az = abs(dz)*2;

   sx = sign(dx);
   sy = sign(dy);
   sz = sign(dz);

   x = x1;
   y = y1;
   z = z1;
   idx = 1;

   if(ax>=max(ay,az))			% x dominant
      yd = ay - ax/2;
      zd = az - ax/2;

      while(1)
         X(idx) = x;
         Y(idx) = y;
         Z(idx) = z;
         idx = idx + 1;

         if(x == x2)		% end
            break;
         end

         if(yd >= 0)		% move along y
            y = y + sy;
            yd = yd - ax;
         end

         if(zd >= 0)		% move along z
            z = z + sz;
            zd = zd - ax;
         end

         x  = x  + sx;		% move along x
         yd = yd + ay;
         zd = zd + az;
      end
   elseif(ay>=max(ax,az))		% y dominant
      xd = ax - ay/2;
      zd = az - ay/2;

      while(1)
         X(idx) = x;
         Y(idx) = y;
         Z(idx) = z;
         idx = idx + 1;

         if(y == y2)		% end
            break;
         end

         if(xd >= 0)		% move along x
            x = x + sx;
            xd = xd - ay;
         end

         if(zd >= 0)		% move along z
            z = z + sz;
            zd = zd - ay;
         end

         y  = y  + sy;		% move along y
         xd = xd + ax;
         zd = zd + az;
      end
   elseif(az>=max(ax,ay))		% z dominant
      xd = ax - az/2;
      yd = ay - az/2;

      while(1)
         X(idx) = x;
         Y(idx) = y;
         Z(idx) = z;
         idx = idx + 1;

         if(z == z2)		% end
            break;
         end

         if(xd >= 0)		% move along x
            x = x + sx;
            xd = xd - az;
         end

         if(yd >= 0)		% move along y
            y = y + sy;
            yd = yd - az;
         end

         z  = z  + sz;		% move along z
         xd = xd + ax;
         yd = yd + ay;
      end
   end

   if precision ~= 0
      X = X/(10^precision);
      Y = Y/(10^precision);
      Z = Z/(10^precision);
   end

   return;					% bresenham_line3d

*/