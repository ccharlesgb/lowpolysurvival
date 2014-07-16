using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Terrain/Map")]
public class Map : MonoBehaviour
{
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

	public float discreteCount;
	public float heightScale;

	//public Texture2D heightTexture;
	
	public FloatField heightField;
	public VectorField splatField;
	public SplatSettings splatSettings;
	public TerrainSettings terrainSettings;

    [System.Serializable]
    public class BrushSettings
    {
        public float size;
        public float opacity;
        public int paintChannel;
    }

    //Called by the editor paints a certain amount of the splat channel to the map
    public void PaintSplat(Vector3 pos, BrushSettings settings)
    {
        Point splatCoord = WorldToFieldIndex(pos, splatField);

        int brushSize = (int)settings.size;
        float brushOpacity = settings.opacity;
        float brushWidth = brushSize / 2.0f;
        for (int x = -brushSize; x < brushSize; x++)
        {
            for (int y = -brushSize; y < brushSize; y++)
            {
                Vector3 originalVal = splatField.GetValue(splatCoord.x + x, splatCoord.y + y);

                float brushStrength = MathTools.Gaussian2D(new Vector2(x, y), brushOpacity, Vector2.zero,
                    new Vector2(brushWidth, brushWidth));

                Vector3 addVal = new Vector3(-brushStrength, -brushStrength, -brushStrength);
                //Debug.Log("Brush Strength " + brushStrength);
                addVal[settings.paintChannel - 1] += brushStrength * 2;
                originalVal += addVal;
                originalVal.x = Mathf.Clamp(originalVal.x, 0.0f, 1.0f);
                originalVal.y = Mathf.Clamp(originalVal.y, 0.0f, 1.0f);
                originalVal.z = Mathf.Clamp(originalVal.z, 0.0f, 1.0f);

                splatField.SetValue(splatCoord.x + x, splatCoord.y + y, originalVal);
            }
        }

        Rect box = new Rect(splatCoord.x - brushSize, splatCoord.y - brushSize, brushSize * 2, brushSize * 2);
        splatField.UpdatePreviewAt(box);
    }

    //Called by the editor paints the vertex heights
    public void PaintHeight(Vector3 pos, BrushSettings settings)
    {
        Point heightCoord = WorldToFieldIndex(pos, heightField);

        int brushSize = (int)settings.size;
        float brushOpacity = settings.opacity;
        float brushWidth = brushSize / 2.0f;
        


        for (int x = -brushSize; x < brushSize; x++)
        {
            for (int y = -brushSize; y < brushSize; y++)
            {
                float originalVal = heightField.GetValue(heightCoord.x + x, heightCoord.y + y);

                //2D gaussian can be used to model a 'soft' paint brush
                float brushStrength = MathTools.Gaussian2D(new Vector2(x, y), brushOpacity, Vector2.zero,
                    new Vector2(brushWidth, brushWidth));

                originalVal += brushStrength;
                originalVal = Mathf.Clamp(originalVal, 0.0f, 1.0f);

                heightField.SetValue(heightCoord.x + x, heightCoord.y + y, originalVal);
            }
        }

        Rect box = new Rect(heightCoord.x - brushSize, heightCoord.y - brushSize, brushSize * 2, brushSize * 2);
        heightField.UpdatePreviewAt(box);

        TilePlacer placer = GetComponent<TilePlacer>();

        float brushSizeWorld = terrainSettings.tileSquareSize * 2.0f;

        TileRender tile = placer.GetTileAt(pos + new Vector3(-brushSizeWorld, 0, -brushSizeWorld));
        tile.CreateMesh();

        tile = placer.GetTileAt(pos + new Vector3(+brushSizeWorld, 0, -brushSizeWorld));
        tile.CreateMesh();

        tile = placer.GetTileAt(pos + new Vector3(+brushSizeWorld, 0, +brushSizeWorld));
        tile.CreateMesh();

        tile = placer.GetTileAt(pos + new Vector3(-brushSizeWorld, 0, +brushSizeWorld));
        tile.CreateMesh();
    }

    public Point WorldToFieldIndex(Vector3 pos, VectorField field)
	{
		Vector3 mapBounds = TileRender.GetTileBounds () * terrainSettings.tileArraySideLength;
		Rect mapRect = new Rect();
		mapRect.center = Vector3.zero;
		mapRect.Set(-mapBounds.x / 2.0f, -mapBounds.z / 2.0f, mapBounds.x, mapBounds.z);
		
		//Transform theses coordinates into a fraction of the total map
		//Then multiply by heightmap dimensions to get the corresponding pixel
		float x_f = (pos.x / mapRect.width) * (field.Width - 1);
		float y_f = (pos.z / mapRect.height) * (field.Height - 1);
		if (x_f < 0 || x_f > field.Width || y_f < 0 || y_f > field.Height)
		{
			Debug.Log("Height coords out of bounds x : " + x_f + " y : " + y_f);
		}
		int x = Mathf.FloorToInt (x_f);
		int y = Mathf.FloorToInt (y_f);
		
		return new Point(x,y);
	}
	public Point WorldToFieldIndex(Vector3 pos, FloatField field)
	{
		Vector3 mapBounds = TileRender.GetTileBounds () * terrainSettings.tileArraySideLength;
		Rect mapRect = new Rect();
		mapRect.center = Vector3.zero;
		mapRect.Set(-mapBounds.x / 2.0f, -mapBounds.z / 2.0f, mapBounds.x, mapBounds.z);
		
		//Transform theses coordinates into a fraction of the total map
		//Then multiply by heightmap dimensions to get the corresponding pixel
		float x_f = (pos.x / mapRect.width) * (field.Width - 1);
		float y_f = (pos.z / mapRect.height) * (field.Height - 1);
		if (x_f < 0 || x_f > field.Width || y_f < 0 || y_f > field.Height)
		{
			Debug.Log("Height coords out of bounds x : " + x_f + " y : " + y_f);
		}
		int x = Mathf.FloorToInt (x_f);
		int y = Mathf.FloorToInt (y_f);

		return new Point(x,y);
	}

	public float GetTerrainHeight(Vector3 pos)
	{
		Point heightPoint = WorldToFieldIndex (pos, heightField);

		float val = heightField.GetValue(heightPoint.x, heightPoint.y);
		
		//Discretize the height (More blocky)
		if (discreteCount > 0.0f)
		{
			float nearest = 1/discreteCount;
			val = Mathf.Round(val / nearest) * nearest;
		}
		return val * heightScale;
	}
}
