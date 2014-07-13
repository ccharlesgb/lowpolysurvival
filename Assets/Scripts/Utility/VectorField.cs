using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VectorField
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
	private List<Vector3> fieldArray = new List<Vector3>();
	
	//Size params
	[SerializeField]
	private int height;
	[SerializeField]
	private int width;
	
	//Field Information
	[SerializeField]
	private Vector3 minVal = new Vector3(float.PositiveInfinity,float.PositiveInfinity,float.PositiveInfinity);
	[SerializeField]
	private Vector3 maxVal = new Vector3(float.NegativeInfinity,float.NegativeInfinity,float.NegativeInfinity);
	
	[SerializeField]
	private Texture2D previewTex;
	
	//Functions
	
	//Create a new float field with a specified size
	public void Create(int _height, int _width, Vector3 value)
	{
		if (!IsEmpty && (_height != Height || _width != Width)) //Do we have anything inside us? Do we need to rebuild our preview?
		{
			Texture2D.DestroyImmediate(previewTex);
			previewTex = new Texture2D(_height, _width); //Recreate the preview
			fieldArray.Clear ();
		}
		height = _height;
		width = _width;
		
		//Assign default value
		for (int i=0; i < Size; i++)
		{
			fieldArray.Add(value);
		}
		minVal = value;
		maxVal = value;
	}
	
	public void UpdatePreview()
	{
		if (previewTex == null)
			previewTex = new Texture2D(Width, Height);
		Point curPoint;
		Color curCol = new Color();
		for (int i=0; i < Size; i++)
		{
			curPoint = IndexToCoord (i);
			curCol.r = GetValue(i).x;
			curCol.g = GetValue(i).y;
			curCol.b = GetValue(i).z;
			previewTex.SetPixel (curPoint.x, curPoint.y, curCol);
		}
		previewTex.Apply ();
	}

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
                curCol.r = GetValue(index).x;
                curCol.g = GetValue(index).y;
                curCol.b = GetValue(index).z;
                previewTex.SetPixel(x, y, curCol);
            }
        }


        previewTex.Apply();
    }
	
	//Create from Texture
	public void CreateFromTexture(Texture2D tex)
	{
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
			Vector3 curVec = new Vector3(tex.GetPixel(coord.x, coord.y).r, tex.GetPixel(coord.x, coord.y).g, tex.GetPixel(coord.x, coord.y).b);
			fieldArray.Add(curVec);
			if (fieldArray[i].sqrMagnitude > maxVal.sqrMagnitude)
				maxVal = fieldArray[i];
			if (fieldArray[i].sqrMagnitude < minVal.sqrMagnitude)
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
        return IndexIsValid(CoordToIndex(x, y));
    }
	
	//Get and Set
	public Vector3 GetValue(int index)
	{
		if (!IndexIsValid (index))
		{
            throw new Exception("Out of bounds on vector field");
            //return;
		}
		return fieldArray[index];
	}
	public Vector3 GetValue(int x, int y)
	{
		int index = CoordToIndex(x,y);
		return GetValue (index);
	}
	
	public void SetValue(int x, int y, Vector3 value)
	{
		int index = CoordToIndex (x,y);
		SetValue (index, value);
	}
	public void SetValue(int index, Vector3 value)
	{
		if (!IndexIsValid (index))
		{
            throw new Exception("Out of bounds on vector field");
            return;
		}
		fieldArray[index] = value;
		if (fieldArray[index].sqrMagnitude > maxVal.sqrMagnitude)
			maxVal = fieldArray[index];
		if (fieldArray[index].sqrMagnitude < minVal.sqrMagnitude)
			minVal = fieldArray[index];
	}
	
	//Convers the float field to a texture
	public Texture2D GetTexture()
	{
		return previewTex;
	}
	
	public Vector2 GetFieldGradient(int index, int gap)
	{
		Point coord = IndexToCoord (index);
		return GetFieldGradient(coord.x, coord.y, gap);
	}
	public Vector2 GetFieldGradient(int x, int y, int gap)
	{
		Vector3 valAtPos = GetValue (x,y);
		Vector2 gradient = new Vector2();
		
		if (x >= Width - gap - 1 || y >= Height - gap - 1)
		{
			return gradient;
		}
		
		//gradient.x = GetValue (x+gap,y) - valAtPos;
		//gradient.y = GetValue (x,y+gap) - valAtPos;
		return gradient;
	}
	
	public void CalculateGradient(VectorField output, int subSamples=1)
	{
		Vector2 gradient = new Vector2();
		output.Create (Height, Width, Vector3.zero);
		float grad = 0.0f;
		float maxGrad = 0.0f;
		for (int i=0; i < Size; i++)
		{
			gradient = GetFieldGradient(i, subSamples);
			grad = gradient.magnitude;
			if (grad > maxGrad)
				maxGrad = grad;
			//output.SetValue (i, grad);
		}
		//Normalize the gradient map
		for (int i=0; i < Size; i++)
		{
			output.SetValue(i, output.GetValue (i) / maxGrad);
		}
		output.UpdatePreview ();
	}
}
