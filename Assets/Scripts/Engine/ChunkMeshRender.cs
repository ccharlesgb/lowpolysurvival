using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Give a class some block data and it will return a mesh
public class ChunkMeshRender : MonoBehaviour
{
	public IntCoord[] nearestNeighbours;
	
	public TextureAtlas textureAtlas;
	
	//Mesh Data
	public List<Vector3> vertices;
	public List<Vector3> normals;
	public List<int> triangles;
	public List<int> waterTriangles;
	public List<Vector2> uvs;
	
	public Mesh meshOutput;
	
	public bool needsUpdate;
	
	public Chunk parentChunk;
	Map map;
	
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
	
	void Awake()
	{
		needsUpdate = false;
		textureAtlas = GetComponent<TextureAtlas>();
		map = GameObject.Find ("Map").GetComponent<Map>();
		//parentChunk = chunk;
		
		nearestNeighbours = new IntCoord[6];
		nearestNeighbours[0] = new IntCoord(0,0,1);
		nearestNeighbours[1] = new IntCoord(0,0,-1);
		nearestNeighbours[2] = new IntCoord(0,1,0);
		nearestNeighbours[3] = new IntCoord(0,-1,0);
		nearestNeighbours[4] = new IntCoord(1,0,0);
		nearestNeighbours[5] = new IntCoord(-1,0,0);
		
		vertices = new List<Vector3>();
		triangles = new List<int>();
		waterTriangles = new List<int>();
		normals = new List<Vector3>();
		uvs = new List<Vector2>();	
	}
	
	void Update()
	{
		if (needsUpdate)
		{
			//Debug.Log ("UPDATING " + parentChunk.gridCoord);
			Mesh mesh;
		 	BuildMesh(out mesh);
			GetComponent<MeshFilter>().sharedMesh = mesh;
			needsUpdate = false;
		}
	}
	
	public void ClearMesh()
	{	
		vertices.Clear ();
		uvs.Clear ();
		triangles.Clear ();
		waterTriangles.Clear ();
		normals.Clear ();
	}
	
	public void BuildMesh(out Mesh mesh)
	{
		ClearMesh ();
		mesh = new Mesh();
		mesh.subMeshCount = 2;
		IntCoord pos = new IntCoord();
		int chunkSize = Map.chunkSize;
		for (int x = 0; x < chunkSize; x++)
		{
			for (int z = 0; z < chunkSize; z++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					pos.x = x;
					pos.y = y;
					pos.z = z;
					BlockHandle block = parentChunk.blocks[x,y,z];
					//Is there a block to render?
					if (!block.IsEmpty() && block.IsVisible())
					{
						//block.GetBlock ().BuildMesh(this);
						//Calculate which faces are visible
						for (int i=0; i < 6; i++)
						{					
							IntCoord neighbourPos = pos + nearestNeighbours[i];
							BlockHandle neighbour;
							//If the position is in our chunk we can just get the neighbour directly
							//Very important optimisation local block fetching is MUCH MUCH faster
							if (parentChunk.InLocalChunkBounds(neighbourPos))
							{
								neighbour = parentChunk.GetBlockLocal(neighbourPos);
							}
							else
							{
								//if it isnt we have to find out which chunk it is in and then get that block vlaue
								neighbour = map.GetBlock(parentChunk.LocalToWorld(neighbourPos));
							}
							if (block.GetBlock ().ShouldRenderFace(neighbour))
							{
								CreateFace(pos.ToVector3() * Map.cubeSize, nearestNeighbours[i].ToVector3(), block);
				 			}
						}
					}
				}
			}	
		}
		
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray ();
		mesh.SetTriangles (triangles.ToArray (), 0);
		mesh.SetTriangles (waterTriangles.ToArray (), 1);
		mesh.normals = normals.ToArray ();
		mesh.RecalculateBounds ();	
		
		//worldGen.UpdateChunkNeighbours (this);
	
	}
	
	void CreateFace(Vector3 blockPos, Vector3 normal, BlockHandle block)
	{
		int vertCount = vertices.Count;
		Vector3 origin = blockPos + (normal * Map.cubeSize * 0.5f);

		float ext = Map.cubeSize / 2.0f; //The 'extent' of the cube
		byte faceID = 0;
		if (normal == Vector3.forward)
		{
			faceID = 1;
			vertices.Add(origin + new Vector3(ext,ext, 0));
			vertices.Add(origin + new Vector3(-ext, ext, 0));
			vertices.Add(origin + new Vector3(-ext,-ext, 0));
			vertices.Add(origin + new Vector3(ext,-ext,0));
		}
		if (normal == -Vector3.forward)
		{
			faceID = 2;
			vertices.Add(origin + new Vector3(-ext,ext, 0));
			vertices.Add(origin + new Vector3(ext, ext, 0));
			vertices.Add(origin + new Vector3(ext,-ext, 0));
			vertices.Add(origin + new Vector3(-ext,-ext,0));
		}
		if (normal == Vector3.up)
		{
			faceID = 0;
			vertices.Add(origin + new Vector3(-ext,  0, ext));
			vertices.Add(origin + new Vector3(ext,  0, ext));
			vertices.Add(origin + new Vector3(ext, 0, -ext));
			vertices.Add(origin + new Vector3(-ext, 0, -ext));
		}
		if (normal == -Vector3.up)
		{
			faceID = 5;
			vertices.Add(origin + new Vector3(-ext,  0, -ext));
			vertices.Add(origin + new Vector3(ext,  0, -ext));
			vertices.Add(origin + new Vector3(ext, 0, ext));
			vertices.Add(origin + new Vector3(-ext, 0, ext));
		}
		if (normal == Vector3.right)
		{
			faceID = 4;
			vertices.Add(origin + new Vector3(0, ext,-ext));
			vertices.Add(origin + new Vector3(0, ext, ext));
			vertices.Add(origin + new Vector3(0,-ext, ext));
			vertices.Add(origin + new Vector3(0,-ext,-ext));
		}
		if (normal == -Vector3.right)
		{
			faceID = 3;
			vertices.Add(origin + new Vector3(0,ext,ext));
			vertices.Add(origin + new Vector3(0,ext, -ext));
			vertices.Add(origin + new Vector3(0,-ext, -ext));
			vertices.Add(origin + new Vector3(0,-ext,ext));
		}

		int texCoord = block.GetBlock ().GetFaceIDs(block)[faceID];

		Vector2[] uvCoords = textureAtlas.CoordToUV(texCoord);
		for (int i=0; i < 4; i++)
		{
			uvs.Add(uvCoords[i]);	
		}
	
//		uvs.Add (new Vector2(0.0f,1.0f));
//		uvs.Add (new Vector2(1.0f,1.0f));
//		uvs.Add (new Vector2(1.0f,0.0f));
//		uvs.Add (new Vector2(0.0f,0.0f));
		
		if (!block.IsTransparent ())
		{
			triangles.Add(vertCount);
			triangles.Add(vertCount + 1);
			triangles.Add(vertCount + 2);
			
			triangles.Add(vertCount);
			triangles.Add(vertCount + 2);
			triangles.Add(vertCount + 3);
		}
		else
		{
			waterTriangles.Add(vertCount);
			waterTriangles.Add(vertCount + 1);
			waterTriangles.Add(vertCount + 2);
			
			waterTriangles.Add(vertCount);
			waterTriangles.Add(vertCount + 2);
			waterTriangles.Add(vertCount + 3);
		}
		
		for (int i=0; i < 4; i++)
		{
			normals.Add(normal);
		}
	}
}