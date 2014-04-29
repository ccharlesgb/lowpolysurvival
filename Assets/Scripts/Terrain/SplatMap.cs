using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

[AddComponentMenu("Terrain/Splat Map")]
public class SplatMap : MonoBehaviour {

	public Texture2D splatPic;

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
	public float mat4_amp;
	public float mat4_pos;
	public float mat4_width;


	public int GetMapHeight()
	{
		return splatPic.height;
	}
	
	public int GetMapWidth()
	{
		return splatPic.width;
	}

	//Auto generate splat map from gradients of height field
	public void CreateMap(Texture2D grads)
	{
		Debug.Log ("CReate splat map");
		//Get the height gradients and create a spalt map from it
		if (splatPic == null)
			splatPic = new Texture2D(grads.width, grads.height);
		else
			splatPic.Resize (grads.width, grads.height);
		Color splatCol = new Color();
		float colSum = 0;

		float steepness = 0.0f;
		for (int x=0; x < grads.width; x++)
		{
			for (int z=0; z < grads.height; z++)
			{
				steepness = grads.GetPixel (x,z).r;
				splatCol.r = MathTools.Gaussian (steepness, mat1_amp, mat1_pos, mat1_width);
				splatCol.g = MathTools.Gaussian (steepness, mat2_amp, mat2_pos, mat2_width);
				splatCol.b = MathTools.Gaussian (steepness, mat3_amp, mat3_pos, mat3_width);
				//splatCol.a = MathTools.Gaussian (steepness, mat4_amp, mat4_pos, mat4_width);
				
				//Renormalize Splat Color
				colSum = splatCol.r + splatCol.g + splatCol.b;// + splatCol.a;
				if (colSum != 0.0f)
				{
					splatCol.r /= colSum;
					splatCol.g /= colSum;
					splatCol.g /= colSum;
					splatCol.a /= colSum;
				}
				splatPic.SetPixel(x,z, splatCol);
			} 
		}
		splatPic.Apply ();
	}
	
	Vector2 WorldToMap(Vector3 pos)
	{
		Vector3 mapBounds = TileRender.GetTileBounds () * 16; //10 by 10 tiles
		
		Rect mapRect = new Rect();
		mapRect.center = Vector3.zero;
		mapRect.Set(-mapBounds.x / 2.0f, -mapBounds.z / 2.0f, mapBounds.x, mapBounds.z);

		//Transform theses coordinates into a fracon of the total map
		//Then multiply by heightmap dimensions to get the corresponding pixel
		float x_f = (pos.x / mapRect.width) * (GetMapWidth () - 1);
		float y_f = (pos.z / mapRect.height) * (GetMapHeight () - 1);
		
		if (x_f < 0 || x_f > GetMapWidth () || y_f < 0 || y_f > GetMapHeight ())
		{
			Debug.Log("Height coords out of bounds x : " + x_f + " y : " + y_f);
		}
		return new Vector2(x_f,y_f);
	}

	public Texture2D GetTileSplat(TileRender tile)
	{
		//Debug.Log ("TILE POS " + tile.transform.position);
		Vector2 splatCoords = WorldToMap (tile.transform.position);

		int x = Mathf.RoundToInt (splatCoords.x);
		int y = Mathf.RoundToInt (splatCoords.y);

		Vector2 splatCoordsMax = WorldToMap (tile.transform.position + TileRender.GetTileBounds ());

		int x_max = Mathf.RoundToInt (splatCoordsMax.x);
		int y_max = Mathf.RoundToInt (splatCoordsMax.y);


		Texture2D control = new Texture2D(x_max - x,y_max - y);
		control.SetPixels (splatPic.GetPixels(x,y, x_max - x, y_max - y));
		control.filterMode = FilterMode.Point;
		control.Apply ();
		return control;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
