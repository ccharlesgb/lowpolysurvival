using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

[ExecuteInEditMode]
public class TileRender : MonoBehaviour
{
	protected MeshFilter meshFilter
	{
		get
		{
			if(this._meshFilter == null)
			{
				this._meshFilter = gameObject.GetComponent<MeshFilter>();
			}
			return this._meshFilter;
		}
	}

	protected Mesh mesh
	{
		get
		{
			if(_mesh != null)
			{
				return _mesh;
			}
			else
			{
				
				if(meshFilter.sharedMesh == null)
				{
					Mesh newMesh = new Mesh();
					_mesh = meshFilter.sharedMesh = newMesh;
				}
				else
				{
					_mesh = meshFilter.sharedMesh;
				}
				return _mesh;
			}
		}
	}

	private void ReCalculateMesh(bool allAttributes)
	{
		if(allAttributes)
		{
			if(mesh == null)
			{
				Debug.LogError("Could not access or create a mesh", this);
				return;
			}
			mesh.Clear();
		}
		mesh.vertices = vertices;
		
		if(allAttributes)
		{
			mesh.uv = uv;
			mesh.triangles = triangles;
		}
		mesh.normals = normals;
		mesh.RecalculateBounds();
	}

	
	private MeshFilter _meshFilter = null;
	private Mesh _mesh = null;


	public float lastCheckForUpdate;
	public Vector3 buildPos;

	//Mesh Data
	public List<Vector3> l_vertices = new List<Vector3>();
	public List<Vector3> l_normals = new List<Vector3>();
	public List<int> l_triangles = new List<int>();
	public List<Vector2> l_uvs = new List<Vector2>();

	protected Vector3[] vertices 
	{
		get
		{
			// Given the side, return the area of a square: 
			return l_vertices.ToArray();
		}
	}
	protected Vector2[] uv 
	{ 
		get
		{
			return l_uvs.ToArray();
		}
	}
	
	
	protected int[] triangles 
	{ 
		get
		{
			return l_triangles.ToArray();
		}
	}

	protected Vector3[] normals
	{ 
		get
		{
			return l_normals.ToArray();
		}
	}
	
	public static int sideLength = 32;
	public static float squareSize = 2.0f;

	public HeightMap mHeights;
	public SplatMap mSplats;

	MeshRenderer _meshRenderer;

	static public Vector3 GetTileBounds()
	{
		return new Vector3(sideLength * squareSize, 0, sideLength * squareSize);
	}

	public void ClearMesh()
	{	
		l_vertices.Clear ();
		l_uvs.Clear ();
		l_triangles.Clear ();
		l_normals.Clear ();
	}
	
	void OnEnable() 
	{
		_meshRenderer = GetComponent<MeshRenderer>();
		//gameObject.hideFlags = HideFlags.HideAndDontSave;
		//gameObject.hideFlags = HideFlags.DontSave;
		//Debug.Log ("Tile Enable" + Application.isPlaying);
	}

	public void CreateMesh()
	{
		Material mat = new Material(_meshRenderer.sharedMaterial);
		mat.SetTexture ("_Control",mSplats.GetTileSplat (this));
		_meshRenderer.material = mat;
		BuildMesh ();
		GetComponent<MeshCollider>().sharedMesh = mesh;
		lastCheckForUpdate = Time.realtimeSinceStartup;

		ReCalculateMesh(true);
	}

	void BuildMesh()
	{
		ClearMesh ();

		for (int x = 0; x < sideLength; x++)
		{
			for (int z = 0; z < sideLength; z++)
			{
				Vector3 position = new Vector3(squareSize * x, 0, squareSize*z);
				CreateFace(position);
			}
		}
		buildPos = transform.position;
	}
	
	void CreateFace(Vector3 pos)
	{
		Vector3 origin = pos;
		int vertCount  = l_vertices.Count;

		Vector3 p0 = origin + new Vector3(0,  0, squareSize);
		p0.y= mHeights.GetHeight (p0 + transform.position);
		l_vertices.Add (p0);

		Vector3 p1 = origin + new Vector3(squareSize,  0, squareSize);
		p1.y = mHeights.GetHeight (p1 + transform.position);
		l_vertices.Add (p1);

		Vector3 p2 = origin + new Vector3(squareSize, 0, 0);
		p2.y = mHeights.GetHeight (p2 + transform.position);
		l_vertices.Add (p2);

		//Build second
		l_vertices.Add (p0);

		l_vertices.Add (p2);

		Vector3 p3 = origin + new Vector3(0, 0, 0);
		p3.y = mHeights.GetHeight (p3 + transform.position);
		l_vertices.Add (p3);

		l_triangles.Add(vertCount);
		l_triangles.Add(vertCount + 1);
		l_triangles.Add(vertCount + 2);

		l_triangles.Add(vertCount + 3);
		l_triangles.Add(vertCount + 4);
		l_triangles.Add(vertCount + 5);

		Vector3 norm = Vector3.Cross(p1 - p0, p2 - p0).normalized;
		for (int i=0; i < 3; i++)
		{
			l_normals.Add(norm);
		}

		Vector3 norm2 = Vector3.Cross(p2 - p0, p3 - p0).normalized;
		for (int i=0; i < 3; i++)
		{
			l_normals.Add(norm2);
		}

		for (int i = 0; i < 6; i++)
		{
			l_uvs.Add (GetUV (l_vertices[vertCount + i]));
		}

	}

	Vector2 GetUV(Vector3 vertPos)
	{
		float uvScale = 32.0f;
		Vector2 uv = new Vector2(vertPos.x, vertPos.z);
		
		uv /= (uvScale * squareSize);

		//if (Random.value < 0.001f)
			//Debug.Log (uv);
		return uv;
	}
}
