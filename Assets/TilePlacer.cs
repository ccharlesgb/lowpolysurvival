using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class TilePlacer : MonoBehaviour {

	public int terrainSize = 8;
	public GameObject terrainFab;
	public List<GameObject> tiles;
	// Use this for initialization
	void Awake() 
	{
		tiles = new List<GameObject>();
		PlaceTerrain ();
	}

	void ClearTerrain()
	{
		foreach(GameObject tile in tiles)
		{
			Destroy (tile);
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

				GameObject tile = Instantiate (terrainFab) as GameObject;
				tile.transform.parent = transform;
				tile.transform.position = tilePos;

				tiles.Add (tile);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
