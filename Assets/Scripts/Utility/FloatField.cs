using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class FloatField : ScriptableObject
{
	//Properties
	public bool IsEmpty
	{
		get
		{
			return fieldArray.Count == 0;
		}
	}
	public int Height
	{
		get { return height; }
	}
	public int Width
	{
		get { return width; }
	}
	public int Size
	{
		get { return Width * Height; }
	}

	//Data
	[SerializeField]
	//[HideInInspector]
	public List<float> fieldArray = new List<float>();

	//Size params
	[SerializeField]
	private int height;
	[SerializeField]
	private int width;

	//Field Information
	[SerializeField]
	private float minVal = float.PositiveInfinity;
	[SerializeField]
	private float maxVal = float.NegativeInfinity;

	[SerializeField]
	private Texture2D previewTex;

	//Functions

	//Create a new float field with a specified size
	public void Create(int _height, int _width, float value = 0.0f)
	{
		height = _height;
		width = _width;
		if (!IsEmpty) //Do we have anything inside us?
		{
			Texture2D.DestroyImmediate(previewTex);
			fieldArray.Clear ();
		}
		previewTex = new Texture2D(height, width); //Recreate the preview

		//Assign default value
		for (int i=0; i < Size; i++)
		{
			fieldArray.Add(value);
		}
	    Debug.Log("FA COUNT " + fieldArray.Count);
		minVal = value;
		maxVal = value;

	    UpdatePreview();
	}

	public void UpdatePreview()
	{
		Point curPoint;
		Color curCol = new Color();
		for (int i=0; i < Size; i++)
		{
			curPoint = IndexToCoord (i);
			curCol.r = GetValue (i);
			curCol.g = GetValue (i);
			curCol.b = GetValue (i);
			previewTex.SetPixel (curPoint.x, curPoint.y, curCol);
		}
		previewTex.Apply ();
	}

    //Update a certain amount of the preview texture
    public void UpdatePreviewAt(Rect box)
    {
        if (previewTex == null)
            previewTex = new Texture2D(Width, Height);
        Point curPoint;
        Color curCol = new Color();

        for (int x = (int)box.left; x < (int)box.right; x++)
        {
            for (int y = (int)box.top; y < (int)box.bottom; y++)
            {
                int index = CoordToIndex(x, y);
                curPoint = IndexToCoord(index);
                curCol.r = GetValue(index);
                curCol.g = GetValue(index);
                curCol.b = GetValue(index);
                previewTex.SetPixel(x, y, curCol);
            }
        }
        Debug.Log("Updating preview");

        previewTex.Apply();
    }

	//Create from Texture
	public void CreateFromTexture(Texture2D tex)
	{
	    Debug.Log("CREATE FROM TEXTURE");
		height = tex.height;
		width = tex.width;
		if (!IsEmpty) //Do we have anything inside us?
		{
			fieldArray.Clear ();
		}
		previewTex = new Texture2D(height, width); //Recreate the preview
		Point coord;
		for (int i=0; i < Size; i++)
		{
			coord = IndexToCoord (i);
			//Debug.Log (i);
			fieldArray.Add(tex.GetPixel(coord.x, coord.y).grayscale);
			if (fieldArray[i] > maxVal)
				maxVal = fieldArray[i];
			if (fieldArray[i] < minVal)
				minVal = fieldArray[i];
		}
		previewTex.SetPixels (tex.GetPixels ());
		previewTex.Apply ();
	}

	//Conversion from coord to index / vice versa
	public int CoordToIndex(int x, int y)
	{
		return (y * width) + x;
	}

	public Point IndexToCoord(int index)
	{
		return new Point(index % width, (int)(index / width));
	}

	//Validations
	public bool IndexIsValid(int index)
	{
		return (index >= 0) && (index <= fieldArray.Count);
	}
	public bool CoordIsValid(int x, int y)
	{
		return IndexIsValid(CoordToIndex (x,y));
	}

	//Get and Set
	public float GetValue(int index)
	{
		if (!IndexIsValid (index))
		{
		    throw new Exception("Get Out of bounds on float field: " + index);
		    // return;
		}
		return fieldArray[index];
	}
	public float GetValue(int x, int y)
	{
		int index = CoordToIndex(x,y);
        if (!IndexIsValid(index))
        {
            throw new Exception("Get Out of bounds on float field: " + x + " " + y + "(INDEX " +index);
            // return;
        }

		return GetValue (index);
	}

	public void SetValue(int x, int y, float value)
	{
		int index = CoordToIndex (x,y);
		SetValue (index, value);
	}
	public void SetValue(int index, float value)
	{
		if (!IndexIsValid (index))
		{
            throw new Exception("Set Out of bounds on float field: " + index);
			return;
		}
		fieldArray[index] = value;
		if (fieldArray[index] > maxVal)
			maxVal = fieldArray[index];
		if (fieldArray[index] < minVal)
			minVal = fieldArray[index];
	}

	//Convers the float field to a texture
	public Texture2D GetTexture()
	{
		Texture2D tex = new Texture2D(Width, Height);
		Point coord;
		Color color;
		for (int i = 0; i < Size; i++)
		{
			coord = IndexToCoord(i);
			color = new Color(fieldArray[i], fieldArray[i], fieldArray[i]); //Get gray colour
			tex.SetPixel(coord.x, coord.y, color);
		}
		tex.Apply();
		return tex;
	}

	public Vector2 GetFieldGradient(int index, int gap)
	{
		Point coord = IndexToCoord (index);
		return GetFieldGradient(coord.x, coord.y, gap);
	}
	public Vector2 GetFieldGradient(int x, int y, int gap)
	{
		float valAtPos = GetValue (x,y);
		Vector2 gradient = new Vector2();

		if (x >= Width - gap - 2 || y >= Height - gap - 2)
		{
			return gradient;
		}

		gradient.x = GetValue (x+gap,y) - valAtPos;
		gradient.y = GetValue (x,y+gap) - valAtPos;
		return gradient;
	}

    public void CalculateGradient(FloatField output, int subSamples = 1)
    {
        Vector2 gradient = new Vector2();
        output.Create(Height, Width);
        float grad = 0.0f;
        float maxGrad = 0.0f;
        for (int i = 0; i < Size; i++)
        {
            gradient = GetFieldGradient(i, subSamples);
            grad = gradient.magnitude;
            if (grad > maxGrad)
                maxGrad = grad;
            output.SetValue(i, grad);
        }
        //Normalize the gradient map
        if (maxGrad != 0.0f)
        {
            for (int i = 0; i < Size; i++)
            {
                output.SetValue(i, output.GetValue(i)/maxGrad);
            }
        }

        output.UpdatePreview ();
	}
}
