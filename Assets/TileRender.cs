using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class TileRender : MonoBehaviour {

	public float lastCheckForUpdate;
	public Vector3 buildPos;

	//Mesh Data
	public List<Vector3> vertices;
	public List<Vector3> normals;
	public List<int> triangles;
	public List<int> waterTriangles;
	public List<Vector2> uvs;

	public static int sideLength = 10;
	public static float squareSize = 1.0f;

	public HeightMap mHeights;

	static public Vector3 GetTileBounds()
	{
		return new Vector3(sideLength * squareSize, 0, sideLength * squareSize);
	}

	void AppendVertices(Vector3[] verts)
	{
		vertices.AddRange(verts);
	}
	
	void AppendNormals(Vector3[] norms)
	{
		normals.AddRange(norms);
	}
	
	void AppendTriangles(int[] tris, int submesh)
	{
		if (submesh == 0)
			triangles.AddRange(tris);
		else
			waterTriangles.AddRange(tris);
	}
	
	void AppendUVs(Vector2[] uv)
	{
		uvs.AddRange (uv);
	}

	public void ClearMesh()
	{	
		vertices.Clear ();
		uvs.Clear ();
		triangles.Clear ();
		waterTriangles.Clear ();
		normals.Clear ();
	}

	// Use this for initialization
	void Start()
	{
		BuildMesh ();
		lastCheckForUpdate = Time.realtimeSinceStartup;
	}

	void Awake() 
	{
		vertices = new List<Vector3>();
		triangles = new List<int>();
		waterTriangles = new List<int>();
		normals = new List<Vector3>();
		uvs = new List<Vector2>();
	}

	void BuildMesh()
	{
		ClearMesh ();
		Mesh mesh = new Mesh();

		for (int x = 0; x < sideLength; x++)
		{
			for (int z = 0; z < sideLength; z++)
			{
				Vector3 position = new Vector3(squareSize * x, 0, squareSize*z);
				CreateFace(position);
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray ();
		mesh.SetTriangles (triangles.ToArray (), 0);
		mesh.normals = normals.ToArray ();
		mesh.RecalculateBounds ();	

		GetComponent<MeshFilter>().sharedMesh = mesh;

		buildPos = transform.position;
	}

	void CreateFace(Vector3 pos)
	{
		int vertCount = vertices.Count;
		Vector3 origin = pos;
		
		float ext = squareSize / 2.0f; //The 'extent' of the square

		float vertHeight = 0.0f;


		Vector3 p0 = origin + new Vector3(0,  0, squareSize);
		p0.y= mHeights.GetHeight (p0 + transform.position);
		vertices.Add(p0);

		Vector3 p1 = origin + new Vector3(squareSize,  0, squareSize);
		p1.y = mHeights.GetHeight (p1 + transform.position);
		vertices.Add(p1);

		Vector3 p2 = origin + new Vector3(squareSize, 0, 0);
		p2.y = mHeights.GetHeight (p2 + transform.position);
		vertices.Add(p2);
	
		//Build second
		vertices.Add(p0);
		vertices.Add(p2);

		Vector3 p3 = origin + new Vector3(0, 0, 0);
		p3.y = mHeights.GetHeight (p3 + transform.position);
		vertices.Add(p3);

		triangles.Add(vertCount);
		triangles.Add(vertCount + 1);
		triangles.Add(vertCount + 2);
		
		triangles.Add(vertCount + 3);
		triangles.Add(vertCount + 4);
		triangles.Add(vertCount + 5);

		Vector3 norm = Vector3.Cross(p1 - p0, p2 - p0);
		for (int i=0; i < 3; i++)
		{
			normals.Add(norm);
		}

		Vector3 norm2 = Vector3.Cross(p1 - p0, p3 - p0);
		for (int i=0; i < 3; i++)
		{
			normals.Add(norm2);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (lastCheckForUpdate + 0.5f < Time.realtimeSinceStartup)
		{
			if (buildPos != transform.position)
			{
				BuildMesh ();
				lastCheckForUpdate = Time.realtimeSinceStartup;
			}
		}
	}
}
