using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class VegetationSpawner : MonoBehaviour 
{
	FloatField vegField;
	Map map;

	public int gridSpacing;

	public float minDensity;
	public float positionNoise;
	public float spawnChance;
	public float maxSteepness;
	public float minHeight;
	public float trunkInset;

	public GameObject treePrefab;

	[HideInInspector]
	public List<GameObject> objects;

	void Awake()
	{
		if (objects == null)
		{
			objects = new List<GameObject>();
		}
	}

	void OnEnable()
	{
		map = Map.Instance();
	    if (vegField == null)
	        vegField = ScriptableObject.CreateInstance<FloatField>();
	}

	public void ClearVeg()
	{
		foreach(GameObject prop in objects)
		{
			if (prop != null)
				DestroyImmediate(prop);
		}
		objects.Clear ();
	}
	public void SpawnVeg()
	{
		ClearVeg ();
		map.heightField.CalculateGradient(vegField,2);
		//Size of the terrain in world units
		int gridSize = (int)(map.terrainSettings.tileArraySideLength * map.terrainSettings.tileSideLength * map.terrainSettings.tileSquareSize);

		//Loop through the grid of possible spawn locations
		for (int x=0; x < gridSize; x+=gridSpacing)
		{
			for (int z=0; z < gridSize; z+=gridSpacing)
			{
				Vector3 treePos = new Vector3(x,0,z);
				bool shouldSpawn = true;
				//Get the steepness of the world here
				Point fieldPoint = map.WorldToFieldIndex(treePos, vegField);
				float steepness = vegField.GetValue (vegField.CoordToIndex (fieldPoint.x, fieldPoint.y));

				//Dont spawn on hills
				shouldSpawn = steepness < maxSteepness;
			

				//Randomize a bit
				shouldSpawn = shouldSpawn && (Random.Range(0.0f,1.0f/spawnChance) <= 1.0f);

				if (shouldSpawn)
				{
					float height = map.GetTerrainHeight (treePos);

					//Don't spawn underwater
					shouldSpawn = height > minHeight;
					if (!shouldSpawn) continue;

					treePos.y = height - trunkInset;
					//Randomize a bit (Disguise the grid)
					Vector3 offset = new Vector3(Random.Range (-positionNoise, positionNoise), 0, Random.Range (-positionNoise,positionNoise));
					//Create the game object
					GameObject prop = Instantiate (treePrefab, treePos + offset, Quaternion.identity) as GameObject;
					prop.transform.parent = transform;
					prop.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
					objects.Add (prop);
				}
			}
		}

		Debug.Log ("Veg Spawner: Spawned " + objects.Count + " props");
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
