using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Terrain/Map")]
public class Map : MonoBehaviour
{
    public List<GameObject> tileList = new List<GameObject>(); 

    public Texture2D splatTexture;
    public Texture2D heightTexture;

	public static Map instance;

	public static Map Instance()
	{
		if (instance == null)
		{
			instance = FindObjectOfType<Map>();
			if (instance == null)
			{
				Debug.Log ("Can't find map instance");
			}
		}
		return instance;
	}

	void OnEnable()
	{
		splatSettings = GetComponent<SplatSettings>();
		terrainSettings = GetComponent<TerrainSettings>();
	}
	
	public SplatSettings splatSettings;
	public TerrainSettings terrainSettings;

    public void ClearTerrain()
    {
        foreach (GameObject tile in tileList)
            DestroyImmediate(tile);
    }

    public Point WorldToTextureCoords(Vector3 pos, float textureSize)
    {
        Vector3 mapBounds = TileRender.GetTileBounds() * terrainSettings.tileArraySideLength;
        Rect mapRect = new Rect();
        mapRect.center = Vector3.zero;
        mapRect.Set(-mapBounds.x / 2.0f, -mapBounds.z / 2.0f, mapBounds.x, mapBounds.z);

        //Transform theses coordinates into a fraction of the total map
        //Then multiply by heightmap dimensions to get the corresponding pixel
        float x_f = (pos.x / mapRect.width) * (textureSize - 1);
        float y_f = (pos.z / mapRect.height) * (textureSize - 1);
        if (x_f < 0 || x_f > textureSize || y_f < 0 || y_f > textureSize)
        {
            Debug.Log("Height coords out of bounds x : " + x_f + " y : " + y_f);
        }
        int x = Mathf.FloorToInt(x_f);
        int y = Mathf.FloorToInt(y_f);

        return new Point(x, y);
    }

	public float GetTerrainHeight(Vector3 pos)
	{
		Point heightPoint = WorldToTextureCoords(pos, heightTexture.width);

		float val = heightTexture.GetPixel(heightPoint.x, heightPoint.y).grayscale;
		
	    return val*terrainSettings.heightScale;
	}
}
