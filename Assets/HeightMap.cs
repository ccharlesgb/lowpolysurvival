using UnityEngine;
using System.Collections;

public class HeightMap : MonoBehaviour {

	public Texture2D heightPic;
	public float heightScale;

	public float lastCheckForUpdate;

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
		Vector3 mapBounds = TileRender.GetTileBounds () * 4; //10 by 10 tiles
		Rect mapRect = new Rect();
		mapRect.center = Vector3.zero;
		mapRect.Set(-mapBounds.x / 2.0f, -mapBounds.z / 2.0f, mapBounds.x, mapBounds.z);

		float x_f = (pos.x / mapRect.width) * GetMapWidth ();
		float y_f = (pos.z / mapRect.height) * GetMapHeight ();
		int x = Mathf.RoundToInt (x_f);
		int y = Mathf.RoundToInt (y_f);
		float val = heightPic.GetPixel(x,y).r;
		if (pos.x == 5)
		{
			Debug.Log ("Pos " + pos + " X_F " + x_f + " Y_F " + y_f + " HEIGHT " + val * heightScale);
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
