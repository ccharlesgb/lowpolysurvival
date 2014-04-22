using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class HeightMap : MonoBehaviour {

	public Texture2D heightPic;
	public Texture2D heightGrad;
	public float heightScale;
	public float discreteCount;
	public bool discreteOn;
	
	void Awake()
	{
		if (heightGrad == null)
			heightGrad = new Texture2D(GetMapWidth (),GetMapHeight ());
		CalculateGradient ();
	}

	void CalculateGradient()
	{
		Vector2 gradient = new Vector2();
		Color[] cols = heightPic.GetPixels();
		Color[] gradPixels = cols;
		Color curCol;
		float maxGrad = 0;
		for (int y=0; y < GetMapHeight()-8; y++)
		{
			for (int x=0; x < GetMapWidth()-8; x++)
			{
				curCol = cols[x + y * GetMapHeight()];
				/*if (x < 20 && y < 10)
				{
					Debug.Log ("FI " + (x + 1) + y * GetMapHeight ());
					Debug.Log ("First" + cols[(x + 1) + y * GetMapHeight ()].r);
					Debug.Log ("SI " + (x) + y * GetMapHeight ());
					Debug.Log ("Second" + cols[(x) + y * GetMapHeight ()].r);
				}*/
				gradient.x = cols[(x + 8) + y * GetMapHeight ()].r - cols[x + y * GetMapHeight ()].r;
				gradient.y = cols[x + (y + 8) * GetMapHeight ()].r - cols[x + y * GetMapHeight ()].r;
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

	public int GetMapHeight()
	{
		return heightPic.height;
	}

	public int GetMapWidth()
	{
		return heightPic.width;
	}

	Vector3 WorldToMap(Vector3 pos)
	{
		return Vector3.zero;
	}

	public float GetHeight(Vector3 pos)
	{
		Vector3 mapBounds = TileRender.GetTileBounds () * 16; //10 by 10 tiles
		Rect mapRect = new Rect();
		mapRect.center = Vector3.zero;
		mapRect.Set(-mapBounds.x / 2.0f, -mapBounds.z / 2.0f, mapBounds.x, mapBounds.z);

		//Transform theses coordinates into a fracon of the total map
		//Then multiply by heightmap dimensions to get the corresponding pixel
		float x_f = (pos.x / mapRect.width) * (GetMapWidth () - 1);
		float y_f = (pos.z / mapRect.height) * (GetMapWidth () - 1);
		if (x_f < 0 || x_f > GetMapWidth () || y_f < 0 || y_f > GetMapHeight ())
		{
			Debug.Log("Height coords out of bounds x : " + x_f + " y : " + y_f);
		}
		int x = Mathf.FloorToInt (x_f);
		int y = Mathf.FloorToInt (y_f);
		float val = heightPic.GetPixel(x,y).r;
		if (x == GetMapHeight ())
		{
			//Debug.Log ("Pos " + pos + " X_F " + x_f + " Y_F " + y_f + " HEIGHT " + val * heightScale);
		}
		if (discreteOn)
		{
			float nearest = 1/discreteCount;
			val = Mathf.Round(val / nearest) * nearest;
		}
		return val * heightScale;
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
