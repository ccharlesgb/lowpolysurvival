using UnityEngine;
using System.Collections;

public class SplatSettings : MonoBehaviour 
{

	//Material 1 parameters
	public float mat1_amp;
	public float mat1_pos;
	public float mat1_width;
	//Material 2 parameters
	public float mat2_amp;
	public float mat2_pos;
	public float mat2_width;
	//Material 3 parameters
	public float mat3_amp;
	public float mat3_pos;
	public float mat3_width;
	//Material 4 parameters
	//public float mat4_amp;
	//public float mat4_pos;
	//public float mat4_width;

	public Vector3 GetSplatChannelValue(float steepness)
	{
		Vector3 splatCol = new Vector3();
		float colSum = 0;
		splatCol.x = MathTools.Gaussian (steepness, mat1_amp, mat1_pos, mat1_width);
		splatCol.y = MathTools.Gaussian (steepness, mat2_amp, mat2_pos, mat2_width);
		splatCol.z = MathTools.Gaussian (steepness, mat3_amp, mat3_pos, mat3_width);
		
		//Renormalize Splat Color
		colSum = splatCol.x + splatCol.y + splatCol.z;
		if (colSum != 0.0f)
		{
			splatCol.x /= colSum;
			splatCol.y /= colSum;
			splatCol.z /= colSum;
		}
		return splatCol;
	}
}
