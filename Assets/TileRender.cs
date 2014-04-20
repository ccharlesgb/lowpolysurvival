using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof

public class TileRender : MonoBehaviour {

	//Mesh Data
	public List<Vector3> vertices;
	public List<Vector3> normals;
	public List<int> triangles;
	public List<int> waterTriangles;
	public List<Vector2> uvs;

	public static int sideLength = 10;
	public static float squareSize = 1.0f;

	static public Vector3 GetTileBounds()
	{
		return Ve
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
				Vector3 position = new Vector3(-
				CreateFace(
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray ();
		mesh.SetTriangles (triangles.ToArray (), 0);
		mesh.SetTriangles (waterTriangles.ToArray (), 1);
		mesh.normals = normals.ToArray ();
		mesh.RecalculateBounds ();	

		GetComponent<MeshFilter>().sharedMesh = mesh;
	}

	void CreateFace(Vector3 pos)
	{
		int vertCount = vertices.Count;
		Vector3 origin = pos;
		
		float ext = squareSize / 2.0f; //The 'extent' of the square
		vertices.Add(origin + new Vector3(-ext,  0, ext));
		vertices.Add(origin + new Vector3(ext,  0, ext));
		vertices.Add(origin + new Vector3(ext, 0, -ext));
		vertices.Add(origin + new Vector3(-ext, 0, -ext));

		triangles.Add(vertCount);
		triangles.Add(vertCount + 1);
		triangles.Add(vertCount + 2);
		
		triangles.Add(vertCount);
		triangles.Add(vertCount + 2);
		triangles.Add(vertCount + 3);

		for (int i=0; i < 4; i++)
		{
			normals.Add(Vector3.up);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
