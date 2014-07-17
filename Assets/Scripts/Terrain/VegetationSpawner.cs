using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

[ExecuteInEditMode]

[System.Serializable]
public class VegetationDetails
{
    public AnimationCurve densityAgainstSlope;
    public AnimationCurve densityAgainstHeight;

    public float spawnChance;
    public float trunkInset;

    public GameObject treePrefab;
}

public class VegetationSpawner : MonoBehaviour
{
    [Range(0,2500)]
    public int maxProps = 2000;

	Texture2D vegField;
	Map map;

    public float globalSpawnChance;

    public float gridPadding;
	public int gridSpacing;
    public float positionNoise;

    public List<VegetationDetails> vegDetails = new List<VegetationDetails>();

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
        map = Map.Instance();
		ClearVeg ();
	    vegField = map.gradTexture;
	    TextureTools.GetDerivativeMap(vegField, 8);
		//Size of the terrain in world units
		int gridSize = (int)(map.terrainSettings.tileArraySideLength * map.terrainSettings.tileSideLength * map.terrainSettings.tileSquareSize);

		//Loop through the grid of possible spawn locations
	    float gridBuffer = positionNoise + gridPadding;

        for (float x = gridBuffer; x < gridSize; x += gridSpacing - gridBuffer)
		{
            for (float z = gridBuffer; z < gridSize; z += gridSpacing - gridBuffer)
            {
                if (objects.Count >= maxProps)
                {
                    Debug.Log("Vegetation Spawner: Reached prop limit early!");
                    goto BREAK;
                }


				Vector3 treePos = new Vector3(x,0,z);
                Vector3 offset = new Vector3(Random.Range(-positionNoise, positionNoise), 0, Random.Range(-positionNoise, positionNoise));
			    treePos += offset;
			    bool shouldSpawn;

                //Get the height of the world here
                float mapHeight = map.GetTerrainHeight(treePos);
                //Get the steepness of the world here
                Point fieldPoint = map.WorldToTextureCoords(treePos, vegField.height);
                float steepness = vegField.GetPixel(fieldPoint.x, fieldPoint.y).r;

                float[] spawnChances = new float[vegDetails.Count];
			    float totalChance = 0.0f;
			    for(int i=0; i < vegDetails.Count; i++)
			    {
                    float density = vegDetails[i].densityAgainstSlope.Evaluate(steepness);
                    density *= vegDetails[i].densityAgainstHeight.Evaluate(mapHeight / map.terrainSettings.heightScale);
                    density *= vegDetails[i].spawnChance;

			        spawnChances[i] = density;
			        totalChance += density;
			    }
                //Renormalise the chance
                for (int i = 0; i < vegDetails.Count; i++)
                {
                    spawnChances[i] /= totalChance;
                }

			    float randomFloat = Random.Range(0.0f, 1.0f);
			    float cumulativeChance = 0.0f;
			    int indexToSpawn = -1;
                for (int i = 0; i < vegDetails.Count; i++)
                {
                    if (randomFloat > cumulativeChance && randomFloat <= cumulativeChance + spawnChances[i])
                    {
                        indexToSpawn = i;
                        break;
                    }
                    cumulativeChance += spawnChances[i];
                }
			    if (indexToSpawn != -1 && (Random.Range(0.0f, 1.0f/globalSpawnChance) <= 1.0f)) 
			    {
			        treePos.y = mapHeight;
			        SpawnProp(treePos, vegDetails[indexToSpawn]);
			    }
			}
		}
	    BREAK:
		Debug.Log ("Veg Spawner: Spawned " + objects.Count + " props");
	}

    void SpawnProp(Vector3 pos, VegetationDetails details)
    {
        pos.y -= details.trunkInset;
        //Randomize a bit (Disguise the grid)
        //Create the game object
        GameObject prop = Instantiate(details.treePrefab, pos, Quaternion.identity) as GameObject;
        if (prop != null)
        {
            prop.transform.parent = transform;
            prop.hideFlags = HideFlags.HideInHierarchy;
            objects.Add(prop);
        }
    }

	// Update is called once per frame
	void Update () 
	{
	
	}
}
