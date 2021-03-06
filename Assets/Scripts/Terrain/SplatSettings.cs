﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class SplatSettings : MonoBehaviour
{
    public Texture2D control;

    //How "smoothed out" the splat map is
    public int splatSubSamples;

	//Material 1 parameters
    public Texture2D mat1;
    [Range(0.0f,1.0f)]
	public float mat1_amp;
    [Range(0.0f, 1.0f)]
	public float mat1_pos;
    [Range(0.0f, 0.1f)]
	public float mat1_width;
	//Material 2 parameters
    public Texture2D mat2;
    [Range(0.0f, 1.0f)]
	public float mat2_amp;
    [Range(0.0f, 1.0f)]
	public float mat2_pos;
    [Range(0.0f, 0.1f)]
	public float mat2_width;
	//Material 3 parameters
    public Texture2D mat3;
    [Range(0.0f, 1.0f)]
	public float mat3_amp;
    [Range(0.0f, 1.0f)]
	public float mat3_pos;
    [Range(0.0f, 0.1f)]
	public float mat3_width;
	//Material 4 parameters
	//public float mat4_amp;
	//public float mat4_pos;
	//public float mat4_width;

	public Color GetSplatChannelValue(float steepness)
	{
		Color splatCol = new Color();
		float colSum = 0;
		splatCol.r = MathTools.Gaussian (steepness, mat1_amp, mat1_pos, mat1_width);
		splatCol.g = MathTools.Gaussian (steepness, mat2_amp, mat2_pos, mat2_width);
		splatCol.b = MathTools.Gaussian (steepness, mat3_amp, mat3_pos, mat3_width);
		
		//Renormalize Splat Color
	    colSum = splatCol.r + splatCol.g + splatCol.b;
		if (colSum != 0.0f)
		{
			splatCol.r /= colSum;
			splatCol.g /= colSum;
			splatCol.b /= colSum;
		}
		return splatCol;
	}
}
