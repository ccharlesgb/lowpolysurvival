using UnityEngine;
using System.Collections;

public class SplatMap : MonoBehaviour {

	public Texture2D splatPic;

	public int GetMapHeight()
	{
		return splatPic.height;
	}
	
	public int GetMapWidth()
	{
		return splatPic.width;
	}
	
	Vector2 WorldToMap(Vector3 pos)
	{
		Vector3 mapBounds = TileRender.GetTileBounds () * 8; //10 by 10 tiles
		
		Rect mapRect = new Rect();
		mapRect.center = Vector3.zero;
		mapRect.Set(-mapBounds.x / 2.0f, -mapBounds.z / 2.0f, mapBounds.x, mapBounds.z);
		
		float x_f = (pos.x / mapRect.width) * GetMapWidth ();
		float y_f = (pos.z / mapRect.height) * GetMapHeight ();
		
		if (x_f < 0 || x_f > GetMapWidth () || y_f < 0 || y_f > GetMapHeight ())
		{
			Debug.Log("Height coords out of bounds x : " + x_f + " y : " + y_f);
		}
		return new Vector2(x_f,y_f);
	}

	public Texture2D GetTileSplat(TileRender tile)
	{
		Vector2 splatCoords = WorldToMap (tile.transform.position);

		int x = Mathf.RoundToInt (splatCoords.x);
		int y = Mathf.RoundToInt (splatCoords.y);

		Vector2 splatCoordsMax = WorldToMap (tile.transform.position + TileRender.GetTileBounds ());

		int x_max = Mathf.RoundToInt (splatCoordsMax.x);
		int y_max = Mathf.RoundToInt (splatCoordsMax.y);


		Texture2D control = new Texture2D(x_max - x,y_max - y);
		control.SetPixels (splatPic.GetPixels(x,y, x_max - x, y_max - y));
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
