using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

[AddComponentMenu("Terrain/Height Map")]
public class HeightMap : MonoBehaviour 
{

	public FloatField heightField;

	public Texture2D heightPreview;
	public Texture2D heightGrad;
	public float heightScale;
	public float discreteCount;
	
	void Awake()
	{
		//if (heightGrad == null)
			//heightGrad = new Texture2D(GetMapWidth (),GetMapHeight ());
		//CalculateGradient ();
	}
	/*
	void CalculateGradient()
	{
		Vector2 gradient = new Vector2();
		Color[] cols = heightPic.GetPixels();
		Color[] gradPixels = cols;
		Color curCol;
		float maxGrad = 0;
		for (int y=0; y < GetMapHeight()-4; y++)
		{
			for (int x=0; x < GetMapWidth()-4; x++)
			{
				curCol = cols[x + y * GetMapHeight()];
				gradient.x = cols[(x + 4) + y * GetMapHeight ()].r - cols[x + y * GetMapHeight ()].r;
				gradient.y = cols[x + (y + 4) * GetMapHeight ()].r - cols[x + y * GetMapHeight ()].r;
				float gradCol = gradient.magnitude;
				if (gradCol > maxGrad)
					maxGrad = gradCol;
				gradPixels[x + y * GetMapHeight()] = new Color(gradCol,gradCol,gradCol);
			}
		}
		//Normalize the gradient map
		for (int i=0; i < GetMapHeight() * GetMapWidth(); i++)
		{
			gradPixels[i].r /= maxGrad;
			gradPixels[i].g /= maxGrad;
			gradPixels[i].b /= maxGrad;
		}
		heightGrad.SetPixels (gradPixels);
		heightGrad.Apply ();
	}
	*/
	Vector3 WorldToMap(Vector3 pos)
	{
		return Vector3.zero;
	}


}
