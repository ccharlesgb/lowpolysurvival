using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class TileRender : MonoBehaviour {

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
				Vector3 position = transform.position + new Vector3(squareSize * x, 0, squareSize*z);
				CreateFace(position);
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray ();
		mesh.SetTriangles (triangles.ToArray (), 0);
		mesh.normals = normals.ToArray ();
		mesh.RecalculateBounds ();	

		GetComponent<MeshFilter>().sharedMesh = mesh;
	}

	void CreateFace(Vector3 pos)
	{
		int vertCount = vertices.Count;
		Vector3 origin = pos;
		
		float ext = squareSize / 2.0f; //The 'extent' of the square

		float vertHeight = 0.0f;

		vertHeight = mHeights.GetHeight (origin + new Vector3(-ext,  0, ext));
		Vector3 p0 = origin + new Vector3(-ext,  vertHeight, ext);
		vertices.Add(p0);

		vertHeight = mHeights.GetHeight (origin + new Vector3(ext,  0, ext));
		Vector3 p1 = origin + new Vector3(ext,  vertHeight, ext);
		vertices.Add(p1);

		vertHeight = mHeights.GetHeight (origin + new Vector3(ext,  0, -ext));
		Vector3 p2 = origin + new Vector3(ext, vertHeight, -ext);
		vertices.Add(p2);
	
		vertHeight = mHeights.GetHeight (origin + new Vector3(-ext,  0, -ext));
		Vector3 p3 = origin + new Vector3(-ext, vertHeight, -ext);
		vertices.Add(p3);

		triangles.Add(vertCount);
		triangles.Add(vertCount + 1);
		triangles.Add(vertCount + 2);
		
		triangles.Add(vertCount);
		triangles.Add(vertCount + 2);
		triangles.Add(vertCount + 3);

		Vector3 norm = Vector3.Cross(p1 - p0, p2 - p0);

		for (int i=0; i < 4; i++)
		{
			normals.Add(norm);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
