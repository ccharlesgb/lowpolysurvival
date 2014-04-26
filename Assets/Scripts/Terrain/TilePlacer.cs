using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class TilePlacer : MonoBehaviour {

	public int terrainSize = 8;
	public GameObject terrainFab;
	public List<GameObject> tiles;
	public HeightMap heightMap;
	public SplatMap splatMap;

	public bool needsUpdate = false;
	
	void Awake()
	{
		if (tiles == null)
			tiles = new List<GameObject>();
		heightMap = GetComponent<HeightMap>();
	}
	
	void OnEnable()
	{
		Debug.Log ("On Enable" + Application.isPlaying);
		//tiles = new List<GameObject>();
	}

	void Start()
	{
		bool missingTerrain = false;
		foreach (GameObject curTile in tiles)
		{
			if (curTile == null)
				missingTerrain = true;
		}

		if (tiles.Count != terrainSize * terrainSize || missingTerrain)
		{
			//StartCoroutine( PlaceTerrain() );
			MarkDirty ();
		}
	}

	public void MarkDirty()
	{
		Debug.Log ("call dirty");
		splatMap.CreateMap(heightMap.heightGrad);
		PlaceTerrain ();
	}

	void OnDisable()
	{
	}

	public void ClearTerrain()
	{
		foreach(GameObject tile in tiles)
		{
			DestroyImmediate(tile);
		}
		tiles.Clear ();
	}

	public void PlaceTerrain()
	{
		ClearTerrain ();
		for (int x=0; x < terrainSize; x++)
		{
			for (int z=0; z < terrainSize; z++)
			{
				Vector3 tilePos = MathTools.ScalarMultiply(new Vector3(x, 0, z),TileRender.GetTileBounds ());

				GameObject tile = Instantiate (terrainFab, tilePos, Quaternion.identity) as GameObject;
				tile.GetComponent<TileRender>().mHeights = heightMap;
				tile.GetComponent<TileRender>().mSplats = splatMap;
				tile.GetComponent<TileRender>().CreateMesh();
				tile.transform.parent = transform;
				tiles.Add (tile);

				//yield return null;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (needsUpdate)
		{
			//StartCoroutine( PlaceTerrain() );
			PlaceTerrain ();
			needsUpdate = false;
		}
	}
}
