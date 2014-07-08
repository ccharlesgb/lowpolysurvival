using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

[ExecuteInEditMode]
[AddComponentMenu("Terrain/Splat Map")]
public class RockMesh : MonoBehaviour
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
	
	//Mesh Data
	private List<Vector3> l_vertices = new List<Vector3>();
	private List<Vector3> l_normals = new List<Vector3>();
	private List<int> l_triangles = new List<int>();
	private List<Vector2> l_uvs = new List<Vector2>();
	
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
	MeshRenderer _meshRenderer;

	//Settings
	public float squareSize = 2.0f;
	public float meshSize = 8.0f;
	public float noiseOffset = 0.5f;
	public float heightNoise = 1.0f;
	public float slopeGradient = 0.1f;

	public enum CenterPoint {Center, Corner}
	public CenterPoint centerPoint;
	
	public void ClearMesh()
	{	
		l_vertices.Clear ();
		l_uvs.Clear ();
		l_triangles.Clear ();
		l_normals.Clear ();
	}
	
	void OnEnable() 
	{
		//gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
		_meshRenderer = GetComponent<MeshRenderer>();
	}
	
	public void CreateMesh()
	{
		//TextureMesh ();
		BuildMesh ();
		GetComponent<MeshCollider>().sharedMesh = mesh;
		ReCalculateMesh(true);
	}
	
	public void TextureMesh()
	{
		
		if (_meshRenderer.sharedMaterial == null)
		{
			Debug.Log ("ERROR: Couldn't find tile material.\n");
			return;
		}
		
		Material mat = new Material(_meshRenderer.sharedMaterial);
		
		_meshRenderer.material = mat; //Update the material
		
	}
	
	void BuildMesh()
	{
		ClearMesh ();
		
		for (int x = 0; x < meshSize; x++)
		{
			for (int z = 0; z < meshSize; z++)
			{
				Vector3 position = new Vector3(squareSize * x, 0, squareSize*z);
				CreateFace(position);
			}
		}
	}
	
	
	float IntNoise(int x)			 
	{
		x = (x<<13) ^ x;
		return ( 1.0f - ( (x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);    
	}
	
	Vector3 GetNoiseOffset(Vector3 pos)
	{
		float scale = noiseOffset;
		int hash1 = (int)(pos.x + pos.z * meshSize);
		int hash2 = (int)(pos.x - pos.z * meshSize);
		Vector3 noise = new Vector3(IntNoise(hash1), IntNoise (hash1), IntNoise ( hash2));
		noise.x *= scale;
		noise.z *= scale;
		noise.y *= heightNoise;
		return noise;
	}

	//Returns global coordinate for where the rock mesh should descend from
	Vector3 GetCenterPoint()
	{
		Vector3 center = transform.position;
		switch(centerPoint)
		{
		case CenterPoint.Center:
			center += new Vector3(squareSize * meshSize * 0.5f, 0, squareSize * meshSize * 0.5f);
			break;
		case CenterPoint.Corner:
			//TL is the origin
			//center += new Vector3();
			break;
		}

		return center;
	}

	float GetRockHeight(Vector3 pos)
	{
		Vector3 center = GetCenterPoint();
		float centerDist = (center - pos).magnitude;

		//Get lower as you fall away from centre
		float height = -centerDist*centerDist;
		height = height * slopeGradient;
		return height;
	}
	
	void CreateFace(Vector3 pos)
	{
		Vector3 origin = pos;
		int vertCount  = l_vertices.Count;
		
		Vector3 p0 = origin + new Vector3(0,  0, squareSize);
		p0.y = GetRockHeight(p0+transform.position);
		p0 = p0 + GetNoiseOffset (p0+transform.position);
		l_vertices.Add (p0);
		
		Vector3 p1 = origin + new Vector3(squareSize, 0, squareSize);
		p1.y = GetRockHeight(p1+transform.position);
		p1 = p1 + GetNoiseOffset (p1+transform.position);
		l_vertices.Add (p1);
		
		Vector3 p2 = origin + new Vector3(squareSize, 0, 0);
		p2.y = GetRockHeight(p2+transform.position);
		p2 = p2 + GetNoiseOffset (p2+transform.position);
		l_vertices.Add (p2);
		
		//Build second
		l_vertices.Add (p0);
		
		l_vertices.Add (p2);
		
		Vector3 p3 = origin + new Vector3(0, 0, 0);
		p3.y = GetRockHeight(p3+transform.position);
		p3 = p3 + GetNoiseOffset (p3+transform.position);
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
			l_normals.Add(norm * 1.0f);
		}
		
		Vector3 norm2 = Vector3.Cross(p2 - p0, p3 - p0).normalized;
		for (int i=0; i < 3; i++)
		{
			l_normals.Add(norm2 * 1.0f);
		}
		
		for (int i = 0; i < 6; i++)
		{
			l_uvs.Add (GetUV (l_vertices[vertCount + i]));
		}
	}
	
	Vector2 GetUV(Vector3 vertPos)
	{
		Vector2 uv = new Vector2(vertPos.x, vertPos.z);
		TerrainSettings settings = Map.Instance ().terrainSettings;
		uv /= (settings.tileTextureScale * settings.tileSquareSize);
		
		return uv;
	}
	
	void OnDestroy()
	{
		ClearMesh();
		DestroyImmediate (mesh);

	}
}