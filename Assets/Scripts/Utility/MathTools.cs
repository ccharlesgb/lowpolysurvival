using UnityEngine;
using System;
using System.Collections;

public struct IntCoord : IEquatable<IntCoord>
{
	public int x;
	public int y;
	public int z;
	
	public static IntCoord zero = new IntCoord(0,0,0);
	
	public static IntCoord up = new IntCoord(0,1,0);
	public static IntCoord down = new IntCoord(0,-1,0);
	public static IntCoord left = new IntCoord(-1,0,0);
	public static IntCoord right = new IntCoord(1,0,0);
	public static IntCoord forward = new IntCoord(0,0,1);
	public static IntCoord back = new IntCoord(0,0,-1);
	
	public enum dirID {up, down, left, right, forward, back};
	public static IntCoord[] directions = new IntCoord[] {up, down, left, right, forward, back};
	
	public IntCoord(IntCoord coord)
	{
		x = coord.x;
		y = coord.y;
		z = coord.z;
	}
	
	public IntCoord(int _x, int _y, int _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}
	
	public IntCoord(float _x, float _y, float _z)
	{
		x = Mathf.RoundToInt(_x);
		y = Mathf.RoundToInt(_y);
		z = Mathf.RoundToInt(_z);
	}
	
	public IntCoord(Vector3 vec)
	{
		x = Mathf.RoundToInt(vec.x);
		y = Mathf.RoundToInt(vec.y);
		z = Mathf.RoundToInt(vec.z);
	}
	
	#region IEquatable<IntCoord> Members
	
	public bool Equals(IntCoord other)
	{
	  return x == other.x && y == other.y && z == other.z;
	}
	
	#endregion
	
	public override int GetHashCode()
	{
		int hash = 23;
		hash = hash * 31 + x.GetHashCode ();
		hash = hash * 31 + y.GetHashCode ();
		hash = hash * 31 + z.GetHashCode ();
		return hash;
	}
	
	public override string ToString()
	{
		return(String.Format("{0}x + {1}y + {2}z", x, y, z));
	}
	
	public static bool operator == (IntCoord c1, IntCoord c2) {
		return c1.x == c2.x && c1.y == c2.y && c1.z == c2.z;
	}
	
	public static bool operator != (IntCoord c1, IntCoord c2) {
		return c1.x != c2.x || c1.y != c2.y || c1.z != c2.z;
	}
	
	public static IntCoord operator *(IntCoord c1, float scale)
	{
		return new IntCoord(c1.x * scale, c1.y * scale,c1.z * scale);
	}
	
	public static int Dot(IntCoord c1, IntCoord c2)
	{
		return (c1.x * c2.x) + (c1.y * c2.y) + (c1.z * c2.z);
	}
	
	public static IntCoord operator +(IntCoord c1, IntCoord c2) 
    {
		return new IntCoord(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
    }
	
	public static IntCoord operator -(IntCoord c1, IntCoord c2) 
    {
		return new IntCoord(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
    }
	
	public Vector3 ToVector3()
	{
		return new Vector3(x, y, z);	
	}
	
	public static implicit operator IntCoord(Vector3 i)
    {
        return new IntCoord(i.x,i.y,i.z);
    }
	
	public int[] ToArray()
	{
		return new int[3] {x,y,z};	
	}
	
	public static IntCoord Abs(IntCoord coord)
	{
		coord.x = Mathf.Abs (coord.x);
		coord.y = Mathf.Abs (coord.y);
		coord.z = Mathf.Abs (coord.z);
		return coord;
	}
	
	public static int MagnitudeSqr(IntCoord a)
	{
		return a.x*a.x + a.y*a.y + a.z*a.z;	
	}
	
	public static float Magnitude(IntCoord a)
	{
		return Mathf.Sqrt (a.x*a.x + a.y*a.y + a.z*a.z);	
	}
}

class MathTools
{
	public static float RoundNearest(float val, float nearest)
	{
		return (float)Math.Round(val / nearest) * nearest;
	}
	
	public static Vector3 RoundNearestVec(Vector3 val, float nearest)
	{
		val.x = (float)Math.Round(val.x / nearest) * nearest;
		val.y = (float)Math.Round(val.y / nearest) * nearest;
		val.z = (float)Math.Round(val.z / nearest) * nearest;
		return val;
	}
	
	public static Vector3 FloorNearestVec(Vector3 val, Vector3 nearest)
	{
		val.x = (float)Math.Floor(val.x / nearest.x) * nearest.x;
		val.y = (float)Math.Floor(val.y / nearest.y) * nearest.y;
		val.z = (float)Math.Floor(val.z / nearest.z) * nearest.z;
		return val;
	}
	
	public static Vector3 RoundNearestVec(Vector3 val, Vector3 nearest)
	{
		val.x = (float)Math.Round(val.x / nearest.x) * nearest.x;
		val.y = (float)Math.Round(val.y / nearest.y) * nearest.y;
		val.z = (float)Math.Round(val.z / nearest.z) * nearest.z;
		return val;
	}
	
	public static Vector3 ScalarMultiply(Vector3 a, Vector3 b)
	{
		Vector3 result = new Vector3(a.x*b.x, a.y*b.y, a.z*b.z);
		return result;
	}
	
	public static bool IsInRange(float val, float min, float max)
	{
		return (val >= min && val <= max);	
	}
	
	public static void SometimesLog(object msg, int chance)
	{
		if (UnityEngine.Random.Range(0,chance) == 0)
		{
			Debug.Log (msg);
		}
	}

	public static float Gaussian(float x, float amp, float position, float width)
	{
		return amp * Mathf.Exp (-((x-position)*(x-position))/(width*width));
	}
}

