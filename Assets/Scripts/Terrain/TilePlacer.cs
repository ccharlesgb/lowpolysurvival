using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

[AddComponentMenu("Terrain/Tile Placer")]
public class TilePlacer : MonoBehaviour {
	
	public GameObject terrainFab;
	public List<GameObject> tiles;

	public bool dirty;

	public bool needsUpdate = false;
	
	void Awake()
	{
		dirty = false;
		//DontDestroyOnLoad (gameObject);
		if (tiles == null)
		{
			Debug.Log ("RECREATE LIST");
			tiles = new List<GameObject>();
		}
		//heightMap = GetComponent<HeightMap>();
	}

	void OnEnable()
	{
		bool missingTerrain = false;
		for (int i = 0; i < tiles.Count; i++)
		{
			if (tiles[i] == null)
			{
				missingTerrain = true;
				break;
			}
		}
		int terrainSize = Map.Instance ().terrainSettings.tileArraySideLength;
		//Debug.Log ("MISSING TERRAIN " + missingTerrain);
		//Debug.Log ("TILE COUNT " + tiles.Count + " SHOULD BE: " + terrainSize * terrainSize);
		if (tiles.Count != terrainSize * terrainSize || missingTerrain)
		{
			//StartCoroutine( PlaceTerrain() );
			MarkDirty ();
		}
	}

	public void MarkDirty()
	{
		dirty = true;
		if (Application.isEditor)
		{
			//Debug.Log ("call dirty");
			//splatMap.CreateMap(heightMap.heightGrad);
			PlaceTerrain ();
			dirty = false;
		}
	}

	void OnDisable()
	{
	}

	public void ClearTerrain()
	{
		Debug.Log ("CLEAR TERRAIN");
		foreach(GameObject tile in tiles)
		{
			DestroyImmediate(tile);
		}
		tiles.Clear ();
	}

    public TileRender GetTileAt(Vector3 pos)
    {
        Vector3 posIndex = MathTools.ScalarDivide(pos, TileRender.GetTileBounds());

        int x = (int) posIndex.x;
        int z = (int) posIndex.z;

        int arrayIndex = z + x * Map.Instance().terrainSettings.tileArraySideLength;
        return tiles[arrayIndex].GetComponent<TileRender>();
    }

    public TileRender[] GetTilesInBounds(Rect rect, out int size)
    {
        Vector3 tL = rect.min;
        Vector3 bR = rect.max;
        Vector3 tR = rect.min + new Vector2(rect.width, 0.0f);
        Vector3 bL = rect.max - new Vector2(rect.width, 0.0f);
        tL.z = tL.y;
        bR.z = bR.y;
        tR.z = tR.y;
        bL.z = bL.y;
        TileRender tLTile = GetTileAt(tL);
        TileRender tRTile = GetTileAt(tR);
        TileRender bLTile = GetTileAt(bL);
        TileRender bRTile = GetTileAt(bR);

        int uniqueTiles = 0;
        var tileArray = new TileRender[4];
        tileArray[0] = tLTile;
        uniqueTiles++;

        if (tRTile != tLTile)
        {
            tileArray[uniqueTiles] = tRTile;
            uniqueTiles++;
        }
        if (bLTile != tLTile && bLTile != tRTile)
        {
            tileArray[uniqueTiles] = bLTile;
            uniqueTiles++;
        }
        if (bRTile != tLTile && bRTile != tRTile && bRTile != bLTile)
        {
            tileArray[uniqueTiles] = bRTile;
            uniqueTiles++;
        }
        size = uniqueTiles;
        return tileArray;
    }

    public void PlaceTerrain()
	{
		ClearTerrain ();
		int terrainSize = Map.Instance ().terrainSettings.tileArraySideLength;
		for (int x=0; x < terrainSize; x++)
		{
			for (int z=0; z < terrainSize; z++)
			{
				Vector3 tilePos = MathTools.ScalarMultiply(new Vector3(x, 0, z),TileRender.GetTileBounds ());

				GameObject tile = Instantiate (terrainFab, tilePos, Quaternion.identity) as GameObject;

				tile.GetComponent<TileRender>().CreateMesh();
				tile.transform.parent = transform.Find ("Tiles");
				tiles.Add (tile);

				//yield return null;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (dirty)
		{
			Debug.Log ("call dirty");
			//splatMap.CreateMap(heightMap.heightGrad);
			PlaceTerrain ();
			dirty = false;
		}
	}
}
