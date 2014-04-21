using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class TilePlacer : MonoBehaviour {

	public int terrainSize = 8;
	public GameObject terrainFab;
	public List<GameObject> tiles;
	// Use this for initialization
	void OnEnable()
	{
		tiles = new List<GameObject>();
		PlaceTerrain ();
	}

	void OnDisable()
	{
		ClearTerrain ();
		tiles = null;
	}

	void ClearTerrain()
	{
		foreach(GameObject tile in tiles)
		{
			DestroyImmediate(tile);
		}
		tiles.Clear ();
	}

	void PlaceTerrain()
	{
		ClearTerrain ();
		for (int x=0; x < terrainSize; x++)
		{
			for (int z=0; z < terrainSize; z++)
			{
				Vector3 tilePos = MathTools.ScalarMultiply(new Vector3(x, 0, z),TileRender.GetTileBounds ());

				GameObject tile = Instantiate (terrainFab, tilePos, Quaternion.identity) as GameObject;
				tile.transform.parent = transform;

				tiles.Add (tile);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
