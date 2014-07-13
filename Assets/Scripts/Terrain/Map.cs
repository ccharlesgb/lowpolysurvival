using System;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Terrain/Map")]
public class Map : MonoBehaviour 
{
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

	public Texture2D heightTexture;
	
	public FloatField heightField;
	public VectorField splatField;
	public SplatSettings splatSettings;
	public TerrainSettings terrainSettings;

    public void EndTexturePaint()
    {
        

    }

    public void BeginTexturePaint()
    {
        int splatChannel = 0;
    }

    public void PaintSplat(Vector3 pos, int channel)
    {
        Debug.Log("pos " + pos);
        Point splatCoord = WorldToFieldIndex(pos, splatField);
        Debug.Log(splatCoord.x + "   " + splatCoord.y);

        int brushSize = 3;
        float brushOpacity = 1.0f;
        float brushWidth = 2.0f;
        for (int x = -brushSize; x < brushSize; x++)
        {
            for (int y = -brushSize; y < brushSize; y++)
            {
                Vector3 originalVal = splatField.GetValue(splatCoord.x + x, splatCoord.y + y);

                float brushStrength = MathTools.Gaussian2D(new Vector2(x, y), brushOpacity, Vector2.zero,
                    new Vector2(brushWidth, brushWidth));

                Vector3 addVal = new Vector3(-brushStrength, -brushStrength, -brushStrength);
                //Debug.Log("Brush Strength " + brushStrength);
                addVal[channel - 1] = brushStrength * 2;
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
	
	public void UpdateFloatFields()
	{
		heightField.CreateFromTexture(heightTexture);
		FloatField gradientMag = new FloatField();
		heightField.CalculateGradient(gradientMag, 4);
		splatField.Create (heightField.Height, heightField.Width, Vector3.zero);
		for (int i=0; i < gradientMag.Size; i++)
		{
			splatField.SetValue (i, splatSettings.GetSplatChannelValue (gradientMag.GetValue (i)));
		}
		splatField.UpdatePreview ();

	}
}
