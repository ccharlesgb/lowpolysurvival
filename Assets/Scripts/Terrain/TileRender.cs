using System.Net.Sockets;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

[ExecuteInEditMode]
[AddComponentMenu("Terrain/Splat Map")]
public class TileRender : MonoBehaviour
{
	//PROCEDURAL MESH DEFINITIONS
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

	//END MESH DEFINITIONS

	public Vector3 buildPos;

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

	//Returns how bix the tile is in world space
	static public Vector3 GetTileBounds()
	{
		float bound = Map.Instance ().terrainSettings.tileSideLength * Map.Instance().terrainSettings.tileSquareSize;
		return new Vector3(bound, 0, bound);
	}

	//Delete all the arrays
	public void ClearMesh()
	{	
		l_vertices.Clear ();
		l_uvs.Clear ();
		l_triangles.Clear ();
		l_normals.Clear ();
	}
	
	void OnEnable() 
	{
		gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
	    gameObject.hideFlags = HideFlags.None;
		_meshRenderer = GetComponent<MeshRenderer>();
	}

	//Returns the shader parameter to decide which part of the control texture this tile should use
	Vector4 CalculateSplatUV()
	{
		//Pos Param is the chunk of the control UV that this particular tile should use
		Point splatCoords = Map.Instance().WorldToFieldIndex(transform.position, Map.Instance().splatField);
		Point splatCoordsTile = Map.Instance().WorldToFieldIndex(TileRender.GetTileBounds(), Map.Instance ().splatField);
		
		//Adjust the control UV
		Vector4 posParam = new Vector4();
		posParam.w = splatCoords.x / (float)Map.Instance().splatField.Width;
		posParam.x = splatCoords.y / (float)Map.Instance().splatField.Height;
		posParam.y = splatCoordsTile.x / ((float)Map.Instance().splatField.Width); // UV Scale parameters
		posParam.z = splatCoordsTile.y / ((float)Map.Instance().splatField.Height) / Map.Instance ().terrainSettings.tileSideLength;
		posParam.z = posParam.z * Map.Instance ().terrainSettings.tileTextureScale;

		return posParam;
	}

	public void CreateMesh()
	{
		TextureMesh ();
		BuildMesh ();
		GetComponent<MeshCollider>().sharedMesh = mesh;

		ReCalculateMesh(true);

#if UNITY_EDITOR
	    string path = "Assets/Terrain/Tiles/tile" + transform.position.x + " " + transform.position.z + ".asset";
        Mesh tryMesh = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
	    //if (tryMesh != null)
	   //     AssetDatabase.DeleteAsset(path);
       // AssetDatabase.CreateAsset( mesh, path );
        //AssetDatabase.SaveAssets();
#endif
	}

	public void TextureMesh()
	{
		if (_meshRenderer.sharedMaterial == null)
		{
			Debug.Log ("ERROR: Couldn't find tile material.\n");
			return;
		}

		Material mat = new Material(_meshRenderer.sharedMaterial);
		mat.SetTexture ("_Control",Map.Instance().splatField.GetTexture()); //Delete?
		
		
		mat.SetVector("_Position", CalculateSplatUV());
		
		_meshRenderer.material = mat; //Update the material

	}

	void BuildMesh()
	{
		ClearMesh ();

		for (int x = 0; x < Map.Instance().terrainSettings.tileSideLength; x++)
		{
			for (int z = 0; z < Map.Instance().terrainSettings.tileSideLength; z++)
			{
				Vector3 position = new Vector3(Map.Instance().terrainSettings.tileSquareSize * x, 0, Map.Instance().terrainSettings.tileSquareSize*z);
				CreateFace(position);
			}
		}
		buildPos = transform.position;
	}

	
	float IntNoise(int x)			 
	{
		x = (x<<13) ^ x;
		return ( 1.0f - ( (x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);    
	}

	Vector3 GetNoiseOffset(Vector3 pos)
	{
		float scale = Map.Instance ().terrainSettings.gridNoiseScale;
		int hash1 = (int)(pos.x + pos.z * Map.Instance ().terrainSettings.tileSideLength);
		int hash2 = (int)(pos.x - pos.z * Map.Instance ().terrainSettings.tileSideLength);
		Vector3 noise = new Vector3(IntNoise(hash1), IntNoise (hash1), IntNoise ( hash2));
		noise.x *= scale;
		noise.z *= scale;
		noise.y *= Map.Instance ().terrainSettings.heightNoiseScale;
		return noise;
	}
		
	void CreateFace(Vector3 pos)
	{
		Vector3 origin = pos;
		int vertCount  = l_vertices.Count;

		float squareSize = Map.Instance().terrainSettings.tileSquareSize;

		Vector3 p0 = origin + new Vector3(0,  0, squareSize);
		p0.y = Map.Instance().GetTerrainHeight(p0+transform.position);
		p0 = p0 + GetNoiseOffset (p0+transform.position);
		l_vertices.Add (p0);

		Vector3 p1 = origin + new Vector3(squareSize, 0, squareSize);
		p1.y = Map.Instance().GetTerrainHeight(p1+transform.position);
		p1 = p1 + GetNoiseOffset (p1+transform.position);
		l_vertices.Add (p1);

		Vector3 p2 = origin + new Vector3(squareSize, 0, 0);
		p2.y = Map.Instance().GetTerrainHeight(p2+transform.position);
		p2 = p2 + GetNoiseOffset (p2+transform.position);
		l_vertices.Add (p2);

		//Build second
		l_vertices.Add (p0);

		l_vertices.Add (p2);

		Vector3 p3 = origin + new Vector3(0, 0, 0);
		p3.y = Map.Instance().GetTerrainHeight(p3+transform.position);
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
		//Debug.Log ("CLEANING MEMORY");
		ClearMesh();
		//DestroyImmediate (mesh);
		//DestroyImmediate (_meshRenderer.sharedMaterial.GetTexture("_Control"), true);
		//DestroyImmediate (_meshRenderer.material);
	}
}
